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
        SSMRMeterCardComponent _sSMRMeterCardComponent2;
        SSMRMeterCardComponent _sSMRMeterCardComponent3;
        SSMRMeterFooterComponent _sSMRMeterFooterComponent;

        UIScrollView _meterReadScrollView;

        UILabel _descriptionLabel;
        nfloat _padding = 16f;
        CGRect scrollViewFrame;

        public override void ViewDidLoad()
        {
            PageName = "SSMRSubmitMeterReading";
            base.ViewDidLoad();
            SetNavigation();
            AddFooterView();
            Initialization();
            PrepareMeterReadingCard();

            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
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
            _meterReadScrollView = new UIScrollView(new CGRect(0, 0, ViewWidth, ViewHeight - _sSMRMeterFooterComponent.GetView().Frame.Height))
            {
                BackgroundColor = UIColor.Clear
            };
            View.AddSubview(_meterReadScrollView);

            _descriptionLabel = new UILabel(new CGRect(_padding, _padding, _meterReadScrollView.Frame.Width - (_padding * 2), 48f))
            {
                BackgroundColor = UIColor.Clear,
                Font = MyTNBFont.MuseoSans16_500,
                TextColor = MyTNBColor.WaterBlue,
                Lines = 0,
                TextAlignment = UITextAlignment.Left,
                Text = "Please enter your meter reading for each respective units."
            };
            _meterReadScrollView.AddSubview(_descriptionLabel);
            scrollViewFrame = _meterReadScrollView.Frame;
        }

        private void PrepareMeterReadingCard()
        {
            _sSMRMeterCardComponent = new SSMRMeterCardComponent(_meterReadScrollView, _descriptionLabel.Frame.GetMaxY() + _padding);
            _meterReadScrollView.AddSubview(_sSMRMeterCardComponent.GetUI());
            _sSMRMeterCardComponent.SetPreviousReading("1324176.7");

            _sSMRMeterCardComponent2 = new SSMRMeterCardComponent(_meterReadScrollView, _sSMRMeterCardComponent.GetView().Frame.GetMaxY() + _padding);
            _meterReadScrollView.AddSubview(_sSMRMeterCardComponent2.GetUI());
            _sSMRMeterCardComponent2.SetPreviousReading("34613478.0");

            _sSMRMeterCardComponent3 = new SSMRMeterCardComponent(_meterReadScrollView, _sSMRMeterCardComponent2.GetView().Frame.GetMaxY() + _padding);
            _meterReadScrollView.AddSubview(_sSMRMeterCardComponent3.GetUI());
            _sSMRMeterCardComponent3.SetPreviousReading("2364798");

            _meterReadScrollView.ContentSize = new CGSize(ViewWidth, _sSMRMeterCardComponent3.GetView().Frame.GetMaxY() + _padding);
            scrollViewFrame = _meterReadScrollView.Frame;
        }

        private void AddFooterView()
        {
            _sSMRMeterFooterComponent = new SSMRMeterFooterComponent(View, ViewHeight);
            View.AddSubview(_sSMRMeterFooterComponent.GetUI());
        }

        void OnKeyboardNotification(NSNotification notification)
        {
            Debug.WriteLine("OnKeyboardNotification");
            if (!IsViewLoaded)
                return;

            bool visible = notification.Name == UIKeyboard.WillShowNotification;
            UIView.BeginAnimations("AnimateForKeyboard");
            UIView.SetAnimationBeginsFromCurrentState(true);
            UIView.SetAnimationDuration(UIKeyboard.AnimationDurationFromNotification(notification));
            UIView.SetAnimationCurve((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification(notification));

            if (visible)
            {
                CGRect r = UIKeyboard.BoundsFromNotification(notification);
                CGRect viewFrame = View.Bounds;
                nfloat currentViewHeight = viewFrame.Height - r.Height;
                _meterReadScrollView.Frame = new CGRect(_meterReadScrollView.Frame.X, _meterReadScrollView.Frame.Y, _meterReadScrollView.Frame.Width, currentViewHeight);
            }
            else
            {
                _meterReadScrollView.Frame = scrollViewFrame;
            }

            UIView.CommitAnimations();
        }
    }
}
