using Foundation;
using System;
using UIKit;
using CoreGraphics;
using myTNB.Registration;

namespace myTNB
{
    public partial class SelectAccountByICNumberViewController : CustomUIViewController
    {
        private UILabel lblTitle, lblYes, lblYesDescription, lblNo, lblNoDescription;
        private UIView viewYes, viewNo;

        private const float margin = 16f;
        private const float inlineMargin = 8f;

        public SelectAccountByICNumberViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = AddAccountConstants.PageName;
            base.ViewDidLoad();
            InitializedSubviews();
            AddBackButton();
            SetUpGestures();
        }

        private void InitializedSubviews()
        {
            NavigationItem.Title = GetI18NValue(AddAccountConstants.I18N_NavTitle);
            View.BackgroundColor = MyTNBColor.LightGrayBG;

            lblTitle = new UILabel
            {
                Frame = new CGRect(18, 16, View.Frame.Width - 36, 80),
                AttributedText = new NSAttributedString(
                   GetI18NValue(AddAccountConstants.I18N_AddByIDMessage)
                    , font: MyTNBFont.MuseoSans18_500
                    , foregroundColor: MyTNBColor.PowerBlue
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };

            viewYes = new UIView
            {
                Frame = new CGRect(18, lblTitle.Frame.GetMaxY() + inlineMargin, View.Frame.Width - (margin * 2), 100),
                BackgroundColor = UIColor.White,
            };
            viewYes.Layer.CornerRadius = 4.0f;

            lblYes = new UILabel
            {
                Frame = new CGRect(margin, margin, viewYes.Frame.Width - (margin * 2), 18),
                AttributedText = new NSAttributedString(string.Format("{0}.", GetCommonI18NValue(Constants.Common_Yes))
                    , font: MyTNBFont.MuseoSans16_500
                    , foregroundColor: MyTNBColor.TunaGrey()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left,
            };

            lblYesDescription = new UILabel
            {
                Frame = new CGRect(margin, lblYes.Frame.GetMaxY() + inlineMargin, viewYes.Frame.Width - (margin * 2), 40),
                AttributedText = new NSAttributedString(
                    GetI18NValue(AddAccountConstants.I18N_AddAsOwnerMessage)
                    , font: MyTNBFont.MuseoSans16_300
                    , foregroundColor: MyTNBColor.TunaGrey()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

            viewYes.AddSubviews(new UIView[] { lblYes, lblYesDescription });

            viewNo = new UIView
            {
                Frame = new CGRect(18, viewYes.Frame.GetMaxY() + inlineMargin, View.Frame.Width - (margin * 2), 100),
                BackgroundColor = UIColor.White
            };
            viewNo.Layer.CornerRadius = 4.0f;

            lblNo = new UILabel
            {
                Frame = new CGRect(margin, margin, viewNo.Frame.Width - (margin * 2), 18),
                AttributedText = new NSAttributedString(string.Format("{0}.", GetCommonI18NValue(Constants.Common_No))
                    , font: MyTNBFont.MuseoSans16_500
                    , foregroundColor: MyTNBColor.TunaGrey()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left,
            };

            lblNoDescription = new UILabel
            {
                Frame = new CGRect(margin, lblNo.Frame.GetMaxY() + inlineMargin, viewNo.Frame.Width - (margin * 2), 40),
                AttributedText = new NSAttributedString(
                    GetI18NValue(AddAccountConstants.I18N_AddAsTenantMessage)
                    , font: MyTNBFont.MuseoSans16_300
                    , foregroundColor: MyTNBColor.TunaGrey()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

            viewNo.AddSubviews(new UIView[] { lblNo, lblNoDescription });

            View.AddSubview(lblTitle);
            View.AddSubview(viewYes);
            View.AddSubview(viewNo);
        }

        private void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                NavigationController.PopViewController(true);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        private void NavigateToPage(string storyboardName, string viewControllerName)
        {
            if (!string.IsNullOrEmpty(storyboardName) && !string.IsNullOrEmpty(viewControllerName))
            {
                UIStoryboard storyBoard = UIStoryboard.FromName(storyboardName, null);
                UIViewController viewController =
                    storyBoard.InstantiateViewController(viewControllerName) as UIViewController;
                if (viewController != null)
                {
                    NavigationController?.PushViewController(viewController, true);
                }
            }
        }

        private void SetUpGestures()
        {
            UITapGestureRecognizer yes = new UITapGestureRecognizer(() =>
            {
                DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex = 0;
                UIStoryboard storyBoard = UIStoryboard.FromName("Registration", null);
                AddAccountViewController addAccountVC = storyBoard.InstantiateViewController("AddAccountViewController") as AddAccountViewController;
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