using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace RegistrationKiosk {
    class RegistrantEntry {

        public enum RegistrantType { General, Student, Employee }
        public enum ClassStanding { None, Freshman, Junior, Sophomore, Senior, PostBac, Graduate, Alumnus }
        public enum Sex { Male, Female }

        //===========================================================================
        #region Properties
        //===========================================================================

        // -------------------------
        #region General Properties
        // -------------------------
        public string code {
            get;
            set;
        }
        public RegistrantType regType {
            get;
            set;
        }
        public string fname {
            get;
            set;
        }
        public string lname {
            get;
            set;
        }
        public Sex sex {
            get;
            set;
        }
        public string email {
            get;
            set;
        }
        public string phone {
            get;
            set;
        }
        
        #endregion
        // -------------------------
        #region Student Properties
        // -------------------------
        public ClassStanding classStanding {
            get;
            set;
        }
        public string college {
            get;
            set;
        }
        public string major {
            get;
            set;
        }
        public string studentID {
            get;
            set;
        }
        public int gradYear {
            get;
            set;
        }
        
        #endregion
        // -------------------------
        #region Employee Properties
        // -------------------------
        public string business {
            get;
            set;
        }
        public string job {
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
        public RegistrantEntry(string lname, string fname, Sex sex, string email, string phone) {
            this.lname = lname;
            this.fname = fname;
            this.sex = sex;
            this.email = email;
            this.phone = phone;
            SetTypeGeneral();
            GenerateHashCode();
        }

        #endregion
        //===========================================================================
        #region Methods
        //===========================================================================

        #region SET REGISTRATION TYPE
        
        public void SetTypeStudent(ClassStanding classStanding, string college, string major, string studentID, int gradYear) {
            this.classStanding = classStanding;
            this.college = college;
            this.major = major;
            this.studentID = studentID;
            this.gradYear = gradYear;
            this.regType = RegistrantType.Student;
        }

        public void SetTypeEmployee(string business, string job) {
            this.business = business;
            this.job = job;
            this.regType = RegistrantType.Employee;
        }

        public void SetTypeGeneral() {
            this.regType = RegistrantType.General;
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
            code = HashFunction(lname + fname + phone);
        }
 
        /// <summary>
        /// A modified hashing function found here: http://stackoverflow.com/a/549352
        /// </summary>
        /// <param name="s">String to hash</param>
        /// <returns></returns>
        public string HashFunction(string s)
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

        #endregion
        //===========================================================================
    }
}
