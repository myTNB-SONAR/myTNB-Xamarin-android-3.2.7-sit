using CoreGraphics;
using Foundation;
using myTNB.Model;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
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
        List<MeterReadSSMRModel> _toolTipList;
        public List<SSMRMeterReadWalkthroughModel> pageData;

        UIView _toolTipParentView, _toolTipContainerView, _toolTipFooterView;
        UIScrollView _toolTipScrollView;
        UIPageControl _pageControl;
        UIScrollView _meterReadScrollView;
        UIImageView _tickView;
        UILabel _descriptionLabel;
        nfloat _padding = 16f;
        CGRect scrollViewFrame;
        int _currentPageIndex;
        bool _isThreePhase = false;
        bool _toolTipFlag = false;

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
            PageName = SSMRConstants.Pagename_SSMRMeterRead;
            base.ViewDidLoad();

            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);

            _previousMeterList = SSMRActivityInfoCache.Instance.GetPreviousMeterReadingList();
            _isThreePhase = _previousMeterList.Count > 1;

            SetNavigation();
            SetWalkthroughData();
            AddFooterView();
            Initialization();
            PrepareMeterReadingCard();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationController.NavigationBarHidden = false;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ShowToolTipByDefault();
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
                PrepareToolTipView();
            });
            NavigationItem.LeftBarButtonItem = btnBack;
            NavigationItem.RightBarButtonItem = btnRight;
            Title = GetI18NValue(SSMRConstants.I18N_NavTitle);
        }

        private void SetWalkthroughData()
        {
            pageData = new List<SSMRMeterReadWalkthroughModel>();
            var item1 = new SSMRMeterReadWalkthroughModel
            {
                Image = SSMRConstants.IMG_BGToolTip1,
                Title = GetI18NValue(SSMRConstants.I18N_ToolTip1),
                Description = GetI18NValue(SSMRConstants.I18N_ToolTipDesc1),
            };
            pageData.Add(item1);

            var item2 = new SSMRMeterReadWalkthroughModel
            {
                Image = SSMRConstants.IMG_BGToolTip2,
                Title = GetI18NValue(SSMRConstants.I18N_ToolTip2),
                Description = GetI18NValue(SSMRConstants.I18N_ToolTipDesc2),
            };
            pageData.Add(item2);

            var item3 = new SSMRMeterReadWalkthroughModel
            {
                Image = SSMRConstants.IMG_BGToolTip3,
                Title = GetI18NValue(SSMRConstants.I18N_ToolTip3),
                Description = GetI18NValue(SSMRConstants.I18N_ToolTipDesc3),
            };
            pageData.Add(item3);
        }

        private void ShowToolTipByDefault()
        {
            if (_isThreePhase)
            {
                if (!SSMRAccounts.IsHideReadMeterWalkthroughV2)
                {
                    if (!_toolTipFlag)
                    {
                        PrepareToolTipView();
                        _toolTipFlag = true;
                    }
                }
            }
            else
            {
                if (!SSMRAccounts.IsHideReadMeterWalkthrough)
                {
                    if (!_toolTipFlag)
                    {
                        PrepareToolTipView();
                        _toolTipFlag = true;
                    }
                }
            }
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
                Text = GetI18NValue(SSMRConstants.I18N_HeaderDesc),
                Tag = 1
            };
            _meterReadScrollView.AddSubview(_descriptionLabel);
            scrollViewFrame = _meterReadScrollView.Frame;

            if (_isThreePhase)
            {
                MeterReadSSMRWalkthroughEntityV2 wsManager = new MeterReadSSMRWalkthroughEntityV2();
                _toolTipList = wsManager.GetAllItems();
            }
            else
            {
                MeterReadSSMRWalkthroughEntity wsManager = new MeterReadSSMRWalkthroughEntity();
                _toolTipList = wsManager.GetAllItems();
            }
            if (_toolTipList != null && _toolTipList.Count > 0)
            {
                pageData.Clear();
                for (int i = 0; i < _toolTipList.Count; i++)
                {
                    SSMRMeterReadWalkthroughModel item = new SSMRMeterReadWalkthroughModel
                    {
                        Title = _toolTipList[i].Title,
                        Description = _toolTipList[i].Description,
                        Image = _toolTipList[i].Image,
                        IsSitecoreData = true
                    };
                    pageData.Add(item);
                }
            }
        }

        private void PrepareToolTipView()
        {
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
            nfloat padding = 18f;
            nfloat width = currentWindow.Frame.Width;
            nfloat height = currentWindow.Frame.Height;
            if (_toolTipParentView == null)
            {
                _toolTipParentView = new UIView(new CGRect(0, 0, ViewWidth, height))
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
            for (int i = 0; i < pageData.Count; i++)
            {
                UIView viewContainer = new UIView(_toolTipScrollView.Bounds);
                viewContainer.BackgroundColor = UIColor.White;

                UIImage displayImage;
                if (pageData[i].IsSitecoreData)
                {
                    if (string.IsNullOrEmpty(pageData[i].Image) || string.IsNullOrWhiteSpace(pageData[i].Image))
                    {
                        displayImage = UIImage.FromBundle(string.Empty);
                    }
                    else
                    {
                        try
                        {
                            displayImage = UIImage.LoadFromData(NSData.FromUrl(new NSUrl(pageData[i].Image)));
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("Image load Error: " + e.Message);
                            displayImage = UIImage.FromBundle(string.Empty);
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(pageData[i].Image) || string.IsNullOrWhiteSpace(pageData[i].Image))
                    {
                        displayImage = UIImage.FromBundle(string.Empty);
                    }
                    else
                    {
                        displayImage = UIImage.FromBundle(pageData[i].Image);
                    }
                }

                nfloat origImageRatio = 155.0f / 284.0f;
                nfloat imageHeight = viewContainer.Frame.Width * origImageRatio;
                UIImageView imageView = new UIImageView(new CGRect(0, 0, viewContainer.Frame.Width, imageHeight))
                {
                    Image = displayImage,
                };
                viewContainer.AddSubview(imageView);

                UILabel title = new UILabel(new CGRect(padding, imageView.Frame.GetMaxY() + 24f, viewContainer.Frame.Width - (padding * 2), 0))
                {
                    Font = MyTNBFont.MuseoSans14_500,
                    TextColor = MyTNBColor.CharcoalGrey,
                    TextAlignment = UITextAlignment.Left,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.TailTruncation,
                    Text = pageData[i]?.Title ?? string.Empty
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
                    Text = pageData[i]?.Description ?? string.Empty
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
            }
            _toolTipScrollView.ContentSize = new CGSize(_toolTipScrollView.Frame.Width * pageData.Count, newHeight);
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
                ClipsToBounds = true,
                UserInteractionEnabled = true
            };
            if (pageData.Count > 1)
            {
                AddPageControl();
                UpdatePageControl(_pageControl, _currentPageIndex, pageData.Count);
            }
            else
            {
                if (_pageControl != null)
                {
                    _pageControl.Hidden = true;
                }
            }

            _tickView = new UIImageView(new CGRect(18f, _pageControl.Frame.GetMaxY() + 32f, 20f, 20f))
            {
                Image = UIImage.FromBundle(SSMRConstants.IMG_Unmark),
                UserInteractionEnabled = true
            };
            _tickView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                DontShowAction();
            }));
            _toolTipFooterView.AddSubview(_tickView);

            UILabel dontShowLabel = new UILabel(new CGRect(_tickView.Frame.GetMaxX() + 8f, _pageControl.Frame.GetMaxY() + 34f, 120f, 14f))
            {
                Font = MyTNBFont.MuseoSans12_500,
                TextColor = MyTNBColor.BrownGreyTwo,
                Text = GetI18NValue(SSMRConstants.I18N_DontShowAgain),
                UserInteractionEnabled = true
            };
            dontShowLabel.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                DontShowAction();
            }));
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
                Text = GetI18NValue(SSMRConstants.I18N_ImReady),
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

        private void DontShowAction()
        {
            if (_isThreePhase)
            {
                SSMRAccounts.IsHideReadMeterWalkthroughV2 = !SSMRAccounts.IsHideReadMeterWalkthroughV2;
                _tickView.Image = UIImage.FromBundle(SSMRAccounts.IsHideReadMeterWalkthroughV2
                    ? SSMRConstants.IMG_Mark : SSMRConstants.IMG_Unmark);
            }
            else
            {
                SSMRAccounts.IsHideReadMeterWalkthrough = !SSMRAccounts.IsHideReadMeterWalkthrough;
                _tickView.Image = UIImage.FromBundle(SSMRAccounts.IsHideReadMeterWalkthrough
                    ? SSMRConstants.IMG_Mark : SSMRConstants.IMG_Unmark);
            }
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
            UpdatePageControl(_pageControl, _currentPageIndex, pageData.Count);
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
            //_sSMRMeterFooterComponent.SetTakePhotoButtonEnabled(!res);
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
                    ReadingDictionary.Add(registerStr, false);
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
                            previousMeter.IsValidManualReading = ocr.IsSuccess;
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
                                if (previousMeter.RegisterNumberType == ocr.RegisterNumberTypeFromRRUnit)
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
                    string currentReadingNewStr = string.Empty;
                    if (double.TryParse(previousMeter.CurrentReading, out double currentReadingDouble))
                    {
                        currentReadingNewStr = Math.Round(currentReadingDouble).ToString();
                    }
                    meterReadingRequest.MeterReadingResult = currentReadingNewStr;
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
                object request = new
                {
                    contractAccount = account.accNum,
                    isOwnedAccount = account.isOwned,
                    meterReadings,
                    serviceManager.usrInf
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
