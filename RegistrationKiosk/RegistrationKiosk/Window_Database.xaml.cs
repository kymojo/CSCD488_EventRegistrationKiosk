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

        //===========================================================================
        #region Window Initialize
        //===========================================================================
        public Window_Database(Window_Main main) {
            InitializeComponent();
            this.main = main;
            GetFieldsFromOldSettings();
            CheckConnectionStatus();
        }
        #endregion
        //===========================================================================
        #region Window Events
        //===========================================================================

        /// <summary>
        /// Click event for Connect button
        /// </summary>
        private void btn_Connect_Click(object sender, RoutedEventArgs e) {
            // Validate port
            if (!ValidateForm())
                return;
            if (!ConnectDatabase()) {
                // If bad connection
                MessageBox.Show("Connection failed! Make sure fields are correct.");
            } else {
                // If good connection
                MessageBox.Show("Connection successful!");
                SaveNewSettings();
                // Exit dialog
                btn_Cancel_Click(sender, e);
            }
            CheckConnectionStatus();
        }

        /// <summary>
        /// Click event for Cancel button
        /// </summary>
        private void btn_Cancel_Click(object sender, RoutedEventArgs e) {
            // If no connected
            if (!main.dbConnection.IsConnected()) {
                // Ask to revert
                MessageBoxResult result = MessageBox.Show("Do you want to revert to the last valid settings?", "Revert?", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
                if (result == MessageBoxResult.Yes) {
                    GetFieldsFromOldSettings();
                    // If success, let them see
                    if (ConnectDatabase()) {
                        CheckConnectionStatus();
                        return;
                    } 
                }
                // If still not connected
                if (!main.dbConnection.IsConnected()) {
                    // Ask if they want to close window
                    result = MessageBox.Show("Database is not currently connected.\nAre you sure you wish to cancel?", "No Connection!", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                        return;
                    // If no, save the settings
                    SaveNewSettings();
                }
            }
            // Close the window
            main.IsEnabled = true;
            this.Close();
        }

        /// <summary>
        /// KeyDown event for textboxes (checks for Return/Enter press)
        /// </summary>
        private void txtbx_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return || e.Key == Key.Enter) {
                btn_Connect_Click(sender, e);
            }
        }

        #endregion
        //===========================================================================
        #region Window Methods
        //===========================================================================

        /// <summary>
        /// Displays the connection status
        /// </summary>
        private void CheckConnectionStatus() {
            if (!main.dbConnection.IsConnected()) {
                lbl_ConnectionStatus.Content = "Database Status: No Connection!";
                lbl_ConnectionStatus.Foreground = new SolidColorBrush(Colors.Red);
            } else {
                lbl_ConnectionStatus.Content = "Database Status: Connected";
                lbl_ConnectionStatus.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        /// <summary>
        /// Validates form data
        /// </summary>
        /// <returns>Is Valid</returns>
        private bool ValidateForm() {
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

        /// <summary>
        /// Populate connection settings with previously accepted settings
        /// </summary>
        private void GetFieldsFromOldSettings() {
            txtbx_Host.Text = main.GetSecurity().DbHost;
            txtbx_Port.Text = main.GetSecurity().DbPort.ToString();
            txtbx_Db.Text = main.GetSecurity().DbName;
            txtbx_User.Text = main.GetSecurity().DbUser;
            pass_Pass.Password = main.GetSecurity().DbPass;
        }

        /// <summary>
        /// Writes newly accepted settings to security.txt
        /// </summary>
        /// <returns>Success flag</returns>
        private void SaveNewSettings() {
            main.GetSecurity().DbHost = txtbx_Host.Text;
            main.GetSecurity().DbPort = Convert.ToInt32(txtbx_Port.Text);
            main.GetSecurity().DbName = txtbx_Db.Text;
            main.GetSecurity().DbUser = txtbx_User.Text;
            main.GetSecurity().DbPass = pass_Pass.Password;
        }

        /// <summary>
        /// Attempts to connect to database with entered settings
        /// </summary>
        /// <returns>Success flag</returns>
        private bool ConnectDatabase() {
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
