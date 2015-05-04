using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Windows.Forms;

namespace DYMO
{
    class IOExcel
    {
        
        public IOExcel()
        {
        }

        public string selectFile()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Setting the filter options
            openFileDialog1.Filter = "Microsoft Excel Worksheet (.xlsx)|*.xlsx|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;

            // Check if user selected file or not
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                return openFileDialog1.FileName;
            else
                return null;
        }

        public void importExcel(string fileName)
        {
            string con = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0 xml;HDR=Yes;'";
            using (OleDbConnection connection = new OleDbConnection(con))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from [Sheet1$]", connection);
                using (OleDbDataReader dr = command.ExecuteReader())
                {
                    dr.Read();


                    while (dr.Read())
                    {
                        //var row1Col0 = dr[0];
                        Console.WriteLine(dr[0] + " " + dr[1] + " " + dr[2] + " " + dr[3] + " " + dr[4]);
                    }
                }
            }
        }
    }
}
