using System;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public class BaseDataViewController
    {
        public OnboardingModel DataObject
        {
            get; set;
        }
        public UIPageViewController that;
        public BaseDataViewController(UIPageViewController controller)
        {
            that = controller;
        }
        public virtual void OnViewDidLoad() { }

        public virtual void OnViewDidLayoutSubViews() { }

        public virtual void SetSubViews() { }

        public virtual void SetBackground() { }
    }
}
