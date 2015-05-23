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
using System.Text.RegularExpressions;

namespace RegistrationKiosk {
    /// <summary>
    /// Interaction logic for Window_ForgotCode.xaml
    /// </summary>
    public partial class Window_ForgotCode : Window {

        private Window_Main main = null;

        //===========================================================================
        #region Window Initialize
        //===========================================================================
        public Window_ForgotCode(Window_Main main) {
            this.main = main;
            InitializeComponent();
            txtbx_FirstName.Focus();
        }

        #endregion

        private bool ValidateInfo() {

            string regex_pattern;

            #region Name
            // Set Regex Pattern
            regex_pattern = @"^[A-Za-z-.\s]{2,}$";
            if (!Regex.IsMatch(txtbx_FirstName.Text, regex_pattern)) {
                // If First Name invalid,
                MessageBox.Show("Invalid First Name!");
                txtbx_FirstName.Focus();
                txtbx_FirstName.SelectAll();
                return false;
            } else if (!Regex.IsMatch(txtbx_LastName.Text, regex_pattern)) {
                // If Last Name invalid,
                MessageBox.Show("Invalid Last Name!");
                txtbx_LastName.Focus();
                txtbx_LastName.SelectAll();
                return false;
            }
            #endregion

            #region Email
            // Set Regex Pattern for Phone
            regex_pattern = @"^(\+?1)?[\s.-]?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$";
            if (!Regex.IsMatch(txtbx_Phone.Text, regex_pattern)) {
                // If phone is invalid
                MessageBox.Show("Invalid Phone Number!");
                txtbx_Phone.Focus();
                txtbx_Phone.SelectAll();
                return false;
            }
            #endregion

            return true;
        }

        //===========================================================================
        #region Window Events
        //===========================================================================
        private void txtbx_PressEnter(object sender, KeyEventArgs e) {
            // Check Enter key
            if (e.Key == Key.Return) {
                // Simulate find button click
                btn_Find_Click(sender, e);
            }
        }
        
        private void btn_Find_Click(object sender, RoutedEventArgs e) {
            if (ValidateInfo()) {
                RegistrantEntry entry = new RegistrantEntry();
                entry.Lname = txtbx_LastName.Text;
                entry.Fname = txtbx_FirstName.Text;
                entry.Phone = txtbx_Phone.Text;
                entry.GenerateHashCode();
                if (main.ValidateRegistrationCode(entry.Code)) {
                    main.IsEnabled = true;
                    main.txtbx_RegCode.Text = entry.Code;
                    this.Close();
                }
            }
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e) {
            main.IsEnabled = true;
            main.txtbx_RegCode.Focus();
            this.Close();
        }

        #endregion  
        //===========================================================================
    }
}
