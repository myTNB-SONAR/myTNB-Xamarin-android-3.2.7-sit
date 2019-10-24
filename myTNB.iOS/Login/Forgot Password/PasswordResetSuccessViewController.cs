using Foundation; using System; using UIKit; using CoreAnimation; using CoreGraphics; using myTNB.Login.ForgotPassword;

namespace myTNB {     public partial class PasswordResetSuccessViewController : CustomUIViewController     {         UIButton btnLogin;         UIView viewContainer;          public bool IsChangePassword = false;         public PasswordResetSuccessViewController(IntPtr handle) : base(handle) { }         public string EmailAddress = string.Empty;          public override void ViewDidLoad()         {             PageName = ForgotPasswordConstants.Pagename_ResetSuccess;             base.ViewDidLoad();              SetupSuperViewBackground();             if (IsChangePassword)
            {                 InitializedSuccessSubviews();             }
            else
            {                 InitializeTemporaryPasswordSubview();             }             SetEvents();         }          public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);             NavigationController.NavigationBar.Hidden = true;
        }          private void SetupSuperViewBackground()         {             UIColor startColor = MyTNBColor.GradientPurpleDarkElement;             UIColor endColor = MyTNBColor.GradientPurpleLightElement;              CAGradientLayer gradientLayer = new CAGradientLayer();             gradientLayer.Colors = new[] { startColor.CGColor, endColor.CGColor };             gradientLayer.Locations = new NSNumber[] { 0, 1 };             gradientLayer.Frame = View.Bounds;             View.Layer.InsertSublayer(gradientLayer, 0);         }          private void InitializedSuccessSubviews()         {             viewContainer = new UIView((new CGRect(18, DeviceHelper.GetScaledHeight(36)                 , View.Frame.Width - 36, DeviceHelper.GetScaledHeight(235))));             viewContainer.Layer.CornerRadius = 4.0f;             viewContainer.BackgroundColor = UIColor.White;              UIImageView imgViewSuccess = new UIImageView(UIImage.FromBundle("Circle-With-Check-Green"))             {                 Frame = new CGRect((View.Frame.Width / 2) - DeviceHelper.GetScaledWidth(25)                     , DeviceHelper.GetScaledHeight(63), DeviceHelper.GetScaledWidth(50), DeviceHelper.GetScaledHeight(50)),                 ContentMode = UIViewContentMode.ScaleAspectFill             };              UILabel lblPasswordSuccess = new UILabel             {                 Frame = new CGRect(18, DeviceHelper.GetScaledHeight(123), View.Frame.Width - 36, 18),                 AttributedText = new NSAttributedString(GetI18NValue(ForgotPasswordConstants.I18N_Title)                     , font: MyTNBFont.MuseoSans16_500                     , foregroundColor: MyTNBColor.PowerBlue                     , strokeWidth: 0),                 TextAlignment = UITextAlignment.Center,             };              UIView viewLine = new UIView((new CGRect(32, DeviceHelper.GetScaledHeight(160), View.Frame.Width - 64, 1)));             viewLine.BackgroundColor = MyTNBColor.PlatinumGrey;              UILabel lblDescription = new UILabel             {                 Frame = new CGRect(30, DeviceHelper.GetScaledHeight(173), View.Frame.Width - 60, 54),                 AttributedText = new NSAttributedString(GetI18NValue(ForgotPasswordConstants.I18N_ResetSuccessMessage)                     , font: MyTNBFont.MuseoSans14_300                     , foregroundColor: MyTNBColor.TunaGrey()                     , strokeWidth: 0),                 Lines = 0,                 LineBreakMode = UILineBreakMode.WordWrap,                 TextAlignment = UITextAlignment.Center,             };              btnLogin = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, View.Frame.Height - DeviceHelper.GetScaledHeight(72)                 , View.Frame.Width - 36, DeviceHelper.GetScaledHeight(48)),
                Font = MyTNBFont.MuseoSans16_500,
                BackgroundColor = MyTNBColor.FreshGreen
            };              btnLogin.Layer.CornerRadius = 4.0f;             btnLogin.SetTitle(GetCommonI18NValue(Constants.Common_Login), UIControlState.Normal);              View.AddSubview(viewContainer);             View.AddSubview(imgViewSuccess);             View.AddSubview(lblPasswordSuccess);             View.AddSubview(viewLine);             View.AddSubview(lblDescription);             View.AddSubview(btnLogin);         }          private void InitializeTemporaryPasswordSubview()
        {             viewContainer = new UIView((new CGRect(18, DeviceHelper.GetScaledHeight(36)                 , View.Frame.Width - 36, DeviceHelper.GetScaledHeight(255))));             viewContainer.Layer.CornerRadius = 2.0f;             viewContainer.BackgroundColor = UIColor.White;              UIImageView imgSuccess = new UIImageView(new CGRect((viewContainer.Frame.Width / 2) - DeviceHelper.GetScaledWidth(25)                 , DeviceHelper.GetScaledHeight(24), DeviceHelper.GetScaledWidth(50), DeviceHelper.GetScaledHeight(50)));             imgSuccess.Image = UIImage.FromBundle("Circle-With-Check-Green");              UILabel lblTitle = new UILabel(new CGRect(0, DeviceHelper.GetScaledHeight(93), viewContainer.Frame.Width, 18))
            {
                Font = MyTNBFont.MuseoSans16_500,
                TextColor = MyTNBColor.PowerBlue,
                Text = GetI18NValue(ForgotPasswordConstants.I18N_Title),
                TextAlignment = UITextAlignment.Center
            };              UIView viewLine = new UIView((new CGRect(14, DeviceHelper.GetScaledHeight(123), viewContainer.Frame.Width - 28, 1)))
            {
                BackgroundColor = MyTNBColor.PlatinumGrey
            };              UILabel lblDetails = new UILabel(new CGRect(12, DeviceHelper.GetScaledHeight(135), viewContainer.Frame.Width - 24, 0))
            {
                Font = MyTNBFont.MuseoSans14_300,
                TextColor = MyTNBColor.TunaGrey(),
                Text = string.Format(GetI18NValue(ForgotPasswordConstants.I18N_Details), EmailAddress),
                TextAlignment = UITextAlignment.Center,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };              CGSize newSize = GetLabelSize(lblDetails, lblDetails.Frame.Width, 150);             lblDetails.Frame = new CGRect(lblDetails.Frame.X, lblDetails.Frame.Y                                           , lblDetails.Frame.Width, newSize.Height);              btnLogin = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, View.Frame.Height - DeviceHelper.GetScaledHeight(72)
                    , View.Frame.Width - 36, DeviceHelper.GetScaledHeight(48)),
                Font = MyTNBFont.MuseoSans16_500,
                BackgroundColor = MyTNBColor.FreshGreen
            };              btnLogin.Layer.CornerRadius = 4.0f;
            btnLogin.SetTitle(GetI18NValue(ForgotPasswordConstants.I18N_ProceedToLogin), UIControlState.Normal);              viewContainer.AddSubviews(new UIView[] { imgSuccess, lblTitle, viewLine, lblDetails });             View.AddSubviews(new UIView[] { viewContainer, btnLogin });         }          private void SetEvents()         {             btnLogin.TouchUpInside += (sender, e) =>
            {
                DismissViewController(true, null);             };         }     } }