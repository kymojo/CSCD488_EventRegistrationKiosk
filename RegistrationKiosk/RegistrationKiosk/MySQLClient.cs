using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace RegistrationKiosk
{

    //Don't forget to add the MySQL.Data dll to your projects references
    //It can be downloaded for free from MySQL's official website.
    //Link to the .NET Connector (MS Installer) http://dev.mysql.com/downloads/connector/net/


    class MySQLClient
    {
        MySqlConnection conn = null;
        MySqlCommand cmd;


        #region Constructors
        public MySQLClient(string hostname, string database, string username, string password)
        {
            conn = new MySqlConnection("host=" + hostname + ";database=" + database +";username=" + username +";password=" + password +";");
        }

        public MySQLClient(string hostname, string database, string username, string password, int portNumber)
        {
            conn = new MySqlConnection("host=" + hostname + ";database=" + database + ";username=" + username + ";password=" + password + ";port=" + portNumber.ToString() +";");
        }

        public MySQLClient(string hostname, string database, string username, string password, int portNumber, int connectionTimeout)
        {
            conn = new MySqlConnection("host=" + hostname + ";database=" + database + ";username=" + username + ";password=" + password + ";port=" + portNumber.ToString() + ";Connection Timeout=" + connectionTimeout.ToString() +";");
        }
        #endregion

        #region Open/Close Connection
        private bool Open()
        {
            //This opens temporary connection
            try
            {
                conn.Open();
                return true;
            }
            catch
            {
                //Here you could add a message box or something like that so you know if there were an error.
                Console.WriteLine("Failed to open");
                return false;
            }
        }

        private bool Close()
        {
            //This method closes the open connection
            try
            {
                conn.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        public void Insert(string table, string column, string value)
        {
            //Insert values into the database.

            //Example: INSERT INTO names (name, age) VALUES('John Smith', '33')
            //Code: MySQLClient.Insert("names", "name, age", "'John Smith, '33'");
            string query = "INSERT INTO " + table + " (" + column + ") VALUES(" + value + ")";

            try
            {
                if (this.Open())
                {
                    //Opens a connection, if succefull; run the query and then close the connection.

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.ExecuteNonQuery();
                    this.Close();
                }
            }
            catch { }
            return;
        }

        public void InsertRegistrant(RegistrantEntry registrant) {

            if (registrant == null)
                return;

            string generalQuery, value, specialQuery = null;

            value =  "'" + registrant.code + "', ";
            value += "'" + registrant.fname + "', ";
            value += "'" + registrant.lname + "', ";
            value += "'" + registrant.phone + "', ";
            value += "'" + registrant.email + "', ";
            value += "'" + registrant.sex.ToString() + "', ";
            value += "'" + registrant.regType.ToString() + "'";
            generalQuery = "INSERT INTO registrant (Code, Fname, Lname, Phone, Email, Sex, RegType) VALUES(" + value + ");";

            if (registrant.regType == RegistrantEntry.RegistrantType.Student) {
                value = "'" + registrant.code + "', ";
                value += "'" + registrant.gradYear + "', ";
                value += "'" + registrant.studentID + "', ";
                value += "'" + registrant.major + "', ";
                value += "'" + registrant.college + "', ";
                value += "'" + registrant.classStanding.ToString() + "'";
                specialQuery = "INSERT INTO student (Code, Graduation, StudentID, Major, College, ClassStanding) VALUES(" + value + ");";
            } else if (registrant.regType == RegistrantEntry.RegistrantType.Employee) {
                value = "'" + registrant.code + "', ";
                value += "'" + registrant.business + "', ";
                value += "'" + registrant.job + "'";
                specialQuery = "INSERT INTO employee (Code, Business, Job) VALUES(" + value + ");";
            }

            try {
                if (this.Open()) {
                    //Opens a connection, if succefull; run the query and then close the connection.

                    MySqlCommand cmd = new MySqlCommand(generalQuery, conn);
                    cmd.ExecuteNonQuery();

                    if (specialQuery != null) {
                        cmd = new MySqlCommand(specialQuery, conn);
                        cmd.ExecuteNonQuery();
                    }
                        
                    this.Close();
                }
            } catch { }
            return;
        }

        public void UpdateRegistrant(int code, RegistrantEntry registrant) {

            string SET;
            string query;
            if (this.Open())
            {
                MySqlCommand cmd;
                try
                {
                    SET = "";
                    SET += "Fname = '" + registrant.fname + "', ";
                    SET += "Lname = '" + registrant.lname + "', ";
                    SET += "Phone = '" + registrant.phone + "', ";
                    SET += "Email = '" + registrant.email + "', ";
                    SET += "Sex = '" + registrant.sex.ToString() + "', ";
                    SET += "RegType = '" + registrant.regType.ToString() + "'";
                    query = "UPDATE registrant SET " + SET + " WHERE Code = " + code + ";";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    if (registrant.regType == RegistrantEntry.RegistrantType.Student) {

                        SET = "";
                        SET += "Graduation = '" + registrant.gradYear + "', ";
                        SET += "StudentID = '" + registrant.studentID + "', ";
                        SET += "Major = '" + registrant.major + "', ";
                        SET += "College = '" + registrant.college + "', ";
                        SET += "ClassStanding = '" + registrant.classStanding.ToString() + "'";
                        query = "UPDATE student SET " + SET + " WHERE Code = " + code + ";";
                        cmd = new MySqlCommand(query, conn);
                        cmd.ExecuteNonQuery();

                    } else if (registrant.regType == RegistrantEntry.RegistrantType.Employee) {

                        SET = "";
                        SET += "Business = '" + registrant.business + "', ";
                        SET += "Job = '" + registrant.job + "'";
                        query = "UPDATE employee SET " + SET + " WHERE Code = " + code + ";";
                        cmd = new MySqlCommand(query, conn);
                        cmd.ExecuteNonQuery();

                    }

                    this.Close();
                }
                catch { this.Close(); }
            }
            return;

        }

        public void Update(string table, string SET, string WHERE)
        {
            //Update existing values in the database.

            //Example: UPDATE names SET name='Joe', age='22' WHERE name='John Smith'
            //Code: MySQLClient.Update("names", "name='Joe', age='22'", "name='John Smith'");
            string query = "UPDATE " + table + " SET " + SET + " WHERE " + WHERE + "";

            if (this.Open())
            {
                try
                {
                    //Opens a connection, if succefull; run the query and then close the connection.

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    this.Close();
                }
                catch { this.Close(); }
            }
            return;
        }

        public void DeleteRegistrant(int code) {
            string query;
            MySqlCommand cmd;

            if (this.Open()) {
                try {
                    //Opens a connection, if succefull; run the query and then close the connection.
                    query = "DELETE FROM registrant WHERE Code = " + code + ";";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    query = "DELETE FROM student WHERE Code = " + code + ";";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    query = "DELETE FROM employee WHERE Code = " + code + ";";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    this.Close();
                } catch { this.Close(); }
            }
            return;
        }

        public void Delete(string table, string WHERE) 
        {
            //Removes an entry from the database.

            //Example: DELETE FROM names WHERE name='John Smith'
            //Code: MySQLClient.Delete("names", "name='John Smith'");
            string query = "DELETE FROM " + table + " WHERE " + WHERE + "";

            if (this.Open())
            {
                try
                {
                    //Opens a connection, if succefull; run the query and then close the connection.

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    this.Close();
                }
                catch { this.Close(); }
            }
            return;
        }

        public List<RegistrantEntry> SelectRegistrant(string WHERE)
        {
            //This methods selects from the database, it retrieves data from it.
            //You must make a dictionary to use this since it both saves the column
            //and the value. i.e. "age" and "33" so you can easily search for values.

            //Example: SELECT * FROM names WHERE name='John Smith'
            // This example would retrieve all data about the entry with the name "John Smith"

            //Code = Dictionary<string, string> myDictionary = Select("names", "name='John Smith'");
            //This code creates a dictionary and fills it with info from the database.

            //string query = "SELECT * FROM registrant WHERE " + WHERE + ";";
            
            RegistrantEntry registrant;
            List<RegistrantEntry> regList = new List<RegistrantEntry>();
            MySqlCommand cmd;
            MySqlDataReader dataReader;

            if (this.Open())
            {
                try
                {
                    // GENERAL REGISTRANT INFO
                    string query = "SELECT * FROM registrant WHERE " + WHERE + ";";
                    cmd = new MySqlCommand(query, conn);
                    dataReader = cmd.ExecuteReader();
                    while (dataReader.Read())
                    {
                        registrant = new RegistrantEntry();
                        registrant.code = (int)dataReader[0];
                        registrant.fname = (string)dataReader[1];
                        registrant.lname = (string)dataReader[2];
                        registrant.phone = (string)dataReader[3];
                        registrant.email = (string)dataReader[4];
                        registrant.sex = (RegistrantEntry.Sex) Enum.Parse(typeof(RegistrantEntry.Sex), (string)dataReader[5]);
                        // Registered [6]
                        // Time [7]
                        registrant.regType = (RegistrantEntry.RegistrantType)Enum.Parse(typeof(RegistrantEntry.RegistrantType), (string)dataReader[8]);

                        regList.Add(registrant);
                    }
                    dataReader.Close();

                    // STUDENT INFO
                    query = "SELECT * FROM student WHERE " + WHERE + ";";
                    cmd = new MySqlCommand(query, conn);
                    dataReader = cmd.ExecuteReader();
                    while (dataReader.Read()) {
                        int code = (int)dataReader[0];
                        int index = regList.FindIndex(reg => reg.code == code);
                        if (index != -1) {
                            regList[index].gradYear = (int)dataReader[1];
                            regList[index].studentID = ((int)dataReader[2]).ToString();
                            regList[index].major = (string)dataReader[3];
                            regList[index].college = (string)dataReader[4];
                            regList[index].classStanding = (RegistrantEntry.ClassStanding)Enum.Parse(typeof(RegistrantEntry.ClassStanding), (string)dataReader[5]);
                        }
                    }
                    dataReader.Close();

                    // EMPLOYEE INFO
                    query = "SELECT * FROM employee WHERE " + WHERE + ";";
                    cmd = new MySqlCommand(query, conn);
                    dataReader = cmd.ExecuteReader();
                    while (dataReader.Read()) {
                        int code = (int)dataReader[0];
                        int index = regList.FindIndex(reg => reg.code == code);
                        if (index != -1) {
                            regList[index].business = (string)dataReader[1];
                            regList[index].job = (string)dataReader[2];
                        }
                    }
                    dataReader.Close();
                    
                }
                catch { }
                this.Close();
            }
            return regList;
        }

        public int Count(string table)
        {
            //This counts the numbers of entries in a table and returns it as an integear

            //Example: SELECT Count(*) FROM names
            //Code: int myInt = MySQLClient.Count("names");

            string query = "SELECT Count(*) FROM " + table + "";
            int Count = -1;
            if (this.Open() == true)
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    Count = int.Parse(cmd.ExecuteScalar() + "");
                    this.Close();
                }
                catch { this.Close(); }
                return Count;
            }
            else
            {
                return Count;
            }
        }

        public void createEvent(string dbname)
        {
            string query;

            try
            {
                if (this.Open())
                {
                    query = @"CREATE TABLE IF NOT EXISTS `" + dbname + "_employees` (" +
                            "`code` INT," +
                            "`businessname` TEXT," +
                            "`jobtitle` TEXT, " +
                            "PRIMARY KEY(code))";

                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    query = @"CREATE TABLE IF NOT EXISTS `" + dbname + "_registrants` (" +
                            "`code` INT," +
                            "`fname` TEXT," +
                            "`lname` TEXT," +
                            "`email` TEXT," +
                            "`phone` VARCHAR(11)," +
                            "`sex` VARCHAR(1)," +
                            "`type` TEXT, " +
                            "PRIMARY KEY(code))";

                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    query = @"CREATE TABLE IF NOT EXISTS `" + dbname + "_students` (" +
                            "`code` INT," +
                            "`studentid` INT," +
                            "`college` TEXT," +
                            "`major` TEXT," +
                            "`gyear` int," +
                            "`standing` TEXT, " +
                            "PRIMARY KEY(code))";

                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    this.Close();
                }
            }
            catch 
            {
                
            }
        }

        public void dropEvent(string dbname)
        {
            string query;

            try
            {
                if (this.Open())
                {
                    query = @"DROP TABLE IF EXISTS " + dbname + "_employees";

                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    query = @"DROP TABLE IF EXISTS " + dbname + "_registrants";

                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    query = @"DROP TABLE IF EXISTS " + dbname + "_students";

                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    this.Close();
                }
            }
            catch
            {

            }
        }
    }
}
