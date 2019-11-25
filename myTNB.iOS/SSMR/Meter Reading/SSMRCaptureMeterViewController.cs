using CoreGraphics;
using Foundation;
using myTNB.SSMR;
using System;
using System.Diagnostics;
using UIKit;
using AVFoundation;
using myTNB.SSMR.MeterReading;
using System.IO;
using Photos;
using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB.Model;
using System.Linq;

namespace myTNB
{
    public partial class SSMRCaptureMeterViewController : CustomUIViewController
    {
        public SSMRCaptureMeterViewController(IntPtr handle) : base(handle)
        {
        }

        public Dictionary<string, bool> ReadingDictionary;

        private UILabel _lblDescription;
        private UIView _viewPreview, _viewCamera, _viewCapture;
        private UIView _viewPreviewOne, _viewPreviewTwo, _viewPreviewThree;
        private UIView _viewCameraActions, _viewMainPreviewParent
            , _viewGallery, _viewOverlay, _viewLoading;
        private UIImageView _imgViewMainPreview;
        private CustomUISlider _zoomSlider;
        private UIButton _btnSubmit, _btnDelete;
        private UIImage _capturedImage;
        private UIBarButtonItem _btnInfo;

        private List<string> _doneList;
        private List<string> _ontoList;

        private AVCaptureSession _captureSession;
        private AVCaptureDevice _captureDevice;
        private AVCaptureDeviceInput _input;
        private AVCapturePhotoOutput _output;

        private bool _isMultiPhase;
        private List<ImageModel> _imageModelList;
        private nint _currentTag;
        private int _expectedReadingCount;

        private bool _isGalleryTooltipDisplayed;
        private string _multiPhaseDescription;

        public Action OCRReadingDone;

        private class ImageModel
        {
            public bool NeedsPhoto { set; get; }
            public UIImage Image { set; get; }
            public string ReadingUnit { set; get; }
            public int Tag { set; get; }
        }

        public override void ViewDidLoad()
        {
            PageName = SSMRConstants.Pagename_SSMRCaptureMeter;
            base.ViewDidLoad();
            EvaluateReadingList();
            _isMultiPhase = ReadingDictionary != null && ReadingDictionary.Count > 1;
            SetImageList();
            SetPreview();
            SetDescription();
            SetCamera();
            ToggleCTA();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            OCRReadingCache.ClearReadings();
            if (_viewLoading != null) { _viewLoading.Hidden = true; }
            SetupLiveCameraStream();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (PHPhotoLibrary.AuthorizationStatus != PHAuthorizationStatus.Authorized)
            {
                PHPhotoLibrary.RequestAuthorization((status) =>
                {
                    if (status == PHAuthorizationStatus.Authorized) { UpdateViewGallery(); }
                });
            }
            else
            {
                UpdateViewGallery();
            }
            if (!SSMRActivityInfoCache.IsPhotoToolTipDisplayed)
            {
                DisplayTooltip();
                SSMRActivityInfoCache.IsPhotoToolTipDisplayed = true;
            }
        }

        public override void ConfigureNavigationBar()
        {
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle(SSMRConstants.IMG_BackIcon)
                , UIBarButtonItemStyle.Done, (sender, e) =>
            {
                NavigationController.PopViewController(true);
            });
            _btnInfo = new UIBarButtonItem(UIImage.FromBundle(SSMRConstants.IMG_Info)
               , UIBarButtonItemStyle.Done, (sender, e) =>
               {
                   DisplayTooltip();
               });
            NavigationItem.LeftBarButtonItem = btnBack;
            NavigationItem.RightBarButtonItem = _btnInfo;
            Title = GetI18NValue(SSMRConstants.I18N_NavTitleTakePhoto);
        }

        private void DisplayTooltip(bool isGallery = false, Action action = null)
        {
            string type;
            string image = _isMultiPhase ? SSMRConstants.IMG_MultiPhase : SSMRConstants.IMG_SinglePhase;
            int ontoCount = _ontoList.Count;
            if (isGallery)
            {
                type = _isMultiPhase ? ontoCount > 1 ? SSMRConstants.Tooltips_MultiPhaseGallery
                    : SSMRConstants.Tooltips_MultiPhaseOneMissingGallery : SSMRConstants.Tooltips_SinglePhaseGallery;
            }
            else
            {
                type = _isMultiPhase ? ontoCount > 1 ? SSMRConstants.Tooltips_MultiPhaseTakePhoto
                    : SSMRConstants.Tooltips_MultiPhaseOneMissingTakePhoto : SSMRConstants.Tooltips_SinglePhaseTakePhoto;
            }
            PopupModel popupData = SSMRActivityInfoCache.GetPopupDetailsByType(type);
            if (popupData != null)
            {
                string description = popupData.Description;
                if (_isMultiPhase)
                {
                    string missingReading = string.Empty;
                    for (int i = 0; i < ontoCount; i++)
                    {
                        missingReading += _ontoList[i];
                        if (i != ontoCount - 1) { missingReading += ", "; }
                    }
                    description = ontoCount > 1 ? string.Format(description, ontoCount, missingReading) : string.Format(description, missingReading);
                }
                DisplayCustomAlert(popupData.Title, description, new Dictionary<string, Action> { { popupData.CTA, action } }, UIImage.FromBundle(image));
            }
            else
            {
                if (action != null) { action.Invoke(); }
            }
        }

