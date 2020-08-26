using System;
using UIKit;

namespace myTNB.Feedback.Enquiry.EnquiryStatusPage
{
    public class EnquiryStatusPageActions
    {

        readonly UIViewController _controller;
        public UIViewController _nextViewController;
        public EnquiryStatusPageActions(UIViewController controller, UIViewController nextViewController = null)
        {
            _controller = controller;
            _nextViewController = nextViewController;
        }

        internal void BackToHome()
        {
            ViewHelper.DismissControllersAndSelectTab(_controller, 0, true);
        }

        internal void TrackApplication()
        {

        }

        internal void SSMRTryAgain()
        {
            _controller.NavigationController.PopViewController(true);
        }

        internal void SSMRReadingTryAgain()
        {
            _controller.NavigationController.PopViewController(true);
        }

        internal void BackToUsage()
        {
            ViewHelper.DismissControllersAndSelectTab(_controller, 0, true);
        }

        internal void ViewReadingHistory()
        {
            _controller.NavigationController.PushViewController(_nextViewController, true);
        }

        internal void BackToFeedback()
        {
            _controller.DismissViewController(true, null);
        }
    }
}
