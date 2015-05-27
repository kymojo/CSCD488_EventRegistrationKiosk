using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows.Forms;
using DYMO.Label.Framework;
using DYMO.Label.Framework.Com;
using System.Drawing;

namespace RegistrationKiosk{

    public class Printer{

        public void Print(RegistrantEntry registrant){
            // Prints a jobfair nametag
            
            var label = DYMO.Label.Framework.Label.Open(Directory.GetCurrentDirectory() + "/jobfair.label");
            
            try {
                if (registrant.Major != null)
                    label.SetObjectText("Title", registrant.Major);

                label.SetObjectText("Fname", registrant.Fname);
                label.SetObjectText("Lname", registrant.Lname);

                if (registrant.College != null)
                    label.SetObjectText("College", registrant.College);
            }
            catch {
                MessageBox.Show("File 'jobfair.label' not found.");
            }

            try {
                label.Print("DYMO LabelWriter 450 DUO Label");
            }
            catch(Exception) {
                MessageBox.Show("'DYMO LabelWriter 450 DUO Label' - Printer not found.");
            }
            
        }

    }
}
