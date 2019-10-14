using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class AccountListEmptyCell : CustomUITableViewCell
    {
        private UILabel _addAcctLbl;

        public AccountListEmptyCell(IntPtr handle) : base(handle)
        {
            UIImageView addIcon = new UIImageView(new CGRect(BaseMarginWidth16, GetScaledHeight(28F), GetScaledWidth(16F), GetScaledHeight(16F)))
            {
                Image = UIImage.FromBundle(DashboardHomeConstants.Img_AddAcctIconWhite)
            };
            AddSubview(addIcon);

            _addAcctLbl = new UILabel(new CGRect(addIcon.Frame.GetMaxX() + GetScaledWidth(8F), GetScaledHeight(28F), _cellWidth - (addIcon.Frame.GetMaxX() + GetScaledWidth(8F) + BaseMarginWidth16), GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = UIColor.White,
                BackgroundColor = UIColor.Clear
            };
            AddSubview(_addAcctLbl);

            UIView lineView = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(_addAcctLbl.Frame, 28F), _cellWidth - GetScaledWidth(32), GetScaledHeight(1F)))
            {
                BackgroundColor = UIColor.FromWhiteAlpha(1, 0.2F)
            };
            AddSubview(lineView);
        }

        public void SetEmptyCell()
        {
            _addAcctLbl.Text = GetI18NValue(DashboardHomeConstants.I18N_AddElectricityAcct);
        }
    }
}
