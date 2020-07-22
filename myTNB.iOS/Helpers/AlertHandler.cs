using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB
{
    public static class AlertHandler
    {
        public static List<string> RedirectTypeList = new List<string> {
            "inAppBrowser=",
            "externalBrowser=",
            "tel=",
            "whatsnew=",
            "faq=",
            "reward=",
            "http",
            "tel:",
            "whatsnewid=",
            "faqid=",
            "rewardid="
        };

        /// <summary>
        /// Displays the no data alert.
        /// </summary>
        /// <param name="controller">Controller.</param>
        public static void DisplayNoDataAlert(UIViewController controller, Action<UIAlertAction> handler = null)
        {
            DisplayAlert(controller
                , LanguageUtility.GetErrorI18NValue(Constants.Error_NoDataConnectionTitle)
                , LanguageUtility.GetErrorI18NValue(Constants.Error_NoDataConnectionMessage)
                , handler);
        }

        /// <summary>
        /// Displays the service error.
        /// </summary>
        /// <param name="controller">Controller.</param>
        /// <param name="message">Message.</param>
        public static void DisplayServiceError(UIViewController controller, string message, Action<UIAlertAction> handler = null)
        {
            DisplayAlert(controller, LanguageUtility.GetErrorI18NValue(Constants.Error_DefaultErrorTitle), message, handler);
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
            message = string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message)
                ? LanguageUtility.GetErrorI18NValue(Constants.Error_DefaultErrorMessage) : message;
            UIAlertController alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create(LanguageUtility.GetCommonI18NValue(Constants.Common_Ok), UIAlertActionStyle.Cancel, handler));
            alert.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            controller.PresentViewController(alert, animated: true, completionHandler: null);
        }

        public static void DisplayForceUpdate(string title, string message
          , string actionTitle = null, Action handler = null)
        {
            DisplayCustomAlert(title, message, new Dictionary<string, Action>() {
                { string.IsNullOrEmpty(actionTitle) ? LanguageUtility.GetCommonI18NValue(Constants.Common_Ok) : actionTitle, handler } }
                , UITextAlignment.Center, UITextAlignment.Center, false, 0.105F);
        }

        public static void DisplayCustomAlert(string title, string message, Dictionary<string, Action> ctaButtons
            , UITextAlignment titleAlignment = UITextAlignment.Left, UITextAlignment descriptionAlignment = UITextAlignment.Left
            , bool shouldDismissAlert = true, float marginPercentage = 0.056F, bool isDefaultURLAction = true
            , UIImage image = null)
        {
            nfloat sidePadding = ScaleUtility.GetScaledWidth(16F);
            nfloat topPadding = ScaleUtility.GetScaledHeight(16F);
            nfloat margin = UIScreen.MainScreen.Bounds.Width * marginPercentage;
            nfloat width = UIScreen.MainScreen.Bounds.Width - (margin * 2);
            nfloat height = UIScreen.MainScreen.Bounds.Height - (ScaleUtility.GetScaledHeight(18F) + (UIScreen.MainScreen.Bounds.Height * 0.202F));
            nfloat maxDescriptionHeight = height - ScaleUtility.GetScaledHeight(53F) - ScaleUtility.GetScaledHeight(16F);
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
                Font = TNBFont.MuseoSans_14_300,
                UnderlineStyle = NSUnderlineStyle.None,
                UnderlineColor = UIColor.Clear
            };

            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(message
                , ref htmlBodyError, TNBFont.FONTNAME_300, (float)TNBFont.GetFontSize(14F));
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.CharcoalGrey,
                ParagraphStyle = new NSMutableParagraphStyle
                {
                    LineSpacing = 3.0f
                }
            }, new NSRange(0, htmlBody.Length));

            UIImageView imgView = new UIImageView(new CGRect(0, 0, width, image == null ? 0 : ScaleUtility.GetScaledHeight(155F)));//width / 1.33F));
            if (image != null)
            {
                imgView.Image = image;
            }

            //Title
            UILabel lblTitle = new UILabel(new CGRect(sidePadding, 0, width - (sidePadding * 2), 0))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = titleAlignment,
                Text = title,
                Lines = 0
            };

            nfloat txtViewY = imgView.Frame.GetMaxY() + ScaleUtility.GetScaledHeight(12F);
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrWhiteSpace(title))
            {
                CGSize titleSize = LabelHelper.GetLabelSize(lblTitle, width - (sidePadding * 2), ScaleUtility.GetScaledHeight(60F));
                lblTitle.Frame = new CGRect(sidePadding, imgView.Frame.GetMaxY() + ScaleUtility.GetScaledHeight(24F), width - (sidePadding * 2), titleSize.Height);
                txtViewY = lblTitle.Frame.GetMaxY() + ScaleUtility.GetScaledHeight(12F);
                maxDescriptionHeight -= (topPadding + titleSize.Height);
            }

            // Body
            UITextView txtViewDetails = new UITextView
            {
                Editable = false,
                ScrollEnabled = true,
                AttributedText = mutableHTMLBody,
                WeakLinkTextAttributes = linkAttributes.Dictionary,
                ContentInset = new UIEdgeInsets(-5, 0, -5, 0)
            };
            txtViewDetails.ScrollIndicatorInsets = UIEdgeInsets.Zero;
            txtViewDetails.TextContainer.LineFragmentPadding = 0F;

            //Resize
            CGSize size = txtViewDetails.SizeThatFits(new CGSize(width - (sidePadding * 2), maxDescriptionHeight));
            nfloat txtViewHeight = size.Height > maxDescriptionHeight ? maxDescriptionHeight : size.Height;
            txtViewDetails.Frame = new CGRect(sidePadding, txtViewY, width - (sidePadding * 2), txtViewHeight);
            txtViewDetails.TextAlignment = descriptionAlignment;

            nfloat containerY = txtViewDetails.Frame.GetMaxY() + topPadding;

            UIView ctaContainer = new UIView(new CGRect(0, containerY, width, ScaleUtility.GetScaledHeight(53F)))
            {
                BackgroundColor = MyTNBColor.LightGrayBG,
                ClipsToBounds = true
            };
            nfloat ctaItemWidth = (ctaContainer.Frame.Width / ctaButtons.Count) - ScaleUtility.GetScaledWidth(0.5F);
            foreach (KeyValuePair<string, Action> item in ctaButtons)
            {
                UIView ctaBtn = new UIView(new CGRect(ctaItemX, ScaleUtility.GetScaledHeight(1F), ctaItemWidth, ScaleUtility.GetScaledHeight(52F)))
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
                    Font = TNBFont.MuseoSans_16_500,
                    TextAlignment = UITextAlignment.Center,
                    TextColor = MyTNBColor.WaterBlue,
                    ClipsToBounds = true
                };
                ctaBtn.AddSubview(ctaLbl);
                ctaContainer.AddSubview(ctaBtn);
                ctaItemX += ctaItemWidth + ScaleUtility.GetScaledWidth(1F);
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
                txtViewDetails.Delegate = new TextViewDelegate(action)
                {
                    InteractWithURL = false //Created by Syahmi ICS 05052020

                };
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
                for (int i = 0; i < RedirectTypeList.Count; i++)
                {
                    if (absURL.Contains(RedirectTypeList[i]))
                    {
                        whileCount = i;
                        isContained = true;
                        break;
                    }
                }

                if (isContained)
                {
                    if (RedirectTypeList[whileCount] == RedirectTypeList[0])
                    {
                        string urlString = absURL.Split(RedirectTypeList[0])[1];
                        UIViewController baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                        UIViewController topVc = AppDelegate.GetTopViewController(baseRootVc);
                        if (topVc != null)
                        {
                            BrowserViewController viewController = new BrowserViewController();
                            if (viewController != null)
                            {
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
                    else if (RedirectTypeList[whileCount] == RedirectTypeList[1])
                    {
                        string urlString = absURL.Split(RedirectTypeList[1])[1];
                        UIApplication.SharedApplication.OpenUrl(new NSUrl(string.Format(urlString)));
                    }
                    else if (RedirectTypeList[whileCount] == RedirectTypeList[2])
                    {
                        string urlString = absURL.Split(RedirectTypeList[2])[1];
                        if (!urlString.Contains("tel:"))
                        {
                            urlString = "tel:" + urlString;
                        }
                        UIApplication.SharedApplication.OpenUrl(new NSUrl(new Uri(urlString).AbsoluteUri));
                    }
                    else if (RedirectTypeList[whileCount] == RedirectTypeList[3])
                    {
                        string key = absURL.Split(RedirectTypeList[3])[1];
                        key = key.Replace("%7B", "{").Replace("%7D", "}");
                        int index = key.IndexOf("}");
                        if (index > -1 && index < key.Length - 1)
                        {
                            key = key.Remove(index + 1);
                        }
                        key = key.Replace("{", "").Replace("}", "");
                        UIViewController baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                        UIViewController topVc = AppDelegate.GetTopViewController(baseRootVc);
                        if (topVc != null)
                        {
                            WhatsNewServices.OpenWhatsNewDetails(key, topVc);
                        }
                    }
                    else if (RedirectTypeList[whileCount] == RedirectTypeList[4])
                    {
                        string key = absURL.Split(RedirectTypeList[4])[1];
                        key = key.Replace("%7B", "{").Replace("%7D", "}");
                        int index = key.IndexOf("}");
                        if (index > -1 && index < key.Length - 1)
                        {
                            key = key.Remove(index + 1);
                        }
                        if (!key.Contains("{"))
                        {
                            key = "{" + key;
                        }
                        if (!key.Contains("}"))
                        {
                            key = key + "}";
                        }
                        ViewHelper.GoToFAQScreenWithId(key);
                        viewParent.RemoveFromSuperview();
                    }
                    else if (RedirectTypeList[whileCount] == RedirectTypeList[5])
                    {
                        string key = absURL.Split(RedirectTypeList[5])[1];
                        key = key.Replace("%7B", "{").Replace("%7D", "}");
                        int index = key.IndexOf("}");
                        if (index > -1 && index < key.Length - 1)
                        {
                            key = key.Remove(index + 1);
                        }
                        key = key.Replace("{", "").Replace("}", "");
                        UIViewController baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                        UIViewController topVc = AppDelegate.GetTopViewController(baseRootVc);
                        if (topVc != null)
                        {
                            if (!(topVc is RewardDetailsViewController) && !(topVc is AppLaunchViewController))
                            {
                                RewardsServices.OpenRewardDetails(key, topVc);
                            }
                        }
                    }
                    else if (RedirectTypeList[whileCount] == RedirectTypeList[6]) //Created by Syahmi ICS 05052020
                    {
                        string urlString = absURL;
                        UIViewController baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                        UIViewController topVc = AppDelegate.GetTopViewController(baseRootVc);
                        if (topVc != null)
                        {
                            BrowserViewController viewController = new BrowserViewController();
                            if (viewController != null)
                            {
                                viewController.NavigationTitle = "";
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
                    else if (RedirectTypeList[whileCount] == RedirectTypeList[7])
                    {
                        string urlString = absURL;
                        if (!urlString.Contains("tel:"))
                        {
                            urlString = "tel:" + urlString;
                        }
                        UIApplication.SharedApplication.OpenUrl(new NSUrl(new Uri(urlString).AbsoluteUri));
                    }
                    else if (RedirectTypeList[whileCount] == RedirectTypeList[8])
                    {
                        string key = absURL.Split(RedirectTypeList[8])[1];
                        key = key.Replace("%7B", "{").Replace("%7D", "}");
                        int index = key.IndexOf("}");
                        if (index > -1 && index < key.Length - 1)
                        {
                            key = key.Remove(index + 1);
                        }
                        key = key.Replace("{", "").Replace("}", "");
                        UIViewController baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                        UIViewController topVc = AppDelegate.GetTopViewController(baseRootVc);
                        if (topVc != null)
                        {
                            WhatsNewServices.OpenWhatsNewDetails(key, topVc);
                        }
                    }
                    else if (RedirectTypeList[whileCount] == RedirectTypeList[9])
                    {
                        string key = absURL.Split(RedirectTypeList[9])[1];
                        key = key.Replace("%7B", "{").Replace("%7D", "}");
                        int index = key.IndexOf("}");
                        if (index > -1 && index < key.Length - 1)
                        {
                            key = key.Remove(index + 1);
                        }
                        if (!key.Contains("{"))
                        {
                            key = "{" + key;
                        }
                        if (!key.Contains("}"))
                        {
                            key = key + "}";
                        }
                        ViewHelper.GoToFAQScreenWithId(key);
                        viewParent.RemoveFromSuperview();
                    }
                    else if (RedirectTypeList[whileCount] == RedirectTypeList[10])
                    {
                        string key = absURL.Split(RedirectTypeList[10])[1];
                        key = key.Replace("%7B", "{").Replace("%7D", "}");
                        int index = key.IndexOf("}");
                        if (index > -1 && index < key.Length - 1)
                        {
                            key = key.Remove(index + 1);
                        }
                        key = key.Replace("{", "").Replace("}", "");
                        UIViewController baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                        UIViewController topVc = AppDelegate.GetTopViewController(baseRootVc);
                        if (topVc != null)
                        {
                            if (!(topVc is RewardDetailsViewController) && !(topVc is AppLaunchViewController))
                            {
                                RewardsServices.OpenRewardDetails(key, topVc);
                            }
                        }
                    }
                }
            }
        }
    }
}