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

            nfloat width = 320;
            nfloat height = 325;
            ScaleUtility.GetValuesFromAspectRatio(ref width, ref height);

            UIImageView imgBackground = new UIImageView(new CGRect(0, 0, width, height))
            {
                Image = displayImage
            };
            UILabel lblTitle = new UILabel(new CGRect(ScaleUtility.GetScaledWidth(16), ScaleUtility.GetYLocationFromFrame(imgBackground.Frame, 23)
                , that.View.Frame.Width - ScaleUtility.GetScaledWidth(32), ScaleUtility.GetScaledHeight(19)))
            {
                TextColor = MyTNBColor.PowerBlue,
                TextAlignment = UITextAlignment.Center,
                Font = TNBFont.MuseoSans_16_500,
                Text = SSMRDataObject.Title ?? string.Empty
            };
            UILabel lblDescription = new UILabel(new CGRect(ScaleUtility.GetScaledWidth(16), ScaleUtility.GetYLocationFromFrame(lblTitle.Frame, 16)
                , that.View.Frame.Width - ScaleUtility.GetScaledWidth(32), ScaleUtility.GetScaledHeight(80)))
            {
                TextColor = MyTNBColor.WarmGrey,
                TextAlignment = UITextAlignment.Center,
                Font = TNBFont.MuseoSans_12_300,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = SSMRDataObject.Description ?? string.Empty
            };

            CGSize size = GetLabelSize(lblDescription, that.View.Frame.Width - ScaleUtility.GetScaledWidth(32), ScaleUtility.GetScaledHeight(80));
            lblDescription.Frame = new CGRect(lblDescription.Frame.X, lblDescription.Frame.Y, lblDescription.Frame.Width, size.Height);
            that.View.AddSubviews(new UIView[] { imgBackground, lblTitle, lblDescription });
        }
    }
}