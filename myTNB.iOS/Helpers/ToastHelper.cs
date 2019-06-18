using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB
{
    public static class ToastHelper
    {
        /// <summary>
        /// Shows the toast.
        /// </summary>
        /// <param name="viewDialog">View dialog.</param>
        /// <param name="isAnimating">If set to <c>true</c> is animating.</param>
        public static void ShowToast(UIView viewDialog, ref bool isAnimating)
        {
            if (!isAnimating)
            {
                isAnimating = true;
            }
            else
            {
                isAnimating = false;
                viewDialog.Hidden = true;
            }
            viewDialog.Layer.ZPosition = TNBGlobal.ToastZPosition;
            viewDialog.Hidden = false;
            viewDialog.Alpha = 1.0f;
            UIView.Animate(10, 1, UIViewAnimationOptions.CurveEaseOut, () =>
            {
                viewDialog.Alpha = 0.0f;
            }, () =>
            {
                viewDialog.Hidden = true;
                viewDialog.Hidden = false;
            });
        }

        /// <summary>
        /// Shows the toast.
        /// </summary>
        /// <param name="viewDialog">View dialog.</param>
        /// <param name="isAnimating">If set to <c>true</c> is animating.</param>
        /// <param name="message">Message.</param>
        public static void ShowToast(UIView viewDialog, ref bool isAnimating, string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                var lblMessage = viewDialog.ViewWithTag(TNBGlobal.Tags.ToastMessageLabel) as UILabel;
                if (lblMessage != null)
                {
                    lblMessage.Text = message;
                    lblMessage.Font = MyTNBFont.MuseoSans12_300;
                    lblMessage.TextColor = MyTNBColor.TunaGrey();
                }

            }
            ShowToast(viewDialog, ref isAnimating);
        }

        /// <summary>
        /// Displays the alert view.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="title">Title.</param>
        /// <param name="message">Message.</param>
        /// <param name="handler">Handler.</param>
        public static void DisplayAlertView(UIViewController view, string title, string message
            , Action<UIAlertAction> handler = null, string actionTitle = null)
        {
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create(string.IsNullOrEmpty(actionTitle) ? "Common_Ok".Translate() : actionTitle
                , UIAlertActionStyle.Cancel, handler));
            view.PresentViewController(alert, animated: true, completionHandler: null);
        }

        public static void DisplayCustomAlert(string title, string message
            , Action handler = null, string actionTitle = null)
        {
            DisplayCustomAlert(title, message, new Dictionary<string, Action>() {
                { string.IsNullOrEmpty(actionTitle) ? "Common_Ok".Translate() : actionTitle, handler }
            });
        }

        public static void DisplayCustomAlert(string title, string message, Dictionary<string, Action> ctaButtons)
        {
            nfloat margin = UIScreen.MainScreen.Bounds.Width * 0.056F;
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

            //Title
            UILabel lblTitle = new UILabel(new CGRect(16, 0, width - 32, 0))
            {
                Font = MyTNBFont.MuseoSans14_500,
                TextColor = MyTNBColor.PayneGray,
                TextAlignment = UITextAlignment.Left,
                Text = title
            };

            nfloat txtViewY = 10;
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrWhiteSpace(title))
            {
                lblTitle.Frame = new CGRect(16, 16, width - 32, 20);
                txtViewY = 46;
                maxDescriptionHeight -= 36;
            }

            // Body
            UITextView txtViewDetails = new UITextView
            {
                Editable = false,
                ScrollEnabled = true,
                TextAlignment = UITextAlignment.Justified,
                AttributedText = mutableHTMLBody,
                WeakLinkTextAttributes = linkAttributes.Dictionary
            };
            txtViewDetails.ScrollIndicatorInsets = UIEdgeInsets.Zero;

            //Resize
            CGSize size = txtViewDetails.SizeThatFits(new CGSize(width - 32, maxDescriptionHeight));
            nfloat txtViewHeight = size.Height > maxDescriptionHeight ? maxDescriptionHeight : size.Height;
            txtViewDetails.Frame = new CGRect(16, txtViewY, width - 32, txtViewHeight);

            UIView viewline = new UIView(new CGRect(0, txtViewDetails.Frame.Height + 32, width, 1))
            {
                BackgroundColor = MyTNBColor.LightGrayBG
            };
            nfloat containerY = txtViewDetails.Frame.Y + txtViewDetails.Frame.Height + 10;

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
                    else
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

            nfloat viewHeight = ctaContainer.Frame.Y + ctaContainer.Frame.Height;
            UIView alertView = new UIView(new CGRect(margin, 0, width, viewHeight))
            {
                BackgroundColor = UIColor.White,
                ClipsToBounds = true
            };
            alertView.Layer.CornerRadius = 4.0F;
            alertView.Center = new CGPoint(UIScreen.MainScreen.Bounds.GetMidX(), UIScreen.MainScreen.Bounds.GetMidY());
            alertView.AddSubviews(new UIView[] { lblTitle, txtViewDetails, ctaContainer });

            viewParent.AddSubview(alertView);
            Action<NSUrl> action = new Action<NSUrl>((url) =>
            {
                string absURL = url?.AbsoluteString;
                string key = !string.IsNullOrEmpty(absURL) && absURL.Contains("faqid=") ? absURL?.Split("faqid=")[1] : string.Empty;
                key = key.Replace("%7B", "{").Replace("%7D", "}");
                ViewHelper.GoToFAQScreenWithId(key);
                viewParent.RemoveFromSuperview();
            });
            txtViewDetails.Delegate = new TextViewDelegate(action);
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
            currentWindow.AddSubview(viewParent);
        }
    }
}