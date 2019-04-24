using System;

using UIKit;

namespace myTNB
{
    public partial class GenericSelectorViewController : UITableViewController
    {
        public string HeaderTitle = string.Empty;

        public GenericSelectorViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            this.Title = HeaderTitle ?? string.Empty;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

