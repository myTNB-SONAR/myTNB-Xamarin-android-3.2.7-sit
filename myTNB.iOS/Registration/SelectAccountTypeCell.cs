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
                lblAccountType.Font = myTNBFont.MuseoSans16_300();
                lblAccountType.TextColor = myTNBColor.TunaGrey();
                return lblAccountType;
            }
        }
    }
}