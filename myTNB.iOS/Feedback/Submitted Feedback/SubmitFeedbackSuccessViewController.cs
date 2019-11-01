using Foundation;
using System;
using UIKit;
using CoreAnimation;
using CoreGraphics;
using System.Diagnostics;

namespace myTNB
{
    public partial class SubmitFeedbackSuccessViewController : UIViewController
    {
        public SubmitFeedbackSuccessViewController(IntPtr handle) : base(handle)
        {
        }

        UIButton _btnBackToFeedback;
        UILabel _lblFeedbackDateValue;
        UILabel _lblFeedbackIDValue;

        public string ServiceReqNo = string.Empty;
        public string DateCreated = string.Empty;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetupSuperViewBackground();
            InitilizedViews();
            SetEvents();
            this.NavigationController.NavigationBarHidden = true;
        }

        internal void SetupSuperViewBackground()
        {
            var startColor = MyTNBColor.GradientPurpleDarkElement;
            var endColor = MyTNBColor.GradientPurpleLightElement;
            CAGradientLayer gradientLayer = new CAGradientLayer
            {
                Colors = new[] { startColor.CGColor, endColor.CGColor },
                Locations = new NSNumber[] { 0, 1 },
                Frame = View.Bounds
            };
            View.Layer.InsertSublayer(gradientLayer, 0);
        }

        internal void InitilizedViews()
        {
            var viewContainer = new UIView((new CGRect(18, 36, View.Frame.Width - 36, 203)))
            {
                BackgroundColor = UIColor.White
            };
            viewContainer.Layer.CornerRadius = 4f;
            View.AddSubview(viewContainer);

            UIImageView imgViewCheck = new UIImageView(new CGRect((viewContainer.Frame.Width - 50) / 2, 16, 50, 50))
            {
                Image = UIImage.FromBundle("Circle-With-Check-Green")
            };
            viewContainer.AddSubview(imgViewCheck);

            var lblFeedback = new UILabel(new CGRect(0, 80, viewContainer.Frame.Width, 18))
            {
                Font = MyTNBFont.MuseoSans16_500,
                TextColor = MyTNBColor.PowerBlue,
                Text = "Feedback_Successful".Translate(),
                TextAlignment = UITextAlignment.Center
            };
            viewContainer.AddSubview(lblFeedback);

            var lblThankYou = new UILabel(new CGRect(0, 98, viewContainer.Frame.Width, 16))
            {
                Font = MyTNBFont.MuseoSans12_300,
                TextColor = MyTNBColor.TunaGrey(),
                Text = "Feedback_SuccessfulMessage".Translate(),
                TextAlignment = UITextAlignment.Center
            };
            viewContainer.AddSubview(lblThankYou);

            var viewLine = new UIView((new CGRect(14, 130, viewContainer.Frame.Width - 28, 1)))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };
            viewContainer.AddSubview(viewLine);

            var lblFeedbackDateTitleWidth = ((viewContainer.Frame.Width - 28) * 2) / 3;
            var lblFeedbackDateTitle = new UILabel(new CGRect(14, 147, lblFeedbackDateTitleWidth, 14))
            {
                Font = MyTNBFont.MuseoSans9_300,
                TextColor = MyTNBColor.SilverChalice,
                Text = "Feedback_DateTimeTitle".Translate().ToUpper(),
                TextAlignment = UITextAlignment.Left
            };
            viewContainer.AddSubview(lblFeedbackDateTitle);

            var lblFeedbackIDTitleWidth = (viewContainer.Frame.Width - 28) / 3;
            var lblFeedbackIDTitle = new UILabel(new CGRect(lblFeedbackDateTitleWidth + 14, 147, lblFeedbackIDTitleWidth, 14))
            {
                Font = MyTNBFont.MuseoSans9_300,
                TextColor = MyTNBColor.SilverChalice,
                Text = "Feedback_ID".Translate().ToUpper(),
                TextAlignment = UITextAlignment.Right
            };
            viewContainer.AddSubview(lblFeedbackIDTitle);

            _lblFeedbackDateValue = new UILabel(new CGRect(14, 161, lblFeedbackDateTitleWidth, 18))
            {
                Font = MyTNBFont.MuseoSans14_300,
                TextColor = MyTNBColor.TunaGrey()
            };
            string createdDate = GetDate();
            _lblFeedbackDateValue.Text = createdDate;
            _lblFeedbackDateValue.TextAlignment = UITextAlignment.Left;
            viewContainer.AddSubview(_lblFeedbackDateValue);

            _lblFeedbackIDValue = new UILabel(new CGRect(lblFeedbackDateTitleWidth + 14, 161, lblFeedbackIDTitleWidth, 18))
            {
                Font = MyTNBFont.MuseoSans14_300,
                TextColor = MyTNBColor.TunaGrey(),
                Text = ServiceReqNo,
                TextAlignment = UITextAlignment.Right
            };
            viewContainer.AddSubview(_lblFeedbackIDValue);

            //Back to Feedback Button
            _btnBackToFeedback = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, View.Frame.Height - DeviceHelper.GetScaledHeight(64), View.Frame.Width - 36, DeviceHelper.GetScaledHeight(48))
            };
            _btnBackToFeedback.SetTitle("Feedback_BackToFeedback".Translate(), UIControlState.Normal);
            _btnBackToFeedback.Font = MyTNBFont.MuseoSans16_500;
            _btnBackToFeedback.Layer.CornerRadius = 5.0f;
            _btnBackToFeedback.BackgroundColor = MyTNBColor.FreshGreen;
            View.AddSubview(_btnBackToFeedback);
        }

        internal void SetEvents()
        {
            _btnBackToFeedback.TouchUpInside += (sender, e) =>
            {
                DismissViewController(true, null);
            };
        }

        internal string GetDate()
        {
            try
            {
                string date = DateHelper.GetFormattedDate(DateCreated.Split(' ')[0], "dd MMM yyyy");
                string time = DateCreated.Split(' ')[1];
                int hr = int.Parse(time.Split(':')[0]);
                int min = int.Parse(time.Split(':')[1]);
                int sec = int.Parse(time.Split(':')[2]);

                TimeSpan timespan = new TimeSpan(hr, min, sec);
                DateTime dt = DateTime.Today.Add(timespan);
                string displayTime = dt.ToString("hh:mm tt");
                string formattedDate = date + " " + displayTime;
                return formattedDate;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return string.Empty;
            }
        }
    }
}