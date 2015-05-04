using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DYMO
{
    public class Printer
    {
        [STAThread]
        static void Main(string[] args)
        {
            IOExcel ioe = new IOExcel();

            Console.WriteLine("Select .xlsx file.");
            string filename = ioe.selectFile();
            ioe.importExcel(filename);

            Console.WriteLine("Enter anything to quit.");
            Console.ReadLine();
            
        }

        public void Print(string firstName, string lastName)
        {
            // Prints
            /*var label = DYMO.Label.Framework.Label.Open("C:/Users/Kevin/Documents/Visual Studio 2013/WebSites/DYMO/LargeAddressTestLabel.label");
            label.SetObjectText("Address", "DYMOn828 San Pablo AvenAlbany CA 94706");
            label.Print("DYMO LabelWriter 450 DUO Label");*/
        }

    }
}
