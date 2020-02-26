using System;
using Java.Text;
using myTNB_Android.Src.Base.Helper;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom;

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
            DecimalFormat decimalFormatter = new DecimalFormat("###,###,###,###,##0.00", new DecimalFormatSymbols(Java.Util.Locale.Us));
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
                formattedValue = "RM " + decimalFormatter.Format(double.Parse(formattedValue));
            }
            return formattedValue;
        }

        public static string GetBillDueNote(int accountType, string amountDue, string dueDate, bool isTaggedSMR)
        {
            string dueAmountNote = "";
            if (accountType == 2)
            {
                if ((amountDue != null && amountDue != "") && (dueDate != null && dueDate != ""))
                {
                    double checkAmount = double.Parse(amountDue) * -1;
                    if (checkAmount <= 0.00)
                    {
                        if (checkAmount < 0.00)
                        {
                            dueAmountNote = Utility.GetLocalizedLabel("DashboardHome", "receivedExtra");
                        }
                        else
                        {
                            dueAmountNote = Utility.GetLocalizedLabel("DashboardHome", "allCleared");
                        }
                    }
                    else
                    {
                        dueAmountNote = Utility.GetLocalizedLabel("DashboardHome", "getBy") + " " + GetFormattedDate(dueDate, DATE_ORIGINAL_FORMAT, DATE_RESULT_FORMAT);
                    }
                }
                else
                {
                    dueAmountNote = Utility.GetLocalizedLabel("DashboardHome", "getBy") + " " + GetFormattedDate(dueDate, DATE_ORIGINAL_FORMAT, DATE_RESULT_FORMAT);
                }
            }
            else
            {
                if ((amountDue != null && amountDue != "") && (dueDate != null && dueDate != ""))
                {
                    if (double.Parse(amountDue) <= 0.00)
                    {
                        if (double.Parse(amountDue) < 0.00)
                        {
                            dueAmountNote = Utility.GetLocalizedLabel("DashboardHome", "paidExtra");
                        }
                        else
                        {
                            dueAmountNote = Utility.GetLocalizedLabel("DashboardHome", "allCleared");
                        }
                    }
                    else
                    {
                        dueAmountNote = Utility.GetLocalizedLabel("DashboardHome", "payBy") + " " + GetFormattedDate(dueDate, DATE_ORIGINAL_FORMAT, DATE_RESULT_FORMAT);
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
                IsNegativeAmountValue = double.Parse(amountDue) < 0.00;
            }
            return IsNegativeAmountValue;
        }

        public static bool IsAmountCleared(string amountDue)
        {
            bool IsAmountClearedValue = false;
            if (amountDue != null && amountDue != "")
            {
                IsAmountClearedValue = double.Parse(amountDue) == 0.00;
            }
            return IsAmountClearedValue;
        }
    }
}
