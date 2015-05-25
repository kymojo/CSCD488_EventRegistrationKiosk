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
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace RegistrationKiosk {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Window_Main : Window {

        //===========================================================================
        #region Window Variables
        //===========================================================================

        // Window State stuff
        private enum WindowView { CheckIn, Admin, Edit };
        private WindowView AppState = WindowView.CheckIn;

        // Admin Window stuff
        private Window_Admin adminWindow = null;
        public delegate void AdminDelegateType();
        public AdminDelegateType Delegate_OnAdminSuccess;

        // Forgot Code Window
        private Window_ForgotCode forgotcodeWindow = null;

        // Database Connection Object
        MySQLClient dbConnection;

        // Excel Interop Object
        IOExcel ioXL;

        // Defined color brushes
        private SolidColorBrush brush_FormBorder = new SolidColorBrush(Color.FromRgb(129, 173, 170));
        private SolidColorBrush brush_FormFill = new SolidColorBrush(Color.FromRgb(198, 232, 232));

        // Collection for viewing search results
        private ObservableCollection<RegistrantEntry> searchEntries = new ObservableCollection<RegistrantEntry>();

        // Lookup code of entry being edited
        private string editingID = "123456";
        private RegistrantEntry editingRegistrant;

        // Flag indicating if user used pre-registration code
        private bool validCodeEntered = false;

        #endregion
        //===========================================================================
        #region Window Initialization
        //===========================================================================
        
        public Window_Main() {
            InitializeComponent();
            ChangeAppState(AppState);
            ChangeSpecialView();
            datagrid_AdminEntries.DataContext = searchEntries;
            dbConnection = new MySQLClient("cscd379.com", "excelimport", "jobfair", "ewu2015");
            ioXL = new IOExcel(dbConnection);
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

            #region CHECK IN VIEW & EDIT VIEW
            if (toState == WindowView.CheckIn || toState == WindowView.Edit) {

                if (toState == WindowView.CheckIn) {
                    lbl_PageHeader.Content = "Event Check In Form";
                    // Enable Admin Button
                    btn_AdminMenu.IsEnabled = true;
                    btn_AdminMenu.Visibility = System.Windows.Visibility.Visible;
                    // Disable Edit Header & Footer
                    grid_EditFooter.IsEnabled = false;
                    grid_EditFooter.Visibility = System.Windows.Visibility.Hidden;
                    grid_EditHeader.IsEnabled = false;
                    grid_EditHeader.Visibility = System.Windows.Visibility.Hidden;
                    // Enable Header & Footer
                    grid_RegFooter.IsEnabled = true;
                    grid_RegFooter.Visibility = System.Windows.Visibility.Visible;
                    grid_RegPre.IsEnabled = true;
                    grid_RegPre.Visibility = System.Windows.Visibility.Visible;
                    // Disable Back Button
                    btn_Back.IsEnabled = false;
                    btn_Back.Visibility = System.Windows.Visibility.Hidden;
                    // Focus on RegCode Textbox
                    txtbx_RegCode.Focus();
                } else {
                    lbl_PageHeader.Content = "Edit Registrant";
                    lbl_EditHeaderCode.Content = "Editing Entry #" + editingID;
                    // Disable Header & Footer
                    grid_RegFooter.IsEnabled = false;
                    grid_RegFooter.Visibility = System.Windows.Visibility.Hidden;
                    grid_RegPre.IsEnabled = false;
                    grid_RegPre.Visibility = System.Windows.Visibility.Hidden;
                    // Enable Edit Header & Footer
                    grid_EditFooter.IsEnabled = true;
                    grid_EditFooter.Visibility = System.Windows.Visibility.Visible;
                    grid_EditHeader.IsEnabled = true;
                    grid_EditHeader.Visibility = System.Windows.Visibility.Visible;
                }
                // Enable View
                grid_Registration.IsEnabled = true;
                grid_Registration.Visibility = System.Windows.Visibility.Visible;
                
            } else {
                // Disable View
                grid_Registration.IsEnabled = false;
                grid_Registration.Visibility = System.Windows.Visibility.Hidden;
                // Disable Admin Button
                btn_AdminMenu.IsEnabled = false;
                btn_AdminMenu.Visibility = System.Windows.Visibility.Hidden;
                // Enable Back Button
                btn_Back.IsEnabled = true;
                btn_Back.Visibility = System.Windows.Visibility.Visible;
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

            ClearRegistrationForm();

            AppState = toState;
        }

        /// <summary>
        /// Calls ChangeAppState(Admin) used by delegate for Window_Admin.
        /// </summary>
        private void GotoAdminPage() {
            ChangeAppState(WindowView.Admin);
        }

        /// <summary>
        /// Calls the method referred to by admin window success delegate.
        /// </summary>
        public void RunAdminDelegate() {
            if (Delegate_OnAdminSuccess != null)
                Delegate_OnAdminSuccess();
        }

        /// <summary>
        /// Sets the method for admin window success delegate.
        /// </summary>
        /// <param name="del">Method to delegate</param>
        private void SetAdminDelegate(AdminDelegateType del) {
            Delegate_OnAdminSuccess = del;
        }

        /// <summary>
        /// Validates a registration code (format and existence).
        /// </summary>
        /// <param name="code">String of registration code</param>
        /// <returns>IsValid flag</returns>
        public bool ValidateRegistrationCode(string code) {
            if (Regex.IsMatch(code, "^[0-9]{6}$")) {
                // check if exists in database
                string where = "Code = '" + code + "'";
                List<RegistrantEntry> regList = dbConnection.SelectRegistrant(where);
                int ct = regList.Count;
                if (ct == 0) {
                    // If no hits, display message
                    MessageBox.Show("The entry does not exist!");
                } else if (ct == 1) {
                    // If only one hit, return true
                    editingRegistrant = regList[0];
                    editingID = code;
                    return true;
                } else if (ct > 1) {
                    // If there is a collision, show an error message
                    MessageBox.Show("An error occurred.");
                }
            } else
                MessageBox.Show("Invalid Registration Code!");
            return false;
        }

        #endregion
        //---------------------------------------------------------------------------
        #region CHECK IN VIEW
        //---------------------------------------------------------------------------

        /// <summary>
        /// Clears all user entered data on check in form.
        /// </summary>
        private void ClearRegistrationForm() {
            
            #region CLEAR TEXT BOXES
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
            combo_Colleges.Text = "";
            txtbx_Major.Text = "";

            // Employee
            txtbx_Business.Text = "";
            txtbx_Job.Text = "";
            #endregion

            #region CLEAR RADIO BUTTONS
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

            #region CLEAR RECTANGLE COLORS
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

            // Resets Registration View (after no radio selected)
            ChangeSpecialView();

            validCodeEntered = false;
        }

        /// <summary>
        /// Checks check in form for any improper or missing data.
        /// </summary>
        /// <returns>IsValid flag</returns>
        private bool ValidateRegistrationForms() {

            string regex_pattern;
            // -------------------------
            #region VALIDATE GENERAL INFO
            // -------------------------
            
            #region Name
            // Set Regex Pattern
            regex_pattern = @"^[A-Za-z-.\s]{2,}$";
            if (!Regex.IsMatch(txtbx_FirstName.Text, regex_pattern)) {
                // If First Name invalid,
                MessageBox.Show("Invalid First Name!");
                txtbx_FirstName.Focus();
                txtbx_FirstName.SelectAll();
                rec_RegName.Stroke = Brushes.Red;
                return false;
            } else if (!Regex.IsMatch(txtbx_LastName.Text, regex_pattern)) {
                // If Last Name invalid,
                MessageBox.Show("Invalid Last Name!");
                txtbx_LastName.Focus();
                txtbx_LastName.SelectAll();
                rec_RegName.Stroke = Brushes.Red;
                return false;
            } else
                // Set normal border color
                rec_RegName.Stroke = brush_FormBorder;
            #endregion

            #region Contact Info
            // Set Regex Pattern for Email
            regex_pattern = "^[A-Za-z0-9!#$%&'*+\u002D/=?^_`{|}~]+@[A-Za-z0-9.-]+\u002E[A-Za-z]{2,6}$";
            if (!Regex.IsMatch(txtbx_Email.Text, regex_pattern)) {
                // If email is invalid
                MessageBox.Show("Invalid Email Address!");
                txtbx_Email.Focus();
                txtbx_Email.SelectAll();
                rec_RegContact.Stroke = Brushes.Red;
                return false;
            }

            // Set Regex Pattern for Phone
            regex_pattern = @"^(\+?1)?[\s.-]?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$";
            if (!Regex.IsMatch(txtbx_Phone.Text, regex_pattern)) {
                // If phone is invalid
                MessageBox.Show("Invalid Phone Number!");
                txtbx_Phone.Focus();
                txtbx_Phone.SelectAll();
                rec_RegContact.Stroke = Brushes.Red;
                return false;
            } else
                // Set normal border color
                rec_RegContact.Stroke = brush_FormBorder;
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

            #endregion
            // -------------------------
            #region VALIDATE STUDENT INFO
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

                #region Major
                regex_pattern = @"^[\w\s\d.+-]{2,24}$";
                if (!Regex.IsMatch(txtbx_Major.Text, regex_pattern)) {
                    MessageBox.Show("Invalid major!/nMajors must be under 24 characters long.");
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
            #region VALIDATE EMPLOYEE INFO
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
                regex_pattern = @"^[\w\s\d.+-]{3,}$";
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
        private void ChangeSpecialView() {

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

        /// <summary>
        /// Creates a new RegistrantEntry object from form data. WARNING: Does not validate data.
        /// </summary>
        /// <returns>New RegistrantEntry object</returns>
        private RegistrantEntry RegistrantFromForm() {
            // General Variables
            string lname = txtbx_LastName.Text.Trim();
            string fname = txtbx_FirstName.Text.Trim();
            RegistrantEntry.SexType sex;
            if (radio_Male.IsChecked == true)
                 sex = RegistrantEntry.SexType.Male;
            else
                sex = RegistrantEntry.SexType.Female;
            string email = txtbx_Email.Text;
            string phone = txtbx_Phone.Text;
            // Create RegistrantEntry
            RegistrantEntry registrant = new RegistrantEntry(lname, fname, sex, email, phone);
            // Check for Student or Employee
            if (radio_Student.IsChecked == true) {
                RegistrantEntry.ClassStandingType classStanding = GetClassStanding();
                string college = combo_Colleges.Text.Trim();
                string major = txtbx_Major.Text.Trim();
                string studentID = txtbx_StudentID.Text;
                int gradYear = Convert.ToInt32(txtbx_Graduation.Text);
                registrant.SetTypeStudent(classStanding, college, major, studentID, gradYear);
            }
            else if (radio_Employee.IsChecked == true) {
                string business = txtbx_Business.Text.Trim();
                string job = txtbx_Job.Text.Trim();
                registrant.SetTypeEmployee(business, job);
            }
            return registrant;
        }

        /// <summary>
        /// Returns the RegistrantEntry ClassStanding enum value corresponding to radio buttons.
        /// </summary>
        /// <returns>RegistrantEntry ClassStanding enum value</returns>
        private RegistrantEntry.ClassStandingType GetClassStanding() {
            if (radio_Freshman.IsChecked == true)
                return RegistrantEntry.ClassStandingType.Freshman;
            if (radio_Sophomore.IsChecked == true)
                return RegistrantEntry.ClassStandingType.Sophomore;
            if (radio_Junior.IsChecked == true)
                return RegistrantEntry.ClassStandingType.Junior;
            if (radio_Senior.IsChecked == true)
                return RegistrantEntry.ClassStandingType.Senior;
            if (radio_Postbac.IsChecked == true)
                return RegistrantEntry.ClassStandingType.PostBac;
            if (radio_Grad.IsChecked == true)
                return RegistrantEntry.ClassStandingType.Graduate;
            if (radio_Alumnus.IsChecked == true)
                return RegistrantEntry.ClassStandingType.Alumnus;
            return RegistrantEntry.ClassStandingType.None;
        }

        /// <summary>
        /// Checks the radio button corresponding to passed enum value
        /// </summary>
        /// <param name="standing">Class standing enum</param>
        private void CheckClassStanding(RegistrantEntry.ClassStandingType standing) {
            if (standing == RegistrantEntry.ClassStandingType.Freshman)
                radio_Freshman.IsChecked = true;
            if (standing == RegistrantEntry.ClassStandingType.Sophomore)
                radio_Sophomore.IsChecked = true;
            if (standing == RegistrantEntry.ClassStandingType.Junior)
                radio_Junior.IsChecked = true;
            if (standing == RegistrantEntry.ClassStandingType.Senior)
                radio_Senior.IsChecked = true;
            if (standing == RegistrantEntry.ClassStandingType.PostBac)
                radio_Postbac.IsChecked = true;
            if (standing == RegistrantEntry.ClassStandingType.Graduate)
                radio_Grad.IsChecked = true;
            if (standing == RegistrantEntry.ClassStandingType.Alumnus)
                radio_Alumnus.IsChecked = true;
        }

        /// <summary>
        /// Populates the form using a registrant entry.
        /// </summary>
        /// <param name="entry">Registrant with data</param>
        private void PopulateFormFromRegistrant(RegistrantEntry entry) {
            if (entry == null)
                return;
            txtbx_LastName.Text = entry.Lname;
            txtbx_FirstName.Text = entry.Fname;
            if (entry.Sex == RegistrantEntry.SexType.Male)
                radio_Male.IsChecked = true;
            else
                radio_Female.IsChecked = true;
            txtbx_Email.Text = entry.Email;
            txtbx_Phone.Text = entry.Phone;

            if (entry.RegType == RegistrantEntry.RegistrantType.Student) {
                radio_Student.IsChecked = true;
                CheckClassStanding(entry.ClassStanding);
                combo_Colleges.Text = entry.College;
                txtbx_Major.Text = entry.Major;
                txtbx_StudentID.Text = entry.StudentID;
                txtbx_Graduation.Text = entry.GradYear.ToString();
            } else if (entry.RegType == RegistrantEntry.RegistrantType.Employee) {
                radio_Employee.IsChecked = true;
                txtbx_Business.Text = entry.Business;
                txtbx_Job.Text = entry.Job;
            } else
                radio_General.IsChecked = true;
        }

        /// <summary>
        /// Returns the registrant resulting from a previously successful code verification.
        /// </summary>
        /// <returns>Found registrant for editing</returns>
        private RegistrantEntry GetEditingRegistrant() {
            RegistrantEntry registrant = editingRegistrant;
            editingRegistrant = null;
            return registrant;
        }

        #endregion
        //---------------------------------------------------------------------------
        #region ADMIN VIEW
        //---------------------------------------------------------------------------

        /// <summary>
        /// Queries the database for search string and populates DataGrid with entries.
        /// </summary>
        /// <param name="search">The search parameter</param>
    

        private void GetSearchResults(string search) {
            // Clear entries from previous search
            searchEntries.Clear();
            /* add entries to search box */
            //MessageBox.Show("Dummy entries.");
            List<RegistrantEntry> Registrants = dbConnection.SelectRegistrant("Code = '" + search + "' or Fname = '" + search + "' or Lname = '" + search + "' or Phone = '" + search + "' or Email = '" + search + "' or Sex = '" + search + "' or RegType = '" + search + "'");
            foreach (RegistrantEntry entry in Registrants)
            {
                searchEntries.Add(entry);
            }
         
            datagrid_AdminEntries.DataContext = searchEntries;
            datagrid_AdminEntries.UpdateLayout();
            //searchEntries.Add(new RegistrantEntry("Johnson", "Kyle", RegistrantEntry.SexType.Male, "myEmail@hotmail.com", "123-456-7890"));
            //searchEntries.Add(new RegistrantEntry("Xia", "Zhenyu", RegistrantEntry.SexType.Male, "myEmail@hotmail.com", "123-456-7890"));
            //searchEntries.Add(new RegistrantEntry("Holliday", "Dylan", RegistrantEntry.SexType.Male, "myEmail@hotmail.com", "123-456-7890"));
            //searchEntries.Add(new RegistrantEntry("Reynolds", "Kevin", RegistrantEntry.SexType.Male, "myEmail@hotmail.com", "123-456-7890"));
        
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

        /// <summary>
        /// Click event for Admin button (visible from check-in form).
        /// </summary>
        private void btn_AdminMenu_Click(object sender, RoutedEventArgs e) {
            // Create admin window and display
            adminWindow = new Window_Admin(this);
            adminWindow.Show();
            // Set method for successful validation
            SetAdminDelegate(new AdminDelegateType(GotoAdminPage));
            // Disable this window (until admin window closes)
            this.IsEnabled = false;
        }

        /// <summary>
        /// Click event for Back button (visible from admin and edit pages).
        /// </summary>
        private void btn_Back_Click(object sender, RoutedEventArgs e) {
            // If admin page, return to check-in page
            if (AppState == WindowView.Admin)
                ChangeAppState(WindowView.CheckIn);
            // if edit page, return to admin page
            else if (AppState == WindowView.Edit)
                btn_EditCancel_Click(sender, e);
        }

        /// <summary>
        /// Click event for Exit button (visible on admin page).
        /// </summary>
        private void btn_ExitProgram_Click(object sender, RoutedEventArgs e) {
            // Ask user for permission to exit program
            MessageBoxResult result = MessageBox.Show("Are you sure you want exit?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                Application.Current.Shutdown();
        }

        #endregion
        // . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .
        #region CHECK IN FORM BUTTONS
        // . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .
        
        /// <summary>
        /// Click event for Registration Code button on CheckIn page.
        /// </summary>
        private void btn_RegCode_Click(object sender, RoutedEventArgs e) {
            if (ValidateRegistrationCode(txtbx_RegCode.Text)) {
                // Populate form
                PopulateFormFromRegistrant(GetEditingRegistrant());
                validCodeEntered = true;
                // Allow user to verify that the code was correct
                MessageBoxResult result = MessageBox.Show("Is this the information you registered with?", "Pre-Reg Validation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No) {
                    ClearRegistrationForm();
                }
            } else {
                validCodeEntered = false;
                txtbx_RegCode.Text = "";
            }
        }

        /// <summary>
        /// Click event for Check-In button on CheckIn page.
        /// </summary>
        private void btn_Checkin_Click(object sender, RoutedEventArgs e) {
            if (ValidateRegistrationForms()) {
                // If entry already exists
                if (validCodeEntered) {
                    dbConnection.UpdateRegistrant(editingID, RegistrantFromForm());
                } else {
                    dbConnection.InsertRegistrant(RegistrantFromForm());
                }
                ClearRegistrationForm();
                txtbx_RegCode.Focus();
            }
        }

        private void btn_ForgotCode_Click(object sender, RoutedEventArgs e) {
            // Create admin window and display
            forgotcodeWindow = new Window_ForgotCode(this);
            forgotcodeWindow.Show();
            // Disable this window (until admin window closes)
            this.IsEnabled = false;
        }

        #endregion
        // . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .
        #region ADMIN FORM BUTTONS
        
        /// <summary>
        /// Click event for Search button on Admin page.
        /// </summary>
        private void btn_AdminEntriesSearch_Click(object sender, RoutedEventArgs e) {
            GetSearchResults(txtbx_AdminEntriesSearch.Text);
        }

        /// <summary>
        /// Click event for Edit Entry button on Admin page.
        /// </summary>
        private void btn_AdminEntriesEdit_Click(object sender, RoutedEventArgs e) {
            // Validate code
            if (ValidateRegistrationCode(txtbx_AdminEntriesCode.Text)) {
                // Set editingID, go to edit view, and populate form
                editingID = txtbx_AdminEntriesCode.Text;
                ChangeAppState(WindowView.Edit);
                RegistrantEntry registrant = GetEditingRegistrant();
                PopulateFormFromRegistrant(registrant);
            } else {
                // Otherwise, select code box
                txtbx_AdminEntriesCode.SelectAll();
                txtbx_AdminEntriesCode.Focus();
            }
        }

        /// <summary>
        /// Click event for Remove Entry button on Admin page.
        /// </summary>
        private void btn_AdminEntriesRemove_Click(object sender, RoutedEventArgs e) {
            // Validate code
            if (ValidateRegistrationCode(txtbx_AdminEntriesCode.Text)) {
                // Ask admin if this action is correct
                MessageBoxResult result = MessageBox.Show("Are you sure you want to remove this entry?", "Remove", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes) {
                    dbConnection.DeleteRegistrant(Convert.ToInt32(txtbx_AdminEntriesCode.Text));
                    MessageBox.Show("Entry successfully deleted.");
                    txtbx_AdminEntriesCode.Text = "";
                }
            } else {
                // If invalid code, select code
                txtbx_AdminEntriesCode.SelectAll();
                txtbx_AdminEntriesCode.Focus();
            }
        }

        /// <summary>
        /// Click event for Clear All Entries button on Admin page.
        /// </summary>
        private void btn_AdminEntriesClear_Click(object sender, RoutedEventArgs e) {
            // Ask admin if this action is correct
            MessageBoxResult result = MessageBox.Show("Are you sure you want to clear the database?\nTHIS CANNOT BE UNDONE!", "Clear Database", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes) {
                // Clear entry database
                MessageBox.Show("I'll do that later.");
                txtbx_AdminEntriesCode.Text = "";
            }
        }

        /// <summary>
        /// Click event for Import Entries button on Admin page.
        /// </summary>
        private void btn_AdminEntriesImport_Click(object sender, RoutedEventArgs e) {
            // Get File Name and Import
            string filename = ioXL.selectFile();
            ioXL.importExcel(filename);
        }

        /// <summary>
        /// Click event for Export Entries button on Admin page.
        /// </summary>
        private void btn_AdminEntriesExport_Click(object sender, RoutedEventArgs e) {
            // Export Entries based upon export type
        }

        #endregion
        // . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .
        #region EDIT FORM BUTTONS

        /// <summary>
        /// Click event for Confirm button on Edit page.
        /// </summary>
        private void btn_EditConfirm_Click(object sender, RoutedEventArgs e) {
            // If form is valid
            if (ValidateRegistrationForms()) {
                dbConnection.UpdateRegistrant(editingID, RegistrantFromForm());
                ChangeAppState(WindowView.Admin);
            }
        }

        /// <summary>
        /// Click event for Cancel button on Edit page.
        /// </summary>
        private void btn_EditCancel_Click(object sender, RoutedEventArgs e) {
            // Ask if user wants to discard edits
            MessageBoxResult result = MessageBox.Show("Cancel edits made?", "Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) {
                // If yes, return to Admin page
                ChangeAppState(WindowView.Admin);
            }
        }

        #endregion
        // . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .

        #endregion
        //---------------------------------------------------------------------------
        #region OTHER
        //---------------------------------------------------------------------------
        
        /// <summary>
        /// Check event of Registration Type radio buttons on form.
        /// </summary>
        private void radio_RegistrantType_Checked(object sender, RoutedEventArgs e) {
            // Change the Registration window to match the radio button selected
            ChangeSpecialView();
        }

        /// <summary>
        /// KeyDown event for Registration Code textbox on CheckIn page.
        /// </summary>
        private void txtbx_RegCode_PressEnter(object sender, KeyEventArgs e) {
            // Check Enter key
            if (e.Key == Key.Return) {
                // Simulate registration code button
                btn_RegCode_Click(sender, e);
            }
        }

        /// <summary>
        /// Change selection event for Admin Entries datagrid on Admin page.
        /// </summary>
        private void datagrid_AdminEntries_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            // Make sure selected index is valid (changes between searches)
            if (datagrid_AdminEntries.SelectedIndex >= searchEntries.Count - 1)
                return;
            // Sets admin code box to entry selected
            string code = searchEntries.ElementAt<RegistrantEntry>(datagrid_AdminEntries.SelectedIndex).Code;
            txtbx_AdminEntriesCode.Text = code.ToString();
        }

        /// <summary>
        /// KeyDown event for Search textbox on Admin page.
        /// </summary>
        private void txtbx_AdminEntriesSearch_KeyDown(object sender, KeyEventArgs e) {
            // Check Enter key
            if (e.Key == Key.Return) {
                // Simulate search button
                btn_AdminEntriesSearch_Click(sender, e);
            }
        }

        #endregion
        //---------------------------------------------------------------------------

        #endregion
        //===========================================================================
    }
}
