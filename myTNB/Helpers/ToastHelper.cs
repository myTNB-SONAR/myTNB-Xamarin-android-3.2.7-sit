using System;
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
            viewDialog.Hidden = false;
            viewDialog.Alpha = 1.0f;
            UIView.Animate(5, 1, UIViewAnimationOptions.CurveEaseOut, () => {
                viewDialog.Alpha = 0.0f;
            }, () => {
                viewDialog.Hidden = true;
                viewDialog.Hidden = false;
            });
        }
    }
}
