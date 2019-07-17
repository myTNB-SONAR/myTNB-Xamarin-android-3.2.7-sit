using System;
using System.Collections.Generic;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public class AccountsPageViewDataSource : UIPageViewControllerDataSource
    {
        private DashboardHomeViewController _parentViewController;
        private List<List<CustomerAccountRecordModel>> _groupedAccountsList;

        public AccountsPageViewDataSource(UIViewController parentViewController, List<List<CustomerAccountRecordModel>> groupedAccountsList)
        {
            _parentViewController = parentViewController as DashboardHomeViewController;
            _groupedAccountsList = groupedAccountsList;
        }

        public override UIViewController GetPreviousViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
        {
            var vc = referenceViewController as AccountsContentViewController;
            var index = vc.pageIndex;
            if (index == 0)
            {
                return null;
            }
            else
            {
                index--;
                return _parentViewController.ViewControllerAtIndex(index);
            }
        }

        public override UIViewController GetNextViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
        {
            var vc = referenceViewController as AccountsContentViewController;
            var index = vc.pageIndex;

            index++;
            if (index == _groupedAccountsList.Count)
            {
                return null;
            }
            else
            {
                return _parentViewController.ViewControllerAtIndex(index);
            }
        }

        public override nint GetPresentationCount(UIPageViewController pageViewController)
        {
            return _groupedAccountsList.Count;
        }

        public override nint GetPresentationIndex(UIPageViewController pageViewController)
        {
            return 0;
        }

    }
}