        private void SetImageList()
        {
            _imageModelList = new List<ImageModel>();
            List<string> keys = ReadingDictionary.Keys.ToList();
            _doneList = new List<string>();
            _ontoList = new List<string>();
            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                if (ReadingDictionary[key]) { _doneList.Add(key); }
                else { _ontoList.Add(key); }
                if (ReadingDictionary[key]) { continue; }
                ImageModel imgModel = new ImageModel
                {
                    NeedsPhoto = !ReadingDictionary[key],
                    ReadingUnit = key,
                    Tag = 1001 + i
                };
                _imageModelList.Add(imgModel);
            }
            _multiPhaseDescription = string.Format(GetI18NValue(SSMRConstants.I18N_MultiTakePhotoDescription), _ontoList.Count);
            if (_isMultiPhase && _doneList.Count > 0)
            {
                string done = _doneList.Count > 1 ? string.Format(GetI18NValue(SSMRConstants.I18N_PluralDone), _doneList[0], _doneList[1])
                    : string.Format(GetI18NValue(SSMRConstants.I18N_SingularDone), _doneList[0]);
                string onto = _ontoList.Count > 1 ? string.Format(GetI18NValue(SSMRConstants.I18N_PluralOnto), _ontoList[0], _ontoList[1])
                    : string.Format(GetI18NValue(SSMRConstants.I18N_SingularOnto), _ontoList[0]);
                _multiPhaseDescription = string.Format("{0} {1}", done, onto);
            }
        }

        private void EvaluateReadingList()
        {
            bool hasSameValue = ReadingDictionary.Values.Distinct().Count() == 1;
            bool firstValue = ReadingDictionary.Values.First();
            if (firstValue && hasSameValue)
            {
                List<string> keys = ReadingDictionary.Keys.ToList();
                for (int i = 0; i < keys.Count; i++)
                { ReadingDictionary[keys[i]] = false; }
            }
        }

        #region Description
        private void SetDescription(string description = "")
        {
            if (string.IsNullOrEmpty(description) || string.IsNullOrWhiteSpace(description))
            {
                description = _isMultiPhase ? _multiPhaseDescription : GetI18NValue(SSMRConstants.I18N_SingleTakePhotoDescription);
            }

            NSMutableAttributedString nsDescription = new NSMutableAttributedString(description);
            NSMutableParagraphStyle msgParagraphStyle = new NSMutableParagraphStyle
            {
                Alignment = UITextAlignment.Center,
                LineBreakMode = UILineBreakMode.WordWrap,
                MinimumLineHeight = 19,
                MaximumLineHeight = 19
            };

            UIStringAttributes msgAttributes = new UIStringAttributes
            {
                Font = TNBFont.MuseoSans_14_300,
                ForegroundColor = UIColor.White,
                BackgroundColor = UIColor.Clear,
                ParagraphStyle = msgParagraphStyle
            };
            nsDescription.AddAttributes(msgAttributes, new NSRange(0, description.Length));
            if (_isMultiPhase) { EvaluateDescription(ref nsDescription); }
            if (_lblDescription == null)
            {
                nfloat baseHeight = (ViewHeight - _viewPreview.Frame.Height) * 0.28F;
                _lblDescription = new UILabel(new CGRect(BaseMargin
                    , (baseHeight - GetScaledHeight(38)) / 2, BaseMarginedWidth, GetScaledHeight(38)))
                {
                    TextAlignment = UITextAlignment.Center,
                    BackgroundColor = UIColor.Clear,
                    LineBreakMode = UILineBreakMode.WordWrap,
                    Lines = 0,
                    TextColor = UIColor.White,
                    Font = TNBFont.MuseoSans_14_300
                };
                View.AddSubview(_lblDescription);
                _lblDescription.Layer.ZPosition = 99;
            }
            _lblDescription.AttributedText = nsDescription;
        }

        private void EvaluateDescription(ref NSMutableAttributedString nsDescription)
        {
            string description = nsDescription.ToString();
            NSMutableParagraphStyle msgParagraphStyle = new NSMutableParagraphStyle
            {
                Alignment = UITextAlignment.Center,
                LineBreakMode = UILineBreakMode.WordWrap,
                MinimumLineHeight = GetScaledHeight(19),
                MaximumLineHeight = GetScaledHeight(19)
            };

            UIStringAttributes msgAttributesDone = new UIStringAttributes
            {
                Font = TNBFont.MuseoSans_14_300,
                ForegroundColor = MyTNBColor.AlgaeGreen,
                BackgroundColor = UIColor.Clear,
                ParagraphStyle = msgParagraphStyle
            };

            UIStringAttributes msgAttributesOnto = new UIStringAttributes
            {
                Font = TNBFont.MuseoSans_14_300,
                ForegroundColor = MyTNBColor.ButterScotch,
                BackgroundColor = UIColor.Clear,
                ParagraphStyle = msgParagraphStyle
            };
            for (int i = 0; i < _doneList.Count; i++)
            {
                int index = -1;
                int whileCount = 0;
                string[] doneArr = { string.Format(" {0} ", _doneList[i]), string.Format(" {0}!", _doneList[i]) };
                while (index == -1 && whileCount < doneArr.Length)
                {
                    index = description.ToLower().IndexOf(doneArr[whileCount].ToLower());
                    if (index > -1) { break; }
                    whileCount++;
                }
                if (index < 0) { continue; }
                nsDescription.AddAttributes(msgAttributesDone, new NSRange(index, doneArr[whileCount].Length - (whileCount == 1 ? 1 : 0)));
            }

            for (int i = 0; i < _ontoList.Count; i++)
            {
                int index = -1;
                int whileCount = 0;
                string[] ontoArr = { string.Format(" {0} ", _ontoList[i]), string.Format(" {0}.", _ontoList[i]) };
                while (index == -1 && whileCount < ontoArr.Length)
                {
                    index = description.ToLower().IndexOf(ontoArr[whileCount].ToLower());
                    if (index > -1) { break; }
                    whileCount++;
                }
                if (index < 0) { continue; }
                nsDescription.AddAttributes(msgAttributesOnto, new NSRange(index, ontoArr[whileCount].Length - (whileCount == 1 ? 1 : 0)));
            }
        }
        #endregion

        private nfloat GetPreviewXLoc(int count)
        {
            if (count == 1)
            {
                return GetScaledWidth(132);
            }
            else if (count == 2)
            {
                return GetScaledWidth(88);
            }
            else
            {
                return GetScaledWidth(44);
            }
        }

        private void SetPreview()
        {
            nfloat previewWidth = GetScaledWidth(56);
            _viewPreview = new UIView() { BackgroundColor = UIColor.White };
            nfloat topMargin = GetScaledHeight(16);
            nfloat xMargin = GetScaledWidth(32);

            if (_isMultiPhase)
            {
                _expectedReadingCount = _imageModelList.Count;
                for (int i = 0; i < _expectedReadingCount; i++)
                {
                    ImageModel item = _imageModelList[i];
                    if (i == 0)
                    {
                        _viewPreviewOne = CreatePhotoPreview(new CGRect(GetPreviewXLoc(_expectedReadingCount)
                            , topMargin, previewWidth, previewWidth), i);
                        _viewPreviewOne.Tag = item.Tag;
                        _viewPreviewOne.AddGestureRecognizer(new UITapGestureRecognizer(() => { PreviewAction(_viewPreviewOne.Tag); }));
                        _viewPreview.AddSubview(_viewPreviewOne);
                    }
                    else if (i == 1)
                    {
                        _viewPreviewTwo = CreatePhotoPreview(new CGRect(_viewPreviewOne.Frame.GetMaxX() + xMargin, topMargin, previewWidth, previewWidth), i);
                        _viewPreviewTwo.Tag = item.Tag;
                        _viewPreviewTwo.AddGestureRecognizer(new UITapGestureRecognizer(() => { PreviewAction(_viewPreviewTwo.Tag); }));
                        _viewPreview.AddSubview(_viewPreviewTwo);
                    }
                    else
                    {
                        _viewPreviewThree = CreatePhotoPreview(new CGRect(_viewPreviewTwo.Frame.GetMaxX() + xMargin, topMargin, previewWidth, previewWidth), i);
                        _viewPreviewThree.Tag = item.Tag;
                        _viewPreviewThree.AddGestureRecognizer(new UITapGestureRecognizer(() => { PreviewAction(_viewPreviewThree.Tag); }));
                        _viewPreview.AddSubview(_viewPreviewThree);
                    }
                }
            }

            nfloat btnYLoc = (_isMultiPhase ? _viewPreviewOne.Frame.GetMaxY() : 0) + GetScaledHeight(16);
            _btnDelete = new CustomUIButtonV2
            {
                Frame = new CGRect(16, btnYLoc, ViewWidth - 32, GetScaledHeight(48)),
                BackgroundColor = UIColor.Clear,
                Tag = 1004,
                Font = TNBFont.MuseoSans_16_500,
                PageName = PageName,
                EventName = SSMRConstants.EVENT_DeleteImage,
                Hidden = true
            };
            _btnDelete.SetTitle(GetI18NValue(SSMRConstants.I18N_DeletePhoto), UIControlState.Normal);
            _btnDelete.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
            _btnDelete.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            _btnDelete.Layer.BorderWidth = GetScaledWidth(1);
            _btnDelete.AddGestureRecognizer(new UITapGestureRecognizer(() => { DeleteImage(); }));

            _btnSubmit = new CustomUIButtonV2()
            {
                Frame = new CGRect(16, btnYLoc, ViewWidth - 32, GetScaledHeight(48)),
                BackgroundColor = MyTNBColor.FreshGreen,
                Tag = 1005,
                Font = TNBFont.MuseoSans_16_500,
                PageName = PageName,
                EventName = SSMRConstants.EVENT_SubmitToOCR
            };
            _btnSubmit.SetTitle(GetCommonI18NValue(SSMRConstants.I18N_Submit), UIControlState.Normal);
            _btnSubmit.AddGestureRecognizer(new UITapGestureRecognizer(() => { OnSubmit(); }));

            _viewPreview.AddSubviews(new UIView[] { _btnDelete, _btnSubmit });
            nfloat containerHeight = _btnSubmit.Frame.GetMaxY() + (DeviceHelper.IsIphoneXUpResolution() ? 36 : 16);
            _viewPreview.Frame = new CGRect(0, ViewHeight - containerHeight, ViewWidth, containerHeight);
            View.AddSubview(_viewPreview);
        }

        private void IsDeleteHidden(bool isHidden)
        {
            nfloat btnYLoc = (_isMultiPhase ? _viewPreviewOne.Frame.GetMaxY() : 0) + GetScaledHeight(16);
            _btnDelete.Hidden = isHidden;
            CGRect submitFrame = _btnSubmit.Frame;
            submitFrame.Y = isHidden ? btnYLoc : GetYLocationFromFrame(_btnDelete.Frame, 16);
            _btnSubmit.Frame = submitFrame;

            nfloat containerHeight = _btnSubmit.Frame.GetMaxY() + (DeviceHelper.IsIphoneXUpResolution() ? 36 : 16);
            _viewPreview.Frame = new CGRect(0, ViewHeight - containerHeight, ViewWidth, containerHeight);
            View.BringSubviewToFront(_viewPreview);
        }

        private void DeleteImage()
        {
            Debug.WriteLine("Delete tapped");
            RemovePreview();
            UIView viewPreview = _viewPreview.ViewWithTag(_currentTag);
            if (viewPreview != null)
            {
                UIImageView imgView = viewPreview.ViewWithTag(99) as UIImageView;
                if (imgView != null && !imgView.Hidden)
                {
                    imgView.Image = null;
                    imgView.Hidden = true;
                    int index = _imageModelList.FindIndex(x => x.Tag == _currentTag);
                    if (index > -1)
                    {
                        ImageModel model = _imageModelList[index];
                        model.NeedsPhoto = true;
                        model.Image = null;
                        _imageModelList[index] = model;
                    }
                }
            }
            PreviewAction(_currentTag);
            ToggleCTA();
            int hasImgIndex = _imageModelList.FindIndex(x => x.Image != null);
            if (hasImgIndex < 0)
            {
                string mDescription = hasImgIndex < 0 ? _multiPhaseDescription : GetI18NValue(SSMRConstants.I18N_MultiTakeNextPhotoDescription);
                SetDescription(_isMultiPhase ? mDescription : GetI18NValue(SSMRConstants.I18N_SingleTakePhotoDescription));
            }
        }

        private bool IsPreviosPreviewHasImage(int index, out int noImageIndex)
        {
            noImageIndex = index;
            for (int i = 0; i < index; i++)
            {
                ImageModel data = _imageModelList[i];
                if (data.Image == null)
                {
                    noImageIndex = i;
                    return false;
                }
            }
            return true;
        }

        private void SetPreviewColors(UIView view, bool isSelected, bool hasImg)
        {
            if (view != null)
            {
                UILabel currentLbl = view.ViewWithTag(98) as UILabel;
                if (currentLbl == null) { currentLbl = new UILabel(); }
                if (isSelected)
                { view.Layer.BorderColor = MyTNBColor.WaterBlue.CGColor; }
                else
                { view.Layer.BorderColor = hasImg ? MyTNBColor.FreshGreen.CGColor : MyTNBColor.WhiteThree.CGColor; }
                currentLbl.TextColor = isSelected ? MyTNBColor.WaterBlue : MyTNBColor.GreyishBrown;
            }
        }

        private void PreviewAction(nint tag)
        {
            _currentTag = tag;
            int count = _imageModelList.Count;
            for (int i = 0; i < count; i++)
            {
                ImageModel data = _imageModelList[i];
                UIView currentView = _viewPreview.ViewWithTag(data.Tag);
                bool isSameTag = data.Tag == tag;
                if (currentView != null)
                {
                    SetPreviewColors(currentView, false, false);
                    if (data.Image != null)
                    { SetPreviewColors(currentView, false, true); }
                    if (isSameTag)
                    {
                        SetPreviewColors(currentView, true, data.Image != null);
                        if (data.Image == null)
                        {
                            RemovePreview();
                        }
                        else
                        {
                            AddMainPreview(data.Image);
                        }
                    }

                    if (isSameTag)
                    {
                        bool isPrevHasImg = IsPreviosPreviewHasImage(i, out int noImageIndex);
                        bool isCurHasImg = data.Image != null;
                        if (isPrevHasImg)
                        { SetPreviewColors(currentView, true, isPrevHasImg); }
                        else
                        {
                            if (!isCurHasImg)
                            {
                                _currentTag = _imageModelList[noImageIndex].Tag;
                                UIView pView = _viewPreview.ViewWithTag(_currentTag);
                                SetPreviewColors(currentView, false, false);
                                SetPreviewColors(pView, true, false);
                            }
                        }

                        int index = _imageModelList.FindIndex(x => x.NeedsPhoto);
                        string mDescription = index > -1 ? GetI18NValue(SSMRConstants.I18N_MultiTakeNextPhotoDescription) : _multiPhaseDescription;
                        SetDescription(data.Image == null ? mDescription : GetI18NValue(SSMRConstants.I18N_EditDescription));
                    }
                }
            }
        }

        private void RemovePreview()
        {
            if (_viewMainPreviewParent != null)
            {
                _viewMainPreviewParent.Hidden = true;
                _viewMainPreviewParent.RemoveFromSuperview();
                _imgViewMainPreview.Image = null;
                IsDeleteHidden(true);
                _viewCameraActions.Hidden = false;
                _viewCamera.SendSubviewToBack(_viewOverlay);
                _capturedImage = null;
            }
        }

        private UIView CreatePhotoPreview(CGRect frame, int index)
        {
            index++;
            UIView view = new UIView(frame) { ClipsToBounds = true };
            view.Layer.CornerRadius = 4.0F;
            view.Layer.BorderWidth = 2.0F;
            view.Layer.BorderColor = (index == 1 ? MyTNBColor.WaterBlue : MyTNBColor.WhiteThree).CGColor;
            nfloat lblHeight = GetScaledHeight(24);
            UILabel label = new UILabel(new CGRect(0, (frame.Height - lblHeight) / 2, frame.Width, lblHeight))
            {
                TextAlignment = UITextAlignment.Center,
                Font = TNBFont.MuseoSans_16_500,
                TextColor = index == 1 ? MyTNBColor.WaterBlue : MyTNBColor.GreyishBrown,
                Text = index.ToString(),
                Tag = 98
            };
            UIImageView imgView = new UIImageView(new CGRect(new CGPoint(0, 0), frame.Size))
            {
                Hidden = true,
                Tag = 99,
                ContentMode = UIViewContentMode.ScaleAspectFill
            };
            view.AddSubviews(new UIView[] { label, imgView });
            return view;
        }

        private void SetCamera()
        {
            _viewCamera = new UIView(new CGRect(0, 0, ViewWidth, ViewHeight - _viewPreview.Frame.Height))
            { ClipsToBounds = true };
            _viewCameraActions = GetCameraActions(_viewCamera);
            _viewOverlay = GetOverlay(_viewCamera);
            _viewCamera.AddSubviews(new UIView[] { _viewOverlay, _viewCameraActions });
            View.AddSubview(_viewCamera);
        }

        #region Box Overlay
        private UIView GetOverlay(UIView viewBase)
        {
            nfloat baseHeight = viewBase.Frame.Height;
            UIView view = new UIView(new CGRect(new CGPoint(0, 0), viewBase.Frame.Size))
            { BackgroundColor = UIColor.Clear, UserInteractionEnabled = false };

            UIView viewTop = new UIView(new CGRect(0, 0, ViewWidth, baseHeight * 0.28F))
            { BackgroundColor = UIColor.Black.ColorWithAlpha(0.60F), UserInteractionEnabled = false };

            UIView viewClear = new UIView(new CGRect(-4, viewTop.Frame.GetMaxY(), ViewWidth + 8, baseHeight * 0.38F))
            { BackgroundColor = UIColor.Clear, UserInteractionEnabled = false };
            viewClear.Layer.BorderColor = UIColor.White.CGColor;
            viewClear.Layer.BorderWidth = 4.0F;

            UIView viewBottom = new UIView(new CGRect(0, viewClear.Frame.GetMaxY(), ViewWidth, baseHeight * 0.34F))
            { BackgroundColor = UIColor.Black.ColorWithAlpha(0.60F), UserInteractionEnabled = false };
            view.AddSubviews(new UIView[] { viewTop, viewClear, viewBottom });
            view.BringSubviewToFront(viewClear);
            return view;
        }
        #endregion

        private UIView GetCameraActions(UIView viewBase)
        {
            nfloat gallerySize = GetScaledWidth(32);
            nfloat captureSize = GetScaledWidth(43);
            UIView view = new UIView() { BackgroundColor = UIColor.Clear };

            _zoomSlider = new CustomUISlider(new CGRect(GetScaledWidth(20), 0, ViewWidth - (GetScaledWidth(20) * 2), 3))
            {
                MinimumTrackTintColor = UIColor.White
            };
            _zoomSlider.SetThumbImage(UIImage.FromBundle(SSMRConstants.IMG_CameraThumb), UIControlState.Normal);
            _zoomSlider.ValueChanged += (sender, e) =>
            {
                Debug.WriteLine("zoomSlider ValueChanged: " + ((UISlider)sender).Value);
                if (_captureDevice == null) { return; }
                nfloat zoomFactor = (nfloat)((UISlider)sender).Value;
                if (zoomFactor > _zoomSlider.MaxValue) { zoomFactor = _zoomSlider.MaxValue; }
                if (zoomFactor < _zoomSlider.MinValue) { zoomFactor = _zoomSlider.MinValue; }
                _captureDevice.LockForConfiguration(out NSError nsError);
                _captureDevice.VideoZoomFactor = zoomFactor;
                _captureDevice.UnlockForConfiguration();
            };

            _viewGallery = new UIView(new CGRect(GetScaledWidth(16)
                , GetYLocationFromFrame(_zoomSlider.Frame, 25), gallerySize, gallerySize))
            { ClipsToBounds = true };
            _viewGallery.Layer.CornerRadius = 4.0F;
            _viewGallery.Layer.BorderWidth = 2.0F;
            _viewGallery.Layer.BorderColor = UIColor.White.CGColor;
            _viewGallery.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                Debug.WriteLine("viewGallery tapped");
                if (_isGalleryTooltipDisplayed) { OnShowGallery(); }
                else
                {
                    _isGalleryTooltipDisplayed = true;
                    DisplayTooltip(true, OnShowGallery);
                }
            }));

            _viewCapture = new UIView(new CGRect((ViewWidth - captureSize) / 2
               , GetYLocationFromFrame(_zoomSlider.Frame, 21), captureSize, captureSize))
            { BackgroundColor = UIColor.FromWhiteAlpha(1, 0.80F) };
            _viewCapture.Layer.CornerRadius = captureSize / 2;
            _viewCapture.Layer.BorderWidth = GetScaledWidth(3);
            _viewCapture.Layer.BorderColor = UIColor.White.CGColor;

            view.AddSubviews(new UIView[] { _viewGallery, _viewCapture, _zoomSlider });
            CGRect viewFrame = new CGRect(0, viewBase.Frame.Height - _viewCapture.Frame.GetMaxY() - 16
                , ViewWidth, _viewCapture.Frame.GetMaxY() + 16);
            view.Frame = viewFrame;
            return view;
        }

        private void OnShowGallery()
        {
            ImagePickerDelegate pickerDelegate = new ImagePickerDelegate();
            pickerDelegate.OnDismiss = () => { DismissViewController(true, null); };
            pickerDelegate.OnSelect = (selectedImg) =>
            {
                if (_isMultiPhase)
                {
                    UpdateImagePreview(selectedImg);
                }
                else
                {
                    AddMainPreview(selectedImg);
                    _capturedImage = selectedImg;
                    SetDescription(GetI18NValue(SSMRConstants.I18N_EditDescription));
                }
                ToggleCTA();
            };
            UIImagePickerController imgPicker = new UIImagePickerController
            {
                Delegate = pickerDelegate,
                SourceType = UIImagePickerControllerSourceType.PhotoLibrary
            };
            imgPicker.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            PresentViewController(imgPicker, true, null);
        }

        #region Update Preview
        private bool HasCompleteImgList()
        {
            int index = _imageModelList.FindIndex(x => x.NeedsPhoto);
            return index < 0;
        }

        private void UpdateImagePreview(UIImage image)
        {
            int count = _imageModelList.Count;
            for (int i = 0; i < count; i++)
            {
                ImageModel model = _imageModelList[i];
                if (model.NeedsPhoto)
                {
                    UIView parent = _viewPreview.ViewWithTag(model.Tag);
                    if (parent == null) { continue; }
                    UIImageView imgView = parent.ViewWithTag(99) as UIImageView;
                    if (imgView == null) { continue; }
                    SetPreviewColors(parent, false, true);
                    imgView.Hidden = false;
                    imgView.Image = image;
                    model.Image = image;
                    model.NeedsPhoto = false;

                    _imageModelList[i] = model;

                    if (HasCompleteImgList())
                    {
                        AddMainPreview(image);
                        SetPreviewColors(parent, true, true);
                        _currentTag = model.Tag;
                        SetDescription(GetI18NValue(SSMRConstants.I18N_EditDescription));
                        break;
                    }

                    if (i + 1 < count)
                    {
                        UIView nextParent = _viewPreview.ViewWithTag(_imageModelList[i + 1].Tag);
                        if (nextParent != null)
                        {
                            UIImageView nextImgView = nextParent.ViewWithTag(99) as UIImageView;
                            if (nextImgView != null && nextImgView.Image != null)
                            {
                                SetPreviewColors(nextParent, false, true);
                                if (i + 2 < count && _viewPreview.ViewWithTag(_imageModelList[i + 2].Tag) != null)
                                {
                                    UIView thirdParent = _viewPreview.ViewWithTag(_imageModelList[i + 2].Tag);
                                    UIImageView thirdImgView = thirdParent.ViewWithTag(99) as UIImageView;
                                    if (thirdImgView != null && thirdImgView.Image == null)
                                    {
                                        SetPreviewColors(thirdParent, true, false);
                                        _currentTag = _imageModelList[i + 2].Tag;
                                        break;
                                    }
                                    else
                                    {
                                        SetPreviewColors(thirdParent, false, false);
                                    }
                                }
                            }
                            else
                            {
                                SetPreviewColors(nextParent, true, false);
                            }
                        }
                    }

                    SetDescription(_isMultiPhase ? GetI18NValue(SSMRConstants.I18N_MultiTakeNextPhotoDescription)
                        : GetI18NValue(SSMRConstants.I18N_EditDescription));

                    if (i + 1 == count)
                    {
                        SetPreviewColors(parent, true, false);
                        AddMainPreview(image);
                        SetDescription(GetI18NValue(SSMRConstants.I18N_EditDescription));
                    }
                    _currentTag = model.Tag;
                    break;
                }
            }
        }
        #endregion

        private void DisplayLoadingPage()
        {
            if (_viewLoading == null)
            {
                _viewLoading = new UIView(new CGRect(0, 0, ViewWidth, ViewHeight)) { BackgroundColor = UIColor.White, Hidden = true };
                nfloat imgHeigth = UIScreen.MainScreen.Bounds.Height * 0.255F;
                nfloat imgWidth = imgHeigth * 0.768F;
                UIImageView imgLoading = new UIImageView(new CGRect((ViewWidth - imgWidth) / 2, GetScaledHeight(90), imgWidth, imgHeigth))
                { Image = UIImage.FromBundle(SSMRConstants.IMG_OCRReading), ContentMode = UIViewContentMode.ScaleAspectFill };

                UILabel lblDescription = new UILabel(new CGRect(GetScaledWidth(20)
                    , GetYLocationFromFrame(imgLoading.Frame, 24), ViewWidth - GetScaledWidth(40), GetScaledHeight(48)))
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = TNBFont.MuseoSans_16_300,
                    TextColor = MyTNBColor.BrownGreyThree,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap,
                    Text = GetI18NValue(SSMRConstants.I18N_OCRReading)
                };

                CGSize newSize = GetLabelSize(lblDescription, lblDescription.Frame.Width, 120);
                lblDescription.Frame = new CGRect(lblDescription.Frame.X, lblDescription.Frame.Y, lblDescription.Frame.Width, newSize.Height);

                _viewLoading.AddSubviews(new UIView[] { imgLoading, lblDescription });
                View.AddSubview(_viewLoading);
            }
            _viewLoading.Hidden = false;
            NavigationItem.RightBarButtonItem = null;
        }

        private void UpdateViewGallery()
        {
            InvokeOnMainThread(() =>
            {
                CGSize thumbnailSize = _viewGallery.Frame.Size;
                PHFetchResult phFetchResult = PHAsset.FetchAssets(PHAssetMediaType.Image, null);
                if (phFetchResult == null) { return; }
                PHAsset lastAsset = (PHAsset)phFetchResult.LastObject;
                if (lastAsset == null) { return; }
                PHImageManager phImageManager = new PHImageManager();
                phImageManager.RequestImageForAsset(lastAsset, thumbnailSize
                    , PHImageContentMode.AspectFill, null
                    , (result, info) =>
                    {
                        if (result != null)
                        {
                            UIImageView thumbnailView = new UIImageView(new CGRect(new CGPoint(0, 0), thumbnailSize))
                            {
                                Image = result
                            };
                            _viewGallery.AddSubview(thumbnailView);
                        }
                    });
            });
        }

        public void SetupLiveCameraStream()
        {
            _captureDevice = AVCaptureDevice.GetDefaultDevice(AVMediaTypes.Video);

            if (_captureDevice != null)
            {
                _zoomSlider.MinValue = (float)_captureDevice.MinAvailableVideoZoomFactor;
                _zoomSlider.MaxValue = (float)_captureDevice.MaxAvailableVideoZoomFactor;
            }

            try
            {
                _input = new AVCaptureDeviceInput(_captureDevice, out NSError nsError);

                _output = new AVCapturePhotoOutput
                {
                    IsHighResolutionCaptureEnabled = true
                };

                _captureSession = new AVCaptureSession();
                _captureSession.AddInput(_input);
                _captureSession.AddOutput(_output);

                AVCaptureVideoPreviewLayer videoPreviewLayer = new AVCaptureVideoPreviewLayer(_captureSession)
                {
                    Frame = new CGRect(0, _viewCamera.Frame.Height - UIScreen.MainScreen.Bounds.Height
                        , _viewCamera.Frame.Width, UIScreen.MainScreen.Bounds.Height),
                    VideoGravity = AVLayerVideoGravity.Resize,
                    ZPosition = -1
                };

                _viewCamera.Layer.AddSublayer(videoPreviewLayer);
                _captureSession.StartRunning();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in camera: " + e.Message);
            }

            CapturePhotoDelegate capturePhotoDelegate = new CapturePhotoDelegate();
            capturePhotoDelegate.OnCapturePhoto = OnCapturePhoto;

            _viewCapture.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                Debug.WriteLine("viewCapture tapped");
                if (_output != null)
                {
                    //Settings cannot be reused, should create at every invoke of capture
                    AVCapturePhotoSettings settings = AVCapturePhotoSettings.Create();
                    settings.IsHighResolutionPhotoEnabled = true;
                    settings.IsAutoStillImageStabilizationEnabled = true;
                    settings.FlashMode = AVCaptureFlashMode.Off;

                    _output.CapturePhoto(settings, capturePhotoDelegate);
                }
            }));
        }

        private void OnCapturePhoto(UIImage capturedImage)
        {
            _capturedImage = capturedImage;
            if (_isMultiPhase)
            {
                UpdateImagePreview(capturedImage);
            }
            else
            {
                AddMainPreview(capturedImage);
                SetDescription(GetI18NValue(SSMRConstants.I18N_EditDescription));
            }
            ToggleCTA();
        }

        private void AddMainPreview(UIImage previewImg)
        {
            if (_imgViewMainPreview != null)
            {
                _imgViewMainPreview.RemoveFromSuperview();
                _imgViewMainPreview = null;
            }

            if (_viewMainPreviewParent == null)
            {
                _viewMainPreviewParent = new UIView(new CGRect(0, 0, ViewWidth, _viewCamera.Frame.Height))
                {
                    ClipsToBounds = true,
                    BackgroundColor = MyTNBColor.WhiteThree
                };
            }

            _imgViewMainPreview = new UIImageView(new CGRect(new CGPoint(0, _viewMainPreviewParent.Frame.Height - UIScreen.MainScreen.Bounds.Height)
                , new CGSize(_viewMainPreviewParent.Frame.Width, UIScreen.MainScreen.Bounds.Height)))
            {
                UserInteractionEnabled = true,
                MultipleTouchEnabled = true,
                ClipsToBounds = true,
                Image = previewImg
            };
            _imgViewMainPreview.AddGestureRecognizer(new UIPinchGestureRecognizer((sender) => { PinchZoomAction(sender); }));
            _imgViewMainPreview.AddGestureRecognizer(new UIPanGestureRecognizer((sender) => { PanAction(sender); }));

            _viewMainPreviewParent.AddSubview(_imgViewMainPreview);
            _viewMainPreviewParent.Hidden = false;

            _viewCamera.AddSubview(_viewMainPreviewParent);
            _viewCamera.BringSubviewToFront(_viewOverlay);
            IsDeleteHidden(false);
            _viewCameraActions.Hidden = true;
        }

        private void ToggleCTA()
        {
            bool isValid = false;
            if (_isMultiPhase)
            {
                int count = _imageModelList.Count;
                for (int i = 0; i < count; i++)
                {
                    ImageModel item = _imageModelList[i];
                    isValid = isValid || !item.NeedsPhoto;
                }
            }
            else
            {
                isValid = _capturedImage != null;
            }
            _btnSubmit.Enabled = isValid;
            _btnSubmit.BackgroundColor = isValid ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
        }

        #region Gestures
        private void PinchZoomAction(UIPinchGestureRecognizer sender)
        {
            if (sender != null && sender.View != null
                && (sender.State == UIGestureRecognizerState.Began || sender.State == UIGestureRecognizerState.Changed))
            {
                nfloat currentScale = _imgViewMainPreview.Frame.Size.Width / _imgViewMainPreview.Bounds.Size.Width;
                nfloat newScale = currentScale * sender.Scale;

                if (newScale < 1) { newScale = 1; }
                if (newScale > 5) { newScale = 5; }

                CGAffineTransform transform = CGAffineTransform.MakeScale(newScale, newScale);
                _imgViewMainPreview.Transform = transform;
                sender.Scale = 1;
            }
        }

        private void PanAction(UIPanGestureRecognizer sender)
        {
            if (sender != null && sender.View != null
                && (sender.State == UIGestureRecognizerState.Began || sender.State == UIGestureRecognizerState.Changed))
            {
                nfloat deltaX = _imgViewMainPreview.Frame.Width - _viewMainPreviewParent.Frame.Width;
                nfloat deltaY = _imgViewMainPreview.Frame.Height - _viewMainPreviewParent.Frame.Height;
                CGPoint point = sender.TranslationInView(sender.View);

                if (point.X + _imgViewMainPreview.Frame.X > 0 || Math.Abs(point.X + _imgViewMainPreview.Frame.X) > deltaX)
                { point.X = 0; }
                if (point.Y + _imgViewMainPreview.Frame.Y > 0 || Math.Abs(point.Y + _imgViewMainPreview.Frame.Y) > deltaY)
                { point.Y = 0; }
                sender.View.Transform = CGAffineTransform.Translate(sender.View.Transform, point.X, point.Y);
                sender.SetTranslation(new CGPoint(0, 0), sender.View);
            }
        }
        #endregion

        #region Service
        private string GetImageData(UIImage img, out double fileSize)
        {
            fileSize = 0;
            if (img != null)
            {
                NSData imgData = img.AsJPEG(0.75F);
                if (imgData != null)
                {
                    fileSize = imgData.Length / 1000;
                    string base64 = imgData.GetBase64EncodedString(NSDataBase64EncodingOptions.None);
                    return base64 ?? string.Empty;
                }
            }
            return string.Empty;
        }

        private void OnSubmit()
        {
            DisplayLoadingPage();
            OnSubmitAllImages();
        }

        private void GetMultiPhaseTasks(ref List<Task> taskList)
        {
            for (int i = 0; i < _imageModelList.Count; i++)
            {
                ImageModel model = _imageModelList[i];
                if (model.Image != null)
                {
                    string base64Value = GetImageData(model.Image, out double imgFileSize);
                    string key = model.ReadingUnit;
                    object meterImage = new
                    {
                        RequestReadingUnit = key,
                        ImageId = string.Format(SSMRConstants.Pattern_ImageName, key, i),
                        ImageSize = imgFileSize,
                        ImageData = base64Value
                    };
                    taskList.Add(GetMeterReadingOCRValue(meterImage));
                }
            }
        }

        private void OnSubmitAllImages()
        {
            InvokeInBackground(() =>
            {
                try
                {
                    List<Task> TaskList = new List<Task>();
                    if (_isMultiPhase)
                    {
                        GetMultiPhaseTasks(ref TaskList);
                    }
                    else
                    {
                        string base64Value = GetImageData(_capturedImage, out double imgFileSize);
                        string key = ReadingDictionary.Keys.ToArray()[0];
                        object meterImage = new
                        {
                            RequestReadingUnit = key,
                            ImageId = string.Format(SSMRConstants.Pattern_ImageName, key, 1),
                            ImageSize = imgFileSize,
                            ImageData = base64Value
                        };
                        TaskList.Add(GetMeterReadingOCRValue(meterImage));
                    }
                    Task.WaitAll(TaskList.ToArray());
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error in OnSubmitAllImages: " + e.Message);
                }
                InvokeOnMainThread(() =>
                {
                    if (_viewLoading != null)
                    {
                        _viewLoading.Hidden = true;
                        NavigationItem.RightBarButtonItem = _btnInfo;
                    }
                    if (NavigationController != null)
                    {
                        NavigationController.PopViewController(true);
                        OCRReadingDone?.Invoke();
                    }
                });
            });
        }

        private Task<GetOCRReadingResponseModel> GetMeterReadingOCRValue(object meterImage)
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object request = new
                {
                    serviceManager.usrInf,
                    contractAccount = DataManager.DataManager.SharedInstance.SelectedAccount.accNum,
                    meterImage
                };
                GetOCRReadingResponseModel response = serviceManager
                    .OnExecuteAPIV6<GetOCRReadingResponseModel>(SSMRConstants.Service_GetMeterReadingOCRValue, request);
                OCRReadingCache.AddOCRReading(response);
                return response;
            });
        }
        #endregion
    }
}