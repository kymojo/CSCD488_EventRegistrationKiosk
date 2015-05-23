using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace RegistrationKiosk {
    class RegistrantEntry {

        public enum RegistrantType { General, Student, Employee }
        public enum ClassStandingType { None, Freshman, Junior, Sophomore, Senior, PostBac, Graduate, Alumnus }
        public enum SexType { Male, Female }

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
        public RegistrantType RegType {
            get;
            set;
        }

        // Name
        public string Fname {
            get;
            set;
        }
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
        public string Phone {
            get { return BeautifyPhone(phoneNormal); }
            set { phoneNormal = NormalizePhone(value); }
        }
        
        #endregion
        // -------------------------
        #region Student Properties
        // -------------------------
        public ClassStandingType ClassStanding {
            get;
            set;
        }
        public string College {
            get;
            set;
        }
        public string Major {
            get;
            set;
        }
        public string StudentID {
            get;
            set;
        }
        public int GradYear {
            get;
            set;
        }
        
        #endregion
        // -------------------------
        #region Employee Properties
        // -------------------------
        public string Business {
            get;
            set;
        }
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
            GenerateHashCode();
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
        public void GenerateHashCode() {
            Code = HashFunction(Lname.ToLower() + Fname.ToLower() + phoneNormal);
        }
 
        /// <summary>
        /// A modified hashing function found here: http://stackoverflow.com/a/549352
        /// </summary>
        /// <param name="s">String to hash</param>
        /// <returns></returns>
        public static string HashFunction(string s)
        {
            uint hash = 0;
            // if you care this can be done much faster with unsafe 
            // using fixed char* reinterpreted as a byte*
            foreach (byte b in System.Text.Encoding.Unicode.GetBytes(s))
            {   
                hash += b;
                hash += (hash << 10);
                hash ^= (hash >> 6);    
            }
            // final avalanche
            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);
            // helpfully we only want positive integer < MUST_BE_LESS_THAN
            // so simple truncate cast is ok if not perfect
            string getstring = ((int)(hash % 1000000)).ToString("000000");
            return getstring;
        }

        public string NormalizePhone(string phone) {
            string result = phone;
            result = Regex.Replace(result, "[^0-9]+", "");
            if (result.Length == 11)
                result = result.Substring(1);
            return result;
        }

        public string BeautifyPhone(string phone) {
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
