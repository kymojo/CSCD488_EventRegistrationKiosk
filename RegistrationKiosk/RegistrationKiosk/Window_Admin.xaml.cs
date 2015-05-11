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

namespace RegistrationKiosk {
    /// <summary>
    /// Interaction logic for RegistrationKiosk.xaml
    /// </summary>
    public partial class Window_Admin : Window {
        
        private Window_Main main = null;

        //===========================================================================
        #region Initialize Window
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
        private void btn_AdminCancel_Click(object sender, RoutedEventArgs e) {
            main.IsEnabled = true;
            this.Close();
        }

        private void btn_AdminOk_Click(object sender, RoutedEventArgs e) {
            if (pass_Admin.Password == "pass") {
                main.IsEnabled = true;
                main.RunAdminDelegate();
                this.Close();
            } else {
                MessageBox.Show("Invalid Password!\n(Default password is 'pass')");
                txtbx_Admin.Focus();
            }
        }
        
        private void pass_Admin_PressEnter(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                btn_AdminOk_Click(sender, e);
            }
        }
        #endregion
        //===========================================================================
    }
}
