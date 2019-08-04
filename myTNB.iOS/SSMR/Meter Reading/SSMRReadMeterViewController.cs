﻿using CoreGraphics;
using Foundation;
using myTNB.Model;
using myTNB.SSMR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using UIKit;

namespace myTNB
{
    public partial class SSMRReadMeterViewController : CustomUIViewController
    {
        public SSMRReadMeterViewController(IntPtr handle) : base(handle) { }

        SSMRMeterFooterComponent _sSMRMeterFooterComponent;
        SMRSubmitMeterReadingResponseModel _submitMeterResponse = new SMRSubmitMeterReadingResponseModel();
        List<SMRMROValidateRegisterDetailsInfoModel> _previousMeterList;

        UIView _toolTipParentView, _toolTipContainerView, _toolTipFooterView;
        UIScrollView _toolTipScrollView;
        UIPageControl _pageControl;
        UIScrollView _meterReadScrollView;
        UILabel _descriptionLabel;
        nfloat _padding = 16f;
        CGRect scrollViewFrame;
        int _currentPageIndex;

        public class MeterReadingRequest
        {
            public string MroID { set; get; }
            public string RegisterNumber { set; get; }
            public string MeterReadingResult { set; get; }
            public string Channel { set; get; }
            public string MeterReadingDate { set; get; }
            public string MeterReadingTime { set; get; }
        }

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
            NavigationController.NavigationBarHidden = false;
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
                PrepareToolTipView();
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
                Text = "Please enter your meter reading for each respective units.",
                Tag = 1
            };
            _meterReadScrollView.AddSubview(_descriptionLabel);
            scrollViewFrame = _meterReadScrollView.Frame;
        }

        private void PrepareToolTipView()
        {
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
            nfloat padding = 18f;
            nfloat width = currentWindow.Frame.Width;
            nfloat height = currentWindow.Frame.Height;
            if (_toolTipParentView == null)
            {
                _toolTipParentView = new UIView(new CGRect(0, 0, ViewWidth, currentWindow.Frame.Height))
                {
                    BackgroundColor = MyTNBColor.Black60,
                    Hidden = false
                };
                currentWindow.AddSubview(_toolTipParentView);

                _toolTipContainerView = new UIView(new CGRect(padding, 104f, width - (padding * 2), 500f))
                {
                    BackgroundColor = UIColor.White,
                    ClipsToBounds = true
                };
                _toolTipContainerView.Layer.CornerRadius = 5f;
                _toolTipParentView.AddSubview(_toolTipContainerView);
                SetToolTipScrollView();
                SetScrollViewSubViews();
            }
            else
            {
                _toolTipParentView.Hidden = false;
            }
        }

        private void MakeToolTipVisible(bool isVisible)
        {
            if (_toolTipParentView != null)
            {
                _toolTipParentView.Hidden = !isVisible;
            }
        }

        private void SetToolTipScrollView()
        {
            _toolTipScrollView = new UIScrollView(new CGRect(0, 0, _toolTipContainerView.Frame.Width, 0f))
            {
                Delegate = new ToolTipScrollViewDelegate(this),
                PagingEnabled = true,
                ShowsHorizontalScrollIndicator = false,
                ShowsVerticalScrollIndicator = false,
                ClipsToBounds = true,
                BackgroundColor = UIColor.Clear,
                Hidden = false
            };

            _toolTipContainerView.AddSubview(_toolTipScrollView);
        }

        private void SetScrollViewSubViews()
        {
            nfloat padding = 16f;
            nfloat width = _toolTipScrollView.Frame.Width;
            nfloat newHeight = 0f;
            for (int i = 0; i < 3; i++)
            {
                UIView viewContainer = new UIView(_toolTipScrollView.Bounds);
                viewContainer.BackgroundColor = UIColor.White;

                nfloat origImageRatio = 155.0f / 284.0f;
                nfloat imageHeight = viewContainer.Frame.Width * origImageRatio;
                UIImageView imageView = new UIImageView(new CGRect(0, 0, viewContainer.Frame.Width, imageHeight))
                {
                    Image = UIImage.FromBundle("ToolTip-BG"),
                };
                viewContainer.AddSubview(imageView);

                UILabel title = new UILabel(new CGRect(padding, imageView.Frame.GetMaxY() + 24f, viewContainer.Frame.Width - (padding * 2), 0))
                {
                    Font = MyTNBFont.MuseoSans14_500,
                    TextColor = MyTNBColor.CharcoalGrey,
                    TextAlignment = UITextAlignment.Left,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.TailTruncation,
                    Text = "Alright, what do I need to read?"
                };

                CGSize titleNewSize = title.SizeThatFits(new CGSize(viewContainer.Frame.Width - (padding * 2), 1000f));
                CGRect titleFrame = title.Frame;
                titleFrame.Height = titleNewSize.Height;
                title.Frame = titleFrame;
                viewContainer.AddSubview(title);

                UILabel description = new UILabel(new CGRect(padding, title.Frame.GetMaxY() + 12f, viewContainer.Frame.Width - (padding * 2), 0))
                {
                    Font = MyTNBFont.MuseoSans14_300,
                    TextColor = MyTNBColor.CharcoalGrey,
                    TextAlignment = UITextAlignment.Left,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.TailTruncation,
                    Text = "You'll need to read 3 reading values (kWh, kVARh, kW). Your meter will automatically flash one after the other."
                };

                CGSize descNewSize = description.SizeThatFits(new CGSize(viewContainer.Frame.Width - (padding * 2), 1000f));
                CGRect descFrame = description.Frame;
                descFrame.Height = descNewSize.Height;
                description.Frame = descFrame;
                viewContainer.AddSubview(description);

                CGRect frame = viewContainer.Frame;
                frame.X = i * width;
                frame.Width = width;
                frame.Height = description.Frame.GetMaxY() + 32f;
                viewContainer.Frame = frame;
                _toolTipScrollView.AddSubview(viewContainer);
                if (newHeight < viewContainer.Frame.GetMaxY())
                {
                    newHeight = viewContainer.Frame.GetMaxY();
                }
                Debug.WriteLine("newHeight== " + newHeight);
            }
            _toolTipScrollView.ContentSize = new CGSize(_toolTipScrollView.Frame.Width * 3, newHeight);
            CGRect svFrame = _toolTipScrollView.Frame;
            svFrame.Height = newHeight;
            _toolTipScrollView.Frame = svFrame;
            SetToolTipFooterView();
        }

        private void SetToolTipFooterView()
        {
            _toolTipFooterView = new UIView(new CGRect(0, _toolTipScrollView.Frame.GetMaxY() + 5f, _toolTipContainerView.Frame.Width, 130f))
            {
                BackgroundColor = UIColor.Clear,
                ClipsToBounds = true
            };
            if (3 > 1)
            {
                AddPageControl();
                UpdatePageControl(_pageControl, _currentPageIndex, 3);
            }
            else
            {
                if (_pageControl != null)
                {
                    _pageControl.Hidden = true;
                }
            }

            UIImageView tickView = new UIImageView(new CGRect(18f, _pageControl.Frame.GetMaxY() + 32f, 20f, 20f))
            {
                Image = UIImage.FromBundle(SSMRConstants.IMG_Unmark)
            };
            tickView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                SSMRAccounts.IsHideReadMeterWalkthrough = !SSMRAccounts.IsHideReadMeterWalkthrough;
                tickView.Image = UIImage.FromBundle(SSMRAccounts.IsHideReadMeterWalkthrough
                    ? SSMRConstants.IMG_Mark : SSMRConstants.IMG_Unmark);
            }));
            _toolTipFooterView.AddSubview(tickView);

            UILabel dontShowLabel = new UILabel(new CGRect(tickView.Frame.GetMaxX() + 8f, _pageControl.Frame.GetMaxY() + 34f, 120f, 14f))
            {
                Font = MyTNBFont.MuseoSans12_500,
                TextColor = MyTNBColor.BrownGreyTwo,
                Text = "Don’t show me again"
            };
            _toolTipFooterView.AddSubview(dontShowLabel);

            UIView line = new UIView(new CGRect(0, dontShowLabel.Frame.GetMaxY() + 21f, _toolTipFooterView.Frame.Width, 1f))
            {
                BackgroundColor = MyTNBColor.VeryLightPink
            };
            _toolTipFooterView.AddSubview(line);

            UILabel proceedLabel = new UILabel(new CGRect(0, line.Frame.GetMaxY() + 16f, _toolTipFooterView.Frame.Width, 24f))
            {
                Font = MyTNBFont.MuseoSans16_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = "I’m Ready!",
                TextAlignment = UITextAlignment.Center,
                UserInteractionEnabled = true
            };
            proceedLabel.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                MakeToolTipVisible(false);
            }));
            _toolTipFooterView.AddSubview(proceedLabel);

            CGRect tooltipViewframe = _toolTipFooterView.Frame;
            tooltipViewframe.Height = proceedLabel.Frame.GetMaxY() + 16f;
            _toolTipFooterView.Frame = tooltipViewframe;

            _toolTipContainerView.AddSubview(_toolTipFooterView);

            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
            CGRect frame = _toolTipContainerView.Frame;
            frame.Height = _toolTipFooterView.Frame.GetMaxY();
            frame.Y = (currentWindow.Frame.Height / 2) - (frame.Height / 2);
            _toolTipContainerView.Frame = frame;
        }

        private void AddPageControl()
        {
            if (_pageControl != null)
            {
                _pageControl.RemoveFromSuperview();
            }
            _pageControl = new UIPageControl(new CGRect(0, 0, _toolTipFooterView.Frame.Width, DashboardHomeConstants.PageControlHeight))
            {
                BackgroundColor = UIColor.Clear,
                TintColor = MyTNBColor.WaterBlue,
                PageIndicatorTintColor = MyTNBColor.VeryLightPinkTwo,
                CurrentPageIndicatorTintColor = MyTNBColor.WaterBlue,
                UserInteractionEnabled = false
            };
            _toolTipFooterView.AddSubview(_pageControl);
        }

        private void UpdatePageControl(UIPageControl pageControl, int current, int pages)
        {
            pageControl.CurrentPage = current;
            pageControl.Pages = pages;
            pageControl.UpdateCurrentPageDisplay();
        }

        private void ScrollViewHasPaginated()
        {
            UpdatePageControl(_pageControl, _currentPageIndex, 3);
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
                    if (!string.IsNullOrEmpty(previousMeter.CurrentReading) && !string.IsNullOrWhiteSpace(previousMeter.CurrentReading))
                    {
                        var currentReading = previousMeter.CurrentReading;
                        if (double.TryParse(currentReading, out double currentReadingDouble))
                        {
                            string currentReadingNewStr = currentReadingDouble.ToString("0.0", CultureInfo.InvariantCulture);
                            sSMRMeterCardComponent.UpdateMeterReadingValueFromOCR(currentReadingNewStr);
                        }
                    }
                    if (previousMeter.IsErrorFromOCR)
                    {
                        sSMRMeterCardComponent.UpdateUI(true, previousMeter.ErrorMessage, 0.00);
                    }
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
            Dictionary<string, bool> ReadingDictionary = new Dictionary<string, bool>();
            if (_previousMeterList != null)
            {
                foreach (var previousMeter in _previousMeterList)
                {
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
                viewController.OCRReadingDone = OCRReadingDone;
                NavigationController.PushViewController(viewController, true);
            }
        }

        /// <summary>
        /// Handler for finished OCR Reading
        /// </summary>
        public void OCRReadingDone()
        {
            UpdateReadings(OCRReadingCache.Instance.GetOCRReadings());
        }

        private void UpdateReadings(List<OCRReadingModel> ocrReadings)
        {
            if (ocrReadings?.Count > 0)
            {
                foreach (var ocr in ocrReadings)
                {
                    if (_previousMeterList != null)
                    {
                        foreach (var previousMeter in _previousMeterList)
                        {
                            if (ocr.IsSuccess)
                            {
                                if (previousMeter.RegisterNumberType == ocr.RegisterNumberTypeFromOCRUnit)
                                {
                                    previousMeter.IsErrorFromOCR = !ocr.IsSuccess;
                                    previousMeter.CurrentReading = ocr.OCRValue;
                                }
                            }
                            else
                            {
                                if (previousMeter.RegisterNumberType == ocr.RegisterNumberTypeFromOCRUnit)
                                {
                                    previousMeter.IsErrorFromOCR = !ocr.IsSuccess;
                                    previousMeter.ErrorMessage = ocr.Message;
                                }
                            }
                        }
                    }
                }
                UpdateUIForReadings();
            }
        }

        private void UpdateUIForReadings()
        {
            ClearScrollViewSubViews();
            PrepareMeterReadingCard();
        }

        private void ClearScrollViewSubViews()
        {
            var subviews = _meterReadScrollView.Subviews;
            foreach (var view in subviews)
            {
                if (view != null & view.Tag != 1)
                {
                    view.RemoveFromSuperview();
                }
            }
        }

        private void OnTapSubmitReading()
        {
            List<MeterReadingRequest> meterReadingRequestList = new List<MeterReadingRequest>();
            if (_previousMeterList != null)
            {
                foreach (var previousMeter in _previousMeterList)
                {
                    MeterReadingRequest meterReadingRequest = new MeterReadingRequest();
                    meterReadingRequest.MroID = previousMeter.MroID;
                    meterReadingRequest.RegisterNumber = previousMeter.RegisterNumber;
                    meterReadingRequest.MeterReadingResult = previousMeter.CurrentReading;
                    meterReadingRequest.Channel = string.Empty;
                    meterReadingRequest.MeterReadingDate = string.Empty;
                    meterReadingRequest.MeterReadingTime = string.Empty;
                    meterReadingRequestList.Add(meterReadingRequest);
                }
            }
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        await CallSubmitSMRMeterReading(DataManager.DataManager.SharedInstance.SelectedAccount, meterReadingRequestList);
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
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

        private async Task CallSubmitSMRMeterReading(CustomerAccountRecordModel account, List<MeterReadingRequest> meterReadings)
        {
            ActivityIndicator.Show();
            await SubmitSMRMeterReading(account, meterReadings).ContinueWith(task =>
            {
                InvokeOnMainThread(() =>
                {
                    if (_submitMeterResponse != null &&
                        _submitMeterResponse.d != null &&
                        _submitMeterResponse.d.IsSuccess &&
                        _submitMeterResponse.d.data != null)
                    {
                        UIStoryboard storyBoard = UIStoryboard.FromName("Feedback", null);
                        GenericStatusPageViewController status = storyBoard.InstantiateViewController("GenericStatusPageViewController") as GenericStatusPageViewController;
                        status.NextViewController = GetSMRReadingHistoryView();
                        status.StatusDisplayType = GenericStatusPageViewController.StatusType.SSMRReading;
                        status.IsSuccess = _submitMeterResponse.d.IsSuccess;
                        status.StatusTitle = _submitMeterResponse.d.DisplayTitle;
                        status.StatusMessage = _submitMeterResponse.d.DisplayMessage;
                        NavigationController.PushViewController(status, true);
                    }
                    else
                    {
                        UIStoryboard storyBoard = UIStoryboard.FromName("Feedback", null);
                        GenericStatusPageViewController status = storyBoard.InstantiateViewController("GenericStatusPageViewController") as GenericStatusPageViewController;
                        status.StatusDisplayType = GenericStatusPageViewController.StatusType.SSMRReading;
                        status.IsSuccess = _submitMeterResponse.d.IsSuccess;
                        status.StatusTitle = _submitMeterResponse.d.DisplayTitle;
                        status.StatusMessage = _submitMeterResponse.d.DisplayMessage;
                        NavigationController.PushViewController(status, true);
                    }
                    ActivityIndicator.Hide();
                });
            });
        }

        private Task SubmitSMRMeterReading(CustomerAccountRecordModel account, List<MeterReadingRequest> meterReadings)
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object usrInf = new
                {
                    eid = DataManager.DataManager.SharedInstance.User.Email,
                    sspuid = DataManager.DataManager.SharedInstance.User.UserID,
                    did = DataManager.DataManager.SharedInstance.UDID,
                    ft = DataManager.DataManager.SharedInstance.FCMToken,
                    lang = TNBGlobal.DEFAULT_LANGUAGE,
                    sec_auth_k1 = TNBGlobal.API_KEY_ID,
                    sec_auth_k2 = string.Empty,
                    ses_param1 = string.Empty,
                    ses_param2 = string.Empty
                };
                object request = new
                {
                    contractAccount = account.accNum,
                    isOwnedAccount = account.IsOwnedAccount,
                    meterReadings,
                    usrInf
                };
                _submitMeterResponse = serviceManager.OnExecuteAPIV6<SMRSubmitMeterReadingResponseModel>("SubmitSMRMeterReading", request);
            });
        }

        private UIViewController GetSMRReadingHistoryView()
        {
            UIStoryboard storyBoard = UIStoryboard.FromName("SSMR", null);
            SSMRReadingHistoryViewController viewController =
                storyBoard.InstantiateViewController("SSMRReadingHistoryViewController") as SSMRReadingHistoryViewController;
            return viewController;
        }

        private class ToolTipScrollViewDelegate : UIScrollViewDelegate
        {
            SSMRReadMeterViewController _controller;
            public ToolTipScrollViewDelegate(SSMRReadMeterViewController controller)
            {
                _controller = controller;
            }
            public override void Scrolled(UIScrollView scrollView)
            {
                int newPageIndex = (int)Math.Round(_controller._toolTipScrollView.ContentOffset.X / _controller._toolTipScrollView.Frame.Width);
                if (newPageIndex == _controller._currentPageIndex)
                    return;

                _controller._currentPageIndex = newPageIndex;
                _controller.ScrollViewHasPaginated();
            }
        }
    }
}
