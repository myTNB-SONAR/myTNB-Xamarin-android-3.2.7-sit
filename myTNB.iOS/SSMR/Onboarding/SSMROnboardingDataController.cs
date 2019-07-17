using System;
using UIKit;

namespace myTNB
{
    public class SSMROnboardingDataController : BaseDataViewController
    {
        public SSMROnboardingDataController(UIPageViewController controller) : base(controller) { }

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
        }

        public override void SetBackground()
        {
            that.View.BackgroundColor = UIColor.Clear;
        }

        public override void OnViewDidLayoutSubViews()
        {
            SetBackground();
        }
    }
}