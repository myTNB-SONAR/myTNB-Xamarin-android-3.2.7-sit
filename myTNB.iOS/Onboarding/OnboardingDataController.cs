using System;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public class OnboardingDataController : BaseDataViewController
    {
        public OnboardingDataController(UIPageViewController controller) : base(controller) { }
       
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
        }

        public override void SetBackground()
        {
            that.View.BackgroundColor = UIColor.Clear;
        }

        public override void OnViewDidLayoutSubViews()
        {
            SetBackground();
        }

        public override void SetSubViews()
        {
            const float padding = 10f;
            const float inlineMargin = 18.7f;
            const float imgSize = 150f;             UIImage imgLogo;             if (DataObject.IsSitecoreData)             {                 if (string.IsNullOrEmpty(DataObject.ImageName) || string.IsNullOrWhiteSpace(DataObject.ImageName))                 {                     imgLogo = UIImage.FromBundle(string.Empty);                 }                 else                 {
                    try
                    {
                        imgLogo = UIImage.LoadFromData(NSData.FromUrl(new NSUrl(DataObject.ImageName)));
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Image load Error: " + e.Message);
                        imgLogo = UIImage.FromBundle(string.Empty);
                    }                 }             }             else             {
                if (string.IsNullOrEmpty(DataObject.ImageName) || string.IsNullOrWhiteSpace(DataObject.ImageName))
                {
                    imgLogo = UIImage.FromBundle(string.Empty);
                }
                else
                {
                    imgLogo = UIImage.FromBundle(DataObject.ImageName);
                }             }

            UIView viewContainer = new UIView(new CGRect(0, (that.View.Frame.Height - 310) / 2, that.View.Frame.Width, 280))
            {
                BackgroundColor = UIColor.Clear
            };

            UIImageView imgViewLogo = new UIImageView(new CGRect((viewContainer.Frame.Width - imgSize) / 2, padding, imgSize, imgSize))
            {
                Image = imgLogo
            };              if (!imgViewLogo.IsDescendantOfView(viewContainer))             {                 viewContainer.AddSubview(imgViewLogo);             }             else             {                 imgViewLogo.RemoveFromSuperview();             }

            UILabel lblTitle = new UILabel(new CGRect(padding, imgViewLogo.Frame.GetMaxY() + inlineMargin, viewContainer.Frame.Width - (padding * 2), 30))
            {
                Font = MyTNBFont.MuseoSans24_500,
                Text = DataObject.Title,
                TextColor = MyTNBColor.SunGlow,
                TextAlignment = UITextAlignment.Center
            };

            UILabel lblMessage = new UILabel(new CGRect(padding, lblTitle.Frame.GetMaxY(), viewContainer.Frame.Width - (padding * 2), 50))
            {
                Font = MyTNBFont.MuseoSans16_300,
                Text = DataObject.Message,
                Lines = 0,
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center
            };             viewContainer.AddSubviews(new UIView[] { lblTitle, lblMessage });

            that.View.AddSubview(viewContainer);
        }
    }
}
