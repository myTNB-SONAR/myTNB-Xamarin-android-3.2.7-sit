using System;
using System.Diagnostics;
using System.Globalization;
using CoreGraphics;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public class AccountListCell : UITableViewCell
    {
        public Func<string, string> GetI18NValue;
        private nfloat _cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        UILabel _nickname, _accountNumber, _amountDue, _dueDate;
        UIImageView _imgRe;
        public AccountListCell(IntPtr handle) : base(handle)
        {
            BackgroundColor = UIColor.Clear;
            _nickname = new UILabel(new CGRect(ScaleUtility.BaseMarginWidth16, GetScaledHeight(12F), _cellWidth * 0.60F, GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = UIColor.White,
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            AddSubview(_nickname);

            _imgRe = new UIImageView(new CGRect(0, GetScaledHeight(14F), GetScaledWidth(12F), GetScaledHeight(12F)))
            {
                Image = UIImage.FromBundle("RE-Leaf"),
                Hidden = true
            };
            AddSubview(_imgRe);

            _accountNumber = new UILabel(new CGRect(ScaleUtility.BaseMarginWidth16, GetYLocationFromFrame(_nickname.Frame, 4F), _cellWidth * 0.50F, GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = UIColor.FromWhiteAlpha(1, 0.60F)
            };
            AddSubview(_accountNumber);

            _amountDue = new UILabel(new CGRect(_cellWidth - ScaleUtility.BaseMarginWidth16 - (_cellWidth * 0.40F), GetScaledHeight(12F), _cellWidth * 0.40F, GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Right
            };
            AddSubview(_amountDue);

            _dueDate = new UILabel(new CGRect(_cellWidth - ScaleUtility.BaseMarginWidth16 - (_cellWidth * 0.50F), GetYLocationFromFrame(_amountDue.Frame, 4F), _cellWidth * 0.50F, GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_300,
                TextColor = UIColor.FromWhiteAlpha(1, 0.60F),
                TextAlignment = UITextAlignment.Right
            };
            AddSubview(_dueDate);

            UIView lineView = new UIView(new CGRect(ScaleUtility.BaseMarginWidth16, GetYLocationFromFrame(_accountNumber.Frame, 12F), _cellWidth - GetScaledWidth(32), GetScaledHeight(1F)))
            {
                BackgroundColor = UIColor.FromWhiteAlpha(1, 0.2F)
            };
            AddSubview(lineView);
        }

        public void SetAccountCell(DueAmountDataModel model)
        {
            if (model != null)
            {
                _nickname.Text = model.accNickName;
                _accountNumber.Text = model.accNum;

                var amount = !model.IsReAccount ? model.amountDue : ChartHelper.UpdateValueForRE(model.amountDue);
                var absAmount = Math.Abs(amount);
                _amountDue.AttributedText = TextHelper.CreateValuePairString(absAmount.ToString("N2", CultureInfo.InvariantCulture)
                    , TNBGlobal.UNIT_CURRENCY + " ", true, TNBFont.MuseoSans_12_500
                    , UIColor.White, TNBFont.MuseoSans_12_500, UIColor.White);
                var dateString = amount > 0 ? model.billDueDate : string.Empty;
                if (string.IsNullOrEmpty(dateString) || dateString.ToUpper().Equals("N/A"))
                {
                    _dueDate.Text = amount < 0 ? GetI18NValue(DashboardHomeConstants.I18N_PaidExtra) : GetI18NValue(DashboardHomeConstants.I18N_AllCleared);
                    _dueDate.TextColor = UIColor.FromWhiteAlpha(1, 0.60F);
                    _amountDue.TextColor = amount < 0 ? MyTNBColor.LightGreenBlue : UIColor.White;
                }
                else
                {
                    _dueDate.TextColor = UIColor.FromWhiteAlpha(1, 0.60F);
                    _amountDue.TextColor = UIColor.White;

                    string datePrefix = model.IsReAccount ? GetI18NValue(DashboardHomeConstants.I18N_GetBy) : GetI18NValue(DashboardHomeConstants.I18N_PayBy);
                    if (model.IsReAccount && model.IncrementREDueDateByDays > 0)
                    {
                        try
                        {
                            var format = @"dd/MM/yyyy";
                            DateTime due = DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);
                            due = due.AddDays(model.IncrementREDueDateByDays);
                            dateString = due.ToString(format);
                        }
                        catch (FormatException)
                        {
                            Debug.WriteLine("Unable to parse '{0}'", dateString);
                        }
                    }
                    string formattedDate = DateHelper.GetFormattedDate(dateString, "dd MMM");
                    _dueDate.AttributedText = TextHelper.CreateValuePairString(formattedDate
                    , datePrefix + " ", true, TNBFont.MuseoSans_12_300
                    , UIColor.FromWhiteAlpha(1, 0.60F), TNBFont.MuseoSans_12_300, UIColor.FromWhiteAlpha(1, 0.60F));
                }

                if (model.IsReAccount)
                {
                    _imgRe.Hidden = false;
                    CGSize nameSize = _nickname.SizeThatFits(new CGSize(1000F, 1000F));
                    ViewHelper.AdjustFrameSetWidth(_nickname, nameSize.Width);
                    ViewHelper.AdjustFrameSetX(_imgRe, _nickname.Frame.GetMaxX() + GetScaledWidth(4F));
                }
            }
        }

        private nfloat GetScaledHeight(nfloat value)
        {
            return ScaleUtility.GetScaledHeight(value);
        }

        private nfloat GetScaledWidth(nfloat value)
        {
            return ScaleUtility.GetScaledWidth(value);
        }

        private nfloat GetYLocationFromFrame(CGRect frame, nfloat value)
        {
            return ScaleUtility.GetYLocationFromFrame(frame, value);
        }
    }
}
