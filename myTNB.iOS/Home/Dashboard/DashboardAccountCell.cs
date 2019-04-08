using myTNB.Model;
using System;
using System.Diagnostics;
using System.Globalization;
using UIKit;

namespace myTNB
{
    public partial class DashboardAccountCell : UITableViewCell
    {
        public DashboardAccountCell(IntPtr handle) : base(handle)
        {
        }

        public DashboardAccountCell(string cellId) : base(UITableViewCellStyle.Default, cellId)
        {
        }

        public void UpdateCell(DueAmountDataModel model)
        {
            UpdateStyle();

            imgRe.Hidden = !model.IsReAccount;
            lblAccountTitle.Text = model.accNickName;
            lblAccountSubTitle.Text = model.accNum;

            var amount = !model.IsReAccount ? model.amountDue : ChartHelper.UpdateValueForRE(model.amountDue);
            lblAmountTitle.AttributedText = TextHelper.CreateValuePairString(amount.ToString("N2", CultureInfo.InvariantCulture)
                , TNBGlobal.UNIT_CURRENCY + " ", true, myTNBFont.MuseoSans14_500()
                , UIColor.White, myTNBFont.MuseoSans14_500(), UIColor.White);

            string formattedDate = string.Empty;

            var dateString = amount > 0 ? model.billDueDate : string.Empty;
            if (string.IsNullOrEmpty(dateString) || dateString.ToUpper().Equals("N/A"))
            {
                formattedDate = TNBGlobal.EMPTY_DATE;
            }
            else
            {
                if (model.IsReAccount && model.IncrementREDueDateByDays > 0)
                {
                    try
                    {
                        var format = @"dd/MM/yyyy";
                        DateTime due = DateTime.ParseExact(dateString, format, System.Globalization.CultureInfo.InvariantCulture);
                        due = due.AddDays(model.IncrementREDueDateByDays);
                        dateString = due.ToString(format);
                    }
                    catch (FormatException)
                    {
                        Debug.WriteLine("Unable to parse '{0}'", dateString);
                    }
                }
                formattedDate = DateHelper.GetFormattedDate(dateString, "dd MMM");
            }
            lblAmountSubTitle.Text = formattedDate;
        }

        private void UpdateStyle()
        {
            lblAccountTitle.Font = myTNBFont.MuseoSans14_500();
            lblAccountTitle.TextColor = UIColor.White;

            lblAccountSubTitle.Font = myTNBFont.MuseoSans12_300();
            lblAccountSubTitle.TextColor = new UIColor(red: 1.0f, green: 1.0f, blue: 1.0f, alpha: 0.7f);

            lblAmountTitle.Font = myTNBFont.MuseoSans14_500();
            lblAmountTitle.TextColor = UIColor.White;

            lblAmountSubTitle.Font = myTNBFont.MuseoSans12_300();
            lblAmountSubTitle.TextColor = new UIColor(red: 1.0f, green: 1.0f, blue: 1.0f, alpha: 0.7f);

            //viewLine.BackgroundColor = UIColor.FromWhiteAlpha(1, 0.3f);
        }
    }
}