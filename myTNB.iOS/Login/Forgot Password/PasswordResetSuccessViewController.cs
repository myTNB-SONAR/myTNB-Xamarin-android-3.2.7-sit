﻿using Foundation; using System; using UIKit; using CoreAnimation; using CoreGraphics; using System.Drawing;
 
namespace myTNB {     public partial class PasswordResetSuccessViewController : UIViewController     {         UIButton btnLogin;         UIView viewContainer;          public bool IsChangePassword = false;         public PasswordResetSuccessViewController(IntPtr handle) : base(handle) { }         public string EmailAddress = string.Empty;          public override void ViewDidLoad()         {             base.ViewDidLoad();              SetupSuperViewBackground();             if (IsChangePassword)
            {                 InitializedSuccessSubviews();             }
            else
            {                 InitializeTemporaryPasswordSubview();             }             SetEvents();         }          public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);             NavigationController.NavigationBar.Hidden = true;
        }          internal void SetupSuperViewBackground()         {             var startColor = myTNBColor.GradientPurpleDarkElement();             var endColor = myTNBColor.GradientPurpleLightElement();              var gradientLayer = new CAGradientLayer();             gradientLayer.Colors = new[] { startColor.CGColor, endColor.CGColor };             gradientLayer.Locations = new NSNumber[] { 0, 1 };             gradientLayer.Frame = View.Bounds;              View.Layer.InsertSublayer(gradientLayer, 0);         }          internal void InitializedSuccessSubviews()         {             viewContainer = new UIView((new CGRect(18, DeviceHelper.GetScaledHeight(36), View.Frame.Width - 36, DeviceHelper.GetScaledHeight(235))));             viewContainer.Layer.CornerRadius = 4.0f;             viewContainer.BackgroundColor = UIColor.White;              var imgViewClose = new UIImageView(UIImage.FromBundle("Delete"))             {                 Frame = new CGRect(View.Frame.Width - 59, DeviceHelper.IsIphoneXUpResolution() ? 76 : 52, 25, 25),                 ContentMode = UIViewContentMode.ScaleAspectFill             };              var imgViewSuccess = new UIImageView(UIImage.FromBundle("Circle-With-Check-Green"))             {                 Frame = new CGRect((View.Frame.Width / 2) - DeviceHelper.GetScaledWidth(25), DeviceHelper.GetScaledHeight(63), DeviceHelper.GetScaledWidth(50), DeviceHelper.GetScaledHeight(50)),                 ContentMode = UIViewContentMode.ScaleAspectFill             };              var lblPasswordSuccess = new UILabel             {                 Frame = new CGRect(18, DeviceHelper.GetScaledHeight(123), View.Frame.Width - 36, 18),                 AttributedText = new NSAttributedString(                              "Password Reset Successful",                                font: myTNBFont.MuseoSans16_500(),                              foregroundColor: myTNBColor.PowerBlue(),                              strokeWidth: 0                             ),                 TextAlignment = UITextAlignment.Center,             };              UIView viewLine = new UIView((new CGRect(32, DeviceHelper.GetScaledHeight(160), View.Frame.Width - 64, 1)));             viewLine.BackgroundColor = myTNBColor.PlatinumGrey();              var lblDescription = new UILabel             {                 Frame = new CGRect(30, DeviceHelper.GetScaledHeight(173), View.Frame.Width - 60, 54),                 AttributedText = new NSAttributedString(                     "Your password has been changed successfully. You may proceed to login with the new password.",                    font: myTNBFont.MuseoSans14_300(),                  foregroundColor: myTNBColor.TunaGrey(),                  strokeWidth: 0                 ),                 Lines = 0,                 LineBreakMode = UILineBreakMode.WordWrap,                 TextAlignment = UITextAlignment.Center,             };              btnLogin = new UIButton(UIButtonType.Custom);             btnLogin.Frame = new CGRect(18, View.Frame.Height - DeviceHelper.GetScaledHeight(72), View.Frame.Width - 36, DeviceHelper.GetScaledHeight(48));             btnLogin.SetTitle("Login", UIControlState.Normal);             btnLogin.Font = myTNBFont.MuseoSans16_500();             btnLogin.Layer.CornerRadius = 4.0f;             btnLogin.BackgroundColor = myTNBColor.FreshGreen();              View.AddSubview(viewContainer);             //View.AddSubview(imgViewClose);             View.AddSubview(imgViewSuccess);             View.AddSubview(lblPasswordSuccess);             View.AddSubview(viewLine);             View.AddSubview(lblDescription);             View.AddSubview(btnLogin);              UITapGestureRecognizer tap = new UITapGestureRecognizer(() =>             {                 this.DismissViewController(false, null);             });             imgViewClose.AddGestureRecognizer(tap);         }          void InitializeTemporaryPasswordSubview()
        {             viewContainer = new UIView((new CGRect(18, DeviceHelper.GetScaledHeight(36), View.Frame.Width - 36, DeviceHelper.GetScaledHeight(255))));             viewContainer.Layer.CornerRadius = 2.0f;             viewContainer.BackgroundColor = UIColor.White;              UIImageView imgSuccess = new UIImageView(new CGRect((viewContainer.Frame.Width / 2) - DeviceHelper.GetScaledWidth(25), DeviceHelper.GetScaledHeight(24), DeviceHelper.GetScaledWidth(50), DeviceHelper.GetScaledHeight(50)));             imgSuccess.Image = UIImage.FromBundle("Circle-With-Check-Green");              UILabel lblTitle = new UILabel(new CGRect(0, DeviceHelper.GetScaledHeight(93), viewContainer.Frame.Width, 18));             lblTitle.Font = myTNBFont.MuseoSans16_500();             lblTitle.TextColor = myTNBColor.PowerBlue();             lblTitle.Text = "Password Reset Successful";             lblTitle.TextAlignment = UITextAlignment.Center;              UIView viewLine = new UIView((new CGRect(14, DeviceHelper.GetScaledHeight(123), viewContainer.Frame.Width - 28, 1)));             viewLine.BackgroundColor = myTNBColor.PlatinumGrey();              string details = string.Format("Log in using the 8-digit temporary\r\npassword that was sent to\r\n{0}." +                                            "\r\n\r\nYou may change your password\r\nafter logging in.", EmailAddress);              UILabel lblDetails = new UILabel(new CGRect(12, DeviceHelper.GetScaledHeight(135), viewContainer.Frame.Width - 24, 0));             lblDetails.Font = myTNBFont.MuseoSans14_300();             lblDetails.TextColor = myTNBColor.TunaGrey();             lblDetails.Text = details;             lblDetails.TextAlignment = UITextAlignment.Center;             lblDetails.Lines = 0;             lblDetails.LineBreakMode = UILineBreakMode.WordWrap;              CGSize newSize = GetLabelSize(lblDetails, lblDetails.Frame.Width, 150);             lblDetails.Frame = new CGRect(lblDetails.Frame.X, lblDetails.Frame.Y                                           , lblDetails.Frame.Width, newSize.Height);              btnLogin = new UIButton(UIButtonType.Custom);             btnLogin.Frame = new CGRect(18, View.Frame.Height - DeviceHelper.GetScaledHeight(72), View.Frame.Width - 36, DeviceHelper.GetScaledHeight(48));             btnLogin.SetTitle("ProceedToLogin".Translate(), UIControlState.Normal);             btnLogin.Font = myTNBFont.MuseoSans16_500();             btnLogin.Layer.CornerRadius = 4.0f;             btnLogin.BackgroundColor = myTNBColor.FreshGreen();              viewContainer.AddSubviews(new UIView[] { imgSuccess, lblTitle, viewLine, lblDetails });              View.AddSubviews(new UIView[] { viewContainer, btnLogin });         }          internal void SetEvents()         {             btnLogin.TouchUpInside += (sender, e) =>
            {
                this.DismissViewController(true, null);             };         }          CGSize GetLabelSize(UILabel label, nfloat width, nfloat height)         {             return label.Text.StringSize(label.Font, new SizeF((float)width, (float)height));         }     } }