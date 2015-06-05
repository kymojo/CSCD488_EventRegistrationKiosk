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
        }
        #endregion
        //===========================================================================
        #region Window Events
        //===========================================================================

        private void btn_Connect_Click(object sender, RoutedEventArgs e) {
            // Set connection variables
            MessageBox.Show("I'll do that later.");
            //main.dbConnection.SetConnection(txtbx_Host.Text, txtbx_Db.Text, txtbx_User.Text, pass_Pass.Password, Convert.ToInt32(txtbx_Port.Text));
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e) {
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


        #endregion
        //===========================================================================
    }
}
