using Foundation;
using System;
using UIKit;
using CoreGraphics;
using myTNB.Registration;

namespace myTNB
{
    public partial class SelectAccountByICNumberViewController : UIViewController
    {
        UIView viewYes;
        UIView viewNo;
        UILabel lblTitle;
        UILabel lblYes;
        UILabel lblYesDescription;
        UILabel lblNo;
        UILabel lblNoDescription;

        public SelectAccountByICNumberViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            InitializedSubviews();
            AddBackButton();
            SetUpGestures();
        }

        internal void InitializedSubviews()
        {
            NavigationItem.Title = "Add Electricity Account";
            View.BackgroundColor = myTNBColor.LightGrayBG();

            lblTitle = new UILabel
            {
                Frame = new CGRect(18, 16, View.Frame.Width - 36, 42),
                AttributedText = new NSAttributedString(
                    "Was this supply account registered with your IC no.?"
                    , font: myTNBFont.MuseoSans16()
                    , foregroundColor: myTNBColor.PowerBlue()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };

            viewYes = new UIView
            {
                Frame = new CGRect(18, 68, View.Frame.Width - 36, 68),
                BackgroundColor = UIColor.White,
            };
            viewYes.Layer.CornerRadius = 4.0f;

            lblYes = new UILabel
            {
                Frame = new CGRect(34, 84, 26, 18),
                AttributedText = new NSAttributedString(
                    "Yes,"
                    , font: myTNBFont.MuseoSans14()
                    , foregroundColor: myTNBColor.TunaGrey()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left,
            };

            lblYesDescription = new UILabel
            {
                Frame = new CGRect(34, 104, View.Frame.Width - 68, 18),
                AttributedText = new NSAttributedString(
                    "I'm the owner of the supply account."
                    , font: myTNBFont.MuseoSans14_300()
                    , foregroundColor: myTNBColor.TunaGrey()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left,
            };

            viewNo = new UIView
            {
                Frame = new CGRect(18, 144, View.Frame.Width - 36, 86),
                BackgroundColor = UIColor.White
            };
            viewNo.Layer.CornerRadius = 4.0f;

            lblNo = new UILabel
            {
                Frame = new CGRect(34, 152, 23, 18),
                AttributedText = new NSAttributedString(
                    "No,"
                    , font: myTNBFont.MuseoSans14()
                    , foregroundColor: myTNBColor.TunaGrey()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left,
            };

            lblNoDescription = new UILabel
            {
                Frame = new CGRect(34, 172, View.Frame.Width - 68, 38),
                AttributedText = new NSAttributedString(
                    "I’m renting this place / it belongs to my parents."
                    , font: myTNBFont.MuseoSans14_300()
                    , foregroundColor: myTNBColor.TunaGrey()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

            View.AddSubview(lblTitle);
            View.AddSubview(viewYes);
            View.AddSubview(viewNo);
            View.AddSubview(lblYes);
            View.AddSubview(lblYesDescription);
            View.AddSubview(lblNo);
            View.AddSubview(lblNoDescription);
        }

        internal void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                NavigationController.PopViewController(true);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        internal void NavigateToPage(string storyboardName, string viewControllerName)
        {
            UIStoryboard storyBoard = UIStoryboard.FromName(storyboardName, null);
            UIViewController viewController =
                storyBoard.InstantiateViewController(viewControllerName) as UIViewController;
            NavigationController.PushViewController(viewController, true);
        }

        internal void SetUpGestures()
        {
            UITapGestureRecognizer yes = new UITapGestureRecognizer(() =>
            {
                DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex = 0;
                UIStoryboard storyBoard = UIStoryboard.FromName("Registration", null);
                UIViewController viewController =
                    storyBoard.InstantiateViewController("AddAccountViewController") as UIViewController;
                AddAccountViewController addAccountVC =
                storyBoard.InstantiateViewController("AddAccountViewController") as AddAccountViewController;
                addAccountVC.isOwner = true;
                NavigationController.PushViewController(addAccountVC, true);
            });
            viewYes.AddGestureRecognizer(yes);

            UITapGestureRecognizer no = new UITapGestureRecognizer(() =>
            {
                NavigateToPage("AccountRecords", "SelectAccountByRightsViewController");
            });
            viewNo.AddGestureRecognizer(no);
        }
    }
}