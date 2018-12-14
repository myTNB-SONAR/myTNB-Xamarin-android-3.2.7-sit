using Foundation;
using System;
using UIKit;
using myTNB.Model;
using CoreGraphics;

namespace myTNB
{
    public partial class OnboardingDataViewController : UIPageViewController
    {
        UIImageView imgViewLogo;
        UILabel lblTitle;
        UILabel lblMessage;

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

        internal void SetupSubViews()         {             UIImage imgLogo;             if (DataObject.IsSitecoreData)             {                 if (string.IsNullOrEmpty(DataObject.ImageName) || string.IsNullOrWhiteSpace(DataObject.ImageName))                 {                     imgLogo = UIImage.FromBundle(string.Empty);                 }                 else                 {                     imgLogo = UIImage.LoadFromData(NSData.FromUrl(new NSUrl(DataObject.ImageName)));                 }             }             else             {                 imgLogo = UIImage.FromBundle(DataObject.ImageName);             }              imgViewLogo = new UIImageView(new CGRect((View.Frame.Width - 148) / 2, 92, 148, 147));             imgViewLogo.Image = imgLogo;              if (!imgViewLogo.IsDescendantOfView(View))             {                 View.AddSubview(imgViewLogo);             }             else             {                 imgViewLogo.RemoveFromSuperview();             }              lblTitle = new UILabel(new CGRect(42, 258, View.Frame.Width - 84, 44));             lblTitle.Font = myTNBFont.MuseoSans24();             lblTitle.Text = DataObject.Title;             lblTitle.TextColor = myTNBColor.SunGlow();             lblTitle.TextAlignment = UITextAlignment.Center;             View.AddSubview(lblTitle);              lblMessage = new UILabel(new CGRect(42, 322, View.Frame.Width - 84, 72));             lblMessage.Font = myTNBFont.MuseoSans16();             lblMessage.Text = DataObject.Message;             lblMessage.Lines = 3;             lblMessage.TextColor = UIColor.White;             lblMessage.TextAlignment = UITextAlignment.Center;             View.AddSubview(lblMessage);
        }

    }
}