using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySQLClass;

namespace Database1
{
    public partial class Test : Form
    {
        //Start new SQLClient class with the information of our database
        MySQLClient sqlClient = new MySQLClient("www.cscd379.com", "ExcelImport", "jobfair", "ewu2015");
        public Test()
        {
            InitializeComponent();
        }

        //Test simple insert
        private void Insert_Click(object sender, EventArgs e)
        {
            sqlClient.Insert("excel", "ID, FirstName, LastName, School, Major", "'0', 'Zhenyu' , 'Xia', 'EWU', 'CS'");
        }

        //Test simple query
        private void Query_Click(object sender, EventArgs e)
        {
            Dictionary<String, String> testData = sqlClient.Select("excel","FirstName = 'Zhenyu'");
            //for (var item : testData)
            {
                
             dataGridView1.DataSource = (from entry in testData
                            orderby entry.Key
                            select new{entry.Key,entry.Value}).ToList();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
