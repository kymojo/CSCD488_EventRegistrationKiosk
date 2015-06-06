using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace RegistrationKiosk {
    /// <summary>
    /// Interaction logic for Window_Database.xaml
    /// </summary>
    public partial class Window_Database : Window {

        private Window_Main main = null;
        string[] oldLines = null, newLines = null;

        //===========================================================================
        #region Window Initialize
        //===========================================================================
        public Window_Database(Window_Main main) {
            InitializeComponent();
            this.main = main;
            loadOldLines();
            getFieldsFromOldLines();
        }
        #endregion
        //===========================================================================
        #region Window Events
        //===========================================================================

        private void btn_Connect_Click(object sender, RoutedEventArgs e) {
            // Validate port
            if (!validateForm())
                return;
            if (!connectDatabase()) {
                // If bad connection
                MessageBox.Show("Connection failed! Make sure fields are correct.");
                getFieldsFromOldLines();
                if (!main.dbConnection.IsConnected()) {
                    if (!connectDatabase())
                        MessageBox.Show("Cannot connect to old settings.");
                }
            } else {
                // If good connection
                MessageBox.Show("Connection successful!");
                writeNewLines();
                // Exit dialog
                btn_Cancel_Click(sender, e);
            }
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e) {
            if (!main.dbConnection.IsConnected()) {
                MessageBoxResult result = MessageBox.Show("Database is not currently connected.\nAre you sure you wish to cancel?", "No Connection!", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No) {
                    return;
                }
            }
            main.IsEnabled = true;
            this.Close();
        }

        private void txtbx_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                btn_Connect_Click(sender, e);
            }
        }

        #endregion
        //===========================================================================
        #region Window Methods
        //===========================================================================

        private bool validateForm() {
            // Validate Port Number
            try {
                int port = Convert.ToInt32(txtbx_Port.Text);
            } catch {
                MessageBox.Show("Invalid port number!");
                txtbx_Port.Text = this.main.dbConnection.portNumber.ToString();
                txtbx_Port.Focus();
                txtbx_Port.SelectAll();
                return false;
            }

            return true;
        }

        private void getFieldsFromOldLines() {
            txtbx_Host.Text = oldLines[1].Substring(9);
            txtbx_Port.Text = oldLines[2].Substring(9);
            txtbx_Db.Text = oldLines[3].Substring(9);
            txtbx_User.Text = oldLines[4].Substring(9);
            pass_Pass.Password = oldLines[5].Substring(9);
        }

        private bool loadOldLines() {
            try {
                oldLines = File.ReadAllLines("../../security.txt");
                return true;
            } catch {
                MessageBox.Show("Error reading from file.");
                return false;
            }
        }

        private bool writeNewLines() {
            try {
                newLines = new string[] { oldLines[0],
                                              "Db Host: " + txtbx_Host.Text,
                                              "Db Port: " + Convert.ToInt32(txtbx_Port.Text),
                                              "Db Name: " + txtbx_Db.Text,
                                              "Db User: " + txtbx_User.Text,
                                              "Db Pass: " + pass_Pass.Password
                                            };
                File.WriteAllLines("../../security.txt", newLines);
                oldLines = newLines;
                return true;
            } catch {
                MessageBox.Show("Error writing to file.");
                return false;
            }
        }

        private bool connectDatabase() {
            // Set connection variables
            main.dbConnection.SetConnection(txtbx_Host.Text, txtbx_Db.Text, txtbx_User.Text, pass_Pass.Password, Convert.ToInt32(txtbx_Port.Text));
            // Try connection
            bool result;
            result = main.dbConnection.Connect();
            result &= main.dbConnection.CreateDatabaseTables();
            // Return result
            return result;
        }

        #endregion
        //===========================================================================
    }
}
