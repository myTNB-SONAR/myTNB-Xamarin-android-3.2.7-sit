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
            AddBackButton();
            InitializedSubviews();
        }

        private void InitializedSubviews()
        {
            UIScrollView scrollView = new UIScrollView(new CGRect(0, 0, ViewWidth, ViewHeight)) { BackgroundColor = UIColor.Clear };

            UILabel lblTitle = new UILabel(new CGRect(BaseMargin, BaseMargin, BaseMarginedWidth, 100))
            {
                Text = GetI18NValue(AddAccountConstants.I18N_AddByRightsMessage),
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.PowerBlue,
                TextAlignment = UITextAlignment.Left,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };
            nfloat newTitleHeight = lblTitle.GetLabelHeight(1000);
            lblTitle.Frame = new CGRect(lblTitle.Frame.Location, new CGSize(lblTitle.Frame.Width, newTitleHeight));

            CustomUIView viewYes = new CustomUIView(new CGRect(BaseMargin, GetYLocationFromFrame(lblTitle.Frame, 16), BaseMarginedWidth, 100)) { BackgroundColor = UIColor.White };
            viewYes.Layer.CornerRadius = GetScaledWidth(4);

            UILabel lblYes = new UILabel(new CGRect(BaseMargin, BaseMargin, viewYes.Frame.Width - (BaseMargin * 2), GetScaledHeight(18)))
            {
                Text = string.Format("{0}.", GetCommonI18NValue(Constants.Common_Yes)),
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.TunaGrey(),
                TextAlignment = UITextAlignment.Left,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };
            UILabel yesDescription = new UILabel(new CGRect(BaseMargin, GetYLocationFromFrame(lblYes.Frame, 5), viewYes.Frame.Width - (BaseMargin * 2), 100))
            {
                Text = GetI18NValue(AddAccountConstants.I18N_AddAsTenantWithICMessage),
                Font = TNBFont.MuseoSans_14_300,
                TextColor = MyTNBColor.TunaGrey(),
                TextAlignment = UITextAlignment.Left,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };
            nfloat newYesDescriptionHeight = yesDescription.GetLabelHeight(1000);
            yesDescription.Frame = new CGRect(yesDescription.Frame.Location, new CGSize(yesDescription.Frame.Width, newYesDescriptionHeight));
            viewYes.AddSubviews(new UIView[] { lblYes, yesDescription });
            viewYes.Frame = new CGRect(viewYes.Frame.Location, new CGSize(viewYes.Frame.Width, yesDescription.Frame.GetMaxY() + GetScaledHeight(16)));

            CustomUIView viewNo = new CustomUIView(new CGRect(BaseMargin, GetYLocationFromFrame(viewYes.Frame, 8), BaseMarginedWidth, 100)) { BackgroundColor = UIColor.White };
            viewNo.Layer.CornerRadius = GetScaledWidth(4);

            UILabel lblNo = new UILabel(new CGRect(BaseMargin, BaseMargin, viewNo.Frame.Width - (BaseMargin * 2), GetScaledHeight(18)))
            {
                Text = string.Format("{0}.", GetCommonI18NValue(Constants.Common_No)),
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.TunaGrey(),
                TextAlignment = UITextAlignment.Left,
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };

            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(GetI18NValue(AddAccountConstants.I18N_AddAsTenantWithoutICMessage)
                , ref htmlBodyError, TNBFont.FONTNAME_300, 14F);
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.TunaGrey()
            }, new NSRange(0, htmlBody.Length));

            UITextView txtViewNoDescription = new UITextView(new CGRect(BaseMargin, lblNo.Frame.GetMaxY(), viewNo.Frame.Width - (BaseMargin * 2), 100))
            {
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                ContentInset = UIEdgeInsets.Zero,
                TextAlignment = UITextAlignment.Left,
                Selectable = false
            };
            txtViewNoDescription.ScrollIndicatorInsets = UIEdgeInsets.Zero;
            txtViewNoDescription.TextContainer.LineFragmentPadding = 0F;
            CGSize size = txtViewNoDescription.SizeThatFits(new CGSize(BaseMarginedWidth, GetScaledHeight(160)));
            txtViewNoDescription.Frame = new CGRect(txtViewNoDescription.Frame.Location, new CGSize(txtViewNoDescription.Frame.Width, size.Height));
            viewNo.AddSubviews(new UIView[] { lblNo, txtViewNoDescription });

            nfloat yLocation = GetYLocationFromFrame(txtViewNoDescription.Frame, 8);
            for (int i = 0; i < rightsList.Count; i++)
            {
                UIView viewRights = new UIView(new CGRect(BaseMargin, yLocation, viewNo.Frame.Width - (BaseMargin * 2), GetScaledHeight(16)));
                UIImageView img = new UIImageView(new CGRect(0, 0, GetScaledWidth(16), GetScaledWidth(16)))
                {
                    Image = imageList[i]
                };
                UILabel lblRights = new UILabel(new CGRect(img.Frame.GetMaxX() + GetScaledWidth(8), 0, viewRights.Frame.Width - GetScaledWidth(24), GetScaledHeight(16)))
                {
                    TextColor = MyTNBColor.TunaGrey(),
                    Font = TNBFont.MuseoSans_12_300,
                    TextAlignment = UITextAlignment.Left,
                    Text = rightsList[i]
                };
                viewRights.AddSubviews(new UIView[] { img, lblRights });
                viewNo.AddSubview(viewRights);
                yLocation += GetScaledHeight(22);
            }

            viewNo.Frame = new CGRect(viewNo.Frame.Location, new CGSize(viewNo.Frame.Width, yLocation + GetScaledHeight(16)));
            scrollView.AddSubviews(new UIView[] { lblTitle, viewYes, viewNo });
            scrollView.ContentSize = new CGSize(scrollView.Frame.Width, viewNo.Frame.GetMaxY());
            View.AddSubview(scrollView);

            viewYes.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                NavigateToPage("Registration", true);
                DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex = 0;
            }));
            viewNo.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                NavigateToPage("Registration", false);
                DataManager.DataManager.SharedInstance.CurrentSelectedAccountTypeIndex = 0;
            }));
        }

        private void AddBackButton()
        {
            NavigationItem.Title = GetI18NValue(AddAccountConstants.I18N_NavTitle);
            View.BackgroundColor = MyTNBColor.LightGrayBG;
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

        private void NavigateToPage(string storyboardName, bool isOwner)
        {
            UIStoryboard storyBoard = UIStoryboard.FromName(storyboardName, null);
            AddAccountViewController addAccountVC = storyBoard.InstantiateViewController("AddAccountViewController") as AddAccountViewController;
            if (addAccountVC != null)
            {
                addAccountVC.isOwner = isOwner;
                NavigationController.PushViewController(addAccountVC, true);
            }
        }
    }
}