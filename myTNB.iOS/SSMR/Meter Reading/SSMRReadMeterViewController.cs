using CoreGraphics;
using Foundation;
using myTNB.Model;
using myTNB.SSMR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UIKit;

namespace myTNB
{
    public partial class SSMRReadMeterViewController : CustomUIViewController
    {
        public SSMRReadMeterViewController(IntPtr handle) : base(handle) { }

        SSMRMeterFooterComponent _sSMRMeterFooterComponent;

        List<SMRMROValidateRegisterDetailsInfoModel> _previousMeterList;

        UIScrollView _meterReadScrollView;
        UILabel _descriptionLabel;
        nfloat _padding = 16f;
        CGRect scrollViewFrame;

        public override void ViewDidLoad()
        {
            PageName = "SSMRSubmitMeterReading";
            base.ViewDidLoad();

            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);

            _previousMeterList = DataManager.DataManager.SharedInstance.SSMRPreviousMeterReadingList;

            SetNavigation();
            AddFooterView();
            Initialization();
            PrepareMeterReadingCard();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        private void SetNavigation()
        {
            UIImage backImg = UIImage.FromBundle(SSMRConstants.IMG_BackIcon);
            UIImage btnRightImg = UIImage.FromBundle(SSMRConstants.IMG_Info);
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
            if (_previousMeterList != null)
            {
                nfloat yPos = _descriptionLabel.Frame.GetMaxY() + _padding;
                foreach (var previousMeter in _previousMeterList)
                {
                    SSMRMeterCardComponent sSMRMeterCardComponent = new SSMRMeterCardComponent(this, _meterReadScrollView, yPos);
                    _meterReadScrollView.AddSubview(sSMRMeterCardComponent.GetUI());
                    sSMRMeterCardComponent.SetModel(previousMeter);
                    sSMRMeterCardComponent.SetPreviousReading(previousMeter.PrevMeterReading);
                    sSMRMeterCardComponent.SetIconText(previousMeter);

                    yPos = sSMRMeterCardComponent.GetView().Frame.GetMaxY() + _padding;
                    _meterReadScrollView.ContentSize = new CGSize(ViewWidth, yPos);
                    scrollViewFrame = _meterReadScrollView.Frame;
                }
            }
        }

        public void SetCurrentReadingValue(SMRMROValidateRegisterDetailsInfoModel model, string currentReading)
        {
            if (_previousMeterList != null)
            {
                foreach (var previousMeter in _previousMeterList)
                {
                    if (previousMeter.RegisterNumber == model.RegisterNumber)
                    {
                        previousMeter.CurrentReading = currentReading;
                        break;
                    }
                }
            }
        }

        public void SetIsValidManualReadingFlags(SMRMROValidateRegisterDetailsInfoModel model, bool isError)
        {
            if (_previousMeterList != null)
            {
                foreach (var previousMeter in _previousMeterList)
                {
                    if (previousMeter.RegisterNumber == model.RegisterNumber)
                    {
                        previousMeter.IsValidManualReading = !isError;
                        break;
                    }
                }
            }
            UpdateButtonsState();
        }

        private void UpdateButtonsState()
        {
            var res = true;
            if (_previousMeterList != null)
            {
                foreach (var previousMeter in _previousMeterList)
                {
                    if (!previousMeter.IsValidManualReading)
                    {
                        res = false;
                        break;
                    }
                }
            }
            _sSMRMeterFooterComponent.SetSubmitButtonEnabled(res);
            _sSMRMeterFooterComponent.SetTakePhotoButtonEnabled(!res);
        }

        private void AddFooterView()
        {
            _sSMRMeterFooterComponent = new SSMRMeterFooterComponent(View, ViewHeight);
            View.AddSubview(_sSMRMeterFooterComponent.GetUI());
            _sSMRMeterFooterComponent._takePhotoBtn.TouchUpInside += (sender, e) =>
            {
                OnTapTakePhoto();
            };
            _sSMRMeterFooterComponent._submitBtn.TouchUpInside += (sender, e) =>
            {
                OnTapSubmitReading();
            };
        }

        private void OnTapTakePhoto()
        {
            Debug.WriteLine("OnTapTakePhoto");
            Dictionary<string, bool> ReadingDictionary = new Dictionary<string, bool>();
            if (_previousMeterList != null)
            {
                foreach (var previousMeter in _previousMeterList)
                {
                    Debug.WriteLine("previousMeter.RegisterNumber== " + previousMeter.RegisterNumber);
                    Debug.WriteLine("previousMeter.IsValidManualReading== " + previousMeter.IsValidManualReading);
                    Debug.WriteLine("previousMeter.PrevMeterReading== " + previousMeter.PrevMeterReading);
                    Debug.WriteLine("previousMeter.CurrentReading== " + previousMeter.CurrentReading);
                    Debug.WriteLine("====================================== ");
                    string registerStr = string.Empty;
                    switch (previousMeter.RegisterNumberType)
                    {
                        case RegisterNumberEnum.kWh:
                            registerStr = "kWh";
                            break;
                        case RegisterNumberEnum.kVARh:
                            registerStr = "kVARh";
                            break;
                        case RegisterNumberEnum.kW:
                            registerStr = "kW";
                            break;
                    }
                    ReadingDictionary.Add(registerStr, previousMeter.IsValidManualReading);
                }
                UIStoryboard storyBoard = UIStoryboard.FromName("SSMR", null);
                SSMRCaptureMeterViewController viewController =
                    storyBoard.InstantiateViewController("SSMRCaptureMeterViewController") as SSMRCaptureMeterViewController;
                viewController.ReadingDictionary = ReadingDictionary;
                var navController = new UINavigationController(viewController);
                PresentViewController(navController, true, null);
            }
        }

        private void OnTapSubmitReading()
        {
            Debug.WriteLine("OnTapSubmitReading");
            if (_previousMeterList != null)
            {
                foreach (var previousMeter in _previousMeterList)
                {
                    Debug.WriteLine("previousMeter.RegisterNumber== " + previousMeter.RegisterNumber);
                    Debug.WriteLine("previousMeter.IsValidManualReading== " + previousMeter.IsValidManualReading);
                    Debug.WriteLine("previousMeter.PrevMeterReading== " + previousMeter.PrevMeterReading);
                    Debug.WriteLine("previousMeter.CurrentReading== " + previousMeter.CurrentReading);
                    Debug.WriteLine("====================================== ");
                }
            }
        }

        void OnKeyboardNotification(NSNotification notification)
        {
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
