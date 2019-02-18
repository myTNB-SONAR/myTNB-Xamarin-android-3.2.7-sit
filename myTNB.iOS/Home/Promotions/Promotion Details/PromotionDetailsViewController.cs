using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Drawing;
using myTNB.SitecoreCMS.Model;
using myTNB.Home.Components;
using System.Collections.Generic;
using myTNB.Extensions;

namespace myTNB
{
    public partial class PromotionDetailsViewController : UIViewController
    {
        public PromotionDetailsViewController(IntPtr handle) : base(handle)
        {
        }

        public PromotionsModelV2 Promotion = new PromotionsModelV2();
        public Action OnDone { get; set; }

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
            NavigationItem.Title = "PromotionsTitle".Translate();
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle("Back-White"), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
                OnDone?.Invoke();

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
                UIBarButtonItem.AppearanceWhenContainedIn(new[] { typeof(UINavigationBar) }).TintColor = UIColor.White;
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

            if (!string.IsNullOrWhiteSpace(Promotion.LandscapeImage))
            {
                try
                {
                    ActivityIndicatorComponent _activityIndicator = new ActivityIndicatorComponent(viewBanner);
                    _activityIndicator.Show();
                    NSUrl url = new NSUrl(Promotion.LandscapeImage);
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
                }
                catch (Exception e)
                {
                    Console.WriteLine("Image load Error: " + e.Message);
                }
            }

            UILabel lblTitle = new UILabel(new CGRect(18, viewBanner.Frame.Height + 16, View.Frame.Width - 36, 18));
            lblTitle.TextAlignment = UITextAlignment.Left;
            lblTitle.TextColor = myTNBColor.PowerBlue();
            lblTitle.Font = myTNBFont.MuseoSans16();
            lblTitle.Lines = 0;
            lblTitle.LineBreakMode = UILineBreakMode.WordWrap;
            lblTitle.Text = Promotion.Title;
            CGSize newLblTitleSize = GetLabelSize(lblTitle, lblTitle.Frame.Width, 1000);
            lblTitle.Frame = new CGRect(lblTitle.Frame.X, lblTitle.Frame.Y, lblTitle.Frame.Width, newLblTitleSize.Height);

#if false
            UILabel lblDetails = new UILabel(new CGRect(18, lblTitle.Frame.Height + lblTitle.Frame.Y + 16, View.Frame.Width - 36, 18));
            lblDetails.TextAlignment = UITextAlignment.Left;
            lblDetails.TextColor = myTNBColor.TunaGrey();
            lblDetails.Font = myTNBFont.MuseoSans14_300();
            lblDetails.Lines = 0;
            lblDetails.LineBreakMode = UILineBreakMode.WordWrap;
            lblDetails.Text = Promotion.SubText;
            CGSize newLblDetailsSize = GetLabelSize(lblDetails, lblDetails.Frame.Width, 1000);
            lblDetails.Frame = new CGRect(lblDetails.Frame.X, lblDetails.Frame.Y, lblDetails.Frame.Width, newLblDetailsSize.Height);
#endif
            scrollViewContent.AddSubviews(new UIView[] { viewBanner, lblTitle });

            CGRect referenceFrame = lblTitle.Frame;
            UIStringAttributes linkAttributes = new UIStringAttributes
            {
                ForegroundColor = myTNBColor.PowerBlue(),
                Font = myTNBFont.MuseoSans14_300(),
                UnderlineStyle = NSUnderlineStyle.Single,
                UnderlineColor = myTNBColor.PowerBlue()
            };

            UIStringAttributes tunaGreyAttr = new UIStringAttributes
            {
                ForegroundColor = myTNBColor.TunaGrey()
            };

            if (!string.IsNullOrWhiteSpace(Promotion.HeaderContent))
            {
                NSError htmlError = null;
                NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(Promotion.HeaderContent, ref htmlError,
                                                                                         myTNBFont.FONTNAME_300, 14f);

                NSMutableAttributedString mutableHTML = new NSMutableAttributedString(htmlBody);
                mutableHTML.AddAttributes(tunaGreyAttr
                                                        , new NSRange(0, htmlBody.Length));

                UITextView txtViewHeader = new UITextView();
                txtViewHeader.Editable = false;
                txtViewHeader.ScrollEnabled = false;
                txtViewHeader.TextAlignment = UITextAlignment.Justified;
                txtViewHeader.AttributedText = mutableHTML;
                txtViewHeader.WeakLinkTextAttributes = linkAttributes.Dictionary;


                CGSize newSize = txtViewHeader.SizeThatFits(new CGSize(View.Frame.Width - 36, 1000f));
                txtViewHeader.Frame = new CGRect(15
                                                         , referenceFrame.GetMaxY() + 1
                                                         , View.Frame.Width - 30
                                                         , newSize.Height);
                scrollViewContent.AddSubview(txtViewHeader);
                referenceFrame = txtViewHeader.Frame;
            }

            if (!string.IsNullOrWhiteSpace(Promotion.BodyContent))
            {
                NSError htmlBodyError = null;
                NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(Promotion.BodyContent, ref htmlBodyError,
                                                                                         myTNBFont.FONTNAME_300, 14f);

                NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
                mutableHTMLBody.AddAttributes(tunaGreyAttr
                                                        , new NSRange(0, htmlBody.Length));

                UITextView txtViewBody = new UITextView();
                txtViewBody.Editable = false;
                txtViewBody.ScrollEnabled = false;
                txtViewBody.TextAlignment = UITextAlignment.Justified;
                txtViewBody.AttributedText = mutableHTMLBody;
                txtViewBody.WeakLinkTextAttributes = linkAttributes.Dictionary;


                CGSize campaignPeriodNewSize = txtViewBody.SizeThatFits(new CGSize(View.Frame.Width - 36, 1000f));
                txtViewBody.Frame = new CGRect(15
                                                         , referenceFrame.GetMaxY() + 1
                                                         , View.Frame.Width - 30
                                                         , campaignPeriodNewSize.Height);
                scrollViewContent.AddSubview(txtViewBody);
                referenceFrame = txtViewBody.Frame;
            }

            if (!string.IsNullOrWhiteSpace(Promotion.FooterContent))
            {
                NSError htmlFooterError = null;
                NSAttributedString htmlFooter = TextHelper.ConvertToHtmlWithFont(Promotion.FooterContent, ref htmlFooterError,
                                                                                         myTNBFont.FONTNAME_300, 10f);
                NSMutableAttributedString mutableHTMLFooter = new NSMutableAttributedString(htmlFooter);

                UIStringAttributes footerLinkAttributes = new UIStringAttributes
                {
                    ForegroundColor = myTNBColor.PowerBlue(),
                    Font = myTNBFont.MuseoSans10_300(),
                    UnderlineStyle = NSUnderlineStyle.Single,
                    UnderlineColor = myTNBColor.PowerBlue()
                };

                mutableHTMLFooter.AddAttributes(tunaGreyAttr, new NSRange(0, htmlFooter.Length));

                UITextView txtViewFooter = new UITextView();
                txtViewFooter.Editable = false;
                txtViewFooter.ScrollEnabled = false;
                txtViewFooter.TextAlignment = UITextAlignment.Justified;
                txtViewFooter.AttributedText = mutableHTMLFooter;
                txtViewFooter.WeakLinkTextAttributes = footerLinkAttributes.Dictionary;

                CGSize footerNewSize = txtViewFooter.SizeThatFits(new CGSize(View.Frame.Width - 36, 1000f));
                txtViewFooter.Frame = new CGRect(15
                                                 , referenceFrame.GetMaxY() + 16
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


    }
}