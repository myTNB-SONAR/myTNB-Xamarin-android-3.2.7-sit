using System;
using myTNB.Model;
using myTNB.SSMR;
using UIKit;

namespace myTNB
{
    public class BaseDataViewController
    {
        public OnboardingModel DataObject
        {
            get; set;
        }

        public SSMROnboardingModel SSMRDataObject
        {
            set; get;
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
