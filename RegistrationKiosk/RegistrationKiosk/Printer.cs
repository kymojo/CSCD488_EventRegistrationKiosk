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

        /// <summary>
        /// Prints a jobfair nametag
        /// </summary>
        /// <param name="registrant">The registrant to print</param>
        public void Print(RegistrantEntry registrant){
            

            var label = (DYMO.Label.Framework.ILabel)null;
            string text = registrant.Fname + "\n" + registrant.Lname + "\n";

            if (registrant.RegType.ToString() == "Student") {
                text += registrant.Major + "\n";
                text += registrant.College;
            }
            else if (registrant.RegType.ToString() == "Employee" ) {
                text += registrant.Job + "\n";
                text += registrant.Business;
            }
            else
                text += "Community\nMember";

            try {
                label = DYMO.Label.Framework.Label.Open(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/jobfair.label");
            }
            catch {
                MessageBox.Show("File 'jobfair.label' not found in " + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));
                return;
            }
                        
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

                            MessageBox.Show("Thank you for registering!\n\n" +
                                            "Retrieve your name tag and enjoy the event!");
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
