using CoreGraphics;
using Foundation;
using myTNB.Model;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using myTNB.SSMR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UIKit;

namespace myTNB
{
    public partial class SSMRReadMeterViewController : CustomUIViewController
    {
        public SSMRReadMeterViewController(IntPtr handle) : base(handle) { }
        public bool IsRoot;
        public bool IsFromDashboard;

        SSMRMeterFooterComponent _sSMRMeterFooterComponent;
        SMRSubmitMeterReadingResponseModel _submitMeterResponse = new SMRSubmitMeterReadingResponseModel();
        List<SMRMROValidateRegisterDetailsInfoModel> _previousMeterList;
        List<MeterReadSSMRModel> _toolTipList;
        public List<SSMRMeterReadWalkthroughModel> pageData;

        UIView _toolTipParentView, _toolTipContainerView, _toolTipFooterView, _takePhotoView, _takePhotoBtnView, _noteView;
        UIScrollView _toolTipScrollView;
        UIPageControl _pageControl;
        UIScrollView _meterReadScrollView;
        UIImageView _cameraIconView;
        UILabel _descriptionLabel, _takePhotoLabel, _errorLabel, _noteLabel;
        nfloat _paddingX = ScaleUtility.GetScaledWidth(16f);
        nfloat _paddingY = ScaleUtility.GetScaledHeight(16f);
        nfloat takePhotoViewRatio = 136.0f / 320.0f;
        nfloat lastCardYPos;
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
            NavigationController.NavigationBarHidden = false;

            base.ViewDidLoad();

            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);

            _previousMeterList = IsFromDashboard ? SSMRActivityInfoCache.DashboardPreviousReading
                : SSMRActivityInfoCache.ViewPreviousReading;
            _isThreePhase = _previousMeterList.Count > 1;

