using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using UIKit;

namespace myTNB.Registration
{
    public partial class TermsAndConditionViewController : UIViewController
    {
        public TermsAndConditionViewController(IntPtr handle) : base(handle)
        {
        }

        List<FullRTEPagesModel> _tncItems = new List<FullRTEPagesModel>();

        public bool isPresentedVC = false;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            AddBackButton();
            InitializedSubviews();
        }

        void InitializedSubviews()
        {
            NSError error = null;
            NSAttributedString htmlString = new NSAttributedString(GetContent()
                , new NSAttributedStringDocumentAttributes { DocumentType = NSDocumentType.HTML }, ref error);
            NSMutableAttributedString mutableHTMLString = new NSMutableAttributedString(htmlString);
            NSMutableParagraphStyle style = new NSMutableParagraphStyle
            {
                Alignment = UITextAlignment.Justified
            };
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

            UITextView txtViewTNC = new UITextView(new CGRect(18, 10, View.Frame.Width - 36, View.Frame.Height - 50))
            {
                Editable = false,
                ScrollEnabled = true,
                TextAlignment = UITextAlignment.Justified,
                AttributedText = mutableHTMLString,
                WeakLinkTextAttributes = linkAttributes.Dictionary
            };
            View.AddSubview(txtViewTNC);
        }

        void AddBackButton()
        {
            Title = "Registration_TnCTitle".Translate();
            NavigationItem.HidesBackButton = true;
            UIImage backImg = UIImage.FromBundle("Back-White");
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

        string GetTNCFromFile()
        {
            string tncStatement = string.Empty;
            try
            {
                tncStatement = System.IO.File.ReadAllText("TermsAndCondition.txt");
            }
            catch (Exception e)
            {
                Debug.WriteLine("ERROR>>>>>> " + e.Message);
            }
            return tncStatement;
        }

        string GetContent()
        {
            if (IsFromSiteCore())
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
                return GetTNCFromFile();
            }
        }

        bool IsFromSiteCore()
        {
            TermsAndConditionEntity tncEntity = new TermsAndConditionEntity();
            _tncItems = tncEntity.GetAllItems();
            return _tncItems != null && _tncItems.Count != 0 && _tncItems[0] != null;
        }
    }
}