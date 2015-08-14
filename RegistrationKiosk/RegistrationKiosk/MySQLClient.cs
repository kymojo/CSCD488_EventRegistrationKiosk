using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace RegistrationKiosk {

    public class MySQLClient {

        //===========================================================================
        #region Class Variables
        //===========================================================================
        
        MySqlConnection conn = null;
        MySqlCommand cmd;
        SecurityMeans security;

        public string hostname { get; set; }
        public string database { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public int portNumber { get; set; }

        #endregion
        //===========================================================================
        #region Class Constructors
        //===========================================================================

        /// <summary>
        /// Creates an instance of MySQLClient without opening a connection.
        /// </summary>
        public MySQLClient(SecurityMeans security) {
            this.security = security;
            hostname = security.DbHost;
            database = security.DbName;
            username = security.DbUser;
            password = security.DbPass;
            portNumber = security.DbPort;
            Connect();

            // Check connection (notify if failed)
            if (!IsConnected())
                MessageBox.Show("Could not connect to database.");
        }

        /// <summary>
        /// Creates an instance of MySQLClient and opens a connection.
        /// </summary>
        /// <param name="hostname">Host name</param>
        /// <param name="database">Database name</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        public MySQLClient(string hostname, string database, string username, string password) {
            SetConnection(hostname, database, username, password, 3306);
            Connect();
        }

        /// <summary>
        /// Creates an instance of MySQLClient and opens a connection.
        /// </summary>
        /// <param name="hostname">Host name</param>
        /// <param name="database">Database name</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="portNumber">Port number</param>
        public MySQLClient(string hostname, string database, string username, string password, int portNumber) {
            SetConnection(hostname, database, username, password, portNumber);
            Connect();
        }

        #endregion
        //===========================================================================
        #region Class Methods
        //===========================================================================

        //---------------------------------------------------------------------------
        #region OPEN/CLOSE CONNECTION
        //---------------------------------------------------------------------------
        
        /// <summary>
        /// Open the connection to database
        /// </summary>
        /// <returns>Connection success flag</returns>
        private bool Open() {
            //This opens temporary connection
            try {
                conn.Open();
                return true;
            }
            catch (Exception e) {
                //Here you could add a message box or something like that so you know if there were an error.
                Console.WriteLine("Failed to open");
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Closes the connection to database
        /// </summary>
        /// <returns>Close success flag</returns>
        private bool Close() {
            //This method closes the open connection
            try {
                conn.Close();
                return true;
            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// Sets values for database connection.
        /// </summary>
        /// <param name="hostname">Host name</param>
        /// <param name="database">Database name</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="portNumber">Port number</param>
        public void SetConnection(string hostname, string database, string username, string password, int portNumber) {
            this.hostname = hostname;
            this.database = database;
            this.username = username;
            this.password = password;
            this.portNumber = portNumber;
        }

        /// <summary>
        /// Creates a new connection using connection parameters (set using SetConnection).
        /// </summary>
        /// <returns>Success flag</returns>
        public bool Connect() {
            try {
                Close();
                conn = new MySqlConnection("host=" + hostname + ";database=" + database +
                                       ";username=" + username + ";password=" + password +
                                       ";port=" + portNumber.ToString() + ";");
                if (!Open())
                    throw new Exception();
                Close();
                return true;
            } catch {
                Close();
                conn = null;
                return false;
            }
        }

        /// <summary>
        /// Returns whether or not the database is connected.
        /// </summary>
        /// <returns>IsConnected flag</returns>
        public bool IsConnected() {
            if (conn == null)
                return false;
            return true;
        }

        #endregion
        //---------------------------------------------------------------------------
        #region DATABASE
        //---------------------------------------------------------------------------

        // -------------------------
        #region INSERT
        // -------------------------

        /// <summary>
        /// Insert values into the database.
        /// </summary>
        /// <param name="table">Name of db table</param>
        /// <param name="column">Columns to write to</param>
        /// <param name="value">Values of columns</param>
        public void Insert(string table, string column, string value)
        {
            //Example: INSERT INTO names (name, age) VALUES('John Smith', '33')
            //Code: MySQLClient.Insert("names", "name, age", "'John Smith, '33'");
            string query = "INSERT IGNORE INTO " + table + " (" + column + ") VALUES(" + value + ")";

            try {
                if (this.Open()) {
                    //Opens a connection, if succefull; run the query and then close the connection.

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.ExecuteNonQuery();
                    this.Close();
                }
            } catch {
                MessageBox.Show("Error adding registrant to database!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            return;
        }

        /// <summary>
        /// Insert questions into the database. Clear beforehand to avoid collisions.
        /// </summary>
        /// <param name="questions"></param>
        public void InsertQuestions(List<QuestionEntry> questions) {
            
            if (questions == null) {
                return;
            }

            try {
                MySqlCommand cmd;
                if (this.Open()) {
                    int i = 0, ii, choiceCount, questionsCount = questions.Count;
                    string questionValues, query, choiceValues;
                    while (i < questionsCount) {
                        // The Question
                        questionValues = "'" + (i + 1) + "', '" + questions[i].questionText.Replace("'","''") + "'";
                        query = "INSERT IGNORE INTO questions (questionID, question) VALUES(" + questionValues + ");";
                        cmd = new MySqlCommand(query, conn);
                        cmd.ExecuteNonQuery();
                        // Its Choices
                        choiceCount = questions[i].GetChoiceCount();
                        ii = 0;
                        while (ii < choiceCount) {
                            choiceValues = "'" + (i + 1) + "', '" + questions[i].GetChoiceAt(ii).choiceText.Replace("'", "''") + "'";
                            query = "INSERT IGNORE INTO choices (questionID, answer) VALUES(" + choiceValues + ");";
                            cmd = new MySqlCommand(query, conn);
                            cmd.ExecuteNonQuery();
                            ii++;
                        }
                        i++;
                    }
                    this.Close();
                }
            } catch {
                MessageBox.Show("Error adding registrant to database!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            return;
        }

        /// <summary>
        /// Inserts a new registrant into the database from RegistrantEntry object
        /// </summary>
        /// <param name="registrant">RegistrantEntry to insert</param>
        public void InsertRegistrant(RegistrantEntry registrant) {

            // If the registrant is null, return.
            if (registrant == null)
                return;

            string generalQuery, value, specialQuery = null;

            #region Initialize General Query
            // =========================
            value =  "'" + registrant.Code + "', ";
            value += "'" + registrant.Fname + "', ";
            value += "'" + registrant.Lname + "', ";
            value += "'" + registrant.Phone + "', ";
            value += "'" + registrant.Email + "', ";
            value += "'" + registrant.Sex.ToString() + "', ";
            value += "'" + registrant.RegType.ToString() + "', ";
            value += "'Yes'";
            generalQuery = "INSERT IGNORE INTO registrant (Code, Fname, Lname, Phone, Email, Sex, RegType, CheckedIn) VALUES(" + value + ");";
            // =========================
            #endregion

            if (registrant.RegType == RegistrantEntry.RegistrantType.Student) {
                #region Initialize Special Query for Student
                // =========================
                value = "'" + registrant.Code + "', ";
                value += "'" + registrant.GradYear + "', ";
                value += "'" + registrant.StudentID + "', ";
                value += "'" + registrant.Major + "', ";
                value += "'" + registrant.College + "', ";
                value += "'" + registrant.ClassStanding.ToString() + "'";
                specialQuery = "INSERT IGNORE INTO student (Code, Graduation, StudentID, Major, College, ClassStanding) VALUES(" + value + ");";
                // =========================
                #endregion
            } else if (registrant.RegType == RegistrantEntry.RegistrantType.Employee) {
                #region Initialize Special Query for Employee
                // =========================
                value = "'" + registrant.Code + "', ";
                value += "'" + registrant.Business + "', ";
                value += "'" + registrant.Job + "'";
                specialQuery = "INSERT IGNORE INTO employee (Code, Business, Job) VALUES(" + value + ");";
                // =========================
                #endregion
            }

            try {
                if (this.Open()) {
                    //Opens a connection, if successful; run the query and then close the connection.

                    // Execute general query
                    MySqlCommand cmd = new MySqlCommand(generalQuery, conn);
                    cmd.ExecuteNonQuery();

                    // If special query initialized, execute special query
                    if (specialQuery != null) {
                        cmd = new MySqlCommand(specialQuery, conn);
                        cmd.ExecuteNonQuery();
                    }
                        
                    this.Close();
                }
            } catch {
                MessageBox.Show("Error adding registrant to database!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            return;
        }
        
        #endregion
        // -------------------------
        #region UPDATE
        // -------------------------

        /// <summary>
        /// Update existing values in the database.
        /// </summary>
        /// <param name="table">The table containing the entry to update</param>
        /// <param name="SET">Set string (eg. name = 'Joe')</param>
        /// <param name="WHERE">Where string (eg. code = 123)</param>
        public void Update(string table, string SET, string WHERE) {
            
            //Example: UPDATE names SET name='Joe', age='22' WHERE name='John Smith'
            //Code: MySQLClient.Update("names", "name='Joe', age='22'", "name='John Smith'");
            string query = "UPDATE " + table + " SET " + SET + " WHERE " + WHERE + "";

            if (this.Open()) {
                try {
                    //Opens a connection, if succefull; run the query and then close the connection.

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    this.Close();
                }
                catch {
                    MessageBox.Show("Unable to update entry in database!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            return;
        }

        /// <summary>
        /// Updates a registrant entry in the database.
        /// </summary>
        /// <param name="code">The entry code to update entry at</param>
        /// <param name="registrant">Registrant data to use for update</param>
        public void UpdateRegistrant(string code, RegistrantEntry registrant) {

            string SET;
            string query;
            if (this.Open()) {
                MySqlCommand cmd;
                try {
                    // Update registrant Table
                    #region Set General SET Query
                    // =========================
                    SET = "";
                    SET += "Fname = '" + registrant.Fname + "', ";
                    SET += "Lname = '" + registrant.Lname + "', ";
                    SET += "Phone = '" + registrant.Phone + "', ";
                    SET += "Email = '" + registrant.Email + "', ";
                    SET += "Sex = '" + registrant.Sex.ToString() + "', ";
                    SET += "RegType = '" + registrant.RegType.ToString() + "', ";
                    SET += "CheckedIn = 'Yes'";
                    query = "UPDATE registrant SET " + SET + " WHERE Code = '" + code + "';";
                    // =========================
                    #endregion
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    if (registrant.RegType == RegistrantEntry.RegistrantType.Student) {
                        // Update student Table
                        #region Set Student SET Query
                        // =========================
                        SET = "";
                        SET += "Graduation = '" + registrant.GradYear + "', ";
                        SET += "StudentID = '" + registrant.StudentID + "', ";
                        SET += "Major = '" + registrant.Major + "', ";
                        SET += "College = '" + registrant.College + "', ";
                        SET += "ClassStanding = '" + registrant.ClassStanding.ToString() + "'";
                        query = "UPDATE student SET " + SET + " WHERE Code = '" + code + "';";
                        // =========================
                        #endregion
                        cmd = new MySqlCommand(query, conn);
                        cmd.ExecuteNonQuery();

                    } else if (registrant.RegType == RegistrantEntry.RegistrantType.Employee) {
                        // Update employee Table
                        #region Set Employee SET Query
                        // =========================
                        SET = "";
                        SET += "Business = '" + registrant.Business + "', ";
                        SET += "Job = '" + registrant.Job + "'";
                        query = "UPDATE employee SET " + SET + " WHERE Code = '" + code + "';";
                        // =========================
                        #endregion
                        cmd = new MySqlCommand(query, conn);
                        cmd.ExecuteNonQuery();

                    }

                    this.Close();

                } catch {
                    MessageBox.Show("Unable to update registrant in database!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            return;

        }

        #endregion
        // -------------------------
        #region DELETE
        // -------------------------

        /// <summary>
        /// Removes an entry from the database.
        /// </summary>
        /// <param name="table">The table containing the entries</param>
        /// <param name="WHERE">Where string (eg. code = 123)</param>
        public void Delete(string table, string WHERE) {

            //Example: DELETE FROM names WHERE name='John Smith'
            //Code: MySQLClient.Delete("names", "name='John Smith'");
            string query = "DELETE FROM " + table + " WHERE " + WHERE + "";

            if (this.Open()) {
                try {
                    //Opens a connection, if succefull; run the query and then close the connection.

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    this.Close();
                }
                catch {
                    MessageBox.Show("Unable to delete entry from database!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            return;
        }

        /// <summary>
        /// Deletes all questions and choices in database
        /// </summary>
        /// <param name="id"></param>
        public void DeleteQuestions() {
            string query;
            MySqlCommand cmd;

            if (this.Open()) {
                try {
                    // Delete all questions
                    query = "DELETE FROM questions;";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    // Delete all question choices
                    query = "DELETE FROM choices;";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    this.Close();
                } catch {
                    MessageBox.Show("Unable to delete question from database!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            return;
        }

        /// <summary>
        /// Deletes all registrant question responses in database.
        /// </summary>
        public void DeleteAnswers() {
            string query;
            MySqlCommand cmd;

            if (this.Open()) {
                try {
                    // Delete all answers
                    query = "DELETE FROM answers;";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    this.Close();
                } catch {
                    MessageBox.Show("Unable to delete answer from database!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            return;
        }
        
        /// <summary>
        /// Delete a registrant entry from database.
        /// </summary>
        /// <param name="code">The entry code of entry to delete</param>
        public void DeleteRegistrant(int code) {
            string query;
            MySqlCommand cmd;

            if (this.Open()) {
                try {
                    // Delete Registrant From registrant Table
                    query = "DELETE FROM registrant WHERE Code = '" + code + "';";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    // Delete Registrant From student Table
                    query = "DELETE FROM student WHERE Code = '" + code + "';";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    // Delete Registrant From employee Table
                    query = "DELETE FROM employee WHERE Code = '" + code + "';";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    this.Close();

                } catch {
                    MessageBox.Show("Unable to delete registrant from database!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            return;
        }

        #endregion
        // -------------------------
        #region SELECT
        // -------------------------

        /// <summary>
        /// Retrieves a list of RegistrantEntries from database.
        /// </summary>
        /// <param name="WHERE">Where string (eg. name = "Bob")</param>
        /// <returns></returns>
        public List<RegistrantEntry> SelectRegistrant(string WHERE)
        {   
            RegistrantEntry registrant;
            List<RegistrantEntry> regList = new List<RegistrantEntry>();
            MySqlCommand cmd;
            MySqlDataReader dataReader;

            if (this.Open()) {
                try {
                    // GENERAL REGISTRANT INFO
                    string query = "SELECT * FROM registrant WHERE " + WHERE + ";";
                    cmd = new MySqlCommand(query, conn);
                    dataReader = cmd.ExecuteReader();
                    while (dataReader.Read())
                    {
                        registrant = new RegistrantEntry();
                        #region Set registrant Properties
                        // =========================
                        registrant.Code = (string)dataReader[0];
                        registrant.Fname = (string)dataReader[1];
                        registrant.Lname = (string)dataReader[2];
                        registrant.Phone = (string)dataReader[3];
                        registrant.Email = (string)dataReader[4];
                        registrant.Sex = (RegistrantEntry.SexType) Enum.Parse(typeof(RegistrantEntry.SexType), (string)dataReader[5]);
                        registrant.RegType = (RegistrantEntry.RegistrantType)Enum.Parse(typeof(RegistrantEntry.RegistrantType), (string)dataReader[6]);
                        // CheckedIn [7]
                        // =========================
                        #endregion
                        regList.Add(registrant);
                    }
                    dataReader.Close();
                    
                    foreach (RegistrantEntry reg_get in regList) {
                        // STUDENT INFO
                        query = "SELECT * FROM student WHERE Code = '" + reg_get.Code + "';";
                        cmd = new MySqlCommand(query, conn);
                        dataReader = cmd.ExecuteReader();
                        while (dataReader.Read()) {
                            #region Set registrant Properties for Students
                            // =========================
                            reg_get.GradYear = (int)dataReader[1];
                            reg_get.StudentID = (string)dataReader[2];
                            reg_get.Major = (string)dataReader[3];
                            reg_get.College = (string)dataReader[4];
                            reg_get.ClassStanding = (RegistrantEntry.ClassStandingType)Enum.Parse(typeof(RegistrantEntry.ClassStandingType), (string)dataReader[5]);
                            // =========================
                            #endregion
                        }
                        dataReader.Close();

                        // EMPLOYEE INFO
                        query = "SELECT * FROM employee WHERE Code = '" + reg_get.Code + "';";
                        cmd = new MySqlCommand(query, conn);
                        dataReader = cmd.ExecuteReader();
                        while (dataReader.Read()) {
                            #region Set registrant Properties for Employees
                            // =========================
                            reg_get.Business = (string)dataReader[1];
                            reg_get.Job = (string)dataReader[2];
                            // =========================
                            #endregion
                        }
                        dataReader.Close();
                    }
                    
                    
                } catch {
                    MessageBox.Show("Error retrieving student from database!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.Close();
            }
            return regList;
        }

        /// <summary>
        /// Grabs all the questions and their choices from the database.
        /// </summary>
        /// <returns></returns>
        public List<QuestionEntry> SelectQuestions() {

            QuestionEntry question;
            List<QuestionEntry> result = new List<QuestionEntry>();
            MySqlCommand cmd;
            MySqlDataReader dataReader;

            if (this.Open()) {
                try {
                    // GET QUESTIONS
                    string query = "SELECT * FROM questions ORDER BY questionID;";
                    cmd = new MySqlCommand(query, conn);
                    dataReader = cmd.ExecuteReader();
                    while (dataReader.Read())
                    {
                        question = new QuestionEntry((string)dataReader[1]);
                        result.Add(question);
                    }
                    dataReader.Close();

                    // GET CHOICES (expects question id's to be sequential, starting with 1)
                    query = "SELECT * FROM choices ORDER BY questionID;";
                    cmd = new MySqlCommand(query, conn);
                    dataReader = cmd.ExecuteReader();
                    int i;
                    while (dataReader.Read()) {
                        i = (int)dataReader[0]-1;
                        if (result[i] != null)
                            result[i].AddNewChoice((string)dataReader[1]);
                    }
                    dataReader.Close();
                    this.Close();
                } catch {
                    MessageBox.Show("Error grabbing questions!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    result = null;
                }
            }
            return result;
        }

        #endregion
        // -------------------------
        #region OTHER
        // -------------------------

        /// <summary>
        /// Counts the number of entries in the given table.
        /// </summary>
        /// <param name="table">Table to count from</param>
        /// <returns>Number of entries in table</returns>
        public int Count(string table) {
            //This counts the numbers of entries in a table and returns it as an integear

            //Example: SELECT Count(*) FROM names
            //Code: int myInt = MySQLClient.Count("names");

            string query = "SELECT Count(*) FROM " + table + "";
            int Count = -1;
            if (this.Open()) {
                try {
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    Count = int.Parse(cmd.ExecuteScalar() + "");
                    this.Close();
                }
                catch { this.Close(); }
                return Count;
            }
            else {
                return Count;
            }
        }

        #endregion
        // -------------------------
        #region CREATE / DROP TABLES
        // -------------------------

        /// <summary>
        /// Creates tables for new event
        /// </summary>
        /// <param name="dbname">Event name</param>
        /// /// <returns>Success flag</returns>
        public bool CreateDatabaseTables() {
            string query;

            try {
                if (this.Open()) {
                    #region Set _employee Table Query
                    // =========================
                    query = @"CREATE TABLE IF NOT EXISTS `employee` (" +
                            "`Code` VARCHAR(6), " +
                            "`Business` VARCHAR(45), " +
                            "`Job` VARCHAR(45), " +
                            "PRIMARY KEY(Code))";
                    // =========================
                    #endregion
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    #region Set _registrant Table Query
                    // =========================
                    query = @"CREATE TABLE IF NOT EXISTS `registrant` (" +
                            "`Code` VARCHAR(6), " +
                            "`Fname` VARCHAR(45), " +
                            "`Lname` VARCHAR(45), " +
                            "`Phone` VARCHAR(20), " +
                            "`Email` VARCHAR(45), " +
                            "`Sex` ENUM('Male', 'Female'), " +
                            "`RegType` ENUM('General', 'Student', 'Employee'), " +
                            "`CheckedIn` ENUM('Yes', 'No'), " +
                            "PRIMARY KEY(Code))";
                    // =========================
                    #endregion
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    #region Set _student Table Query
                    // =========================
                    query = @"CREATE TABLE IF NOT EXISTS `student` (" +
                            "`Code` VARCHAR(6), " +
                            "`Graduation` INT(4), " +
                            "`StudentID` VARCHAR(11), " +
                            "`Major` VARCHAR(45), " +
                            "`College` VARCHAR(45), " +
                            "`ClassStanding` ENUM('None', 'Freshman', 'Junior', 'Sophomore', " +
                                              "'Senior', 'PostBac', 'Graduate', 'Alumnus'), " +
                            "PRIMARY KEY(Code))";
                    // =========================
                    #endregion
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    #region Set _questions Table Query
                    // =========================
                    query = @"CREATE TABLE IF NOT EXISTS `questions` (" +
                            "`questionID` INT, " +
                            "`question` VARCHAR(64), " +
                            "PRIMARY KEY(questionID))";
                    // =========================
                    #endregion
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    #region Set _answers Table Query
                    // =========================
                    query = @"CREATE TABLE IF NOT EXISTS `answers` (" +
                            "`Code` VARCHAR(6), " +
                            "`questionID` INT, " +
                            "`answer` VARCHAR(64), " +
                            "PRIMARY KEY(Code, questionID))";
                    // =========================
                    #endregion
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    #region Set _choices Table Query
                    // =========================
                    query = @"CREATE TABLE IF NOT EXISTS `choices` (" +
                            "`questionID` INT, " +
                            "`answer` VARCHAR(64), " +
                            "PRIMARY KEY(questionID, answer))";
                    // =========================
                    #endregion
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    this.Close();
                    return true;
                }
            } catch {
                MessageBox.Show("Error Creating Tables!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            return false;
        }

        /// <summary>
        /// Deletes tables for a given event
        /// </summary>
        /// <returns>Success flag</returns>
        public bool DropDatabaseTables() {
            string query;

            try {
                if (this.Open()) {

                    query = @"DROP TABLE IF EXISTS `employee`";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    query = @"DROP TABLE IF EXISTS `registrant`";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    query = @"DROP TABLE IF EXISTS `student`";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    query = @"DROP TABLE IF EXISTS `questions`";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    query = @"DROP TABLE IF EXISTS `answers`";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    query = @"DROP TABLE IF EXISTS `choices`";
                    cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();

                    this.Close();
                    return true;
                }
            } catch {
                MessageBox.Show("Error clearing database!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            return false;
        }

        #endregion
        // -------------------------
        #region EXPORT DATABASE
        // -------------------------

        /// <summary>
        /// Exports entries from database to an Excel file
        /// </summary>
        /// <param name="filename">Filename to export to</param>
        public void ExportDatabaseEntries(string filename) {
            int sheetNum;
            string tableName = "";
            string query = "";

            if (this.Open()) {
                MySqlDataAdapter dataAdapter;
                DataSet ds = new DataSet("jobfair");
                //Creae an Excel application instance
                Excel.Application excelApp = new Excel.Application();

                //Create an Excel workbook instance and open it from the predefined location

                Excel.Workbook excelWorkBook = excelApp.Workbooks.Add(Type.Missing);

                for (sheetNum = 1; sheetNum < 7; sheetNum++) {
                    switch (sheetNum) {
                        case 1:
                            tableName = "choices";
                            break;
                        case 2:
                            tableName = "answers";
                            break;
                        case 3:
                            tableName = "questions";
                            break;
                        case 4:
                            tableName = "employee";
                            break;
                        case 5:
                            tableName = "student";
                            break;
                        case 6:
                            tableName = "registrant";
                            break;
                    }

                    query = "SELECT * FROM " + tableName;

                    dataAdapter = new MySqlDataAdapter(query, conn);
                    dataAdapter.FillSchema(ds, SchemaType.Source);
                    dataAdapter.Fill(ds, tableName);
                }

                query = "SELECT StudentID, Fname, Lname, College, Major, ClassStanding, Email, RegType, CheckedIn FROM registrant R LEFT JOIN student S ON S.Code = R.Code LEFT JOIN employee E ON E.Code = S.Code";

                dataAdapter = new MySqlDataAdapter(query, conn);
                dataAdapter.FillSchema(ds, SchemaType.Source);
                dataAdapter.Fill(ds, "view");

                foreach (DataTable table in ds.Tables) {
                    //Add a new worksheet to workbook with the Datatable name
                    Excel.Worksheet excelWorkSheet = (Excel.Worksheet)excelWorkBook.Sheets.Add();
                    excelWorkSheet.Name = table.TableName;

                    for (int i = 1; i < table.Columns.Count + 1; i++) {
                        excelWorkSheet.Cells[1, i] = table.Columns[i - 1].ColumnName;
                    }

                    for (int j = 0; j < table.Rows.Count; j++) {
                        for (int k = 0; k < table.Columns.Count; k++) {
                            excelWorkSheet.Cells[j + 2, k + 1] = table.Rows[j].ItemArray[k].ToString();
                        }
                    }
                    excelWorkSheet.Cells.Columns.AutoFit();
                }

                try {
                    Excel.Worksheet worksheet = (Excel.Worksheet)excelWorkBook.Worksheets[8];
                    excelApp.DisplayAlerts = false;
                    worksheet.Delete();
                    excelApp.DisplayAlerts = true;

                    worksheet = (Excel.Worksheet)excelWorkBook.Worksheets[8];
                    excelApp.DisplayAlerts = false;
                    worksheet.Delete();
                    excelApp.DisplayAlerts = true;
                } catch  { }

                excelWorkBook.SaveAs(filename, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                excelWorkBook.Close();
                excelApp.Quit();

                this.Close();

                MessageBox.Show("The file was exported successfully.");
            }
        }

        #endregion
        // -------------------------

        #endregion
        //---------------------------------------------------------------------------

        #endregion
        //===========================================================================
    }
}