            SetNavigation();
            SetWalkthroughData();
            AddFooterView();
            Initialization();
            PrepareTakePhotoHeaderView();
            PrepareMeterReadingCard();
            PrepareNoteView();
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
                if (IsRoot && NavigationController != null)
                {
                    NavigationController.PopViewController(true);
                }
                else
                {
                    DismissViewController(true, null);
                    //ViewHelper.DismissControllersAndSelectTab(this, 0, true);
                }
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
                Image = _isThreePhase ? SSMRConstants.IMG_BGToolTip1 : SSMRConstants.IMG_BGToolTip2,
                Title = GetI18NValue(SSMRConstants.I18N_ToolTip1),
                Description = _isThreePhase ? GetI18NValue(SSMRConstants.I18N_ToolTipDesc1) : GetI18NValue(SSMRConstants.I18N_ToolTipDesc4)
            };
            pageData.Add(item1);

            var item2 = new SSMRMeterReadWalkthroughModel
            {
                Image = _isThreePhase ? SSMRConstants.IMG_BGToolTip2 : SSMRConstants.IMG_BGToolTip3,
                Title = _isThreePhase ? GetI18NValue(SSMRConstants.I18N_ToolTip2) : GetI18NValue(SSMRConstants.I18N_ToolTip3),
                Description = _isThreePhase ? GetI18NValue(SSMRConstants.I18N_ToolTipDesc2) : GetI18NValue(SSMRConstants.I18N_ToolTipDesc5)
            };
            pageData.Add(item2);

            if (_isThreePhase)
            {
                var item3 = new SSMRMeterReadWalkthroughModel
                {
                    Image = SSMRConstants.IMG_BGToolTip3,
                    Title = GetI18NValue(SSMRConstants.I18N_ToolTip3),
                    Description = GetI18NValue(SSMRConstants.I18N_ToolTipDesc3),
                };
                pageData.Add(item3);
            }
        }

        private void ShowToolTipByDefault()
        {
            if (_isThreePhase)
            {
                if (!SSMRAccounts.IsHideReadMeterWalkthroughV2)
                {
                    PrepareToolTipView();
                    SSMRAccounts.IsHideReadMeterWalkthroughV2 = true;
                }
            }
            else
            {
                if (!SSMRAccounts.IsHideReadMeterWalkthrough)
                {
                    PrepareToolTipView();
                    SSMRAccounts.IsHideReadMeterWalkthrough = true;
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

        private void PrepareNoteView()
        {
            nfloat margin = ScaleUtility.GetScaledWidth(16f);
            _noteView = new UIView(new CGRect(margin, lastCardYPos + margin, _meterReadScrollView.Frame.Width - (margin * 2), GetScaledHeight(108f)))
            {
                BackgroundColor = UIColor.Clear,
                Tag = 2
            };
            _meterReadScrollView.AddSubview(_noteView);

            _errorLabel = new UILabel(new CGRect(0, 0, _noteView.Frame.Width, GetScaledHeight(16f)))
            {
                BackgroundColor = UIColor.Clear,
                Font = MyTNBFont.MuseoSans12_500,
                TextColor = MyTNBColor.Tomato,
                Lines = 0,
                TextAlignment = UITextAlignment.Left,
                Hidden = false
            };
            _noteView.AddSubview(_errorLabel);

            _noteLabel = new UILabel(new CGRect(0, _errorLabel.Frame.GetMaxY() + GetScaledHeight(24f), _noteView.Frame.Width, GetScaledHeight(32f)))
            {
                BackgroundColor = UIColor.Clear,
                Font = MyTNBFont.MuseoSans12_300,
                TextColor = MyTNBColor.GreyishBrownTwo,
                Lines = 0,
                TextAlignment = UITextAlignment.Left,
                Text = "Note: Please round up the numbers if the reading values are showing decimal points on your meter.",
                Hidden = false
            };
            _noteView.AddSubview(_noteLabel);

            ShowGeneralInlineError(false, string.Empty);
        }

        private void ShowGeneralInlineError(bool isError, string errorMessage)
        {
            if (_noteView == null)
                return;

            _errorLabel.Hidden = !isError;
            if (isError)
            {
                _errorLabel.Text = errorMessage ?? string.Empty;
                ViewHelper.AdjustFrameSetY(_noteLabel, _errorLabel.Frame.GetMaxY() + GetScaledHeight(24f));
            }
            else
            {
                ViewHelper.AdjustFrameSetY(_noteLabel, 0);
            }
            ViewHelper.AdjustFrameSetHeight(_noteView, _noteLabel.Frame.GetMaxY() + _paddingY);

            _meterReadScrollView.ContentSize = new CGSize(ViewWidth, _noteView.Frame.GetMaxY() + _paddingY);
            scrollViewFrame = _meterReadScrollView.Frame;

            if (isError)
            {
                CGPoint point = new CGPoint(0, _meterReadScrollView.ContentSize.Height - _meterReadScrollView.Bounds.Size.Height + _meterReadScrollView.ContentInset.Bottom);
                _meterReadScrollView.SetContentOffset(point, true);
            }
        }

        private void PrepareTakePhotoHeaderView()
        {
            nfloat takePhotoViewHeight = View.Frame.Width * takePhotoViewRatio;
            nfloat descLabelWidth = _meterReadScrollView.Frame.Width - (_paddingX * 2);
            _takePhotoView = new UIView(new CGRect(0, 0, View.Frame.Width, takePhotoViewHeight))
            {
                BackgroundColor = UIColor.White,
                Tag = 1
            };

            _descriptionLabel = new UILabel(new CGRect(_paddingX, _paddingY, descLabelWidth, 44f))
            {
                BackgroundColor = UIColor.Clear,
                Font = MyTNBFont.MuseoSans14_300,
                TextColor = MyTNBColor.CharcoalGrey,
                Lines = 0,
                TextAlignment = UITextAlignment.Left,
                Text = GetI18NValue(SSMRConstants.I18N_HeaderDesc)
            };

            CGSize cGSize = _descriptionLabel.SizeThatFits(new CGSize(descLabelWidth, 1000f));

            ViewHelper.AdjustFrameSetHeight(_descriptionLabel, cGSize.Height);
            _takePhotoView.AddSubview(_descriptionLabel);

            nfloat btnViewWidth = _meterReadScrollView.Frame.Width - (_paddingX * 2);
            _takePhotoBtnView = new UIView(new CGRect(_paddingX, _descriptionLabel.Frame.GetMaxY() + GetScaledHeight(12f), btnViewWidth, GetScaledHeight(48f)))
            {
                BackgroundColor = UIColor.White
            };
            _takePhotoBtnView.Layer.CornerRadius = 4f;
            _takePhotoBtnView.Layer.BorderColor = MyTNBColor.AlgaeGreen.CGColor;
            _takePhotoBtnView.Layer.BorderWidth = 1f;

            _takePhotoBtnView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                OnTapTakePhoto();
            }));

            _takePhotoView.AddSubview(_takePhotoBtnView);

            ViewHelper.AdjustFrameSetHeight(_takePhotoView, _takePhotoBtnView.Frame.GetMaxY() + _paddingY);

            UIView containerView = new UIView(new CGRect(0, 0, 250f, _takePhotoBtnView.Frame.Height))
            {
                BackgroundColor = UIColor.Clear
            };
            _takePhotoBtnView.AddSubview(containerView);

            nfloat width = GetScaledWidth(20.6f);
            nfloat height = GetScaledHeight(16f);
            _cameraIconView = new UIImageView(new CGRect(0, 0, width, height))
            {
                Image = UIImage.FromBundle("Camera-Icon-Green")
            };
            containerView.AddSubview(_cameraIconView);

            nfloat takePhotoLabelWidth = GetScaledWidth(166f);
            _takePhotoLabel = new UILabel(new CGRect(_cameraIconView.Frame.GetMaxX() + GetScaledWidth(12f), 0, takePhotoLabelWidth, 44f))
            {
                BackgroundColor = UIColor.Clear,
                Font = MyTNBFont.MuseoSans16_500,
                TextColor = MyTNBColor.AlgaeGreen,
                Lines = 0,
                TextAlignment = UITextAlignment.Left,
                Text = "Take Or Upload Photo"
            };
            containerView.AddSubview(_takePhotoLabel);

            CGSize cGSizeLbl = _takePhotoLabel.SizeThatFits(new CGSize(takePhotoLabelWidth, 1000f));

            ViewHelper.AdjustFrameSetHeight(_takePhotoLabel, cGSizeLbl.Height);
            ViewHelper.AdjustFrameSetWidth(_takePhotoLabel, cGSizeLbl.Width);
            ViewHelper.AdjustFrameSetY(_takePhotoLabel, GetYLocationToCenterObject(_takePhotoLabel.Frame.Height, _takePhotoBtnView));

            ViewHelper.AdjustFrameSetY(_cameraIconView, GetYLocationToCenterObject(_cameraIconView.Frame.Height, _takePhotoBtnView));

            ViewHelper.AdjustFrameSetWidth(containerView, _takePhotoLabel.Frame.GetMaxX());
            ViewHelper.AdjustFrameSetY(containerView, GetYLocationToCenterObject(containerView.Frame.Height, _takePhotoBtnView));
            ViewHelper.AdjustFrameSetX(containerView, GetXLocationToCenterObject(containerView.Frame.Width, _takePhotoBtnView));

            AddHeaderhadow(ref _takePhotoView);
            _meterReadScrollView.AddSubview(_takePhotoView);
        }

        private void PrepareToolTipView()
        {
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
            nfloat widthMargin = GetScaledWidth(18f);
            nfloat width = currentWindow.Frame.Width;
            nfloat height = currentWindow.Frame.Height;
            if (_toolTipParentView != null)
            {
                _toolTipParentView.RemoveFromSuperview();
            }
            _toolTipParentView = new UIView(new CGRect(0, 0, ViewWidth, height))
            {
                BackgroundColor = MyTNBColor.Black60
            };
            currentWindow.AddSubview(_toolTipParentView);

            _toolTipContainerView = new UIView(new CGRect(widthMargin, 104f, width - (widthMargin * 2), 500f))
            {
                BackgroundColor = UIColor.White,
                ClipsToBounds = true
            };
            _toolTipContainerView.Layer.CornerRadius = 5f;
            _toolTipParentView.AddSubview(_toolTipContainerView);
            SetToolTipScrollView();
            SetScrollViewSubViews();

            _toolTipParentView.Hidden = false;
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
            nfloat widthMargin = GetScaledWidth(16f);
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

                UILabel title = new UILabel(new CGRect(widthMargin, imageView.Frame.GetMaxY() + GetScaledHeight(24f), viewContainer.Frame.Width - (widthMargin * 2), 0))
                {
                    Font = MyTNBFont.MuseoSans14_500,
                    TextColor = MyTNBColor.CharcoalGrey,
                    TextAlignment = UITextAlignment.Left,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.TailTruncation,
                    Text = pageData[i]?.Title ?? string.Empty
                };

                CGSize titleNewSize = title.SizeThatFits(new CGSize(viewContainer.Frame.Width - (widthMargin * 2), 1000f));
                ViewHelper.AdjustFrameSetHeight(title, titleNewSize.Height);

                viewContainer.AddSubview(title);
                string desc = pageData[i]?.Description ?? string.Empty;
                if (!string.IsNullOrEmpty(desc) && !string.IsNullOrWhiteSpace(desc) && desc.Contains("{0}"))
                {
                    int count = _previousMeterList.Count;
                    string missingReading = string.Empty;
                    for (int j = 0; j < count; j++)
                    {
                        missingReading += _previousMeterList[j].RegisterNumberType;
                        if (j != count - 1)
                        {
                            missingReading += ", ";
                        }
                    }
                    desc = string.Format(desc, count, missingReading);

                }
                UILabel description = new UILabel(new CGRect(widthMargin, title.Frame.GetMaxY() + GetScaledHeight(12f), viewContainer.Frame.Width - (widthMargin * 2), 0))
                {
                    Font = MyTNBFont.MuseoSans14_300,
                    TextColor = MyTNBColor.CharcoalGrey,
                    TextAlignment = UITextAlignment.Left,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.TailTruncation,
                    Text = desc
                };

                CGSize descNewSize = description.SizeThatFits(new CGSize(viewContainer.Frame.Width - (widthMargin * 2), 1000f));
                ViewHelper.AdjustFrameSetHeight(description, descNewSize.Height);

                viewContainer.AddSubview(description);

                ViewHelper.AdjustFrameSetX(viewContainer, i * width);
                ViewHelper.AdjustFrameSetWidth(viewContainer, width);
                ViewHelper.AdjustFrameSetHeight(viewContainer, description.Frame.GetMaxY() + GetScaledHeight(32f));

                _toolTipScrollView.AddSubview(viewContainer);
                if (newHeight < viewContainer.Frame.GetMaxY())
                {
                    newHeight = viewContainer.Frame.GetMaxY();
                }
            }
            _toolTipScrollView.ContentSize = new CGSize(_toolTipScrollView.Frame.Width * pageData.Count, newHeight);
            ViewHelper.AdjustFrameSetHeight(_toolTipScrollView, newHeight);

            SetToolTipFooterView();
        }

        private void SetToolTipFooterView()
        {
            _toolTipFooterView = new UIView(new CGRect(0, _toolTipScrollView.Frame.GetMaxY(), _toolTipContainerView.Frame.Width, 130f))
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

            UIView line = new UIView(new CGRect(0, _pageControl.Frame.GetMaxY() + GetScaledHeight(16f), _toolTipFooterView.Frame.Width, GetScaledHeight(1f)))
            {
                BackgroundColor = MyTNBColor.VeryLightPink
            };
            _toolTipFooterView.AddSubview(line);

            UILabel proceedLabel = new UILabel(new CGRect(0, line.Frame.GetMaxY() + GetScaledHeight(16f), _toolTipFooterView.Frame.Width, GetScaledHeight(24f)))
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

            ViewHelper.AdjustFrameSetHeight(_toolTipFooterView, proceedLabel.Frame.GetMaxY() + GetScaledHeight(16f));

            _toolTipContainerView.AddSubview(_toolTipFooterView);

            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;

            ViewHelper.AdjustFrameSetHeight(_toolTipContainerView, _toolTipFooterView.Frame.GetMaxY());
            ViewHelper.AdjustFrameSetY(_toolTipContainerView, (currentWindow.Frame.Height / 2) - (_toolTipContainerView.Frame.Height / 2));
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

        private void PrepareMeterReadingCard(bool isBusinessError = false)
        {
            if (_previousMeterList != null)
            {
                bool hasOCRError = false;
                string errorMessage = string.Empty;
                nfloat yPos = _takePhotoView.Frame.GetMaxY() + _paddingY;
                foreach (var previousMeter in _previousMeterList)
                {
                    SSMRMeterCardComponent sSMRMeterCardComponent = new SSMRMeterCardComponent(this, _meterReadScrollView, yPos);
                    _meterReadScrollView.AddSubview(sSMRMeterCardComponent.GetUI());
                    sSMRMeterCardComponent.SetModel(previousMeter);
                    sSMRMeterCardComponent.SetPreviousReading(previousMeter.PrevMeterReading);
                    sSMRMeterCardComponent.SetIconText(previousMeter);

                    if (!string.IsNullOrEmpty(previousMeter.CurrentReading) && !string.IsNullOrWhiteSpace(previousMeter.CurrentReading))
                    {
                        if (isBusinessError)
                        {
                            sSMRMeterCardComponent.UpdateMeterReadingValueFromSubmission(previousMeter.CurrentReading);
                        }
                        else
                        {
                            sSMRMeterCardComponent.UpdateMeterReadingValueFromOCR(previousMeter.CurrentReading);
                        }
                    }
                    if (previousMeter.IsErrorFromOCR)
                    {
                        hasOCRError = true;
                        errorMessage = previousMeter.ErrorMessage;

                    }
                    if (isBusinessError)
                    {
                        sSMRMeterCardComponent.UpdateUI(!previousMeter.IsValidManualReading, previousMeter.ErrorMessage, previousMeter.CurrentReading);
                    }
                    yPos = sSMRMeterCardComponent.GetView().Frame.GetMaxY() + _paddingY;
                    _meterReadScrollView.ContentSize = new CGSize(ViewWidth, yPos);
                    scrollViewFrame = _meterReadScrollView.Frame;
                }
                lastCardYPos = yPos;
                ShowGeneralInlineError(hasOCRError, errorMessage);
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
                    if (string.IsNullOrEmpty(previousMeter.CurrentReading) || string.IsNullOrWhiteSpace(previousMeter.CurrentReading))
                    {
                        res = false;
                        break;
                    }
                }
            }
            _sSMRMeterFooterComponent.SetSubmitButtonEnabled(res);
        }

        private void AddFooterView()
        {
            _sSMRMeterFooterComponent = new SSMRMeterFooterComponent(View, ViewHeight);
            View.AddSubview(_sSMRMeterFooterComponent.GetUI());
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
            UpdateReadingsFromOCR(OCRReadingCache.GetOCRReadings());
        }

        private void UpdateReadingsFromOCR(List<OCRReadingModel> ocrReadings)
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

        private void UpdateReadingsFromSubmit(List<MeterReadingItemModel> submitReadingsResponse)
        {
            if (submitReadingsResponse?.Count > 0)
            {
                foreach (var response in submitReadingsResponse)
                {
                    if (_previousMeterList != null)
                    {
                        foreach (var previousMeter in _previousMeterList)
                        {
                            if (previousMeter.RegisterNumberType == response.RegisterNumberType)
                            {
                                previousMeter.IsValidManualReading = response.IsSuccess;
                                previousMeter.ErrorMessage = response.Message;
                            }
                        }
                    }
                }
                UpdateUIForReadings(true);
            }
        }

        private void UpdateUIForReadings(bool isBusinessError = false)
        {
            ClearScrollViewSubViews();
            PrepareMeterReadingCard(isBusinessError);
        }

        private void ClearScrollViewSubViews()
        {
            var subviews = _meterReadScrollView.Subviews;
            foreach (var view in subviews)
            {
                if (view != null & view.Tag != 1 & view.Tag != 2)
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
                        _submitMeterResponse.d.data != null)
                    {
                        if (_submitMeterResponse.d.IsSuccess)
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
                        else if (_submitMeterResponse.d.IsBusinessFail)
                        {
                            UpdateReadingsFromSubmit(_submitMeterResponse.d.data.SubmitSMRMeterReadingsResp);
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

        private void AddHeaderhadow(ref UIView view)
        {
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.BrownGrey10.CGColor;
            view.Layer.ShadowOpacity = 0.5f;
            view.Layer.ShadowOffset = new CGSize(0, 5);
            view.Layer.ShadowRadius = 5;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
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
