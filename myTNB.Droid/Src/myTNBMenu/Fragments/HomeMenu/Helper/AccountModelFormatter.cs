using System;
using Java.Text;
using myTNB_Android.Src.Base.Helper;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Helper
{
    public class AccountModelFormatter : ModelFormatter
    {
        const string DATE_ORIGINAL_FORMAT = "dd/MM/yyyy";
        const string DATE_RESULT_FORMAT = "dd MMM";
        private AccountModelFormatter()
        {
        }

        public static string GetFormatAmount(string amountDue)
        {
            DecimalFormat decimalFormatter = new DecimalFormat("###,###,###,###,##0.00");
            string formattedValue = "";
            if (amountDue != null && amountDue != "")
            {
                if (amountDue.Contains("-"))
                {
                    formattedValue = amountDue.Replace("-", "");
                }
                else
                {
                    formattedValue = amountDue;
                }
                formattedValue = "RM " + decimalFormatter.Format(float.Parse(formattedValue));
            }
            return formattedValue;
        }

        public static string GetBillDueNote(int accountType, string amountDue, string dueDate, bool isTaggedSMR)
        {
            string dueAmountNote = "";
            if (accountType == 2)
            {
                dueAmountNote = "get by " + GetFormattedDate(dueDate, DATE_ORIGINAL_FORMAT, DATE_RESULT_FORMAT);
            }
            else
            {
                if ((amountDue != null && amountDue != "") && (dueDate != null && dueDate != ""))
                {
                    if (float.Parse(amountDue) < 0f)
                    {
                        dueAmountNote = "paid extra";
                    }
                    else if (float.Parse(amountDue) == 0f)
                    {
                        dueAmountNote = "all cleared";
                    }
                    else
                    {
                        dueAmountNote = "pay by " + GetFormattedDate(dueDate, DATE_ORIGINAL_FORMAT, DATE_RESULT_FORMAT);
                    }
                }
            }
            return dueAmountNote;
        }

        public static bool IsNegativeAmount(string amountDue)
        {
            bool IsNegativeAmountValue = false;
            if (amountDue != null && amountDue != "")
            {
                IsNegativeAmountValue = float.Parse(amountDue) < 0f;
            }
            return IsNegativeAmountValue;
        }

        public static bool IsAmountCleared(string amountDue)
        {
            bool IsAmountClearedValue = false;
            if (amountDue != null && amountDue != "")
            {
                IsAmountClearedValue = float.Parse(amountDue) == 0f;
            }
            return IsAmountClearedValue;
        }
    }
}
