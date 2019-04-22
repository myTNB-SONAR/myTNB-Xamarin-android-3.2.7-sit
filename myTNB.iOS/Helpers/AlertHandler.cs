using System;
using UIKit;

namespace myTNB
{
    public static class AlertHandler
    {
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
            controller.PresentViewController(alert, animated: true, completionHandler: null);
        }
    }
}