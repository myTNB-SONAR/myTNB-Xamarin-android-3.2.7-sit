using Foundation;

using System;
using UIKit;

namespace myTNB
{
    public partial class AppUpdateViewController : UIViewController
    {
        public AppUpdateViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var titleStr = @"<p style=""text-align:center;""><strong>" + "NewUpdateTitle".Translate() + "</strong></p>";
            NSError htmlError = null;
            NSAttributedString attrString = TextHelper.ConvertToHtmlWithFont(titleStr, ref htmlError,
                                                                                         myTNBFont.FONTNAME_300, 14f);
            lblTitle.AttributedText = attrString;
            lblTitle.TextColor = myTNBColor.TunaGrey();

            lblMessage.Font = myTNBFont.MuseoSans14_300();
            lblMessage.TextColor = myTNBColor.TunaGrey();
            lblMessage.Lines = 0;

            btnUpdate.SetTitle("NewUpdateAction".Translate(), UIControlState.Normal);
            btnUpdate.SetTitleColor(myTNBColor.PowerBlue(), UIControlState.Normal);
            btnUpdate.Font = myTNBFont.MuseoSans16_300();
            btnUpdate.BackgroundColor = UIColor.Clear;
            btnUpdate.TouchUpInside += OpenUpdateLink;
        }

        /// <summary>
        /// Opens the update link.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="args">Arguments.</param>
        private void OpenUpdateLink(object sender, EventArgs args)
        {
            int index = DataManager.DataManager.SharedInstance.WebLinks?.FindIndex(x => x.Code.ToLower().Equals("ios")) ?? -1;
            if (index > -1)
            {
                string url = DataManager.DataManager.SharedInstance.WebLinks[index].Url;
                if (!string.IsNullOrWhiteSpace(url))
                {
                    if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                    {
                        UIApplication.SharedApplication.OpenUrl(new NSUrl(url), new NSDictionary(), null);
                    }
                    else
                    {
                        UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
                    }

                }
            }

        }
    }
}