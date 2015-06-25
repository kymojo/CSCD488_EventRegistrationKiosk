using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows.Forms;
using System.Management;

namespace RegistrationKiosk{

    public class Printer{

        public void Print(RegistrantEntry registrant){
            // Prints a jobfair nametag

            var label = (DYMO.Label.Framework.ILabel)null;
            string text = registrant.Fname + " " + registrant.Lname + "\n\n";

            if (registrant.RegType.ToString() == "Student") {
                text += registrant.Major + "\n\n";
                text += registrant.College;
            }
            else if (registrant.RegType.ToString() == "Employee" ) {
                text += registrant.Job + "\n\n";
                text += registrant.Business;
            }
            else
                text += "Community Member";

            try {
                label = DYMO.Label.Framework.Label.Open("../../jobfair.label");
            }
            catch {
                MessageBox.Show("File 'jobfair.label' not found.");
                return;
            }
            finally {
                label.SetObjectText("Text", text);

                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");
                string printerName = "";

                foreach (ManagementObject printer in searcher.Get()) {
                    printerName = printer["Name"].ToString();
                    if (printerName.Equals(@"DYMO LabelWriter 450 DUO Label"))
                        if (printer["WorkOffline"].ToString().ToLower().Equals("true"))
                        {
                            MessageBox.Show("'DYMO LabelWriter 450 DUO Label' - Printer not found.");
                        }
                        else
                        {
                            try
                            {
                                label.Print("DYMO LabelWriter 450 DUO Label");
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("'DYMO LabelWriter 450 DUO Label' - Failed to print");
                            }
                        }
                }
            }
        }
    }
}
