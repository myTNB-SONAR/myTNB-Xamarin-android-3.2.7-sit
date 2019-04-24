using Foundation;
using System;
using UIKit;
using CoreGraphics;
using myTNB.Registration;
using System.Collections.Generic;

namespace myTNB
{
    public partial class SelectAccountByRightsViewController : UIViewController
    {
        UILabel lblTitle;
        UIView viewYes;
        UIView viewNo;
        UILabel lblYes;
        UILabel lblYesDescription;
        UILabel lblNo;
        UILabel lblNoDescription;

        List<string> rightsList = new List<string>()
            {
                "Registration_OutstandingPayment".Translate()
                , "Registration_UsageGraph".Translate()
                , "Registration_PaymentHistory".Translate()
                , "Registration_CurrentBill".Translate()
                , "Registration_PastBills".Translate()
            };

        List<UIImage> imageList = new List<UIImage>()
            {
                UIImage.FromBundle("Check-Green")
                , UIImage.FromBundle("Check-Green")
                , UIImage.FromBundle("Check-Green")
                , UIImage.FromBundle("Check-Green")
                , UIImage.FromBundle("X-Red")
            };

        public SelectAccountByRightsViewController(IntPtr handle) : base(handle)
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
            NavigationItem.Title = "Common_AddElectricityAccount".Translate();
            View.BackgroundColor = MyTNBColor.LightGrayBG;

            lblTitle = new UILabel
            {
                Frame = new CGRect(18, 16, View.Frame.Width - 36, 90),
                AttributedText = new NSAttributedString(
                    "Registration_RightsQuestion".Translate()
                    , font: MyTNBFont.MuseoSans18_500
                    , foregroundColor: MyTNBColor.PowerBlue
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };

            viewYes = new UIView(new CGRect(18, lblTitle.Frame.GetMaxY() + 10, View.Frame.Width - 36, 104));
            viewYes.BackgroundColor = UIColor.White;
            viewYes.Alpha = 1f;
            viewYes.Layer.CornerRadius = 4.0f;

            lblYes = new UILabel
            {
                Frame = new CGRect(16, 16, viewYes.Frame.Width - 32, 18),
                AttributedText = new NSAttributedString(string.Format("{0},", "Common_Yes".Translate())
                    , font: MyTNBFont.MuseoSans16_500
                    , foregroundColor: MyTNBColor.TunaGrey()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left,
            };

            lblYesDescription = new UILabel
            {
                Frame = new CGRect(16, 34, viewYes.Frame.Width - 32, 60),
                AttributedText = new NSAttributedString(
                    "Registration_RightsAnswer".Translate()
                    , font: MyTNBFont.MuseoSans16_300
                    , foregroundColor: MyTNBColor.TunaGrey()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };

            viewYes.AddSubviews(new UIView[] { lblYes, lblYesDescription });

            viewNo = new UIView(new CGRect(18, viewYes.Frame.GetMaxY() + 10, View.Frame.Width - 36, 250));
            viewNo.BackgroundColor = UIColor.White;
            viewNo.Alpha = 1f;
            viewNo.Layer.CornerRadius = 4.0f;

            lblNo = new UILabel
            {
                Frame = new CGRect(16, 16, viewNo.Frame.Width - 32, 18),
                AttributedText = new NSAttributedString(string.Format("{0},", "Common_No".Translate())
                    , font: MyTNBFont.MuseoSans16_500
                    , foregroundColor: MyTNBColor.TunaGrey()
                    , strokeWidth: 0),
                TextAlignment = UITextAlignment.Left,
            };

            lblNoDescription = new UILabel
            {
                Frame = new CGRect(16, 34, viewNo.Frame.Width - 32, 90),
                AttributedText = new NSAttributedString("Registration_NoRightsAnswer".Translate()
                    , font: MyTNBFont.MuseoSans16_300
                    , foregroundColor: MyTNBColor.TunaGrey()
                    , strokeWidth: 0),
                TextAlignment = UITextAlignment.Left,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

            viewNo.AddSubviews(new UIView[] { lblNo, lblNoDescription });
            int yLocation = 131;
            for (int i = 0; i < rightsList.Count; i++)
            {
                UIView viewRights = new UIView(new CGRect(16, yLocation, viewNo.Frame.Width - 32, 16));
                UIImageView img = new UIImageView(new CGRect(0, 0, 16, 16));
                img.Image = imageList[i];
                UILabel lblRights = new UILabel(new CGRect(24, 0, viewNo.Frame.Width - 24, 16));
                lblRights.TextColor = MyTNBColor.TunaGrey();
                lblRights.Font = MyTNBFont.MuseoSans14_300;
                lblRights.TextAlignment = UITextAlignment.Left;
                lblRights.Text = rightsList[i];
                viewRights.AddSubviews(new UIView[] { img, lblRights });
                viewNo.AddSubview(viewRights);
                yLocation += 22;
            }
            View.AddSubview(lblTitle);
            View.AddSubview(viewYes);
            View.AddSubview(viewNo);
        }

        internal void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                this.NavigationController?.PopViewController(true);
            });
            if (this.NavigationItem != null)
            {
                this.NavigationItem.LeftBarButtonItem = btnBack;
            }
        }

        internal void NavigateToPage(string storyboardName, string viewControllerName, bool isOwner)
        {
            UIStoryboard storyBoard = UIStoryboard.FromName(storyboardName, null);
            UIViewController viewController =
                storyBoard.InstantiateViewController(viewControllerName) as UIViewController;
            AddAccountViewController addAccountVC =
            storyBoard.InstantiateViewController("AddAccountViewController") as AddAccountViewController;
            if (addAccountVC != null)
            {
                addAccountVC.isOwner = isOwner;
                NavigationController.PushViewController(addAccountVC, true);
            }
        }

        internal void SetUpGestures()
        {
            UITapGestureRecognizer yes = new UITapGestureRecognizer(() =>
            {
                NavigateToPage("Registration", "AddAccountViewController", true);
                DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex = 0;
            });
            viewYes.AddGestureRecognizer(yes);

            UITapGestureRecognizer no = new UITapGestureRecognizer(() =>
            {
                NavigateToPage("Registration", "AddAccountViewController", false);
                DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex = 0;
            });
            viewNo.AddGestureRecognizer(no);
        }
    }
}