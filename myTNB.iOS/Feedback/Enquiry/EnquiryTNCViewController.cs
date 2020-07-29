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
        public EnquiryTNCViewController(IntPtr handle) : base(handle)
        {
        }
        public bool IsOwner;

        private bool IsDataDisclamer = true;
        private bool IsTNC = false;
        private bool IsMobileNumber = false;
        private bool IsEmail = false;
        private bool IsMailing = false;

        public string Name;
        public bool isPresentedVC;
        private UIScrollView _svContainer;
        private UIView _containerDataDisclamer;
        private UILabel lblTitleTNC;
        private UIImageView imgViewDataDisclamer;
        private UIView viewLineTitleTNC;
        private UIView _containerRedirectTNC;
        private UIView _containerRedirectTNC1;
        private UILabel lblRedirectTNC;
        private UIImageView imgViewRedirectTNC;
        private UIView viewLineRedirectTNC;
        private UIView _containerTNCContent;
        private UITextView lblContentTNC;
        private UIView _containerPrivacyPolicy;
        private UILabel lblPrivacyPolicy;
        private UIImageView imgViewPrivacyPolicy;
        private UIView viewLinePrivacyPolicy;
        private UIView _containerAntiSpamPolicy;
        private UILabel lblAntiSpamPolicy;
        private UIImageView imgViewAntiSpamPolicy;
        private UIView viewLineAntiSpamPolicy;
        private UIView _containerPersonalDataProtectionPolicy;
        private UILabel lblPersonalDataProtectionPolicy;
        private UIImageView imgViewPersonalDataProtectionPolicy;
        private UIView viewLinePersonalDataProtectionPolicy;
        private UIView _containerRedirectTNC2;
        private UILabel lblRedirectTNC2;
        private UIView viewLineRedirectTNC2;
        private UIImageView imgViewRedirectTNC2;

        public override void ViewDidLoad()
        {
            PageName = EnquiryConstants.Pagename_Enquiry;

            base.ViewDidLoad();
            AddBackButton();
            AddScrollView();

            ContainerDataDisclamer();
            ContainerTNCRedirect1();
            ContainerTNCRedirect2();
            //ContainerTNCContent();
            //ContainerTNC();
            //ContainerPrivacyPolicy();
            //ContainerAntiSpamPolicy();
            //ContainerPersonalDataProtectionPolicy();

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

        public void ContainerDataDisclamer()
        {
            {
                _containerDataDisclamer = new UIView(new CGRect(0, 0, View.Frame.Width, 53))
                {
                    BackgroundColor = UIColor.White
                };

                lblTitleTNC = new UILabel(new CGRect(18, 16, _containerDataDisclamer.Frame.Width - 16, 20))
                {
                    Font = TNBFont.MuseoSans_14_500,
                    TextColor = MyTNBColor.WaterBlue,
                    Text = GetI18NValue(EnquiryConstants.personalDisclamer),
                    LineBreakMode = UILineBreakMode.WordWrap,
                };

                imgViewDataDisclamer = new UIImageView(new CGRect(View.Frame.Width - 16 - 20, 16, GetScaledWidth(20F), GetScaledHeight(20F)))
                {
                    Image = IsDataDisclamer ? UIImage.FromBundle("Arrow-Expand-Down") : UIImage.FromBundle("Arrow-Expand"),
                    ContentMode = UIViewContentMode.ScaleAspectFill,
                };

                _containerDataDisclamer.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    //IsDataDisclamer = !IsDataDisclamer;

                    //RefreshView();

                }));

                viewLineTitleTNC = GenericLine.GetLine(new CGRect(0, lblTitleTNC.Frame.GetMaxY() + 16, View.Frame.Width, 1));

                _containerDataDisclamer.AddSubviews(new UIView[] { lblTitleTNC, imgViewDataDisclamer, viewLineTitleTNC });
                _svContainer.AddSubview(_containerDataDisclamer);

                //ContainerTNCContent
                if (IsDataDisclamer)
                {
                UserEntity userInfo = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                      ? DataManager.DataManager.SharedInstance.UserEntity[0] : new UserEntity();

                _containerTNCContent = new UIView(new CGRect(0, _containerDataDisclamer.Frame.GetMaxY(), View.Frame.Width - 18, GetScaledHeight(48)))
                {
                    BackgroundColor = UIColor.White
                };

                NSError htmlBodyError = null;
                NSAttributedString htmlBody = !IsOwner ? TextHelper.ConvertToHtmlWithFont(string.Format(GetI18NValue(EnquiryConstants.tncAgreeNonOwner), Name, userInfo.email, DataManager.DataManager.SharedInstance.CurrentSelectedEnquiryCA)
                            , ref htmlBodyError, TNBFont.FONTNAME_300, (float)TNBFont.GetFontSize(14F)) : TextHelper.ConvertToHtmlWithFont(string.Format(GetI18NValue(EnquiryConstants.tncAgreeOwner), Name, userInfo.email, DataManager.DataManager.SharedInstance.CurrentSelectedEnquiryCA)
                            , ref htmlBodyError, TNBFont.FONTNAME_300, (float)TNBFont.GetFontSize(14F));

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

                CGSize newSize = lblContentTNC.SizeThatFits(new CGSize(View.Frame.Width - 23, GetScaledHeight(160)));
                lblContentTNC.Frame = new CGRect(18, 16, _containerTNCContent.Frame.Width - 18, newSize.Height + 16);
                _containerTNCContent.Frame = new CGRect(0, _containerDataDisclamer.Frame.GetMaxY(), View.Frame.Width, lblContentTNC.Frame.GetMaxY());

                viewLineTitleTNC = GenericLine.GetLine(new CGRect(0, lblContentTNC.Frame.GetMaxY() + 16, View.Frame.Width, 1));
                _containerTNCContent.Frame = new CGRect(0, _containerDataDisclamer.Frame.GetMaxY(), View.Frame.Width, viewLineTitleTNC.Frame.GetMaxY());

                    _containerDataDisclamer.Frame = new CGRect(0, 0, View.Frame.Width, _containerTNCContent.Frame.GetMaxY());

                _containerTNCContent.AddSubviews(lblContentTNC, viewLineTitleTNC);
                    _containerDataDisclamer.AddSubview(_containerTNCContent);
                _svContainer.AddSubview(_containerDataDisclamer);

                }
            }
        }

        public void ContainerTNCRedirect1()
        {
           
                _containerRedirectTNC1 = new UIView(new CGRect(0, _containerDataDisclamer.Frame.GetMaxY(), View.Frame.Width, 53))
                {
                    BackgroundColor = UIColor.White
                };

                lblRedirectTNC = new UILabel(new CGRect(18, 16, _containerRedirectTNC1.Frame.Width - 16, 20))
                {
                    Font = TNBFont.MuseoSans_14_500,
                    TextColor = MyTNBColor.WaterBlue,
                    Text = GetI18NValue(EnquiryConstants.tnbTermUse),
                    LineBreakMode = UILineBreakMode.WordWrap,
                };

                imgViewRedirectTNC = new UIImageView(new CGRect(View.Frame.Width - 16 - 20, 16, GetScaledWidth(20F), GetScaledHeight(20F)))
                {
                    Image = UIImage.FromBundle("Arrow-Expand"),
                    ContentMode = UIViewContentMode.ScaleAspectFill,
                };

                UITapGestureRecognizer tapAccounType = new UITapGestureRecognizer(() =>
                {
                    OpenLink(GetI18NValue("antiSpamPolicy"), GetI18NValue(EnquiryConstants.tnbTermUse));

                });
                _containerRedirectTNC1.AddGestureRecognizer(tapAccounType);

                viewLineRedirectTNC = GenericLine.GetLine(new CGRect(0, lblRedirectTNC.Frame.GetMaxY() + 16, View.Frame.Width, 1));

                _containerRedirectTNC1.AddSubviews(new UIView[] { lblRedirectTNC, imgViewRedirectTNC, viewLineRedirectTNC });
                _svContainer.AddSubview(_containerRedirectTNC1);

          
        }

        public void ContainerTNCRedirect2()
        {

            _containerRedirectTNC2 = new UIView(new CGRect(0, _containerRedirectTNC1.Frame.GetMaxY(), View.Frame.Width, 53))
            {
                BackgroundColor = UIColor.White
            };

            lblRedirectTNC2 = new UILabel(new CGRect(18, 16, _containerRedirectTNC2.Frame.Width - 16, 20))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = GetI18NValue("privacyPolicyTitle"),
                LineBreakMode = UILineBreakMode.WordWrap,
            };

            imgViewRedirectTNC2 = new UIImageView(new CGRect(View.Frame.Width - 16 - 20, 16, GetScaledWidth(20F), GetScaledHeight(20F)))
            {
                Image = UIImage.FromBundle("Arrow-Expand"),
                ContentMode = UIViewContentMode.ScaleAspectFill,
            };

            UITapGestureRecognizer tapAccounType = new UITapGestureRecognizer(() =>
            {
                OpenLink(GetI18NValue("privacyPolicy"), GetI18NValue("privacyPolicyTitle"));

            });
            _containerRedirectTNC2.AddGestureRecognizer(tapAccounType);

            viewLineRedirectTNC2 = GenericLine.GetLine(new CGRect(0, lblRedirectTNC2.Frame.GetMaxY() + 16, View.Frame.Width, 1));

            _containerRedirectTNC2.AddSubviews(new UIView[] { lblRedirectTNC2, imgViewRedirectTNC2, viewLineRedirectTNC2 });
            _svContainer.AddSubview(_containerRedirectTNC2);


        }

        /*public void ContainerTNC()
        {
            {
                _containerRedirectTNC = new UIView(new CGRect(0, _containerDataDisclamer.Frame.GetMaxY(), View.Frame.Width, 53))
                {
                    BackgroundColor = UIColor.White
                };

                lblRedirectTNC = new UILabel(new CGRect(18, 16, _containerRedirectTNC.Frame.Width - 16, 20))
                {
                    Font = TNBFont.MuseoSans_14_500,
                    TextColor = MyTNBColor.WaterBlue,
                    Text = GetI18NValue(EnquiryConstants.tnbTermUse),
                    LineBreakMode = UILineBreakMode.WordWrap,
                };

                imgViewRedirectTNC = new UIImageView(new CGRect(View.Frame.Width - 16 - 20, 16, GetScaledWidth(20F), GetScaledHeight(20F)))
                {
                    Image = IsTNC ? UIImage.FromBundle("Arrow-Expand-Down") : UIImage.FromBundle("Arrow-Expand"),
                    ContentMode = UIViewContentMode.ScaleAspectFill,
                };

                _containerRedirectTNC.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    IsTNC = !IsTNC;

                    RefreshView();
                }));

                viewLineRedirectTNC = GenericLine.GetLine(new CGRect(0, lblRedirectTNC.Frame.GetMaxY() + 16, View.Frame.Width, 1));

                _containerRedirectTNC.AddSubviews(new UIView[] { lblRedirectTNC, imgViewRedirectTNC, viewLineRedirectTNC });
                _svContainer.AddSubview(_containerRedirectTNC);

                //ContainerRedirectTNCContent
                if (IsTNC)
                {
                    _containerRedirectTNCContent = new UIView(new CGRect(0, viewLineRedirectTNC.Frame.GetMaxY(), View.Frame.Width - 18, GetScaledHeight(48)))
                    {
                        BackgroundColor = UIColor.White
                    };

                    NSError htmlBodyError = null;
                    NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(string.Format(GetI18NValue("tnbTermUseContent"))
                                , ref htmlBodyError, TNBFont.FONTNAME_300, (float)TNBFont.GetFontSize(14F));

                    NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
                    mutableHTMLBody.AddAttributes(new UIStringAttributes
                    {
                        ForegroundColor = MyTNBColor.CharcoalGrey
                    }, new NSRange(0, htmlBody.Length));


                    lblRedirectTNCContent = new UITextView(new CGRect(18, 16, _containerRedirectTNCContent.Frame.Width - 18, 14))
                    {
                        Editable = false,
                        ScrollEnabled = false,
                        AttributedText = mutableHTMLBody,
                        TextAlignment = UITextAlignment.Left
                    };

                    CGSize newSize = lblRedirectTNCContent.SizeThatFits(new CGSize(View.Frame.Width - 23, GetScaledHeight(160)));//update size  label
                    lblRedirectTNCContent.Frame = new CGRect(18, 16, _containerRedirectTNCContent.Frame.Width - 18, newSize.Height + 16);
                    _containerRedirectTNCContent.Frame = new CGRect(0, _containerRedirectTNC.Frame.GetMaxY(), View.Frame.Width, lblRedirectTNCContent.Frame.GetMaxY());

                    viewLineRedirectTNC = GenericLine.GetLine(new CGRect(0, lblRedirectTNCContent.Frame.GetMaxY() + 16, View.Frame.Width, 1));
                    _containerRedirectTNCContent.Frame = new CGRect(0, _containerRedirectTNC.Frame.GetMaxY(), View.Frame.Width, viewLineRedirectTNC.Frame.GetMaxY());

                    _containerRedirectTNC.Frame = new CGRect(0, _containerDataDisclamer.Frame.GetMaxY(), View.Frame.Width, _containerRedirectTNCContent.Frame.GetMaxY());

                    _containerRedirectTNCContent.AddSubviews(lblRedirectTNCContent, viewLineRedirectTNC);
                    _containerRedirectTNC.AddSubview(_containerRedirectTNCContent);
                    _svContainer.AddSubview(_containerRedirectTNC);

                }
            }

        }

        public void ContainerTNC()
        {
            {
                _containerRedirectTNC = new UIView(new CGRect(0, _containerDataDisclamer.Frame.GetMaxY(), View.Frame.Width, 53))
                {
                    BackgroundColor = UIColor.White
                };

                lblRedirectTNC = new UILabel(new CGRect(18, 16, _containerRedirectTNC.Frame.Width - 16, 20))
                {
                    Font = TNBFont.MuseoSans_14_500,
                    TextColor = MyTNBColor.WaterBlue,
                    Text = GetI18NValue(EnquiryConstants.tnbTermUse),
                    LineBreakMode = UILineBreakMode.WordWrap,
                };

                imgViewRedirectTNC = new UIImageView(new CGRect(View.Frame.Width - 16 - 20, 16, GetScaledWidth(20F), GetScaledHeight(20F)))
                {
                    Image = UIImage.FromBundle("Arrow-Expand"),
                    ContentMode = UIViewContentMode.ScaleAspectFill,
                };

                imgViewRedirectTNC.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {


                }));

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

        public void ContainerPrivacyPolicy()
        {
            _containerPrivacyPolicy = new UIView(new CGRect(0, _containerRedirectTNC.Frame.GetMaxY(), View.Frame.Width, 53))
            {
                BackgroundColor = UIColor.White
            };

            lblPrivacyPolicy = new UILabel(new CGRect(18, 16, _containerPrivacyPolicy.Frame.Width - 16, 20))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = GetI18NValue(EnquiryConstants.tnbTermUse),
                LineBreakMode = UILineBreakMode.WordWrap,
            };

            imgViewPrivacyPolicy = new UIImageView(new CGRect(View.Frame.Width - 16 - 20, 16, GetScaledWidth(20F), GetScaledHeight(20F)))
            {
                Image = UIImage.FromBundle("Arrow-Expand"),
                ContentMode = UIViewContentMode.ScaleAspectFill,
            };

            viewLinePrivacyPolicy = GenericLine.GetLine(new CGRect(0, lblPrivacyPolicy.Frame.GetMaxY() + 16, View.Frame.Width, 1));

            _containerPrivacyPolicy.AddSubviews(new UIView[] { lblPrivacyPolicy, imgViewPrivacyPolicy, viewLinePrivacyPolicy });
            _svContainer.AddSubview(_containerPrivacyPolicy);
        }

        public void ContainerAntiSpamPolicy()
        {
            _containerAntiSpamPolicy = new UIView(new CGRect(0, _containerPrivacyPolicy.Frame.GetMaxY(), View.Frame.Width, 53))
            {
                BackgroundColor = UIColor.White
            };

            lblAntiSpamPolicy = new UILabel(new CGRect(18, 16, _containerAntiSpamPolicy.Frame.Width - 16, 20))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = GetI18NValue(EnquiryConstants.tnbTermUse),
                LineBreakMode = UILineBreakMode.WordWrap,
            };

            imgViewAntiSpamPolicy = new UIImageView(new CGRect(View.Frame.Width - 16 - 20, 16, GetScaledWidth(20F), GetScaledHeight(20F)))
            {
                Image = UIImage.FromBundle("Arrow-Expand"),
                ContentMode = UIViewContentMode.ScaleAspectFill,
            };

            viewLineAntiSpamPolicy = GenericLine.GetLine(new CGRect(0, lblPrivacyPolicy.Frame.GetMaxY() + 16, View.Frame.Width, 1));

            _containerAntiSpamPolicy.AddSubviews(new UIView[] { lblAntiSpamPolicy, imgViewAntiSpamPolicy, viewLineAntiSpamPolicy });
            _svContainer.AddSubview(_containerAntiSpamPolicy);
        }

        public void ContainerPersonalDataProtectionPolicy()
        {
            _containerPersonalDataProtectionPolicy = new UIView(new CGRect(0, _containerAntiSpamPolicy.Frame.GetMaxY(), View.Frame.Width, 53))
            {
                BackgroundColor = UIColor.White
            };

            lblPersonalDataProtectionPolicy = new UILabel(new CGRect(18, 16, _containerPersonalDataProtectionPolicy.Frame.Width - 16, 20))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = GetI18NValue(EnquiryConstants.tnbTermUse),
                LineBreakMode = UILineBreakMode.WordWrap,
            };

            imgViewPersonalDataProtectionPolicy = new UIImageView(new CGRect(View.Frame.Width - 16 - 20, 16, GetScaledWidth(20F), GetScaledHeight(20F)))
            {
                Image = UIImage.FromBundle("Arrow-Expand"),
                ContentMode = UIViewContentMode.ScaleAspectFill,
            };

            viewLinePersonalDataProtectionPolicy = GenericLine.GetLine(new CGRect(0, lblPrivacyPolicy.Frame.GetMaxY() + 16, View.Frame.Width, 1));

            _containerPersonalDataProtectionPolicy.AddSubviews(new UIView[] { lblPersonalDataProtectionPolicy, imgViewPersonalDataProtectionPolicy, viewLinePersonalDataProtectionPolicy });
            _svContainer.AddSubview(_containerPersonalDataProtectionPolicy);
        }
        */

        private void RefreshView()
        {
            _containerDataDisclamer.RemoveFromSuperview();
            //_containerRedirectTNC.RemoveFromSuperview();
            //_containerPrivacyPolicy.RemoveFromSuperview();
            //_containerAntiSpamPolicy.RemoveFromSuperview();
            //_containerPersonalDataProtectionPolicy.RemoveFromSuperview();

            ContainerDataDisclamer();
            ContainerTNCRedirect1();
            ContainerTNCRedirect2();
            //ContainerTNC();
            //ContainerPrivacyPolicy();
            //ContainerAntiSpamPolicy();
            //ContainerPersonalDataProtectionPolicy();
            UpdateContentSize();

        }

        private void OpenLink(string absURL,string title)
        {
            string urlString = absURL;
            UIViewController baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
            UIViewController topVc = AppDelegate.GetTopViewController(baseRootVc);
            if (topVc != null)
            {
                BrowserViewController viewController = new BrowserViewController();
                if (viewController != null)
                {
                        viewController.NavigationTitle = title;
                        viewController.URL = urlString;
                        viewController.IsDelegateNeeded = false;
                        UINavigationController navController = new UINavigationController(viewController)
                        {
                            ModalPresentationStyle = UIModalPresentationStyle.FullScreen
                        };
                        topVc.PresentViewController(navController, true, null);
                }
            }
      
        }

        private nfloat GetScrollHeight()
        {
            return (nfloat)((_containerRedirectTNC2.Frame.GetMaxY()));
        }

        private void UpdateContentSize()
        {
            _svContainer.ContentSize = new CGRect(0f, 0f, View.Frame.Width, GetScrollHeight() + 64).Size;
        }

    }
}