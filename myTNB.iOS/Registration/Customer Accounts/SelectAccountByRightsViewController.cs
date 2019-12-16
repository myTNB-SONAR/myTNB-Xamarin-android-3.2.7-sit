using Foundation;
using System;
using UIKit;
using CoreGraphics;
using myTNB.Registration;
using System.Collections.Generic;

namespace myTNB
{
    public partial class SelectAccountByRightsViewController : CustomUIViewController
    {
        private UILabel lblTitle, lblYes, lblYesDescription, lblNo;
        private UITextView txtViewNoDescription;
        private UIView viewYes, viewNo;

        private List<string> rightsList;

        private readonly List<UIImage> imageList = new List<UIImage>()
            {
                UIImage.FromBundle("Check-Green")
                , UIImage.FromBundle("Check-Green")
                , UIImage.FromBundle("Check-Green")
                , UIImage.FromBundle("Check-Green")
                , UIImage.FromBundle("X-Red")
            };

        public SelectAccountByRightsViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = AddAccountConstants.PageName;
            base.ViewDidLoad();
            rightsList = new List<string>(){
                GetI18NValue(AddAccountConstants.I18N_OutstandingPayment)
                , GetI18NValue(AddAccountConstants.I18N_UsageGraph)
                , GetI18NValue(AddAccountConstants.I18N_PaymentHistory)
                , GetI18NValue(AddAccountConstants.I18N_CurrentBill)
                , GetI18NValue(AddAccountConstants.I18N_PastBills)
            };
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
                Frame = new CGRect(18, 16, View.Frame.Width - 36, 90),
                AttributedText = new NSAttributedString(
                   GetI18NValue(AddAccountConstants.I18N_AddByRightsMessage)
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
                AttributedText = new NSAttributedString(string.Format("{0},", GetCommonI18NValue(Constants.Common_Yes))
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
                    GetI18NValue(AddAccountConstants.I18N_AddAsTenantWithICMessage)
                    , font: MyTNBFont.MuseoSans16_300
                    , foregroundColor: MyTNBColor.TunaGrey()
                    , strokeWidth: 0
                ),
                TextAlignment = UITextAlignment.Left,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };

            viewYes.AddSubviews(new UIView[] { lblYes, lblYesDescription });

            viewNo = new UIView(new CGRect(18, viewYes.Frame.GetMaxY() + 10, View.Frame.Width - 36, 280));
            viewNo.BackgroundColor = UIColor.White;
            viewNo.Alpha = 1f;
            viewNo.Layer.CornerRadius = 4.0f;

            lblNo = new UILabel
            {
                Frame = new CGRect(16, 16, viewNo.Frame.Width - 32, 18),
                AttributedText = new NSAttributedString(string.Format("{0},", GetCommonI18NValue(Constants.Common_No))
                    , font: MyTNBFont.MuseoSans16_500
                    , foregroundColor: MyTNBColor.TunaGrey()
                    , strokeWidth: 0),
                TextAlignment = UITextAlignment.Left,
            };

            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(GetI18NValue(AddAccountConstants.I18N_AddAsTenantWithoutICMessage)
                , ref htmlBodyError, MyTNBFont.FONTNAME_300, 16F);
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.TunaGrey()
            }, new NSRange(0, htmlBody.Length));

            txtViewNoDescription = new UITextView(new CGRect(16, 34, viewNo.Frame.Width - 32, 120))
            {
                Editable = false,
                ScrollEnabled = true,
                AttributedText = mutableHTMLBody,
                ContentInset = new UIEdgeInsets(-5, 0, -5, 0),
                TextAlignment = UITextAlignment.Left
            };
            txtViewNoDescription.ScrollIndicatorInsets = UIEdgeInsets.Zero;
            txtViewNoDescription.TextContainer.LineFragmentPadding = 0F;

            viewNo.AddSubviews(new UIView[] { lblNo, txtViewNoDescription });
            int yLocation = 161;
            for (int i = 0; i < rightsList.Count; i++)
            {
                UIView viewRights = new UIView(new CGRect(16, yLocation, viewNo.Frame.Width - 32, 16));
                UIImageView img = new UIImageView(new CGRect(0, 0, 16, 16))
                {
                    Image = imageList[i]
                };
                UILabel lblRights = new UILabel(new CGRect(24, 0, viewNo.Frame.Width - 24, 16))
                {
                    TextColor = MyTNBColor.TunaGrey(),
                    Font = MyTNBFont.MuseoSans14_300,
                    TextAlignment = UITextAlignment.Left,
                    Text = rightsList[i]
                };
                viewRights.AddSubviews(new UIView[] { img, lblRights });
                viewNo.AddSubview(viewRights);
                yLocation += 22;
            }
            View.AddSubview(lblTitle);
            View.AddSubview(viewYes);
            View.AddSubview(viewNo);
        }

        private void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                NavigationController?.PopViewController(true);
            });
            if (NavigationItem != null)
            {
                NavigationItem.LeftBarButtonItem = btnBack;
            }
        }

        private void NavigateToPage(string storyboardName, string viewControllerName, bool isOwner)
        {
            UIStoryboard storyBoard = UIStoryboard.FromName(storyboardName, null);
            AddAccountViewController addAccountVC = storyBoard.InstantiateViewController("AddAccountViewController") as AddAccountViewController;
            if (addAccountVC != null)
            {
                addAccountVC.isOwner = isOwner;
                NavigationController.PushViewController(addAccountVC, true);
            }
        }

        private void SetUpGestures()
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