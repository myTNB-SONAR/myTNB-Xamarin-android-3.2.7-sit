﻿using CoreGraphics;
using Force.DeepCloner;
using Foundation;
using myTNB.Model;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using myTNB.SSMR;
using System;
using System.Collections.Generic;
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
        List<SSMRMeterReadWalkthroughModel> pageDefaultData;

        UIView _toolTipParentView, _takePhotoView, _manualInputView, _takePhotoBtnView, _noteView;
        UIScrollView _meterReadScrollView;
        UIImageView _cameraIconView;
        UILabel _takePhotoLabel, _errorLabel;
        UITextView _txtViewNote;
        nfloat _paddingX = ScaleUtility.GetScaledWidth(16f);
        nfloat _paddingY = ScaleUtility.GetScaledHeight(16f);
        nfloat takePhotoViewRatio = 136.0f / 320.0f;
        nfloat lastCardYPos;
        CGRect scrollViewFrame;
        bool _isThreePhase;

        private bool _isKeyboardActive;
        private List<SSMRMeterCardComponent> _meterCardComponentList = new List<SSMRMeterCardComponent>();

        private UIView _tutorialContainer;
        private nfloat manualInputCardYPos;

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
            View.BackgroundColor = MyTNBColor.SectionGrey;
            PageName = SSMRConstants.Pagename_SSMRMeterRead;
            NavigationController.NavigationBarHidden = false;

            base.ViewDidLoad();

            NotifCenterUtility.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NotifCenterUtility.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);

            _previousMeterList = IsFromDashboard ? SSMRActivityInfoCache.DashboardPreviousReading.DeepClone()
                : SSMRActivityInfoCache.ViewPreviousReading.DeepClone();
            _isThreePhase = _previousMeterList.Count > 1;

            SetNavigation();
            SetWalkthroughData();
            AddFooterView();
            Initialization();
            PrepareManualInputView();
            PrepareTakePhotoHeaderView();
            PrepareMeterReadingCard(false);
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
            CheckTutorialOverlay();
        }

        #region Tutorial Overlay Methods
        private void CheckTutorialOverlay()
        {
            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            var tutorialOverlayHasShown = sharedPreference.BoolForKey(SSMRConstants.Pref_SSMRReadTutorialOverlay);
            if (tutorialOverlayHasShown) { return; }
            ShowTutorialOverlay();
        }

        private void ShowTutorialOverlay()
        {
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;

            nfloat width = currentWindow.Frame.Width;
            nfloat height = currentWindow.Frame.Height;
            if (_tutorialContainer != null)
            {
                _tutorialContainer.RemoveFromSuperview();
            }
            _tutorialContainer = new UIView(new CGRect(0, 0, width, height))
            {
                BackgroundColor = UIColor.Clear
            };
            currentWindow.AddSubview(_tutorialContainer);

            bool ocrIsDown = IsFromDashboard ? SSMRActivityInfoCache.DB_IsOCRDown : SSMRActivityInfoCache.RH_IsOCRDown;
            bool ocrIsDisabled = IsFromDashboard ? SSMRActivityInfoCache.DB_IsOCRDisabled : SSMRActivityInfoCache.RH_IsOCRDisabled;

            SSMReadTutorialOveraly tutorialView = new SSMReadTutorialOveraly(_tutorialContainer)
            {
                GetI18NValue = GetI18NValue,
                NavigationHeight = DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height,
                ManualInputCardYPos = manualInputCardYPos,
                OnDismissAction = HideTutorialOverlay,
                OCRIsDown = AppLaunchMasterCache.IsOCRDown || ocrIsDown || ocrIsDisabled
            };
            _tutorialContainer.AddSubview(tutorialView.GetView());
        }

        private void HideTutorialOverlay()
        {
            if (_tutorialContainer != null)
            {
                _tutorialContainer.Alpha = 1F;
                _tutorialContainer.Transform = CGAffineTransform.MakeIdentity();
                UIView.Animate(0.3, 0, UIViewAnimationOptions.CurveEaseInOut, () =>
                {
                    _tutorialContainer.Alpha = 0F;
                }, _tutorialContainer.RemoveFromSuperview);

                var sharedPreference = NSUserDefaults.StandardUserDefaults;
                sharedPreference.SetBool(true, SSMRConstants.Pref_SSMRReadTutorialOverlay);
            }
        }
        #endregion

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
            pageDefaultData = new List<SSMRMeterReadWalkthroughModel>();
            var item1 = new SSMRMeterReadWalkthroughModel
            {
                Image = _isThreePhase ? SSMRConstants.IMG_BGToolTip1 : SSMRConstants.IMG_BGToolTip2,
                Title = GetI18NValue(SSMRConstants.I18N_ToolTip1),
                Description = _isThreePhase ? GetI18NValue(SSMRConstants.I18N_ToolTipDesc1) : GetI18NValue(SSMRConstants.I18N_ToolTipDesc4)
            };
            pageData.Add(item1);
            pageDefaultData.Add(item1);
            var item2 = new SSMRMeterReadWalkthroughModel
            {
                Image = _isThreePhase ? SSMRConstants.IMG_BGToolTip2 : SSMRConstants.IMG_BGToolTip3,
                Title = _isThreePhase ? GetI18NValue(SSMRConstants.I18N_ToolTip2) : GetI18NValue(SSMRConstants.I18N_ToolTip3),
                Description = _isThreePhase ? GetI18NValue(SSMRConstants.I18N_ToolTipDesc2) : GetI18NValue(SSMRConstants.I18N_ToolTipDesc5)
            };
            pageData.Add(item2);
            pageDefaultData.Add(item2);
            if (_isThreePhase)
            {
                var item3 = new SSMRMeterReadWalkthroughModel
                {
                    Image = SSMRConstants.IMG_BGToolTip3,
                    Title = GetI18NValue(SSMRConstants.I18N_ToolTip3),
                    Description = GetI18NValue(SSMRConstants.I18N_ToolTipDesc3),
                };
                pageData.Add(item3);
                pageDefaultData.Add(item3);
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
                        IsSitecoreData = true,
                        NSDataImage = _toolTipList[i].ImageByteArray.ToNSData()
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
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.Tomato,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                TextAlignment = UITextAlignment.Left,
                Hidden = false
            };
            _noteView.AddSubview(_errorLabel);

            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(GetI18NValue(SSMRConstants.I18N_Note)
                , ref htmlBodyError, MyTNBFont.FONTNAME_300, (float)GetScaledHeight(12F));
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.GreyishBrownTwo
            }, new NSRange(0, htmlBody.Length));
            _txtViewNote = new UITextView(new CGRect(0, _errorLabel.Frame.GetMaxY() + GetScaledHeight(24f)
                , _noteView.Frame.Width, GetScaledHeight(40f)))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = true,
                AttributedText = mutableHTMLBody,
                UserInteractionEnabled = false,
                ContentInset = new UIEdgeInsets(0, -5, 0, -5)
            };
            _txtViewNote.ScrollIndicatorInsets = UIEdgeInsets.Zero;
            _noteView.AddSubview(_txtViewNote);

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
                nfloat newHeight = _errorLabel.GetLabelHeight(1000);
                _errorLabel.Frame = new CGRect(_errorLabel.Frame.Location, new CGSize(_errorLabel.Frame.Width, newHeight));
                ViewHelper.AdjustFrameSetY(_txtViewNote, _errorLabel.Frame.GetMaxY() + GetScaledHeight(24f));
            }
            else
            {
                ViewHelper.AdjustFrameSetY(_txtViewNote, 0);
            }
            ViewHelper.AdjustFrameSetHeight(_noteView, _txtViewNote.Frame.GetMaxY() + _paddingY);

            _meterReadScrollView.ContentSize = new CGSize(ViewWidth, _noteView.Frame.GetMaxY() + _paddingY);
            scrollViewFrame = _meterReadScrollView.Frame;

            if (isError)
            {
                if (ViewHeight - GetScaledHeight(80.0f) < _noteView.Frame.GetMaxY() + NavigationController?.NavigationBar?.Frame.Height)
                {
                    CGPoint point = new CGPoint(0, _meterReadScrollView.ContentSize.Height - _meterReadScrollView.Bounds.Size.Height + _meterReadScrollView.ContentInset.Bottom);
                    _meterReadScrollView.SetContentOffset(point, true);
                }
            }
        }

        private void PrepareManualInputView()
        {
            bool ocrIsDown = IsFromDashboard ? SSMRActivityInfoCache.DB_IsOCRDown : SSMRActivityInfoCache.RH_IsOCRDown;
            bool ocrIsDisabled = IsFromDashboard ? SSMRActivityInfoCache.DB_IsOCRDisabled : SSMRActivityInfoCache.RH_IsOCRDisabled;
            if (!AppLaunchMasterCache.IsOCRDown && !ocrIsDown && !ocrIsDisabled)
                return;

            UIColor bgColor = UIColor.Clear;
            UIColor descriptionColor = MyTNBColor.WaterBlue;
            string descriptionKey = SSMRConstants.I18N_ManualInputTitle;
            string fontSize = MyTNBFont.FONTNAME_300;
            nfloat lessHeight = 0;
            if (ocrIsDisabled)
            {
                descriptionColor = MyTNBColor.WaterBlue;
                fontSize = MyTNBFont.FONTNAME_500;
                bgColor = UIColor.Clear;
                lessHeight = -GetScaledHeight(16F);
            }
            else if (ocrIsDown)
            {
                descriptionColor = MyTNBColor.CharcoalGrey;
                descriptionKey = SSMRConstants.I18N_OCRDownMessage;
                fontSize = MyTNBFont.FONTNAME_300;
                bgColor = UIColor.White;
                lessHeight = 0;
            }

            _manualInputView = new UIView(new CGRect(0, 0, View.Frame.Width, 0))
            {
                BackgroundColor = bgColor,
                Tag = 3
            };

            if (ocrIsDown)
            {
                _manualInputView.Layer.CornerRadius = 5f;
                _manualInputView.Layer.MasksToBounds = false;
                _manualInputView.Layer.ShadowColor = MyTNBColor.SilverChalice10.CGColor;
                _manualInputView.Layer.ShadowOpacity = 0.5f;
                _manualInputView.Layer.ShadowOffset = new CGSize(0, 1);
                _manualInputView.Layer.ShadowRadius = 8;
                _manualInputView.Layer.ShadowPath = UIBezierPath.FromRect(_manualInputView.Bounds).CGPath;
            }

            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(GetI18NValue(descriptionKey)
                , ref htmlBodyError, fontSize, (float)GetScaledHeight(14));
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = descriptionColor
            }, new NSRange(0, htmlBody.Length));
            UITextView description = new UITextView(new CGRect(BaseMarginWidth16, GetScaledHeight(16), _manualInputView.Frame.Width - (BaseMarginWidth16 * 2), 0))
            {
                Editable = false,
                ScrollEnabled = true,
                AttributedText = mutableHTMLBody,
                UserInteractionEnabled = false,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Left,
                ContentInset = new UIEdgeInsets(0, -5, 0, -5),
                TextContainerInset = UIEdgeInsets.Zero
            };
            CGSize size = description.SizeThatFits(new CGSize(description.Frame.Width, 1000F));

            ViewHelper.AdjustFrameSetHeight(description, size.Height);
            ViewHelper.AdjustFrameSetHeight(_manualInputView, description.Frame.Height + (GetScaledHeight(16) * 2) + lessHeight);

            _manualInputView.AddSubview(description);
            _meterReadScrollView.AddSubview(_manualInputView);
        }

        private void PrepareTakePhotoHeaderView()
        {
            bool ocrIsDown = IsFromDashboard ? SSMRActivityInfoCache.DB_IsOCRDown : SSMRActivityInfoCache.RH_IsOCRDown;
            bool ocrIsDisabled = IsFromDashboard ? SSMRActivityInfoCache.DB_IsOCRDisabled : SSMRActivityInfoCache.RH_IsOCRDisabled;
            if (AppLaunchMasterCache.IsOCRDown || ocrIsDown || ocrIsDisabled)
                return;

            nfloat takePhotoViewHeight = View.Frame.Width * takePhotoViewRatio;
            nfloat descLabelWidth = _meterReadScrollView.Frame.Width - (_paddingX * 2);
            _takePhotoView = new UIView(new CGRect(0, 0, View.Frame.Width, takePhotoViewHeight))
            {
                BackgroundColor = UIColor.White,
                Tag = 1
            };

            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(GetI18NValue(_previousMeterList != null
                && _previousMeterList.Count > 1 ? SSMRConstants.I18N_HeaderDesc : SSMRConstants.I18N_SingleHeaderDesc)
                , ref htmlBodyError, MyTNBFont.FONTNAME_300, (float)GetScaledHeight(14F));
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.CharcoalGrey
            }, new NSRange(0, htmlBody.Length));
            UITextView description = new UITextView(new CGRect(_paddingX, _paddingY, descLabelWidth, GetScaledHeight(44)))
            {
                Editable = false,
                ScrollEnabled = true,
                AttributedText = mutableHTMLBody,
                UserInteractionEnabled = false
            };
            description.ScrollIndicatorInsets = UIEdgeInsets.Zero;
            CGSize size = description.SizeThatFits(new CGSize(description.Frame.Width, 1000F));
            description.Frame = new CGRect(description.Frame.Location, new CGSize(description.Frame.Width, size.Height));

            _takePhotoView.AddSubview(description);

            nfloat btnViewWidth = _meterReadScrollView.Frame.Width - (_paddingX * 2);
            _takePhotoBtnView = new UIView(new CGRect(_paddingX, description.Frame.GetMaxY() + GetScaledHeight(12f)
                , btnViewWidth, GetScaledHeight(48f)))
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
                Image = UIImage.FromBundle(SSMRConstants.IMG_CameraIcon)
            };
            containerView.AddSubview(_cameraIconView);

            nfloat takePhotoLabelWidth = GetScaledWidth(166f);
            _takePhotoLabel = new UILabel(new CGRect(_cameraIconView.Frame.GetMaxX() + GetScaledWidth(12f), 0, takePhotoLabelWidth, 44f))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.AlgaeGreen,
                Lines = 0,
                TextAlignment = UITextAlignment.Left,
                Text = GetI18NValue(SSMRConstants.I18N_TakeOrUploadPhoto)
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
            if (_toolTipParentView == null)
            {
                UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
                nfloat height = currentWindow.Frame.Height;
                _toolTipParentView = new UIView(new CGRect(0, 0, ViewWidth, height))
                {
                    BackgroundColor = MyTNBColor.Black60
                };
                currentWindow.AddSubview(_toolTipParentView);
                PaginatedTooltipComponent tooltipComponent = new PaginatedTooltipComponent(_toolTipParentView);
                tooltipComponent.GetI18NValue = GetI18NValue;
                tooltipComponent.SetSSMRData(pageData, pageDefaultData);
                tooltipComponent.SetPreviousMeterData(_previousMeterList);
                _toolTipParentView.AddSubview(tooltipComponent.GetSSMRTooltip());
                tooltipComponent.SetGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    MakeToolTipVisible(false);
                }));
            }
            _toolTipParentView.Hidden = false;
        }

        private void MakeToolTipVisible(bool isVisible)
        {
            if (_toolTipParentView != null)
            {
                _toolTipParentView.Hidden = !isVisible;
            }
        }

        private void PrepareMeterReadingCard(bool fromOCR, bool isBusinessError = false)
        {
            if (_previousMeterList != null)
            {
                bool ocrIsDown = IsFromDashboard ? SSMRActivityInfoCache.DB_IsOCRDown : SSMRActivityInfoCache.RH_IsOCRDown;
                bool ocrIsDisabled = IsFromDashboard ? SSMRActivityInfoCache.DB_IsOCRDisabled : SSMRActivityInfoCache.RH_IsOCRDisabled;
                bool hasOCRError = false;
                string errorMessage = string.Empty;
                nfloat yPos = (AppLaunchMasterCache.IsOCRDown || ocrIsDown || ocrIsDisabled) ? _manualInputView.Frame.GetMaxY() + GetScaledHeight(16F) : _takePhotoView.Frame.GetMaxY() + _paddingY;
                manualInputCardYPos = yPos;
                _meterCardComponentList.Clear();
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
                    _meterCardComponentList.Add(sSMRMeterCardComponent);
                }
                lastCardYPos = yPos;
                if (fromOCR)
                {
                    ShowGeneralInlineError(hasOCRError, errorMessage);
                }
                else
                {
                    ShowGeneralInlineError(false, string.Empty);
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
            _sSMRMeterFooterComponent._submitBtn.SetTitle(GetI18NValue(SSMRConstants.I18N_SubmitReading), UIControlState.Normal);
            _sSMRMeterFooterComponent._submitBtn.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                OnTapSubmitReading();
            }));
        }

        private void OnTapTakePhoto()
        {
            if (_isKeyboardActive && _meterCardComponentList != null)
            {
                for (int i = 0; i < _meterCardComponentList.Count; i++)
                {
                    SSMRMeterCardComponent item = _meterCardComponentList[i];
                    if (item != null && item.IsActive)
                    {
                        item.ValidateTextField();
                        item.IsActive = false;
                    }
                }
            }
            Dictionary<string, bool> ReadingDictionary = new Dictionary<string, bool>();
            if (_previousMeterList != null)
            {
                foreach (var previousMeter in _previousMeterList)
                {
                    bool needsPhoto = string.IsNullOrEmpty(previousMeter.CurrentReading) || !previousMeter.IsValidManualReading;
                    ReadingDictionary.Add(previousMeter.ReadingUnitDisplayTitle, !needsPhoto);
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
                                if (previousMeter.ReadingUnit.ToUpper() == ocr.OCRUnit.ToUpper())
                                {
                                    previousMeter.IsErrorFromOCR = !ocr.IsSuccess;
                                    previousMeter.CurrentReading = ocr.OCRValue;
                                }
                            }
                            else
                            {
                                if (previousMeter.ReadingUnit.ToUpper() == ocr.RequestReadingUnit.ToUpper())
                                {
                                    previousMeter.IsErrorFromOCR = !ocr.IsSuccess;
                                    previousMeter.ErrorMessage = ocr.Message;
                                }
                            }
                        }
                    }
                }
                UpdateUIForReadings(true);
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
                            if (previousMeter.ReadingUnit.ToUpper() == response.ReadingUnit.ToUpper())
                            {
                                previousMeter.IsValidManualReading = response.IsSuccess;
                                previousMeter.ErrorMessage = response.Message;
                            }
                        }
                    }
                }
                UpdateUIForReadings(false, true);
            }
        }

        private void UpdateUIForReadings(bool fromOCR, bool isBusinessError = false)
        {
            ClearScrollViewSubViews();
            PrepareMeterReadingCard(fromOCR, isBusinessError);
        }

        private void ClearScrollViewSubViews()
        {
            var subviews = _meterReadScrollView.Subviews;
            foreach (var view in subviews)
            {
                if (view != null & view.Tag != 1 & view.Tag != 2 & view.Tag != 3)
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
                        await CallSubmitSMRMeterReading(IsFromDashboard
                            ? SSMRActivityInfoCache.DashboardAccount : SSMRActivityInfoCache.ViewHistoryAccount
                            , meterReadingRequestList);
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

            _isKeyboardActive = notification.Name == UIKeyboard.WillShowNotification;
            UIView.BeginAnimations("AnimateForKeyboard");
            UIView.SetAnimationBeginsFromCurrentState(true);
            UIView.SetAnimationDuration(UIKeyboard.AnimationDurationFromNotification(notification));
            UIView.SetAnimationCurve((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification(notification));

            if (_isKeyboardActive)
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
                            SSMRActivityInfoCache.SubmittedAccount = account;
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
            viewController.FromStatusPage = true;
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
    }
}