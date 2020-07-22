using CoreGraphics;
using Foundation;
using myTNB.Feedback;
using myTNB.Model;
using myTNB.Registration;
using myTNB.SQLite.SQLiteDataManager;
using Newtonsoft.Json;
using System;
using System.Drawing;
using UIKit;

namespace myTNB
{
    public partial class EnquiryTNCViewController : CustomUIViewController
    {
        public EnquiryTNCViewController (IntPtr handle) : base (handle)
        {
        }

        public bool isPresentedVC;
        private UIScrollView _svContainer;
        private UIView _containerTitleTNC;
        private UILabel lblTitleTNC;
        private UIImageView imgViewTitleTNC;
        private UIView viewLineTitleTNC;
        private UIView _containerRedirectTNC;
        private UILabel lblRedirectTNC;
        private UIImageView imgViewRedirectTNC;
        private UIView viewLineRedirectTNC;
        private UIView _containerTNCContent;
        private UITextView lblContentTNC;

        public override void ViewDidLoad()
        {
            PageName = EnquiryConstants.Pagename_Enquiry;

            base.ViewDidLoad();
            AddBackButton();
            AddScrollView();

            ContainerTNCTitle();
            ContainerTNCContent();
            ContainerTNCRedirect();

            UpdateContentSize();

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        private void AddBackButton()
        {
            NavigationController.NavigationItem.HidesBackButton = true;
            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;

            Title = GetI18NValue(EnquiryConstants.tnc);
        }

        private void AddScrollView()
        {
            _svContainer = new UIScrollView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };
            View.AddSubview(_svContainer);
        }

        public void ContainerTNCTitle()
        {
            {
                _containerTitleTNC = new UIView(new CGRect(0, 0, View.Frame.Width, 53))
                {
                    BackgroundColor = UIColor.White
                };

                lblTitleTNC = new UILabel(new CGRect(18, 16, _containerTitleTNC.Frame.Width - 16, 20))
                {
                    Font = TNBFont.MuseoSans_14_500,
                    TextColor = MyTNBColor.WaterBlue,
                    Text = GetI18NValue(EnquiryConstants.personalDisclamer), //"Update Personal Data Disclamer",
                    LineBreakMode = UILineBreakMode.WordWrap,
                };

                imgViewTitleTNC = new UIImageView(new CGRect(View.Frame.Width - 16 - 20, 16, GetScaledWidth(20F), GetScaledHeight(20F)))
                {
                    //Image = UIImage.FromBundle("Arrow-Expand-Down"),
                    ContentMode = UIViewContentMode.ScaleAspectFill,
                };

                viewLineTitleTNC = GenericLine.GetLine(new CGRect(0, lblTitleTNC.Frame.GetMaxY() + 16, View.Frame.Width, 1));

                _containerTitleTNC.AddSubviews(new UIView[] { lblTitleTNC, imgViewTitleTNC, viewLineTitleTNC });
                _svContainer.AddSubview(_containerTitleTNC);

            }
        }

        private void ContainerTNCContent()
        {
            UserEntity userInfo = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                      ? DataManager.DataManager.SharedInstance.UserEntity[0] : new UserEntity();

            _containerTNCContent = new UIView(new CGRect(0, _containerTitleTNC.Frame.GetMaxY(), View.Frame.Width - 18, GetScaledHeight(48)))
            {
                BackgroundColor = UIColor.White
            };

            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(string.Format(GetI18NValue(EnquiryConstants.tncAgree), userInfo.email, DataManager.DataManager.SharedInstance.CurrentSelectedEnquiryCA, GetAccountDetail())
                        , ref htmlBodyError, TNBFont.FONTNAME_300, (float)TNBFont.GetFontSize(14F));
            NSMutableAttributedString mutableHTMLFooter = new NSMutableAttributedString(htmlBody);

            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.CharcoalGrey
            }, new NSRange(0, htmlBody.Length));


            lblContentTNC = new UITextView(new CGRect(18, 16, _containerTNCContent.Frame.Width - 18, 14))
            {
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                TextAlignment = UITextAlignment.Left
            };

