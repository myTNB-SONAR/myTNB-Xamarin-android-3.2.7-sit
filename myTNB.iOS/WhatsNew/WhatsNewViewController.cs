using Foundation;
using myTNB.WhatsNew;
using System;
using UIKit;

namespace myTNB
{
    public partial class WhatsNewViewController : CustomUIViewController
    {
        public WhatsNewViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = WhatsNewConstants.Pagename_WhatsNew;
            base.ViewDidLoad();
        }
    }
}