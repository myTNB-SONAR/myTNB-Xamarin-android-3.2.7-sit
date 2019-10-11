using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class AccountListEmptyCell : UITableViewCell
    {
        public Func<string, string> GetI18NValue;
        private nfloat _cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        private UILabel _addAcctLbl;

        public AccountListEmptyCell(IntPtr handle) : base(handle)
        {
            BackgroundColor = UIColor.Clear;
            UIImageView addIcon = new UIImageView(new CGRect(ScaleUtility.BaseMarginWidth16, GetScaledHeight(28F), GetScaledWidth(16F), GetScaledHeight(16F)))
            {
                Image = UIImage.FromBundle(DashboardHomeConstants.Img_AddAcctIconWhite)
            };
            AddSubview(addIcon);

            _addAcctLbl = new UILabel(new CGRect(addIcon.Frame.GetMaxX() + GetScaledWidth(8F), GetScaledHeight(28F), _cellWidth - (addIcon.Frame.GetMaxX() + GetScaledWidth(8F) + ScaleUtility.BaseMarginWidth16), GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = UIColor.White,
                BackgroundColor = UIColor.Clear
            };
            AddSubview(_addAcctLbl);

            UIView lineView = new UIView(new CGRect(ScaleUtility.BaseMarginWidth16, GetYLocationFromFrame(_addAcctLbl.Frame, 28F), _cellWidth - GetScaledWidth(32), GetScaledHeight(1F)))
            {
                BackgroundColor = UIColor.FromWhiteAlpha(1, 0.2F)
            };
            AddSubview(lineView);
        }

        public void SetEmptyCell()
        {
            _addAcctLbl.Text = GetI18NValue(DashboardHomeConstants.I18N_AddElectricityAcct);
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
