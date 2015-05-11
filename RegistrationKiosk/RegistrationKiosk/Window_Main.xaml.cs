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
using System.Text.RegularExpressions;

namespace RegistrationKiosk {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Window_Main : Window {

        private enum WindowView { CheckIn, Admin, Edit };
        private WindowView AppState = WindowView.CheckIn;

        private Window_Admin adminWindow = null;
        public delegate void AdminDelegateType();
        public AdminDelegateType Delegate_OnAdminSuccess;

        private SolidColorBrush brush_FormBorder = new SolidColorBrush(Color.FromRgb(129, 173, 170));
        private SolidColorBrush brush_FormFill = new SolidColorBrush(Color.FromRgb(198, 232, 232));

        //===========================================================================
        #region Initialize Window
        //===========================================================================
        public Window_Main() {
            InitializeComponent();
            ChangeAppState(AppState);
            ChangeRegistrationView();
        }

        #endregion
        //===========================================================================
        #region Window Methods
        //===========================================================================

        //---------------------------------------------------------------------------
        #region GENERAL
        //---------------------------------------------------------------------------
        /// <summary>
        /// Changes visibility of view-specific elements to match passed state.
        /// </summary>
        /// <param name="toState">The state to change to</param>
        private void ChangeAppState(WindowView toState) {

            #region CHECK IN VIEW
            if (toState == WindowView.CheckIn) {
                lbl_PageHeader.Content = "Event Check In Form";
                // Enable View
                grid_Registration.IsEnabled = true;
                grid_Registration.Visibility = System.Windows.Visibility.Visible;
                // Enable Adin Button
                btn_AdminMenu.IsEnabled = true;
                btn_AdminMenu.Visibility = System.Windows.Visibility.Visible;
                // Disable Back Button
                btn_AdminBack.IsEnabled = false;
                btn_AdminBack.Visibility = System.Windows.Visibility.Hidden;
            } else {
                // Disable View
                grid_Registration.IsEnabled = false;
                grid_Registration.Visibility = System.Windows.Visibility.Hidden;
                // Disable Admin Button
                btn_AdminMenu.IsEnabled = false;
                btn_AdminMenu.Visibility = System.Windows.Visibility.Hidden;
                // Enable Back Button
                btn_AdminBack.IsEnabled = true;
                btn_AdminBack.Visibility = System.Windows.Visibility.Visible;
            }
            #endregion

            #region ADMIN VIEW
            if (toState == WindowView.Admin) {
                lbl_PageHeader.Content = "Administrator Tools";
                // Enable View
                grid_Admin.IsEnabled = true;
                grid_Admin.Visibility = System.Windows.Visibility.Visible;
                // Enable Exit Button
                btn_ExitProgram.IsEnabled = true;
                btn_ExitProgram.Visibility = System.Windows.Visibility.Visible;
            } else {
                // Disable View
                grid_Admin.IsEnabled = false;
                grid_Admin.Visibility = System.Windows.Visibility.Hidden;
                // Disable Exit Button
                btn_ExitProgram.IsEnabled = false;
                btn_ExitProgram.Visibility = System.Windows.Visibility.Hidden;
            }
            #endregion

            #region EDIT VIEW

            #endregion

            // When leaving CheckIn view 
            if (AppState == WindowView.CheckIn && AppState != toState)
                ClearRegistrationForm();

            AppState = toState;
        }

        private void GotoAdminPage() {
            ChangeAppState(WindowView.Admin);
        }

        public void RunAdminDelegate() {
            if (Delegate_OnAdminSuccess != null)
                Delegate_OnAdminSuccess();
        }

        private void SetAdminDelegate(AdminDelegateType del) {
            Delegate_OnAdminSuccess = del;
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

            #region RECTANGLES
            // General Info
            rec_RegName.Stroke = brush_FormBorder;
            rec_RegSex.Stroke = brush_FormBorder;
            rec_RegRegistrant.Stroke = brush_FormBorder;
            rec_RegContact.Stroke = brush_FormBorder;
            // Student Info
            rec_RegClass.Stroke = brush_FormBorder;
            rec_RegStudMore.Stroke = brush_FormBorder;
            // Employer Info
            rec_RegEmployer.Stroke = brush_FormBorder;
            #endregion

            ChangeRegistrationView();

        }

