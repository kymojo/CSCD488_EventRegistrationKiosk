using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Odbc;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack == false)
        {
            //  Local...
           //string conString = "DRIVER={MySQL ODBC 5.2 ANSI Driver}; SERVER=localhost; PORT=3306;DATABASE=salestransactions; USER=root; OPTION=0;";

            //  Arvixe...
            string conString = "DRIVER={MySQL ODBC 5.1 Driver}; SERVER=localhost; PORT=3306;DATABASE=SalesOrders; USER=jobfair; PASSWORD=ewu2015; OPTION=3;";

            string sCommand = "";
            string sWork = "";
            string custNo = "";
            string custName = "";
            string street = "";
            string city = "";
            string state = "";
            string zip = "";

            try
            {
                OdbcConnection cn = new OdbcConnection(conString);

                cn.Open();

                Label1.Text = " DB: " + cn.Database;
                Label1.Text += " DS: " + cn.DataSource;
                Label1.Text += " Driver: " + cn.Driver;
                Label1.Text += " -: " + cn.ToString();

                OdbcCommand cmd = new OdbcCommand(sCommand, cn);

                sCommand = "SELECT * FROM customer;";
                cmd.CommandText = sCommand;

                OdbcDataReader dr;
                
                dr = cmd.ExecuteReader();
                GridView1.DataSource = dr;
                GridView1.DataBind();
                while (dr.Read() == true)
                {

                    custNo = dr.GetValue(0).ToString();
                    custName = dr.GetValue(1).ToString();
                    street = dr.GetValue(2).ToString();
                    city = dr.GetValue(3).ToString();
                    state = dr.GetValue(4).ToString();
                    zip = dr.GetValue(5).ToString();

                    sWork = custNo + " " + custName + " " + street + " " + city + ", " + state + " " + zip;
                    lbCustomer.Items.Add(sWork);
                }

                cn.Close();

                cn = null;

            }
            catch (Exception err)
            {
                this.Label1.Text = "Error:  " + err.Message;
            }

        }


    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        string conString = "DRIVER={MySQL ODBC 5.1 Driver}; SERVER=localhost; PORT=3306;DATABASE=SalesOrders; USER=jobfair; PASSWORD=ewu2015; OPTION=3;";
        OdbcConnection cn;

        string sqlstr;
        sqlstr = "INSERT INTO Customer VALUES (";
        sqlstr += "?, ";
        sqlstr += "?, ";
        sqlstr += "?, ";
        sqlstr += "?, ";
        sqlstr += "?, ";
        sqlstr += "?)";

        try
        {

            cn = new OdbcConnection(conString);

            cn.Open();

            OdbcCommand cmd = cn.CreateCommand();
            //cmd.Connection = cn;
            cmd.CommandText = sqlstr;

            cmd.Parameters.AddWithValue("CustNum", txtCustNum.Text);
            cmd.Parameters.AddWithValue("CustName", txtName.Text);
            cmd.Parameters.AddWithValue("Street", txtStreet.Text);
            cmd.Parameters.AddWithValue("City", txtCity.Text);
            cmd.Parameters.AddWithValue("State", txtState.Text);
            cmd.Parameters.AddWithValue("Zip", txtZip.Text);

            string temp = "insert into `customer`(`CustNum`,`CustName`,`Street`,`City`,`State`,`Zip`) values (503,'Fender','456 Convoy St.','Fullerton','CA','96001');";
            //OdbcCommand cmd = new OdbcCommand(temp, cn);
            cmd.ExecuteNonQuery();

            cn.Close();
            cn = null;
        }
        catch (Exception err)
        {
            Label1.Text = err.Message;
        }

    }
}