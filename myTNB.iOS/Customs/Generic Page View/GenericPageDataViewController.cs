using System;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public partial class GenericPageDataViewController : UIPageViewController
    {
        protected GenericPageDataViewController(IntPtr handle) : base(handle) { }
        public OnboardingModel DataObject
        {
            get; set;
        }
        BaseDataViewController _dataController;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib
            _dataController = new OnboardingDataController(this)
            {
                DataObject = DataObject
            };
            _dataController.OnViewDidLoad();
            _dataController.SetSubViews();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            SetupSuperViewBackground();
        }

        internal void SetupSuperViewBackground()
        {
            _dataController.SetBackground();
        }
    }
}