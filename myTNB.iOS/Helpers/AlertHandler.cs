using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB
{
    public static class AlertHandler
    {
        public static List<string> RedirectTypeList = new List<string> { "faqid=", "inAppBrowser=", "externalBrowser=" };

        /// <summary>
        /// Displays the no data alert.
        /// </summary>
        /// <param name="controller">Controller.</param>
        public static void DisplayNoDataAlert(UIViewController controller, Action<UIAlertAction> handler = null)
        {
            DisplayAlert(controller, "Error_NoNetworkTitle".Translate(), "Error_NoNetworkMsg".Translate(), handler);
        }

        /// <summary>
        /// Displays the service error.
        /// </summary>
        /// <param name="controller">Controller.</param>
        /// <param name="message">Message.</param>
        public static void DisplayServiceError(UIViewController controller, string message, Action<UIAlertAction> handler = null)
        {
            DisplayAlert(controller, "Error_DefaultTitle".Translate(), message, handler);
        }

        /// <summary>
        /// Displays the generic error.
        /// </summary>
        /// <param name="controller">Controller.</param>
        /// <param name="title">Title.</param>
        /// <param name="message">Message.</param>
        public static void DisplayGenericAlert(UIViewController controller, string title, string message, Action<UIAlertAction> handler = null)
        {
            DisplayAlert(controller, title, message, handler);
        }

        /// <summary>
        /// Displaies the alert.
        /// </summary>
        /// <param name="controller">Controller.</param>
        /// <param name="title">Title.</param>
        /// <param name="message">Message.</param>
        /// <param name="handler">Handler.</param>
        private static void DisplayAlert(UIViewController controller, string title, string message, Action<UIAlertAction> handler = null)
        {
            message = message ?? "Error_DefaultMessage".Translate();
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Common_Ok".Translate(), UIAlertActionStyle.Cancel, handler));
            alert.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            controller.PresentViewController(alert, animated: true, completionHandler: null);
        }

        public static void DisplayForceUpdate(string title, string message
          , string actionTitle = null, Action handler = null)
        {
            DisplayCustomAlert(title, message, new Dictionary<string, Action>() {
                { string.IsNullOrEmpty(actionTitle) ? "Common_Ok".Translate() : actionTitle, handler } }
                , UITextAlignment.Center, UITextAlignment.Center, false, 0.105F);
        }

        public static void DisplayCustomAlert(string title, string message, Dictionary<string, Action> ctaButtons
            , UITextAlignment titleAlignment = UITextAlignment.Left, UITextAlignment descriptionAlignment = UITextAlignment.Left
            , bool shouldDismissAlert = true, float marginPercentage = 0.056F, bool isDefaultURLAction = true
            , UIImage image = null)
        {
            nfloat margin = UIScreen.MainScreen.Bounds.Width * marginPercentage;
            nfloat width = UIScreen.MainScreen.Bounds.Width - (margin * 2);
            nfloat height = UIScreen.MainScreen.Bounds.Height - (18 + (UIScreen.MainScreen.Bounds.Height * 0.202F));
            nfloat maxDescriptionHeight = height - 51 - 10;
            nfloat ctaItemX = 0;
            NSError htmlBodyError = null;

            // Semi Transparent BG
            UIView viewParent = new UIView(UIScreen.MainScreen.Bounds)
            {
                BackgroundColor = new UIColor(73 / 255, 73 / 255, 74 / 255, 0.5F),
                ClipsToBounds = true
            };
            // HTML / Plain text for UITextView
            UIStringAttributes linkAttributes = new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.PowerBlue,
                Font = MyTNBFont.MuseoSans14_300,
                UnderlineStyle = NSUnderlineStyle.None,
                UnderlineColor = UIColor.Clear
            };

            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(message
                , ref htmlBodyError, MyTNBFont.FONTNAME_300, 14f);
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.TunaGrey()
            }, new NSRange(0, htmlBody.Length));

            UIImageView imgView = new UIImageView(new CGRect(0, 0, width, image == null ? 0 : width / 1.33F));
            if (image != null)
            {
                imgView.Image = image;
            }

            //Title
            UILabel lblTitle = new UILabel(new CGRect(16, 0, width - 32, 0))
            {
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.TunaGrey(),
                TextAlignment = titleAlignment,
                Text = title,
                Lines = 0
            };

            nfloat txtViewY = 10;
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrWhiteSpace(title))
            {
                CGSize titleSize = LabelHelper.GetLabelSize(lblTitle, width - 32, 60);
                lblTitle.Frame = new CGRect(16, imgView.Frame.GetMaxY() + 16, width - 32, titleSize.Height);
                txtViewY += lblTitle.Frame.GetMaxY();
                maxDescriptionHeight -= (16 + titleSize.Height);
            }

            // Body
            UITextView txtViewDetails = new UITextView
            {
                Editable = false,
                ScrollEnabled = true,
                AttributedText = mutableHTMLBody,
                WeakLinkTextAttributes = linkAttributes.Dictionary,
                ContentInset = new UIEdgeInsets(-5, -5, -5, -5)
            };
            txtViewDetails.ScrollIndicatorInsets = UIEdgeInsets.Zero;

            //Resize
            CGSize size = txtViewDetails.SizeThatFits(new CGSize(width - 24, maxDescriptionHeight));
            nfloat txtViewHeight = size.Height > maxDescriptionHeight ? maxDescriptionHeight : size.Height;
            txtViewDetails.Frame = new CGRect(12, txtViewY, width - 24, txtViewHeight +15);
            txtViewDetails.TextAlignment = descriptionAlignment;
            txtViewDetails.Layer.BorderColor = UIColor.Red.CGColor;
            txtViewDetails.Layer.BorderWidth = 1;
            UIView viewline = new UIView(new CGRect(0, txtViewDetails.Frame.GetMaxY(), width, 1))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };
            nfloat containerY = txtViewDetails.Frame.GetMaxY() + 10;

            UIView ctaContainer = new UIView(new CGRect(0, containerY, width, 51))
            {
                BackgroundColor = MyTNBColor.LightGrayBG,
                ClipsToBounds = true
            };
            nfloat ctaItemWidth = (ctaContainer.Frame.Width / ctaButtons.Count) - 0.5F;
            foreach (var item in ctaButtons)
            {
                UIView ctaBtn = new UIView(new CGRect(ctaItemX, 1, ctaItemWidth, 50))
                {
                    BackgroundColor = UIColor.White,
                    ClipsToBounds = true
                };
                ctaBtn.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    if (item.Value != null)
                    {
                        item.Value?.Invoke();
                    }
                    if (shouldDismissAlert)
                    {
                        viewParent.RemoveFromSuperview();
                    }
                }));
                UILabel ctaLbl = new UILabel(new CGRect(0, 0, ctaBtn.Frame.Width, ctaBtn.Frame.Height))
                {
                    Text = item.Key,
                    Font = MyTNBFont.MuseoSans16_500,
                    TextAlignment = UITextAlignment.Center,
                    TextColor = MyTNBColor.PowerBlue,
                    ClipsToBounds = true
                };
                ctaBtn.AddSubview(ctaLbl);
                ctaContainer.AddSubview(ctaBtn);
                ctaItemX += ctaItemWidth + 1;
            }

            nfloat viewHeight = ctaContainer.Frame.GetMaxY();
            UIView alertView = new UIView(new CGRect(margin, 0, width, viewHeight))
            {
                BackgroundColor = UIColor.White,
                ClipsToBounds = true
            };
            alertView.Layer.CornerRadius = 6.0F;
            alertView.Center = new CGPoint(UIScreen.MainScreen.Bounds.GetMidX(), UIScreen.MainScreen.Bounds.GetMidY());
            alertView.AddSubviews(new UIView[] { imgView, lblTitle, txtViewDetails, ctaContainer });

            viewParent.AddSubview(alertView);
            if (!isDefaultURLAction)
            {
                Action<NSUrl> action = new Action<NSUrl>((url) =>
                {
                    RedirectAlert(url, viewParent);
                });
                txtViewDetails.Delegate = new TextViewDelegate(action);
            }
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
            currentWindow.AddSubview(viewParent);
        }

        private static void RedirectAlert(NSUrl url, UIView viewParent)
        {
            string absURL = url?.AbsoluteString;
            if (!string.IsNullOrEmpty(absURL))
            {
                int whileCount = 0;
                bool isContained = false;
                while (!isContained && whileCount < RedirectTypeList.Count)
                {
                    isContained = absURL.Contains(RedirectTypeList[whileCount]);
                    if (isContained) { break; }
                    whileCount++;
                }

                if (isContained)
                {
                    if (RedirectTypeList[whileCount] == RedirectTypeList[0])
                    {
                        string key = absURL.Split(RedirectTypeList[0])[1];
                        key = key.Replace("%7B", "{").Replace("%7D", "}");
                        ViewHelper.GoToFAQScreenWithId(key);
                        viewParent.RemoveFromSuperview();
                    }
                    else if (RedirectTypeList[whileCount] == RedirectTypeList[1])
                    {
                        string urlString = absURL.Split(RedirectTypeList[1])[1];
                        var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                        var topVc = AppDelegate.GetTopViewController(baseRootVc);
                        if (topVc != null)
                        {
                            UIStoryboard storyBoard = UIStoryboard.FromName("Browser", null);
                            BrowserViewController viewController =
                                storyBoard.InstantiateViewController("BrowserViewController") as BrowserViewController;
                            if (viewController != null)
                            {
                                viewController.URL = urlString;
                                viewController.IsDelegateNeeded = false;
                                var navController = new UINavigationController(viewController);
                                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                                topVc.PresentViewController(navController, true, null);
                            }
                        }
                    }
                    else
                    {
                        string urlString = absURL.Split(RedirectTypeList[2])[1];
                        UIApplication.SharedApplication.OpenUrl(new NSUrl(string.Format(urlString)));
                    }
                }
            }
        }
    }
}