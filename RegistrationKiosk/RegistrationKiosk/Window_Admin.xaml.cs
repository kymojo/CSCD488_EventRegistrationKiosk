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
    /// Interaction logic for Window_Admin.xaml
    /// </summary>
    public partial class Window_Admin : Window {
        
        private Window_Main main = null;
        SecurityMeans security = new SecurityMeans();

        //===========================================================================
        #region Window Initialize
        //===========================================================================
        public Window_Admin(Window_Main main) {
            this.main = main;
            InitializeComponent();
            pass_Admin.Focus();
        }
        #endregion
        //===========================================================================
        #region Window Events
        //===========================================================================

        /// <summary>
        /// Click event for Cancel button.
        /// </summary>
        private void btn_AdminCancel_Click(object sender, RoutedEventArgs e) {
            main.IsEnabled = true;
            this.Close();
        }

        /// <summary>
        /// Click event for Okay button.
        /// </summary>
        private void btn_AdminOk_Click(object sender, RoutedEventArgs e) {
            // If file doesn't exist, set pass to default "pass"
            if (!File.Exists("../../security.txt")) {
                try {
                    string[] lines = {
                                         "Admin Pass: 1a1dc91c907325c69271ddf0c944bc72",
                                         "Db Host: cscd379.com",
                                         "Db Port: 3306",
                                         "Db Name: jobfair",
                                         "Db User: jobfair",
                                         "Db Pass: 068797696407d2f65f89b82ec5aad84e"
                                     };
                    File.WriteAllLines("../../security.txt", lines);
                } catch { }
            }
            // Check password
            try {
                // Open file and read password
                string[] lines = File.ReadAllLines("../../security.txt");
                string hash = lines[0].Substring(12);
                string pass = pass_Admin.Password;
                // Verify Password
                if (security.VerifyMd5Hash(pass, hash)) {
                    main.IsEnabled = true;
                    main.GotoAdminPage();
                    this.Close();
                } else {
                    MessageBox.Show("Invalid Password!");
                    pass_Admin.Focus();
                    pass_Admin.Password = "";
                }
            } catch { }
        }

        /// <summary>
        /// KeyDown event for password boxes.
        /// </summary>
        private void pass_Admin_PressEnter(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                btn_AdminOk_Click(sender, e);
            }
        }

        #endregion
        //===========================================================================
    }
}
