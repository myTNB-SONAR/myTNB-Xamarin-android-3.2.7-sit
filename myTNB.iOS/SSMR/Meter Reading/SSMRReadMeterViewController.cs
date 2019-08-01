using CoreGraphics;
using Foundation;
using myTNB.SSMR;
using System;
using System.Diagnostics;
using UIKit;

namespace myTNB
{
    public partial class SSMRReadMeterViewController : CustomUIViewController
    {
        public SSMRReadMeterViewController(IntPtr handle) : base(handle) { }

        SSMRMeterCardComponent _sSMRMeterCardComponent;
        SSMRMeterFooterComponent _sSMRMeterFooterComponent;

        UILabel _descriptionLabel;
        nfloat _padding = 16f;

        public override void ViewDidLoad()
        {
            PageName = "SSMRSubmitMeterReading";
            base.ViewDidLoad();
            SetNavigation();
            Initialization();
            PrepareMeterReadingCard();
            AddFooterView();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        private void SetNavigation()
        {
            UIImage backImg = UIImage.FromBundle(SSMRConstants.IMG_BackIcon);
            UIImage btnRightImg = UIImage.FromBundle(SSMRConstants.IMG_PrimaryIcon);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            UIBarButtonItem btnRight = new UIBarButtonItem(btnRightImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                Debug.WriteLine("btnRight tapped");
            });
            NavigationItem.LeftBarButtonItem = btnBack;
            NavigationItem.RightBarButtonItem = btnRight;
            Title = GetI18NValue(SSMRConstants.I18N_NavTitle);
        }

        private void Initialization()
        {
            _descriptionLabel = new UILabel(new CGRect(_padding, _padding, ViewWidth - (_padding * 2), 48f))
            {
                BackgroundColor = UIColor.Clear,
                Font = MyTNBFont.MuseoSans16_500,
                TextColor = MyTNBColor.WaterBlue,
                Lines = 0,
                TextAlignment = UITextAlignment.Left,
                Text = "Please enter your meter reading for each respective units."
            };
            View.AddSubview(_descriptionLabel);
        }

        private void PrepareMeterReadingCard()
        {
            _sSMRMeterCardComponent = new SSMRMeterCardComponent(View, _descriptionLabel.Frame.GetMaxY() + _padding);
            View.AddSubview(_sSMRMeterCardComponent.GetUI());
        }

        private void AddFooterView()
        {
            _sSMRMeterFooterComponent = new SSMRMeterFooterComponent(View, ViewHeight);
            View.AddSubview(_sSMRMeterFooterComponent.GetUI());
        }
    }
}
