using System;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB.SSMR
{
    public class SSMROnboardingDataController : BaseDataViewController
    {
        public SSMROnboardingDataController(UIPageViewController controller) : base(controller) { }

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
            UIImage displayImage;             if (SSMRDataObject.IsSitecoreData)             {                 if (string.IsNullOrEmpty(SSMRDataObject.Image) || string.IsNullOrWhiteSpace(SSMRDataObject.Image))                 {                     displayImage = UIImage.FromBundle(string.Empty);                 }                 else                 {
                    try
                    {
                        displayImage = UIImage.LoadFromData(NSData.FromUrl(new NSUrl(SSMRDataObject.Image)));
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Image load Error: " + e.Message);
                        displayImage = UIImage.FromBundle(string.Empty);
                    }                 }             }             else             {
                if (string.IsNullOrEmpty(SSMRDataObject.Image) || string.IsNullOrWhiteSpace(SSMRDataObject.Image))
                {
                    displayImage = UIImage.FromBundle(string.Empty);
                }
                else
                {
                    displayImage = UIImage.FromBundle(SSMRDataObject.Image);
                }             }

            UIImageView imgBackground = new UIImageView(new CGRect(0, 0, that.View.Frame.Width, that.View.Frame.Height * 0.60F))
            {
                Image = displayImage
            };
            UILabel lblTitle = new UILabel(new CGRect(26, imgBackground.Frame.GetMaxY() + 23, that.View.Frame.Width - 52, 19))
            {
                TextColor = MyTNBColor.PowerBlue,
                TextAlignment = UITextAlignment.Center,
                Font = MyTNBFont.MuseoSans16_500,
                Text = SSMRDataObject.Title ?? string.Empty
            };
            UILabel lblDescription = new UILabel(new CGRect(26, lblTitle.Frame.GetMaxY() + 16, that.View.Frame.Width - 52, 80))
            {
                TextColor = MyTNBColor.WarmGrey,
                TextAlignment = UITextAlignment.Center,
                Font = MyTNBFont.MuseoSans12_300,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = SSMRDataObject.Description ?? string.Empty
            };

            CGSize size = GetLabelSize(lblDescription, that.View.Frame.Width - 32, 80);
            lblDescription.Frame = new CGRect(26, lblTitle.Frame.GetMaxY() + 16, that.View.Frame.Width - 52, size.Height);
            that.View.AddSubviews(new UIView[] { imgBackground, lblTitle, lblDescription });
        }
    }
}