            CGSize newSize = GetTitleLabelSize(GetI18NValue(EnquiryConstants.tncAgree));//get height content tnc
            lblContentTNC.Frame = new CGRect(18, 16, _containerTNCContent.Frame.Width - 18, GetScaledHeight(newSize.Height) + 16);
            _containerTNCContent.Frame = new CGRect(0, _containerTitleTNC.Frame.GetMaxY(), View.Frame.Width, lblContentTNC.Frame.GetMaxY());

            viewLineTitleTNC = GenericLine.GetLine(new CGRect(0, lblContentTNC.Frame.GetMaxY() + 16, View.Frame.Width, 1));
            _containerTNCContent.Frame = new CGRect(0, _containerTitleTNC.Frame.GetMaxY(), View.Frame.Width, viewLineTitleTNC.Frame.GetMaxY());

            _containerTNCContent.AddSubviews(lblContentTNC, viewLineTitleTNC);
            _svContainer.AddSubview(_containerTNCContent);
        }

        public void ContainerTNCRedirect()
        {
            {
                _containerRedirectTNC = new UIView(new CGRect(0, _containerTNCContent.Frame.GetMaxY(), View.Frame.Width, 52))
                {
                    BackgroundColor = UIColor.White
                };

                lblRedirectTNC = new UILabel(new CGRect(18, 16, _containerTitleTNC.Frame.Width - 16, 20))
                {
                    Font = TNBFont.MuseoSans_14_500,
                    TextColor = MyTNBColor.WaterBlue,
                    Text = GetI18NValue(EnquiryConstants.tnc),
                    LineBreakMode = UILineBreakMode.WordWrap,
                };

                imgViewRedirectTNC = new UIImageView(new CGRect(View.Frame.Width - 16 - 20, 16, GetScaledWidth(20F), GetScaledHeight(20F)))
                {
                    Image = UIImage.FromBundle("Arrow-Expand"),
                    ContentMode = UIViewContentMode.ScaleAspectFill,
                };

                UITapGestureRecognizer tapAccounType = new UITapGestureRecognizer(() =>
                {
                    UIStoryboard storyBoard = UIStoryboard.FromName("Registration", null);
                    TermsAndConditionViewController viewController =
                        storyBoard.InstantiateViewController("TermsAndConditionViewController") as TermsAndConditionViewController;
                    if (viewController != null)
                    {
                        viewController.isPresentedVC = true;
                        UINavigationController navController = new UINavigationController(viewController);
                        navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        PresentViewController(navController, true, null);
                    }

                });
                _containerRedirectTNC.AddGestureRecognizer(tapAccounType);

                viewLineRedirectTNC = GenericLine.GetLine(new CGRect(0, lblRedirectTNC.Frame.GetMaxY() + 16, View.Frame.Width, 1));

                _containerRedirectTNC.AddSubviews(new UIView[] { lblRedirectTNC, imgViewRedirectTNC, viewLineRedirectTNC });
                _svContainer.AddSubview(_containerRedirectTNC);

            }
        }

        private string GetAccountDetail()
        {
            BillingAccountDetailsDataModel model = null;

            if (!string.IsNullOrEmpty(DataManager.DataManager.SharedInstance.CurrentSelectedEnquiryCA))
            {
                var entity = BillingAccountEntity.GetItem(DataManager.DataManager.SharedInstance.CurrentSelectedEnquiryCA);
                if (entity != null)
                {
                    model = JsonConvert.DeserializeObject<BillingAccountDetailsDataModel>(entity.Data);
                    
                }
            }
            if (model != null)
                return model.accName;
            else
                return string.Empty;
              
        }


        private nfloat GetScrollHeight()
        {
            return (nfloat)((_containerRedirectTNC.Frame.GetMaxY()));
        }

        private void UpdateContentSize()
        {
            _svContainer.ContentSize = new CGRect(0f, 0f, View.Frame.Width, GetScrollHeight() + 64).Size;
        }

        private CGSize GetTitleLabelSize(string text)
        {
            UILabel label = new UILabel(new CGRect(18, 16, UIApplication.SharedApplication.KeyWindow.Frame.Width - 18, 1000))
            {
                Font = MyTNBFont.MuseoSans14,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = text
            };
            return label.Text.StringSize(label.Font, new SizeF((float)label.Frame.Width, 1000F));
        }

    }
}