using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Drawing;
using myTNB.SitecoreCMS.Model;
using myTNB.Home.Components;
using System.Collections.Generic;

namespace myTNB
{
    public partial class PromotionDetailsViewController : UIViewController
    {
        public PromotionDetailsViewController(IntPtr handle) : base(handle)
        {
        }

        public PromotionsModel Promotion = new PromotionsModel();

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetNavigationBar();
            SetSubViews();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        void SetNavigationBar()
        {
            NavigationItem.HidesBackButton = true;
            NavigationItem.Title = "Promotions";
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle("Back-White"), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;

            UIBarButtonItem btnDownload = new UIBarButtonItem(UIImage.FromBundle("IC-Header-Share"), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                var message = NSObject.FromObject(Promotion.Title);
                string url = Promotion.GeneralLinkUrl;
                var item = NSObject.FromObject(url);
                var activityItems = new NSObject[] { message, item };
                UIActivity[] applicationActivities = null;
                var activityController = new UIActivityViewController(activityItems, applicationActivities);
                PresentViewController(activityController, true, null);
            });
            NavigationItem.RightBarButtonItem = btnDownload;
        }

        void SetSubViews()
        {
            UIScrollView scrollViewContent = new UIScrollView(new CGRect(0, 0, View.Frame.Width, View.Frame.Height - 64));
            scrollViewContent.ShowsVerticalScrollIndicator = false;
            UIView viewBanner = new UIView(new CGRect(0, 0, View.Frame.Width, View.Frame.Width / 1.777));
            UIImageView imgBanner = new UIImageView(new CGRect(0, 0, View.Frame.Width, View.Frame.Width / 1.777));
            viewBanner.AddSubview(imgBanner);
            ActivityIndicatorComponent _activityIndicator = new ActivityIndicatorComponent(viewBanner);
            _activityIndicator.Show();
            NSUrl url = new NSUrl(Promotion.Image);
            NSUrlSession session = NSUrlSession
                .FromConfiguration(NSUrlSessionConfiguration.DefaultSessionConfiguration);
            NSUrlSessionDataTask dataTask = session.CreateDataTask(url, (data, response, error) =>
            {
                if (error == null && response != null && data != null)
                {
                    InvokeOnMainThread(() =>
                    {
                        imgBanner.Image = UIImage.LoadFromData(data);
                        _activityIndicator.Hide();
                    });
                }
            });
            dataTask.Resume();

            UILabel lblTitle = new UILabel(new CGRect(18, viewBanner.Frame.Height + 16, View.Frame.Width - 36, 18));
            lblTitle.TextAlignment = UITextAlignment.Left;
            lblTitle.TextColor = myTNBColor.PowerBlue();
            lblTitle.Font = myTNBFont.MuseoSans16();
            lblTitle.Lines = 0;
            lblTitle.LineBreakMode = UILineBreakMode.WordWrap;
            lblTitle.Text = Promotion.Title;
            CGSize newLblTitleSize = GetLabelSize(lblTitle, lblTitle.Frame.Width, 1000);
            lblTitle.Frame = new CGRect(lblTitle.Frame.X, lblTitle.Frame.Y, lblTitle.Frame.Width, newLblTitleSize.Height);

            UILabel lblDetails = new UILabel(new CGRect(18, lblTitle.Frame.Height + lblTitle.Frame.Y + 16, View.Frame.Width - 36, 18));
            lblDetails.TextAlignment = UITextAlignment.Left;
            lblDetails.TextColor = myTNBColor.TunaGrey();
            lblDetails.Font = myTNBFont.MuseoSans14_300();
            lblDetails.Lines = 0;
            lblDetails.LineBreakMode = UILineBreakMode.WordWrap;
            lblDetails.Text = Promotion.SubText;
            CGSize newLblDetailsSize = GetLabelSize(lblDetails, lblDetails.Frame.Width, 1000);
            lblDetails.Frame = new CGRect(lblDetails.Frame.X, lblDetails.Frame.Y, lblDetails.Frame.Width, newLblDetailsSize.Height);

            scrollViewContent.AddSubviews(new UIView[] { viewBanner, lblTitle, lblDetails });

            CGRect referenceFrame = lblDetails.Frame;
            UIStringAttributes linkAttributes = new UIStringAttributes
            {
                ForegroundColor = myTNBColor.PowerBlue(),
                Font = myTNBFont.MuseoSans14_300(),
                UnderlineStyle = NSUnderlineStyle.Single,
                UnderlineColor = myTNBColor.PowerBlue()
            };

            // Creates a UIFont for the body text
            UIFontDescriptor bodyDescriptor = UIFontDescriptor.PreferredBody;

            // Creates a bold version of it:
            UIFontDescriptor boldDescriptor = bodyDescriptor.CreateWithTraits(UIFontDescriptorSymbolicTraits.Bold);
            UIFont boldBodyFont = UIFont.FromDescriptor(boldDescriptor, 14f);

            UIStringAttributes boldAttributes = new UIStringAttributes
            {
                Font = boldBodyFont,
                ForegroundColor = myTNBColor.TunaGrey()
            };

            UIStringAttributes tunaGrey14_300Attr = new UIStringAttributes
            {
                Font = myTNBFont.MuseoSans14_300(),
                ForegroundColor = myTNBColor.TunaGrey()
            };

            if (!string.IsNullOrEmpty(Promotion.CampaignPeriod)
                || !string.IsNullOrWhiteSpace(Promotion.CampaignPeriod))
            {
                List<string> toBoldList = GetToBoldList(Promotion.CampaignPeriod);

                NSError htmlCampaignPeriodError = null;
                NSAttributedString htmlCampaignPeriod = new NSAttributedString(Promotion.CampaignPeriod
                                                                       , new NSAttributedStringDocumentAttributes { DocumentType = NSDocumentType.HTML }
                                                                       , ref htmlCampaignPeriodError);
                NSMutableAttributedString mutableHTMLCampaignPeriod = new NSMutableAttributedString(htmlCampaignPeriod);
                mutableHTMLCampaignPeriod.AddAttributes(tunaGrey14_300Attr
                                                        , new NSRange(0, htmlCampaignPeriod.Length));

                UITextView txtViewCampaignPeriod = new UITextView();
                txtViewCampaignPeriod.Editable = false;
                txtViewCampaignPeriod.ScrollEnabled = false;
                txtViewCampaignPeriod.TextAlignment = UITextAlignment.Justified;
                txtViewCampaignPeriod.AttributedText = mutableHTMLCampaignPeriod;
                txtViewCampaignPeriod.WeakLinkTextAttributes = linkAttributes.Dictionary;

                try
                {
                    for (int i = 0; i < toBoldList.Count; i++)
                    {
                        int startIndex = txtViewCampaignPeriod.Text.IndexOf(toBoldList[i], StringComparison.CurrentCultureIgnoreCase);
                        int length = toBoldList[i].Length;
                        mutableHTMLCampaignPeriod.AddAttributes(boldAttributes, new NSRange(startIndex, length));
                    }
                    txtViewCampaignPeriod.AttributedText = mutableHTMLCampaignPeriod;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in attributes: " + e.Message);
                }

                CGSize campaignPeriodNewSize = txtViewCampaignPeriod.SizeThatFits(new CGSize(View.Frame.Width - 36, 1000f));
                txtViewCampaignPeriod.Frame = new CGRect(15
                                                         , referenceFrame.Y + referenceFrame.Height + 1
                                                         , View.Frame.Width - 30
                                                         , campaignPeriodNewSize.Height);
                scrollViewContent.AddSubview(txtViewCampaignPeriod);
                referenceFrame = txtViewCampaignPeriod.Frame;
            }
            /*
            if (!string.IsNullOrEmpty(Promotion.Prizes)
               || !string.IsNullOrWhiteSpace(Promotion.Prizes))
            {
                List<string> toBoldList = GetToBoldList(Promotion.Prizes);
                NSError htmlPrizesError = null;
                NSAttributedString htmlPrizes = new NSAttributedString(Promotion.Prizes
                                                                       , new NSAttributedStringDocumentAttributes { DocumentType = NSDocumentType.HTML }
                                                                       , ref htmlPrizesError);
                NSMutableAttributedString mutableHTMLPrizes = new NSMutableAttributedString(htmlPrizes);

                mutableHTMLPrizes.AddAttributes(tunaGrey14_300Attr, new NSRange(0, htmlPrizes.Length));

                UITextView txtViewPrizes = new UITextView();
                txtViewPrizes.Editable = false;
                txtViewPrizes.ScrollEnabled = false;
                txtViewPrizes.TextAlignment = UITextAlignment.Justified;
                txtViewPrizes.AttributedText = mutableHTMLPrizes;
                txtViewPrizes.WeakLinkTextAttributes = linkAttributes.Dictionary;

                for (int i = 0; i < toBoldList.Count; i++)
                {
                    int startIndex = txtViewPrizes.Text.IndexOf(toBoldList[i], StringComparison.CurrentCultureIgnoreCase);
                    int length = toBoldList[i].Length;
                    mutableHTMLPrizes.AddAttributes(boldAttributes, new NSRange(startIndex, length));
                }
                txtViewPrizes.AttributedText = mutableHTMLPrizes;

                CGSize prizesNewSize = txtViewPrizes.SizeThatFits(new CGSize(View.Frame.Width - 36, 1000f));
                txtViewPrizes.Frame = new CGRect(15
                                                 , referenceFrame.Y + referenceFrame.Height + 1
                                                         , View.Frame.Width - 30
                                                         , prizesNewSize.Height);
                scrollViewContent.AddSubview(txtViewPrizes);
                referenceFrame = txtViewPrizes.Frame;
            }

            if (!string.IsNullOrEmpty(Promotion.HowToWin)
                || !string.IsNullOrWhiteSpace(Promotion.HowToWin))
            {
                List<string> toBoldList = GetToBoldList(Promotion.HowToWin);
                NSError htmlHowToWinError = null;
                NSAttributedString htmlHowToWin = new NSAttributedString(Promotion.HowToWin
                                                                       , new NSAttributedStringDocumentAttributes { DocumentType = NSDocumentType.HTML }
                                                                       , ref htmlHowToWinError);
                NSMutableAttributedString mutableHTMLHowToWin = new NSMutableAttributedString(htmlHowToWin);

                mutableHTMLHowToWin.AddAttributes(tunaGrey14_300Attr, new NSRange(0, htmlHowToWin.Length));

                UITextView txtViewHowToWin = new UITextView();
                txtViewHowToWin.Editable = false;
                txtViewHowToWin.ScrollEnabled = false;
                txtViewHowToWin.TextAlignment = UITextAlignment.Justified;
                txtViewHowToWin.AttributedText = mutableHTMLHowToWin;
                txtViewHowToWin.WeakLinkTextAttributes = linkAttributes.Dictionary;

                for (int i = 0; i < toBoldList.Count; i++)
                {
                    int startIndex = txtViewHowToWin.Text.IndexOf(toBoldList[i], StringComparison.CurrentCultureIgnoreCase);
                    int length = toBoldList[i].Length;
                    mutableHTMLHowToWin.AddAttributes(boldAttributes, new NSRange(startIndex, length));
                }
                txtViewHowToWin.AttributedText = mutableHTMLHowToWin;

                CGSize howToWinNewSize = txtViewHowToWin.SizeThatFits(new CGSize(View.Frame.Width - 36, 1000f));
                txtViewHowToWin.Frame = new CGRect(15
                                                   , referenceFrame.Y + referenceFrame.Height + 1
                                                         , View.Frame.Width - 30
                                                   , howToWinNewSize.Height);
                scrollViewContent.AddSubview(txtViewHowToWin);
                referenceFrame = txtViewHowToWin.Frame;
            }
            */
            if (!string.IsNullOrEmpty(Promotion.FooterNote)
                || !string.IsNullOrWhiteSpace(Promotion.FooterNote))
            {
                List<string> toBoldList = GetToBoldList(Promotion.FooterNote);
                NSError htmlFooterError = null;
                NSAttributedString htmlFooter = new NSAttributedString(Promotion.FooterNote
                                                                       , new NSAttributedStringDocumentAttributes { DocumentType = NSDocumentType.HTML }
                                                                       , ref htmlFooterError);
                NSMutableAttributedString mutableHTMLFooter = new NSMutableAttributedString(htmlFooter);

                UIStringAttributes footerAttributes = new UIStringAttributes
                {
                    Font = myTNBFont.MuseoSans12_300(),
                    ForegroundColor = myTNBColor.TunaGrey()
                };
                UIStringAttributes footerLinkAttributes = new UIStringAttributes
                {
                    ForegroundColor = myTNBColor.PowerBlue(),
                    Font = myTNBFont.MuseoSans12_300(),
                    UnderlineStyle = NSUnderlineStyle.Single,
                    UnderlineColor = myTNBColor.PowerBlue()
                };

                // Creates a UIFont for the body text
                UIFontDescriptor footerBodyDescriptor = UIFontDescriptor.PreferredBody;

                // Creates a bold version of it:
                UIFontDescriptor footerBoldDescriptor = footerBodyDescriptor.CreateWithTraits(UIFontDescriptorSymbolicTraits.Bold);
                UIFont footerBoldBodyFont = UIFont.FromDescriptor(footerBoldDescriptor, 12f);

                UIStringAttributes footerBoldAttributes = new UIStringAttributes
                {
                    Font = footerBoldBodyFont,
                    ForegroundColor = myTNBColor.TunaGrey()
                };

                mutableHTMLFooter.AddAttributes(footerAttributes, new NSRange(0, htmlFooter.Length));

                UITextView txtViewFooter = new UITextView();
                txtViewFooter.Editable = false;
                txtViewFooter.ScrollEnabled = false;
                txtViewFooter.TextAlignment = UITextAlignment.Justified;
                txtViewFooter.AttributedText = mutableHTMLFooter;
                txtViewFooter.WeakLinkTextAttributes = footerLinkAttributes.Dictionary;
                try
                {
                    for (int i = 0; i < toBoldList.Count; i++)
                    {
                        int startIndex = txtViewFooter.Text.IndexOf(toBoldList[i], StringComparison.CurrentCultureIgnoreCase);
                        int length = toBoldList[i].Length;
                        mutableHTMLFooter.AddAttributes(footerBoldAttributes, new NSRange(startIndex, length));
                    }
                    txtViewFooter.AttributedText = mutableHTMLFooter;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in attributes: " + e.Message);
                }

                CGSize footerNewSize = txtViewFooter.SizeThatFits(new CGSize(View.Frame.Width - 36, 1000f));
                txtViewFooter.Frame = new CGRect(15
                                                 , referenceFrame.Y + referenceFrame.Height + 16
                                                 , View.Frame.Width - 30
                                                 , footerNewSize.Height);
                scrollViewContent.AddSubviews(new UIView[] { txtViewFooter });
                referenceFrame = txtViewFooter.Frame;
            }

            scrollViewContent.ContentSize = new CGSize(View.Frame.Width, referenceFrame.Height + referenceFrame.Y + 30);
            View.AddSubview(scrollViewContent);
        }

        CGSize GetLabelSize(UILabel label, nfloat width, nfloat height)
        {
            return label.Text.StringSize(label.Font, new SizeF((float)width, (float)height));
        }

        List<int> GetAllIndex(string str, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new List<int>();
            }
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index, StringComparison.CurrentCultureIgnoreCase);
                if (index == -1)
                {
                    return indexes;
                }
                indexes.Add(index);
            }
        }

        List<string> GetToBoldList(string body)
        {
            List<int> openTags = GetAllIndex(body, "<strong>");
            List<int> closeTags = GetAllIndex(body, "</strong>");
            List<string> toBoldList = new List<string>();
            if (openTags.Count > 0 && openTags.Count == closeTags.Count)
            {
                toBoldList = new List<string>();
                for (int i = 0; i < openTags.Count; i++)
                {
                    int start = openTags[i] + 8;
                    int length = closeTags[i] - start;
                    string str = body.Substring(start, length);
                    toBoldList.Add(str);
                }
            }
            return toBoldList;
        }
    }
}