using System;
using UIKit;

namespace myTNB
{
    public partial class SelectAccountTypeCell : UITableViewCell
    {
        public SelectAccountTypeCell(IntPtr handle) : base(handle)
        {
        }

        public UILabel AccountTypeLabel
        {
            get
            {
                lblAccountType.Font = MyTNBFont.MuseoSans16_300;
                lblAccountType.TextColor = MyTNBColor.TunaGrey();
                return lblAccountType;
            }
        }
    }
}