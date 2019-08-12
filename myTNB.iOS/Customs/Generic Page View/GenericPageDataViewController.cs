using System;
using myTNB.Model;
using myTNB.SSMR;
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
        public SSMROnboardingModel SSMRDataObject
        {
            get; set;
        }
        private BaseDataViewController _dataController;
        public GenericPageViewEnum.Type PageType;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            if (PageType == GenericPageViewEnum.Type.Onboarding)
            {
                _dataController = new OnboardingDataController(this)
                {
                    DataObject = DataObject
                };
            }
            if (PageType == GenericPageViewEnum.Type.SSMR)
            {
                _dataController = new SSMROnboardingDataController(this)
                {
                    SSMRDataObject = SSMRDataObject
                };
            }
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