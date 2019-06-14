﻿using System;
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
                    lblMessage.Font = myTNBFont.MuseoSans12_300();
                    lblMessage.TextColor = myTNBColor.TunaGrey();
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
            , Action<UIAlertAction> handler = null, string actionTitle = "Ok", bool isCustom = false)
        {
            if (isCustom)
            {
                DisplayCustomAlert(view, title, message, handler, actionTitle);
            }
            else
            {
                var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create(actionTitle, UIAlertActionStyle.Cancel, handler));
                view.PresentViewController(alert, animated: true, completionHandler: null);
            }
        }

        private static void DisplayCustomAlert(UIViewController controller, string title
            , string message, Action<UIAlertAction> handler = null, string actionTitle = "Ok")
        {
            nfloat width = controller.View.Frame.Width - 36;
            nfloat height = controller.View.Frame.Height - 36;
            nfloat maxDescriptionHeight = height - 50 - 32;

            UIStringAttributes linkAttributes = new UIStringAttributes
            {
                ForegroundColor = myTNBColor.PowerBlue(),
                Font = myTNBFont.MuseoSans14_300(),
                UnderlineStyle = NSUnderlineStyle.None,
                UnderlineColor = UIColor.Clear
            };
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(message
                , ref htmlBodyError, myTNBFont.FONTNAME_300, 14f);

            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = myTNBColor.TunaGrey()
            }, new NSRange(0, htmlBody.Length));
            UITextView txtViewDetails = new UITextView
            {
                Editable = false,
                ScrollEnabled = false,
                TextAlignment = UITextAlignment.Justified,
                AttributedText = mutableHTMLBody,
                WeakLinkTextAttributes = linkAttributes.Dictionary
            };

            CGSize size = txtViewDetails.SizeThatFits(new CGSize(width - 32, maxDescriptionHeight));
            txtViewDetails.Frame = new CGRect(16, 16, width - 32, size.Height);

            txtViewDetails.Frame = new CGRect(txtViewDetails.Frame.X
                , txtViewDetails.Frame.Y, txtViewDetails.Frame.Width, size.Height);
            UIView viewline = new UIView(new CGRect(0, txtViewDetails.Frame.Height + 32, width, 1))
            {
                BackgroundColor = myTNBColor.LightGrayBG()
            };
            UIView viewBtn = new UIView(new CGRect(0, viewline.Frame.Y + 1, width, 50));

            UILabel lblAction = new UILabel(new CGRect(0, 0, viewBtn.Frame.Width, viewBtn.Frame.Height))
            {
                Text = actionTitle,
                Font = myTNBFont.MuseoSans16_500(),
                TextAlignment = UITextAlignment.Center,
                TextColor = myTNBColor.PowerBlue()
            };
            viewBtn.AddSubview(lblAction);
            nfloat viewHeight = viewBtn.Frame.Y + viewBtn.Frame.Height;
            UIView alertView = new UIView(new CGRect(18, 0, width, viewHeight))
            {
                BackgroundColor = UIColor.White
            };
            alertView.Layer.CornerRadius = 4.0F;
            alertView.Center = controller.View.Center;
            alertView.AddSubviews(new UIView[] { txtViewDetails, viewline, viewBtn });
            UIView viewParent = new UIView(controller.View.Frame)
            {
                BackgroundColor = new UIColor(73 / 255, 73 / 255, 74 / 255, 0.5F)
            };
            UIView viewTabbar = new UIView(new CGRect(0, 0
                , controller.TabBarController.TabBar.Frame.Width, controller.TabBarController.TabBar.Frame.Height))
            {
                BackgroundColor = new UIColor(73 / 255, 73 / 255, 74 / 255, 0.5F)
            };
            viewParent.AddSubview(alertView);
            viewBtn.AddGestureRecognizer(new UITapGestureRecognizer((obj) =>
            {
                viewParent.RemoveFromSuperview();
                viewTabbar.RemoveFromSuperview();
            }));
            Action<NSUrl> action = new Action<NSUrl>((url) =>
            {
                string scheme = url.Scheme + "://";
                string urlWithoutScheme = url?.AbsoluteString?.Replace(scheme, string.Empty);
                string key = !string.IsNullOrEmpty(urlWithoutScheme) && urlWithoutScheme.Contains("/") ? urlWithoutScheme?.Split("/")[1] : string.Empty;
                key = key.Replace("%7B", "{").Replace("%7D", "}");
                ViewHelper.GoToFAQScreenWithId(key);
                viewParent.RemoveFromSuperview();
                viewTabbar.RemoveFromSuperview();
            });
            txtViewDetails.Delegate = new TextViewDelegate(action);
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
            currentWindow.AddSubview(viewParent);
            controller?.TabBarController?.TabBar?.AddSubview(viewTabbar);
        }
    }
}