using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using myTNB.Profile;
using myTNB.SitecoreCMS;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using UIKit;

namespace myTNB.Registration
{
    public partial class TermsAndConditionViewController : CustomUIViewController
    {
        public TermsAndConditionViewController(IntPtr handle) : base(handle) { }

        private List<FullRTEPagesModel> _tncItems = new List<FullRTEPagesModel>();

        public bool isPresentedVC;

        public override void ViewDidLoad()
        {
            PageName = ProfileConstants.Pagename_TnC;
            base.ViewDidLoad();
            AddBackButton();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            GetTandC();
        }

        private void GetTandC()
        {
            if (NetworkUtility.isReachable)
            {
                InvokeOnMainThread(() =>
                {
                    ActivityIndicator.Show();
                    InvokeInBackground(() =>
                    {
                        SitecoreServices.Instance.LoadTermsAndCondition().ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                ActivityIndicator.Hide();
                                InitializedSubviews();
                            });
                        });
                    });
                });
            }
            else
            {
                DisplayNoDataAlert();
            }
        }

        private void InitializedSubviews()
        {
            NSError error = null;
            NSAttributedString htmlString = new NSAttributedString(GetContent()
                , new NSAttributedStringDocumentAttributes { DocumentType = NSDocumentType.HTML }, ref error);
            NSMutableAttributedString mutableHTMLString = new NSMutableAttributedString(htmlString);

            UIStringAttributes attributes = new UIStringAttributes
            {
                Font = MyTNBFont.MuseoSans12
            };
            UIStringAttributes linkAttributes = new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.PowerBlue,
                Font = MyTNBFont.MuseoSans12,
                UnderlineStyle = NSUnderlineStyle.Single,
                UnderlineColor = MyTNBColor.PowerBlue
            };
            mutableHTMLString.AddAttributes(attributes, new NSRange(0, htmlString.Length));

            UITextView txtViewTNC = new UITextView(new CGRect(16, 10, View.Frame.Width - 32, View.Frame.Height))
            {
                Editable = false,
                ScrollEnabled = true,
                TextAlignment = UITextAlignment.Justified,
                AttributedText = mutableHTMLString,
                WeakLinkTextAttributes = linkAttributes.Dictionary,
                ShowsVerticalScrollIndicator = false
            };
            View.AddSubview(txtViewTNC);
        }

        private void AddBackButton()
        {
            Title = GetI18NValue(ProfileConstants.I18N_NavTitle);
            NavigationItem.HidesBackButton = true;
            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                if (isPresentedVC)
                {
                    DismissViewController(true, null);
                }
                else
                {
                    NavigationController.PopViewController(true);
                }
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        private string GetContent()
        {
            if (IsFromSitecore)
            {
                string content = string.Empty;
                content += _tncItems[0].Title;
                content += "<br>";
                content += _tncItems[0].PublishedDate;
                content += "<br><br>";
                content += _tncItems[0].GeneralText;
                return content;
            }
            else
            {
                return TnCManager.Instance.GetTnC(TNBGlobal.APP_LANGUAGE == "EN" ? TnCManager.Language.EN : TnCManager.Language.MS);
            }
        }

        private bool IsFromSitecore
        {
            get
            {
                TermsAndConditionEntity tncEntity = new TermsAndConditionEntity();
                _tncItems = tncEntity.GetAllItems();
                return _tncItems != null && _tncItems.Count != 0 && _tncItems[0] != null;
            }
        }
    }
}