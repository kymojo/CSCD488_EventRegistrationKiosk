using System;
using System.IO;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;

namespace RegistrationKiosk {
    
    public class IOExcel {

        //===========================================================================
        #region Class Variables
        //===========================================================================

        MySQLClient sqlClient;

        #endregion
        //===========================================================================
        #region Class Constructor
        //===========================================================================

        public IOExcel(MySQLClient sqlClient) {
            this.sqlClient = sqlClient;
        }

        #endregion
        //===========================================================================
        #region Class Methods
        //===========================================================================

        /// <summary>
        /// Allows user to select a file.
        /// </summary>
        /// <returns>Filename (null if none selected)</returns>
        public string SelectFile() {

            OpenFileDialog ofd = new OpenFileDialog();

            try
            {

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
            catch(Exception){
                MessageBox.Show("Incorrect file type (.xlsx is needed) or \nMicrosoft Excel isn't installed.");
                return null;
            }
        }

        /// <summary>
        /// Allows user to select filename and save directory.
        /// </summary>
        /// <returns></returns>
        public string SelectSaveFile() {

            SaveFileDialog fbd = new SaveFileDialog();
            try
            {
                fbd.Filter = "Microsoft Excel Worksheet (.xlsx)|*.xlsx|All Files (*.*)|*.*";
                fbd.FilterIndex = 1;

                // Check if user selected file or not
                if (fbd.ShowDialog() == DialogResult.OK)
                    return fbd.FileName;
                else
                    return null;
            }
            catch (Exception){
                MessageBox.Show("Microsoft Excel needs to be installed on this computer in order to use this feature.");
                return null;
            }

        }

        /// <summary>
        /// Imports entries to database from Excel worksheet
        /// </summary>
        /// <param name="filename">Filename of Excel worksheet</param>
        public void ImportExcel(string filename) {

            int i, j, sheetNum = 0;
            string columns = "", data = "";

            ApplicationClass app = new ApplicationClass();
            Workbook book = null;
            Range range = null;

            try {
                app.Visible = false;
                app.ScreenUpdating = false;
                app.DisplayAlerts = false;

                string execPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);

                book = app.Workbooks.Open(@filename, Missing.Value, Missing.Value, Missing.Value
                                                  , Missing.Value, Missing.Value, Missing.Value, Missing.Value
                                                 , Missing.Value, Missing.Value, Missing.Value, Missing.Value
                                                , Missing.Value, Missing.Value, Missing.Value);
                foreach (Worksheet sheet in book.Worksheets) {
                    if (sheet.Name.ToLower().Equals("registrant") || sheet.Name.ToLower().Equals("student") || sheet.Name.ToLower().Equals("employee") ||
                            sheet.Name.ToLower().Equals("questions") || sheet.Name.ToLower().Equals("answers") || sheet.Name.ToLower().Equals("choices"))
                    {
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
                        for (i = 2; i <= values.GetLength(0); i++)
                        {
                            data = "";

                            if (values[i, 1] != null)
                            {
                                if (values.GetLength(1) > 0)
                                    data += "'" + values[i, 1] + "'";

                                for (j = 2; j <= values.GetLength(1); j++)
                                    data += ", '" + values[i, j] + "'";

                                if (sheetNum == 0)
                                    sqlClient.Insert("registrant", columns, data);
                                else if (sheetNum == 1)
                                    sqlClient.Insert("student", columns, data);
                                else if (sheetNum == 2)
                                    sqlClient.Insert("employee", columns, data);
                                else if (sheetNum == 3)
                                    sqlClient.Insert("questions", columns, data);
                                else if (sheetNum == 4)
                                    sqlClient.Insert("answers", columns, data);
                                else if (sheetNum == 5)
                                    sqlClient.Insert("choices", columns, data);
                            }
                            else
                                break;
                        }
                        sheetNum++;
                    }
                }

                MessageBox.Show("File was successfully uploaded.");
            }
            catch (Exception) {
                MessageBox.Show("File failed to upload.");
            }
            finally {
                range = null;
                if (book != null)
                    book.Close(false, Missing.Value, Missing.Value);
                book = null;
                if (app != null)
                    app.Quit();
                app = null;
            }

            
        }

        /// <summary>
        /// Exports database entries to file.
        /// </summary>
        /// <param name="filename">Filename to export to</param>
        public void ExportExcel(string filename) {
            sqlClient.ExportDatabaseEntries(filename);
        }

        #endregion
        //===========================================================================
    }
}
