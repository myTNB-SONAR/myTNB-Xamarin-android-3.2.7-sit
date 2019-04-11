
using UIKit;

namespace myTNB
{
    public static class AlertHandler
    {
        /// <summary>
        /// Displays the no data alert.
        /// </summary>
        /// <param name="controller">Controller.</param>
        public static void DisplayNoDataAlert(UIViewController controller)
        {
            DisplayAlert(controller, "Error_NoNetworkTitle".Translate(), "Error_NoNetworkMsg".Translate());
        }

        /// <summary>
        /// Displays the service error.
        /// </summary>
        /// <param name="controller">Controller.</param>
        /// <param name="message">Message.</param>
        public static void DisplayServiceError(UIViewController controller, string message)
        {
            string title = "Error_DefaultTitle".Translate();
            DisplayAlert(controller, title, message);
        }

        /// <summary>
        /// Displays the generic error.
        /// </summary>
        /// <param name="controller">Controller.</param>
        /// <param name="title">Title.</param>
        /// <param name="message">Message.</param>
        public static void DisplayGenericAlert(UIViewController controller, string title, string message)
        {
            DisplayAlert(controller, title, message);
        }

        /// <summary>
        /// Displays the alert.
        /// </summary>
        /// <param name="controller">Controller.</param>
        /// <param name="title">Title.</param>
        /// <param name="message">Message.</param>
        private static void DisplayAlert(UIViewController controller, string title, string message)
        {
            if (string.IsNullOrWhiteSpace(message) || string.IsNullOrEmpty(message))
            {
                message = "Error_DefaultMessage".Translate();
            }
            var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create("Common_Ok".Translate(), UIAlertActionStyle.Cancel, null));
            controller.PresentViewController(alert, animated: true, completionHandler: null);
        }
    }
}