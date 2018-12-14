using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreGraphics;
using myTNB.Dashboard.DashboardComponents;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public partial class RatingViewController : UIViewController
    {
        public RatingViewController(IntPtr handle) : base(handle)
        {
        }

        TextFieldHelper _textFieldHelper = new TextFieldHelper();
        Dictionary<int, string> _rateValueDictionary = new Dictionary<int, string>(){
            {1, "Very Bad"}
            , {2, "Poor"}
            , {3, "Ok"}
            , {4, "Good"}
            , {5, "Excellent"}
        };

        UIView _viewRating;
        UILabel _lblRateTitle;
        UITextField _txtFieldComments;

        string _message = string.Empty;

        public int Rating = 5;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetNavigationBar();
            SetSubViews();
        }

        void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle("Rating");
            titleBarComponent.SetNotificationVisibility(true);
            titleBarComponent.SetBackVisibility(true);
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        void SetSubViews()
        {
            UILabel lblTitle = new UILabel(new CGRect(18, 104, View.Frame.Width - 36, 18));
            lblTitle.Font = myTNBFont.MuseoSans16();
            lblTitle.TextColor = myTNBColor.PowerBlue();
            lblTitle.TextAlignment = UITextAlignment.Center;
            lblTitle.Text = "Please rate your payment experience.";

            _lblRateTitle = new UILabel(new CGRect(42, 146, View.Frame.Width - 82, 16));
            _lblRateTitle.Font = myTNBFont.MuseoSans12();
            _lblRateTitle.TextColor = myTNBColor.TunaGrey();
            _lblRateTitle.TextAlignment = UITextAlignment.Center;
            _lblRateTitle.Text = _rateValueDictionary[Rating];

            _viewRating = new UIView(new CGRect((View.Frame.Width / 2) - 92, 168, 184, 32));
            CreateRatingSubViews();

            _txtFieldComments = new UITextField(new CGRect(34, 248, View.Frame.Width - 68, 24));
            _txtFieldComments.AttributedPlaceholder = new Foundation.NSAttributedString(
                "Comments"
                , font: myTNBFont.MuseoSans16()
                , foregroundColor: myTNBColor.SilverChalice()
            );
            _txtFieldComments.TextColor = myTNBColor.SilverChalice();
            _textFieldHelper.CreateTextFieldLeftView(_txtFieldComments, "IC-Field-Text");
            _textFieldHelper.SetKeyboard(_txtFieldComments);
            _txtFieldComments.KeyboardType = UIKeyboardType.Default;
            _txtFieldComments.ShouldReturn = (sender) =>
            {
                sender.ResignFirstResponder();
                return false;
            };

            UIView viewLine = new UIView(new CGRect(34, 273, View.Frame.Width - 68, 1));
            viewLine.BackgroundColor = myTNBColor.SilverChalice();

            UIButton btnCTA = new UIButton(UIButtonType.Custom);
            btnCTA.Frame = new CGRect(18, View.Frame.Height - 72, View.Frame.Width - 36, 48);
            btnCTA.Layer.CornerRadius = 4;
            btnCTA.Layer.BorderColor = myTNBColor.FreshGreen().CGColor;
            btnCTA.BackgroundColor = myTNBColor.FreshGreen();
            btnCTA.Layer.BorderWidth = 1;
            btnCTA.SetTitle("Submit", UIControlState.Normal);
            btnCTA.Font = myTNBFont.MuseoSans16();
            btnCTA.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnCTA.TouchUpInside += (sender, e) =>
            {
                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
               {
                   InvokeOnMainThread(() =>
                   {
                       if (NetworkUtility.isReachable)
                       {
                           _message = _txtFieldComments.Text;
                           SubmitExperienceRating().ContinueWith(task =>
                           {
                               InvokeOnMainThread(() =>
                               {
                                   DisplayDashboard();
                               });
                           });
                       }
                       else
                       {
                           Console.WriteLine("No Network");
                           var alert = UIAlertController.Create("No Data Connection", "Please check your data connection and try again.", UIAlertControllerStyle.Alert);
                           alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                           PresentViewController(alert, animated: true, completionHandler: null);
                       }
                   });
               });
            };

            View.AddSubviews(new UIView[] { lblTitle, _lblRateTitle, _viewRating, _txtFieldComments, viewLine, btnCTA });
        }

        void CreateRatingSubViews()
        {
            UIView viewStar;
            UIImageView imgStar;
            int xLocation = 0;
            for (int i = 0; i < 5; i++)
            {
                viewStar = new UIView(new CGRect(xLocation, 0, 32, 32));
                imgStar = new UIImageView(new CGRect(0, 0, 32, 32));
                imgStar.Image = UIImage.FromBundle("IC-Action-Rating-Active");
                viewStar.AddSubview(imgStar);
                int index = i;
                viewStar.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    ChooseRating(index);
                }));
                _viewRating.AddSubview(viewStar);
                xLocation += 32 + 6;
            }
            ChooseRating(Rating - 1);
        }

        void ChooseRating(int index)
        {
            Rating = index + 1;
            _lblRateTitle.Text = _rateValueDictionary[Rating];
            UIImageView imgView;
            for (int i = 0; i < 5; i++)
            {
                imgView = _viewRating.Subviews[i].Subviews[0] as UIImageView;
                if (i <= index)
                {
                    imgView.Image = UIImage.FromBundle("IC-Action-Rating-Active");
                }
                else
                {
                    imgView.Image = UIImage.FromBundle("IC-Action-Rating-Inactive");
                }
            }
        }

        void DisplayDashboard()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("Dashboard", null);
            UIViewController loginVC = storyBoard.InstantiateViewController("HomeTabBarController") as UIViewController;
            ShowViewController(loginVC, this);
        }

        Task SubmitExperienceRating()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    email = DataManager.DataManager.SharedInstance.UserEntity[0].email,
                    rating = Rating.ToString(),
                    message = _message,
                    ratingFor = "PAY"
                };
                BaseResponseModel response = serviceManager.BaseServiceCall("SubmitExperienceRating", requestParameter);
            });
        }
    }
}