﻿using System;
using UIKit;

namespace myTNB.Customs.GenericStatusPage
{
    public class StatusPageActions
    {
        readonly UIViewController _controller;
        public StatusPageActions(UIViewController controller)
        {
            _controller = controller;
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
            ViewHelper.DismissControllersAndSelectTab(_controller, 0, true);
        }

        internal void BackToUsage()
        {
            ViewHelper.DismissControllersAndSelectTab(_controller, 0, true);
        }

        internal void ViewReadingHistory()
        {

        }

        internal void BackToFeedback()
        {
            _controller.DismissViewController(true, null);
        }
    }
}