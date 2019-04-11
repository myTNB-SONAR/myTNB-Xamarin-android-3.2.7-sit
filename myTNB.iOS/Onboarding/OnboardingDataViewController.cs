using Foundation;
using System;
using UIKit;
using myTNB.Model;
using CoreGraphics;
using System.Diagnostics;

namespace myTNB
{
    public partial class OnboardingDataViewController : UIPageViewController
    {
        UIView viewContainer;
        UIImageView imgViewLogo;
        UILabel lblTitle;
        UILabel lblMessage;
        const float padding = 10f;
        const float inlineMargin = 18.7f;
        const float imgSize = 150f;

        public OnboardingModel DataObject
        {
            get; set;
        }

        protected OnboardingDataViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib
            SetupSubViews();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            SetupSuperViewBackground();
        }

        internal void SetupSuperViewBackground()
        {
            //var startColor = myTNBColor.GradientPurpleDarkElement();
            //var endColor = myTNBColor.GradientPurpleLightElement();

            //var gradientLayer = new CAGradientLayer();
            //gradientLayer.Colors = new[] { startColor.CGColor, endColor.CGColor };
            //gradientLayer.Locations = new NSNumber[] { 0, 1 };
            //gradientLayer.Frame = View.Bounds;

            //View.Layer.InsertSublayer(gradientLayer, 0);
            View.BackgroundColor = UIColor.Clear;

        }

        internal void SetupSubViews()         {             UIImage imgLogo;             if (DataObject.IsSitecoreData)             {                 if (string.IsNullOrEmpty(DataObject.ImageName) || string.IsNullOrWhiteSpace(DataObject.ImageName))                 {                     imgLogo = UIImage.FromBundle(string.Empty);                 }                 else                 {
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
            viewContainer = new UIView(new CGRect(0, (View.Frame.Height - 310) / 2, View.Frame.Width, 280));
            viewContainer.BackgroundColor = UIColor.Clear;
             imgViewLogo = new UIImageView(new CGRect((viewContainer.Frame.Width - imgSize) / 2, padding, imgSize, imgSize));             imgViewLogo.Image = imgLogo;              if (!imgViewLogo.IsDescendantOfView(viewContainer))             {                 viewContainer.AddSubview(imgViewLogo);             }             else             {                 imgViewLogo.RemoveFromSuperview();             }              lblTitle = new UILabel(new CGRect(padding, imgViewLogo.Frame.GetMaxY() + inlineMargin, viewContainer.Frame.Width - (padding * 2), 30));             lblTitle.Font = myTNBFont.MuseoSans24_500();             lblTitle.Text = DataObject.Title;             lblTitle.TextColor = myTNBColor.SunGlow();             lblTitle.TextAlignment = UITextAlignment.Center;             viewContainer.AddSubview(lblTitle);              lblMessage = new UILabel(new CGRect(padding, lblTitle.Frame.GetMaxY(), viewContainer.Frame.Width - (padding * 2), 50));             lblMessage.Font = myTNBFont.MuseoSans16_300();             lblMessage.Text = DataObject.Message;             lblMessage.Lines = 0;             lblMessage.TextColor = UIColor.White;             lblMessage.TextAlignment = UITextAlignment.Center;             viewContainer.AddSubview(lblMessage);

            View.AddSubview(viewContainer);
        }

    }
}