using myTNB.Dashboard;
using System;
using UIKit;

namespace myTNB
{
    public partial class DashboardNavigationController : UINavigationController
    {
        public DashboardNavigationController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (DataManager.DataManager.SharedInstance.GetAccountsCount() > 1)
            {
                ShowDashboardHome();
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (DataManager.DataManager.SharedInstance.GetAccountsCount() > 1)
            {
                bool isHomeInNavigationStack = false;

                foreach (var vc in ViewControllers)
                {
                    if (vc is DashboardHomeViewController)
                    {
                        isHomeInNavigationStack = true;
                        break;
                    }
                }

                if (!isHomeInNavigationStack)
                {
                    ShowDashboardHome();
                }
            }
            else
            {
                ShowDashboard();
            }

        }

        /// <summary>
        /// Shows the dashboard home.
        /// </summary>
        private void ShowDashboardHome()
        {
            var storyBoard = UIStoryboard.FromName("Dashboard", null);
            var vc = storyBoard.InstantiateViewController("DashboardHomeViewController") as DashboardHomeViewController;
            SetNavigationBarHidden(true, false);
            SetViewControllers(new UIViewController[] { vc }, false);
        }

        /// <summary>
        /// Shows the dashboard.
        /// </summary>
        private void ShowDashboard()
        {
            var storyBoard = UIStoryboard.FromName("Dashboard", null);
            var vc = storyBoard.InstantiateViewController("DashboardViewController") as DashboardViewController;
            SetNavigationBarHidden(true, false);
            SetViewControllers(new UIViewController[] { vc }, false);
        }
    }
}