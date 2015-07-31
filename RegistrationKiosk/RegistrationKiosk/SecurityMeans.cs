using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace RegistrationKiosk {

    [Serializable]
    public class SecurityMeans {

        //===========================================================================
        #region Class Properties
        //===========================================================================

        public string AdminPass { get; set; }
        public string DbHost { get; set; }
        public string DbName { get; set; }
        public string DbUser { get; set; }
        public string DbPass { get; set; }
        public int DbPort { get; set; }

        #endregion
        //===========================================================================
        #region Class Constructor
        //===========================================================================
        public SecurityMeans() {
            // Default settings
            AdminPass = GetMd5Hash("pass");
            DbHost = "";
            DbName = "";
            DbUser = "";
            DbPass = "";
            DbPort = 3306;
        }
        #endregion
        //===========================================================================
        #region Class Methods
        //===========================================================================

        //---------------------------------------------------------------------------
        #region MD5
        //---------------------------------------------------------------------------
        /// <summary>
        /// Hashes string. From example at: https://msdn.microsoft.com/en-us/library/s02tk69a%28v=vs.110%29.aspx
        /// </summary>
        /// <param name="md5Hash">Instance of MD5 for hashing</param>
        /// <param name="input">Input string to be hashed</param>
        /// <returns>String of hash</returns>
        public string GetMd5Hash(string input) {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BytesToString(data);
        }

        /// <summary>
        /// Verifies hash. From example at: https://msdn.microsoft.com/en-us/library/s02tk69a%28v=vs.110%29.aspx
        /// </summary>
        /// <param name="md5Hash">Instance of MD5 for hashing</param>
        /// <param name="input">Input string to be hashed</param>
        /// <param name="hash">String of hash to check against</param>
        /// <returns></returns>
        public bool VerifyMd5Hash(string input, string hash) {
            // Hash the input. 
            string hashOfInput = GetMd5Hash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash)) {
                return true;
            } else {
                return false;
            }
        }

        #endregion
        //---------------------------------------------------------------------------
        #region Security Settings
        //---------------------------------------------------------------------------

        /// <summary>
        /// Sets the admin password if the given password is correct
        /// </summary>
        /// <param name="password">Old password for verification</param>
        /// <param name="newPass">New password to have</param>
        /// <returns>Success flag</returns>
        public bool SetAdminPassword(string password, string newPass) {
            bool success = false;
            if (CheckAdminPassword(password)) {
                AdminPass = GetMd5Hash(newPass);
                success = true;
            }
            return success;
        }

        /// <summary>
        /// Verifies whether or not a given password is valid
        /// </summary>
        /// <param name="password"></param>
        /// <returns>IsValid flag</returns>
        public bool CheckAdminPassword(string password) {
            return VerifyMd5Hash(password, AdminPass);
        }

        #endregion
        //---------------------------------------------------------------------------
        #region Other

        /// <summary>
        /// Turns byte array into string. From example at: https://msdn.microsoft.com/en-us/library/s02tk69a%28v=vs.110%29.aspx
        /// </summary>
        /// <param name="data">Byte array to be turned into string</param>
        /// <returns>String from byte array</returns>
        public string BytesToString(byte[] data) {
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++) {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        #endregion
        //---------------------------------------------------------------------------

        #endregion
        //===========================================================================
    }
}
