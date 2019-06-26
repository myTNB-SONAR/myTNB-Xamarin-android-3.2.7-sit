using System;
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
    }
}