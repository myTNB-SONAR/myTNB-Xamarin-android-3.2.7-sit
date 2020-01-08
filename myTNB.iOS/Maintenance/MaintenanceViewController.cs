using System;
using CoreGraphics;
using Foundation;
using myTNB.Dashboard.DashboardComponents;
using UIKit;

namespace myTNB.Maintenance
{
    public partial class MaintenanceViewController : CustomUIViewController
    {
        private UIView maintenanceView;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            CreateMaintenanceView();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        private void CreateMaintenanceView()
        {
            var response = AppLaunchMasterCache.GetAppLaunchResponse();
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, (float)UIScreen.MainScreen.Bounds.Height, false);
            maintenanceView = gradientViewComponent.GetUI();
            float screenWidth = (float)UIApplication.SharedApplication.KeyWindow.Frame.Width;
            float imageWidth = DeviceHelper.GetScaledWidth(151f);
            float imageHeight = DeviceHelper.GetScaledHeight(136f);
            float labelWidth = screenWidth - 40f;
            float lineTextHeight = 24f;

            UIImageView imageView = new UIImageView(UIImage.FromBundle(AppLaunchConstants.IMG_MaintenanceIcon))
            {
                Frame = new CGRect(DeviceHelper.GetCenterXWithObjWidth(imageWidth), DeviceHelper.GetScaledHeightWithY(90f), imageWidth, imageHeight)
            };

            var titleMsg = response?.d?.DisplayTitle;
            var descMsg = response?.d?.DisplayMessage;

            titleMsg = !string.IsNullOrEmpty(titleMsg) ? titleMsg : LanguageUtility.GetCommonI18NValue(Constants.Common_MaintenanceTitle);
            descMsg = !string.IsNullOrEmpty(descMsg) ? descMsg : LanguageUtility.GetCommonI18NValue(Constants.Common_MaintenanceMsg);

            UILabel lblTitle = new UILabel(new CGRect(DeviceHelper.GetCenterXWithObjWidth(labelWidth), imageView.Frame.GetMaxY() + 24f, labelWidth, 44f))
            {
                Text = titleMsg,
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.SunGlow,
                Font = MyTNBFont.MuseoSans24_500,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

            nfloat newTitleHeight = lblTitle.GetLabelHeight(1000);
            lblTitle.Frame = new CGRect(lblTitle.Frame.Location, new CGSize(lblTitle.Frame.Width, newTitleHeight));

            NSMutableParagraphStyle msgParagraphStyle = new NSMutableParagraphStyle
            {
                Alignment = UITextAlignment.Center,
                MinimumLineHeight = lineTextHeight,
                MaximumLineHeight = lineTextHeight
            };

            UIStringAttributes msgAttributes = new UIStringAttributes
            {
                Font = MyTNBFont.MuseoSans16_300,
                ForegroundColor = UIColor.White,
                BackgroundColor = UIColor.Clear,
                ParagraphStyle = msgParagraphStyle
            };

            var attributedText = new NSMutableAttributedString(descMsg);
            attributedText.AddAttributes(msgAttributes, new NSRange(0, descMsg.Length));

            UILabel lblDesc = new UILabel()
            {
                AttributedText = attributedText,
                Lines = 0
            };

            CGSize cGSize = lblDesc.SizeThatFits(new CGSize(labelWidth, 1000f));
            lblDesc.Frame = new CGRect(DeviceHelper.GetCenterXWithObjWidth(labelWidth), lblTitle.Frame.GetMaxY() + 8f, labelWidth, cGSize.Height);

            maintenanceView.AddSubviews(new UIView[] { imageView, lblTitle, lblDesc });
            if (!maintenanceView.IsDescendantOfView(View))
            {
                View.AddSubview(maintenanceView);
            }
        }
    }
}