using System;
using System.Text.RegularExpressions;

namespace myTNB_Android.Src.Utils
{
    public class Utility
    {
        public Utility()
        {
        }


        public static bool AccountNumberValidation(int length) {           
                return (length == 12 || length == 14);
        }

        public static bool AddAccountNumberValidation(int length)
        {
            return (length == 12);
        }

        public static bool isAlphaNumeric(string strToCheck)
        {
            Regex rg = new Regex(@"^[a-zA-Z0-9\s,]*$");
            return rg.IsMatch(strToCheck);
        }

        public static bool isAlpha(string strToCheck)
        {
            Regex rg = new Regex(@"^[a-zA-Z\s,]*$");
            return rg.IsMatch(strToCheck);
        }


        public static bool IsValidMobileNumber(string mobileNumber) {
            if (!string.IsNullOrEmpty(mobileNumber)) {
                if (mobileNumber.StartsWith("+60")) {
                    if (mobileNumber.Substring(3).Length == 9 || mobileNumber.Substring(3).Length == 10) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
