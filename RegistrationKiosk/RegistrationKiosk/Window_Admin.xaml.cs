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
            // Check password
            if (main.GetSecurity().CheckAdminPassword(pass_Admin.Password)) {
                main.IsEnabled = true;
                main.GotoAdminPage();
                this.Close();
            } else {
                MessageBox.Show("Invalid Password!");
                pass_Admin.Focus();
                pass_Admin.Password = "";
            }
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
