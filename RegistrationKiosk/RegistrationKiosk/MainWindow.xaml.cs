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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RegistrationKiosk {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private enum WindowView { Menu, CheckIn, Admin };
        private WindowView AppState = WindowView.Menu;

        //===========================================================================
        #region Initialize Window
        //===========================================================================
        public MainWindow() {
            InitializeComponent();
            ChangeAppState(WindowView.Menu);
            ChangeRegistrationView();
        }

        #endregion
        //===========================================================================
        #region Application Methods
        //===========================================================================

        //---------------------------------------------------------------------------
        #region GENERAL
        //---------------------------------------------------------------------------
        /// <summary>
        /// Changes visibility of view-specific elements to match passed state.
        /// </summary>
        /// <param name="toState">The state to change to</param>
        private void ChangeAppState(WindowView toState) {

            #region MENU VIEW
            if (toState == WindowView.Menu) {
                lbl_PageHeader.Content = "Main Menu";
                // Enable View
                grid_Menu.IsEnabled = true;
                grid_Menu.Visibility = System.Windows.Visibility.Visible;
                // Disable Button
                btn_GotoMenu.IsEnabled = false;
                btn_GotoMenu.Visibility = System.Windows.Visibility.Hidden;
            } else {
                // Disable View
                grid_Menu.IsEnabled = false;
                grid_Menu.Visibility = System.Windows.Visibility.Hidden;
                // Enable Button
                btn_GotoMenu.IsEnabled = true;
                btn_GotoMenu.Visibility = System.Windows.Visibility.Visible;
            }
            #endregion

            #region CHECK IN VIEW
            if (toState == WindowView.CheckIn) {
                lbl_PageHeader.Content = "Event Check In Form";
                // Enable View
                grid_Registration.IsEnabled = true;
                grid_Registration.Visibility = System.Windows.Visibility.Visible;
            } else {
                // Disable View
                grid_Registration.IsEnabled = false;
                grid_Registration.Visibility = System.Windows.Visibility.Hidden;
            }
            #endregion

            #region ADMIN VIEW
            if (toState == WindowView.Admin) {
                lbl_PageHeader.Content = "Administrator Tools";
                // Enable View
                grid_Admin.IsEnabled = true;
                grid_Admin.Visibility = System.Windows.Visibility.Visible;
            } else {
                // Disable View
                grid_Admin.IsEnabled = false;
                grid_Admin.Visibility = System.Windows.Visibility.Hidden;
            }
            #endregion

            // When leaving CheckIn view 
            if (AppState == WindowView.CheckIn && AppState != toState)
                ClearRegistrationForm();

            AppState = toState;
        }

        #endregion
        //---------------------------------------------------------------------------
        #region CHECK IN VIEW
        //---------------------------------------------------------------------------

        /// <summary>
        /// Clears all user entered data on check in form.
        /// </summary>
        private void ClearRegistrationForm() {
            
            #region TEXT BOXES
            // Prereg
            txtbx_RegCode.Text = "";
            // Name
            txtbx_FirstName.Text = "";
            txtbx_LastName.Text = "";
            // Contact
            txtbx_Email.Text = "";
            txtbx_Phone.Text = "";
            // Student
            txtbx_StudentID.Text = "";
            txtbx_Graduation.Text = "";
            // Employee
            txtbx_Business.Text = "";
            txtbx_Job.Text = "";
            #endregion

            #region RADIO BUTTONS
            // Sex
            radio_Male.IsChecked = false;
            radio_Female.IsChecked = false;
            // Registrant Type
            radio_Student.IsChecked = false;
            radio_Employee.IsChecked = false;
            radio_General.IsChecked = false;        
            // Student
            radio_Freshman.IsChecked = false;
            radio_Sophomore.IsChecked = false;
            radio_Junior.IsChecked = false;
            radio_Senior.IsChecked = false;
            radio_Postbac.IsChecked = false;
            radio_Grad.IsChecked = false;
            radio_Alumnus.IsChecked = false;
            #endregion

            #region COMBO BOX
            // Colleges
            combo_Colleges.SelectedIndex = -1;
            #endregion

            ChangeRegistrationView();

        }

        /// <summary>
        /// Checks check in form for any improper or missing data.
        /// </summary>
        /// <returns>Is valid</returns>
        private bool ValidateRegistrationForms() {
            return true;
        }

        /// <summary>
        /// Changes visibility of view-specific elements to match registrant type selection.
        /// </summary>
        private void ChangeRegistrationView() {

            #region STUDENT VIEW
            if (radio_Student.IsChecked == true) {
                grid_RegStudent.IsEnabled = true;
                grid_RegStudent.Visibility = System.Windows.Visibility.Visible;
            } else {
                grid_RegStudent.IsEnabled = false;
                grid_RegStudent.Visibility = System.Windows.Visibility.Hidden;
            }
            #endregion

            #region EMPLOYEE VIEW
            if (radio_Employee.IsChecked == true) {
                grid_RegEmployer.IsEnabled = true;
                grid_RegEmployer.Visibility = System.Windows.Visibility.Visible;
            } else {
                grid_RegEmployer.IsEnabled = false;
                grid_RegEmployer.Visibility = System.Windows.Visibility.Hidden;
            }
            #endregion

            #region NO VIEW
            if (radio_Employee.IsChecked == false && radio_Student.IsChecked == false) {
                grid_RegBlank.IsEnabled = true;
                grid_RegBlank.Visibility = System.Windows.Visibility.Visible;
            } else {
                grid_RegBlank.IsEnabled = false;
                grid_RegBlank.Visibility = System.Windows.Visibility.Hidden;
            }
            #endregion

        }

        #endregion
        //---------------------------------------------------------------------------

        #endregion
        //===========================================================================
        #region Application Events
        //===========================================================================

        //---------------------------------------------------------------------------
        #region BUTTONS
        //---------------------------------------------------------------------------

        // . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .
        #region NAVIGATION BUTTONS
        // . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .
        private void btn_GotoRegistration_Click(object sender, RoutedEventArgs e) {
            ChangeAppState(WindowView.CheckIn);
        }

        private void btn_GotoAdmin_Click(object sender, RoutedEventArgs e) {
            ChangeAppState(WindowView.Admin);
        }

        private void btn_GotoMenu_Click(object sender, RoutedEventArgs e) {
            ChangeAppState(WindowView.Menu);
        }

        private void btn_ExitProgram_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        #endregion
        // . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .
        #region CHECK IN FORM BUTTONS
        // . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .
        private void btn_RegCode_Click(object sender, RoutedEventArgs e) {
            txtbx_RegCode.Text = "";
        }

        private void btn_Checkin_Click(object sender, RoutedEventArgs e) {
            if (ValidateRegistrationForms())
                ClearRegistrationForm();
        }

        #endregion
        // . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .

        #endregion
        //---------------------------------------------------------------------------
        
        private void radio_RegistrantType_Checked(object sender, RoutedEventArgs e) {
            ChangeRegistrationView();
        }

        #endregion
        //===========================================================================
    }
}
