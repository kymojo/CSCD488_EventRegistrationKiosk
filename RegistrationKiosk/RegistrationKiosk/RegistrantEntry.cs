using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace RegistrationKiosk {

    public class RegistrantEntry {

        //===========================================================================
        #region Enums
        //===========================================================================

        public enum RegistrantType { General, Student, Employee }
        public enum ClassStandingType { None, Freshman, Junior, Sophomore, Senior, PostBac, Graduate, Alumnus }
        public enum SexType { Male, Female }

        #endregion
        //===========================================================================
        #region Properties
        //===========================================================================

        // -------------------------
        #region General Properties
        // -------------------------

        // General
        public string Code {
            get;
            set;
        }

        //Registrant Type
        public RegistrantType RegType {
            get;
            set;
        }

        // Name
        public string Fname {
            get;
            set;
        }

        // last name
        public string Lname {
            get;
            set;
        }

        // Sex
        public SexType Sex {
            get;
            set;
        }

        //Contact Info
        private string phoneNormal;
        public string Email {
            get;
            set;
        }
        //Phone
        public string Phone {
            get { return FormatPhone(phoneNormal); }
            set { phoneNormal = NormalizePhone(value); }
        }
        
        #endregion
        // -------------------------
        #region Student Properties
        // -------------------------

        //Class standing
        public ClassStandingType ClassStanding {
            get;
            set;
        }
        //College
        public string College {
            get;
            set;
        }
        //Major
        public string Major {
            get;
            set;
        }
        //Student ID
        public string StudentID {
            get;
            set;
        }
        //graduation year
        public int GradYear {
            get;
            set;
        }
        
        #endregion
        // -------------------------
        #region Employee Properties
        // -------------------------

        //business
        public string Business {
            get;
            set;
        }
        //job
        public string Job {
            get;
            set;
        }
        
        #endregion
        // -------------------------

        #endregion
        //===========================================================================
        #region Constructor
        //===========================================================================

        /// <summary>
        /// A constructor for a blank registrant.
        /// </summary>
        public RegistrantEntry() { }

        /// <summary>
        /// A constructor for a general registrant
        /// </summary>
        /// <param name="lname">Last Name</param>
        /// <param name="fname">First Name</param>
        /// <param name="sex">Sex (Male, Female)</param>
        /// <param name="email">Email Address</param>
        /// <param name="phone">Phone Number</param>
        public RegistrantEntry(string lname, string fname, SexType sex, string email, string phone) {
            this.Lname = lname;
            this.Fname = fname;
            this.Sex = sex;
            this.Email = email;
            this.Phone = phone;
            SetTypeGeneral();
            GenerateCode();
        }

        #endregion
        //===========================================================================
        #region Methods
        //===========================================================================

        #region SET REGISTRATION TYPE
        
        public void SetTypeStudent(ClassStandingType classStanding, string college, string major, string studentID, int gradYear) {
            this.ClassStanding = classStanding;
            this.College = college;
            this.Major = major;
            this.StudentID = studentID;
            this.GradYear = gradYear;
            this.RegType = RegistrantType.Student;
        }

        public void SetTypeEmployee(string business, string job) {
            this.Business = business;
            this.Job = job;
            this.RegType = RegistrantType.Employee;
        }

        public void SetTypeGeneral() {
            this.RegType = RegistrantType.General;
        }

        #endregion

        public string GetQueryString() {
            return "";
        }

        /// <summary>
        /// Returns a six-digit integer for database lookup of the registrant.
        /// </summary>
        /// <returns>Six-digit hash code</returns>
        public void GenerateCode() {
            Random r = new Random(this.GetHashCode());
            Code = r.Next(1000000).ToString("000000");
        }
 
        /// <summary>
        /// A modified hashing function found here: http://stackoverflow.com/a/549352
        /// </summary>
        /// <param name="s">String to hash</param>
        /// <returns></returns>
        public static string HashFunction(string s)
        {
            SecurityMeans security = new SecurityMeans();
            string hash = security.GetMd5Hash(s);
            hash = hash.Substring(0, 5);
            return hash;
        }

        /// <summary>
        /// Normalizes the phone number string to 10 digit number
        /// </summary>
        /// <param name="phone">Phone number</param>
        /// <returns>Normalized phone number</returns>
        public string NormalizePhone(string phone) {
            string result = phone;
            result = Regex.Replace(result, "[^0-9]+", "");
            if (result.Length == 11)
                result = result.Substring(1);
            return result;
        }

        /// <summary>
        /// Changes normalized phone number into (xxx) xxx-xxxx format.
        /// </summary>
        /// <param name="phone">Normalized phone number</param>
        /// <returns>Formatted phone (or original if error)</returns>
        public static string FormatPhone(string phone) {
            try {
                double num = Convert.ToDouble(phone);
                string result = String.Format("{0:(###) ###-####}", num);
                return result;
            } catch (Exception) {
                return phone;
            }
            
        }

        #endregion
        //===========================================================================
    }
}