        /// <summary>
        /// Checks check in form for any improper or missing data.
        /// </summary>
        /// <returns>Is valid</returns>
        private bool ValidateRegistrationForms() {

            string regex_pattern;
            // -------------------------
            #region GENERAL INFO
            // -------------------------
            
            #region Name
            regex_pattern = @"^[A-Za-z-\s]{2,}$";
            if (!Regex.IsMatch(txtbx_FirstName.Text, regex_pattern)) {
                MessageBox.Show("Invalid First Name!");
                txtbx_FirstName.Focus();
                txtbx_FirstName.SelectAll();
                rec_RegName.Stroke = Brushes.Red;
                return false;
            } else if (!Regex.IsMatch(txtbx_LastName.Text, regex_pattern)) {
                MessageBox.Show("Invalid Last Name!");
                txtbx_LastName.Focus();
                txtbx_LastName.SelectAll();
                rec_RegName.Stroke = Brushes.Red;
                return false;
            } else
                rec_RegName.Stroke = brush_FormBorder;
            #endregion

            #region Sex
            if (radio_Male.IsChecked == false && radio_Female.IsChecked == false) {
                MessageBox.Show("Please indicate sex.");
                rec_RegSex.Stroke = Brushes.Red;
                return false;
            } else
                rec_RegSex.Stroke = brush_FormBorder;
            #endregion

            #region Registrant Type
            // REGISTRANT TYPE
            if (radio_Student.IsChecked == false && radio_Employee.IsChecked == false && radio_General.IsChecked == false) {
                MessageBox.Show("Please indicate registrant type.");
                rec_RegRegistrant.Stroke = Brushes.Red;
                return false;
            } else
                rec_RegRegistrant.Stroke = brush_FormBorder;
            #endregion

            #region Contact Info
            // Email
            regex_pattern = "^[A-Za-z0-9!#$%&'*+\u002D/=?^_`{|}~]+@[A-Za-z0-9.-]+\u002E[A-Za-z]{2,6}$";
            if (!Regex.IsMatch(txtbx_Email.Text, regex_pattern)) {
                MessageBox.Show("Invalid Email Address!");
                txtbx_Email.Focus();
                txtbx_Email.SelectAll();
                rec_RegContact.Stroke = Brushes.Red;
                return false;
            }

            // PHONE
            regex_pattern = @"^(\+?1)?[\s.-]?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$";
            if (!Regex.IsMatch(txtbx_Phone.Text, regex_pattern)) {
                MessageBox.Show("Invalid Phone Number!");
                txtbx_Phone.Focus();
                txtbx_Phone.SelectAll();
                rec_RegContact.Stroke = Brushes.Red;
                return false;
            } else
                rec_RegContact.Stroke = brush_FormBorder;
            #endregion

            #endregion
            // -------------------------
            #region STUDENT INFO
            // -------------------------
            if (radio_Student.IsChecked == true) {

                #region Class Standing
                if (radio_Freshman.IsChecked == false && radio_Sophomore.IsChecked == false &&
                    radio_Junior.IsChecked == false && radio_Senior.IsChecked == false &&
                    radio_Postbac.IsChecked == false && radio_Grad.IsChecked == false &&
                    radio_Alumnus.IsChecked == false) {
                    MessageBox.Show("Please indicate class standing.");
                    rec_RegClass.Stroke = Brushes.Red;
                    return false;
                } else
                    rec_RegClass.Stroke = brush_FormBorder;
                #endregion

                #region College
                if (combo_Colleges.SelectedIndex == -1) {
                    MessageBox.Show("Please indicate college.");
                    rec_RegStudMore.Stroke = Brushes.Red;
                    return false;
                }
                #endregion

                #region Student ID
                regex_pattern = @"^\d{5,10}$";
                if (!Regex.IsMatch(txtbx_StudentID.Text, regex_pattern)) {
                    MessageBox.Show("Invalid student ID!");
                    txtbx_StudentID.Focus();
                    txtbx_StudentID.SelectAll();
                    rec_RegStudMore.Stroke = Brushes.Red;
                    return false;
                }
                #endregion

                #region Grad Year
                regex_pattern = @"^\d{4}$";
                if (!Regex.IsMatch(txtbx_Graduation.Text, regex_pattern)) {
                    MessageBox.Show("Invalid graduation year!");
                    txtbx_Graduation.Focus();
                    txtbx_Graduation.SelectAll();
                    rec_RegStudMore.Stroke = Brushes.Red;
                    return false;
                } else
                    rec_RegStudMore.Stroke = brush_FormBorder;
                #endregion
            }

            #endregion
            // -------------------------
            #region EMPLOYEE INFO
            // -------------------------
            if (radio_Employee.IsChecked == true) {

                #region Business Name
                regex_pattern = @"^[\w\s\d.+-]{3,}$";
                if (!Regex.IsMatch(txtbx_Business.Text, regex_pattern)) {
                    MessageBox.Show("Invalid business name!");
                    txtbx_Business.Focus();
                    txtbx_Business.SelectAll();
                    rec_RegEmployer.Stroke = Brushes.Red;
                    return false;
                }
                #endregion

                #region Job Title
                regex_pattern = @"^[\w\s\d.+-]{3}$";
                if (!Regex.IsMatch(txtbx_Job.Text, regex_pattern)) {
                    MessageBox.Show("Invalid job title!");
                    txtbx_Job.Focus();
                    txtbx_Job.SelectAll();
                    rec_RegEmployer.Stroke = Brushes.Red;
                    return false;
                } else
                    rec_RegEmployer.Stroke = brush_FormBorder;
                #endregion
            }

            #endregion
            // -------------------------
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
        #region Window Events
        //===========================================================================

        //---------------------------------------------------------------------------
        #region BUTTONS
        //---------------------------------------------------------------------------

        // . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .
        #region NAVIGATION BUTTONS
        // . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .

        private void btn_AdminMenu_Click(object sender, RoutedEventArgs e) {
            adminWindow = new Window_Admin(this);
            adminWindow.Show();
            SetAdminDelegate(new AdminDelegateType(GotoAdminPage));
            this.IsEnabled = false;
        }

        private void btn_AdminBack_Click(object sender, RoutedEventArgs e) {
            ChangeAppState(WindowView.CheckIn);
        }

        private void btn_ExitProgram_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        #endregion
        // . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .
        #region CHECK IN FORM BUTTONS
        // . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .
        private void btn_RegCode_Click(object sender, RoutedEventArgs e) {
            if (Regex.IsMatch(txtbx_RegCode.Text, "^[0-9]{6}$")) {
                MessageBox.Show("It's good!");
            } else
                MessageBox.Show("Invalid Registration Code!");
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
        #region OTHER
        //---------------------------------------------------------------------------
        private void radio_RegistrantType_Checked(object sender, RoutedEventArgs e) {
            ChangeRegistrationView();
        }

        private void txtbx_RegCode_PressEnter(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                btn_RegCode_Click(sender, e);
            }
        }
        #endregion
        //---------------------------------------------------------------------------

        #endregion
        //===========================================================================
    }
}
