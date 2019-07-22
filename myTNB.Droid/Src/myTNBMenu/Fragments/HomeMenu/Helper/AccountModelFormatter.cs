using System;
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
            return "RM " + amountDue;
        }

        public static string GetBillDueNote(int accountType, string amountDue, string dueDate)
        {
            string dueAmountNote;
            if (accountType == 2)
            {
                dueAmountNote = "Get by " + GetFormattedDate(dueDate, DATE_ORIGINAL_FORMAT, DATE_RESULT_FORMAT);
            }
            else
            {
                if (amountDue.Contains("-") || amountDue == "0.00")
                {
                    dueAmountNote = "All cleared";
                }
                else
                {
                    dueAmountNote = "Pay by " + GetFormattedDate(dueDate, DATE_ORIGINAL_FORMAT, DATE_RESULT_FORMAT);
                }
            }
            return dueAmountNote;
        }


    }
}
