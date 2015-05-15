using System;
using System.IO;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;

namespace RegistrationKiosk
{
    
    class IOExcel
    {
        
     
        public IOExcel()
        {
        }

        public string selectFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            // Setting the filter options
            ofd.Filter = "Microsoft Excel Worksheet (.xlsx)|*.xlsx|All Files (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.Multiselect = false;

            // Check if user selected file or not
            if (ofd.ShowDialog() == DialogResult.OK)
                return ofd.FileName;
            else
                return null;
        }

        public string selectFolder()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
                return fbd.SelectedPath;
            else
                return null;
        }

        public void importExcel(string filename)
        {
            int i, j, sheetNum = 0;
            string columns = "", data = "";
            MySQLClient msc = new MySQLClient("cscd379.com", "jobfair", "jobfair", "ewu2015");
            
            string eventName = Path.GetFileNameWithoutExtension(filename);
            msc.createEvent(eventName);

            ApplicationClass app = new ApplicationClass();
            Workbook book = null;
            Range range = null;

            try
            {
                app.Visible = false;
                app.ScreenUpdating = false;
                app.DisplayAlerts = false;

                string execPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);

                book = app.Workbooks.Open(@filename, Missing.Value, Missing.Value, Missing.Value
                                                  , Missing.Value, Missing.Value, Missing.Value, Missing.Value
                                                 , Missing.Value, Missing.Value, Missing.Value, Missing.Value
                                                , Missing.Value, Missing.Value, Missing.Value);
                foreach (Worksheet sheet in book.Worksheets)
                {
                    Console.WriteLine(@"Values for Sheet " + sheet.Index);

                    // get a range to work with
                    range = sheet.get_Range("A1", Missing.Value);
                    // get the end of values to the right (will stop at the first empty cell)
                    range = range.get_End(XlDirection.xlToRight);
                    // get the end of values toward the bottom, looking in the last column (will stop at first empty cell)
                    range = range.get_End(XlDirection.xlDown);

                    // get the address of the bottom, right cell
                    string downAddress = range.get_Address(
                        false, false, XlReferenceStyle.xlA1,
                        Type.Missing, Type.Missing);

                    // Get the range, then values from a1
                    range = sheet.get_Range("A1", downAddress);
                    object[,] values = (object[,])range.Value2;


                    columns = "";
                    if (values.GetLength(1) > 0)
                        columns += values[1, 1];
                    for (i = 2; i <= values.GetLength(1); i++)
                        columns += "," + values[1, i];

                    // Enter into the database
                    for ( i = 2; i <= values.GetLength(0); i++)
                    {
                        data = "";
                        if (values.GetLength(1) > 0)
                        data += "'" + values[i, 1] + "'";

                        for (j = 2; j <= values.GetLength(1); j++)
                            data += ", '" + values[i, j] + "'";

                        //Console.WriteLine(data);

                        if (sheetNum == 0)
                            msc.Insert(eventName + "_registrants", columns, data);
                        else if (sheetNum == 1)
                            msc.Insert(eventName + "_students", columns, data);
                        else if (sheetNum == 2)
                            msc.Insert(eventName + "_employees", columns, data);
                        
                    }



                    sheetNum++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                range = null;
                if (book != null)
                    book.Close(false, Missing.Value, Missing.Value);
                book = null;
                if (app != null)
                    app.Quit();
                app = null;
            }
        }

        public void exportExcel( System.Data.DataTable dt, String filename)
        {
            Microsoft.Office.Interop.Excel.Application excel;
            Microsoft.Office.Interop.Excel.Workbook excelworkBook;
            Microsoft.Office.Interop.Excel.Worksheet excelSheet;
            Microsoft.Office.Interop.Excel.Range excelCellrange;

            // Start Excel and get Application object.
            excel = new Microsoft.Office.Interop.Excel.Application();

            // for making Excel visible
            excel.Visible = false;
            excel.DisplayAlerts = false;

            // Creation a new Workbook
            excelworkBook = excel.Workbooks.Add(Type.Missing);

            // Work sheet
            excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelworkBook.ActiveSheet;
            excelSheet.Name = "Test work sheet";

            excelSheet.Cells[1, 1] = "Sample test data";
            excelSheet.Cells[1, 2] = "Date : " + DateTime.Now.ToShortDateString();

            excelCellrange = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[dt.Rows.Count, dt.Columns.Count]];
            excelCellrange.EntireColumn.AutoFit();
            Microsoft.Office.Interop.Excel.Borders border = excelCellrange.Borders;
            border.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            border.Weight = 2d;

            excelSheet.SaveAs(@Path.GetDirectoryName(filename) + "/Org.xlsx");
            excel.Quit();

            /*//Creae an Excel application instance
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();

            //Create an Excel workbook instance and open it from the predefined location
            Microsoft.Office.Interop.Excel.Workbook excelWorkBook = excelApp.Workbooks.Open(@Path.GetDirectoryName(filename)+"/Org.xlsx");

            Microsoft.Office.Interop.Excel.Worksheet sheet = excelWorkBook.Sheets["Sheet1"] as Microsoft.Office.Interop.Excel.Worksheet; 
            Microsoft.Office.Interop.Excel.Range range = sheet.get_Range("A1", Missing.Value);

            if (range != null)
                 foreach (Microsoft.Office.Interop.Excel.Range r in range)
                 {
                     string user = (string) r.Text;
                     string value = (string) r.Value2;

                 }
                //Add a new worksheet to workbook with the Datatable name
            Microsoft.Office.Interop.Excel.Worksheet excelWorkSheet = excelWorkBook.Sheets["Data"] as Microsoft.Office.Interop.Excel.Worksheet;
                excelWorkSheet.Name = dt.TableName;

                for (int i = 1; i < dt.Columns.Count + 1; i++)
                {
                    excelWorkSheet.Cells[1, i] = dt.Columns[i - 1].ColumnName;
                }

                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        excelWorkSheet.Cells[j + 2, k + 1] = dt.Rows[j].ItemArray[k].ToString();
                    }
                }
            

            excelWorkBook.Save();
            excelWorkBook.Close();
            excelApp.Quit();*/
            
        }
    }
}
