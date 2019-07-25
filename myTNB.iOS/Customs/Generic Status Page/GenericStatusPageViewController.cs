using System;
using System.Diagnostics;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB
{
    public partial class GenericStatusPageViewController : UIViewController
    {
        public GenericStatusPageViewController(IntPtr handle) : base(handle)
        {
        }

        public string ServiceRequestNumber = string.Empty;
        public string DateCreated = string.Empty;
        public bool IsSuccess;

        UIView viewContainer;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            foreach (var view in this.View.Subviews)
            {
                view.RemoveFromSuperview();
            }
            SetupSuperViewBackground();
            InitilizedViews();
            this.NavigationController.NavigationBarHidden = true;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        void SetupSuperViewBackground()
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

        void InitilizedViews()
        {
            nfloat yOffset = DeviceHelper.IsIphoneXUpResolution() ? 50 : 0;
            viewContainer = new UIView(new CGRect(18, 36 + yOffset, View.Frame.Width - 36, 203))
            {
                BackgroundColor = UIColor.White,
                Layer = { CornerRadius = 4f }
            };

            UIImageView imgViewStatus = new UIImageView(new CGRect((viewContainer.Frame.Width - 64) / 2, 16, 64, 64))
            {
                Image = UIImage.FromBundle(IsSuccess ? "Circle-With-Check-Green" : "Red-Cross")
            };

            UILabel lblFeedback = new UILabel(new CGRect(0, 80, viewContainer.Frame.Width, 18))
            {
                Font = IsSuccess ? MyTNBFont.MuseoSans16_500 : MyTNBFont.MuseoSans16,
                TextColor = MyTNBColor.PowerBlue,
                Text = IsSuccess ? "Feedback_Successful".Translate() : "Feedback_Unsuccessful".Translate(),
                TextAlignment = UITextAlignment.Center
            };
            if (IsSuccess)
            {
                UILabel lblThankYou = new UILabel(new CGRect(0, 98, viewContainer.Frame.Width, 16))
                {
                    Font = MyTNBFont.MuseoSans12_300,
                    TextColor = MyTNBColor.TunaGrey(),
                    Text = "Feedback_SuccessfulMessage".Translate(),
                    TextAlignment = UITextAlignment.Center
                };
                viewContainer.AddSubview(lblThankYou);
            }

            UIView viewLine = new UIView((new CGRect(14, IsSuccess ? 130 : 114, viewContainer.Frame.Width - 28, 1)))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };

            viewContainer.AddSubviews(new UIView[] { imgViewStatus, lblFeedback, viewLine });

            if (IsSuccess)
            {
                AddSuccessDetails();
            }/*
            else
            {
                AddFailedDetails();
            }*/

            View.AddSubview(viewContainer);
        }

        string GetDate()
        {
            if (string.IsNullOrEmpty(DateCreated) || string.IsNullOrWhiteSpace(DateCreated))
            {
                return TNBGlobal.EMPTY_DATE;
            }
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
                return TNBGlobal.EMPTY_DATE;
            }
        }

        void AddSuccessDetails()
        {
            var lblFeedbackDateTitleWidth = ((viewContainer.Frame.Width - 28) * 2) / 3;
            var lblFeedbackDateTitle = new UILabel(new CGRect(14, 147, lblFeedbackDateTitleWidth, 14))
            {
                Font = MyTNBFont.MuseoSans9_300,
                TextColor = MyTNBColor.SilverChalice,
                Text = "Feedback_DateTimeTitle".Translate().ToUpper(),
                TextAlignment = UITextAlignment.Left
            };

            var lblFeedbackIDTitleWidth = (viewContainer.Frame.Width - 28) / 3;
            var lblFeedbackIDTitle = new UILabel(new CGRect(lblFeedbackDateTitleWidth + 14, 147, lblFeedbackIDTitleWidth, 14))
            {
                Font = MyTNBFont.MuseoSans9_300,
                TextColor = MyTNBColor.SilverChalice,
                Text = "Feedback_ID".Translate().ToUpper(),
                TextAlignment = UITextAlignment.Right
            };

            string createdDate = GetDate();
            UILabel lblFeedbackDateValue = new UILabel(new CGRect(14, 161, lblFeedbackDateTitleWidth, 18))
            {
                Font = MyTNBFont.MuseoSans14_300,
                TextColor = MyTNBColor.TunaGrey(),
                Text = createdDate,
                TextAlignment = UITextAlignment.Left
            };

            UILabel _lblFeedbackIDValue = new UILabel(new CGRect(lblFeedbackDateTitleWidth + 14, 161, lblFeedbackIDTitleWidth, 18))
            {
                Font = MyTNBFont.MuseoSans14_300,
                TextColor = MyTNBColor.TunaGrey(),
                Text = ServiceRequestNumber ?? TNBGlobal.EMPTY_DATE,
                TextAlignment = UITextAlignment.Right
            };

            //Back to Feedback Button
            UIButton btnBackToFeedback = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, View.Frame.Height - DeviceHelper.GetScaledHeight(64), View.Frame.Width - 36, DeviceHelper.GetScaledHeight(48))
            };
            btnBackToFeedback.SetTitle("Feedback_BackToFeedback".Translate(), UIControlState.Normal);
            btnBackToFeedback.Font = MyTNBFont.MuseoSans16_500;
            btnBackToFeedback.Layer.CornerRadius = 5.0f;
            btnBackToFeedback.BackgroundColor = MyTNBColor.FreshGreen;
            btnBackToFeedback.TouchUpInside += (sender, e) =>
            {
                DismissViewController(true, null);
            };

            viewContainer.AddSubviews(new UIView[] { lblFeedbackDateTitle, lblFeedbackIDTitle
                , lblFeedbackDateValue, _lblFeedbackIDValue });

            View.AddSubview(btnBackToFeedback);
        }

        /*void AddFailedDetails()
        {
            viewContainer.AddSubviews(new UIView[] { new UILabel(new CGRect(0, 131, viewContainer.Frame.Width, 16))
                {
                    Font = MyTNBFont.MuseoSans12,
                    TextColor = MyTNBColor.TunaGrey(),
                    Text = "Feedback_UnsuccessfulMessage".Translate(),
                    TextAlignment = UITextAlignment.Center
                }, new UILabel(new CGRect(0, 150, viewContainer.Frame.Width, 16))
                {
                    Font = MyTNBFont.MuseoSans12,
                    TextColor = MyTNBColor.TunaGrey(),
                    Text = "Error_DefaultMessage".Translate(),
                    TextAlignment = UITextAlignment.Center
                }
            });

            //Back to Dashboard Button
            UIButton btnDashBoard = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, View.Frame.Height - 118, View.Frame.Width - 36, 48)
            };
            btnDashBoard.SetTitle("Common_BackToDashboard".Translate(), UIControlState.Normal);
            btnDashBoard.Layer.CornerRadius = 5.0f;
            btnDashBoard.Layer.BorderWidth = 1.0f;
            btnDashBoard.Layer.BorderColor = UIColor.White.CGColor;
            btnDashBoard.BackgroundColor = UIColor.Clear;
            btnDashBoard.TouchUpInside += (sender, e) =>
            {
                DismissViewController(true, null);
            };

            //Try Again Button
            UIButton btnTryAgain = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(18, View.Frame.Height - 64, View.Frame.Width - 36, 48)
            };
            btnTryAgain.SetTitle("Common_TryAgain".Translate(), UIControlState.Normal);
            btnTryAgain.Font = MyTNBFont.MuseoSans16;
            btnTryAgain.Layer.CornerRadius = 5.0f;
            btnTryAgain.BackgroundColor = MyTNBColor.FreshGreen;
            btnTryAgain.TouchUpInside += (sender, e) =>
            {
                this.NavigationController.NavigationBarHidden = false;
                NavigationController.PopViewController(false);
            };

            View.AddSubviews(new UIView[] { btnDashBoard, btnTryAgain });
        }*/
    }
}