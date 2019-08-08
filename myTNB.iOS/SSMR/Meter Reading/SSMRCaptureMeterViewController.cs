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

        /// <summary>
        /// Key as kWh, KVarh or KW and Value as isValid based on textbox validation
        /// </summary>
        public Dictionary<string, bool> ReadingDictionary;

        private UITextView _txtViewDescription;
        private UIView _viewPreview, _viewCamera, _viewCapture;
        private UIView _viewPreviewOne, _viewPreviewTwo, _viewPreviewThree;
        private UIView _viewDelete, _viewCameraActions, _viewMainPreviewParent
            , _viewGallery, _viewOverlay, _viewLoading;
        private UIImageView _imgViewMainPreview;
        private CustomUISlider _zoomSlider;
        private UIButton _btnSubmit;
        private UIImage _capturedImage;
        private UIBarButtonItem _btnInfo;

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
            SetDescription();
            SetPreview();
            SetCamera();
            ToggleCTA();
            DisplayTooltip();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            OCRReadingCache.Instance.ClearReadings();
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
               Debug.WriteLine("Info Tapped");
               DisplayTooltip();
           });
            NavigationItem.LeftBarButtonItem = btnBack;
            NavigationItem.RightBarButtonItem = _btnInfo;
            Title = GetI18NValue(SSMRConstants.I18N_NavTitleTakePhoto);
            HideInfoIcon(false);
        }

        private void HideInfoIcon(bool hidden)
        {
            NavigationItem.RightBarButtonItem = hidden ? null : _btnInfo;
        }

        private void DisplayTooltip(bool isGallery = false, Action action = null)
        {
            string type;
            string image = _isMultiPhase ? SSMRConstants.IMG_MultiPhase : SSMRConstants.IMG_SinglePhase;
            if (isGallery)
            {
                type = _isMultiPhase ? SSMRConstants.Tooltips_MultiPhaseGallery : SSMRConstants.Tooltips_SinglePhaseGallery;
            }
            else
            {
                type = _isMultiPhase ? SSMRConstants.Tooltips_MultiPhaseTakePhoto : SSMRConstants.Tooltips_SinglePhaseTakePhoto;
            }
            PopupModel popupData = SSMRActivityInfoCache.Instance.GetPopupDetailsByType(type);
            if (popupData != null)
            {
                DisplayCustomAlert(popupData.Title, popupData.Description
                    , new Dictionary<string, Action> { { popupData.CTA, action } }, UIImage.FromBundle(image));
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
            List<string> doneList = new List<string>();
            List<string> ontoList = new List<string>();
            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                if (ReadingDictionary[key]) { doneList.Add(key); }
                else { ontoList.Add(key); }
                if (ReadingDictionary[key]) { continue; }
                ImageModel imgModel = new ImageModel
                {
                    NeedsPhoto = !ReadingDictionary[key],
                    ReadingUnit = key,
                    Tag = 1001 + i
                };
                _imageModelList.Add(imgModel);
            }
            _multiPhaseDescription = GetI18NValue(SSMRConstants.I18N_MultiTakePhotoDescription);
            if (_isMultiPhase && doneList.Count > 0)
            {
                string done = doneList.Count > 1 ? string.Format(GetI18NValue(SSMRConstants.I18N_PluralDone), doneList[0], doneList[1])
                    : string.Format(GetI18NValue(SSMRConstants.I18N_SingularDone), doneList[0]);
                string onto = ontoList.Count > 1 ? string.Format(GetI18NValue(SSMRConstants.I18N_PluralOnto), ontoList[0], ontoList[1])
                    : string.Format(GetI18NValue(SSMRConstants.I18N_SingularOnto), ontoList[0]);
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

        private void SetDescription(string description = "")
        {
            if (string.IsNullOrEmpty(description) || string.IsNullOrWhiteSpace(description))
            {
                description = _isMultiPhase ? _multiPhaseDescription : GetI18NValue(SSMRConstants.I18N_SingleTakePhotoDescription);
            }
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(description, ref htmlBodyError, MyTNBFont.FONTNAME_300, 14f);
            UIStringAttributes linkAttributes = new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.WaterBlue,
                Font = MyTNBFont.MuseoSans14_500,
                UnderlineStyle = NSUnderlineStyle.None,
                UnderlineColor = UIColor.Clear
            };
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.CharcoalGrey,
                Font = MyTNBFont.MuseoSans14_300
            }, new NSRange(0, htmlBody.Length));
            if (_txtViewDescription == null)
            {
                _txtViewDescription = new UITextView(new CGRect(16, 0, ViewWidth - 32, 42))
                {
                    Editable = false,
                    ScrollEnabled = true,
                    WeakLinkTextAttributes = linkAttributes.Dictionary,
                    TextAlignment = UITextAlignment.Left,
                    UserInteractionEnabled = false,
                };
                View.AddSubview(_txtViewDescription);
            }
            _txtViewDescription.AttributedText = mutableHTMLBody;
        }

        private nfloat GetPreviewXLoc(int count, nfloat refWidth)
        {
            if (count == 1)
            {
                return (ViewWidth - refWidth) / 2;
            }
            else if (count == 2)
            {
                return (ViewWidth - (refWidth * 2) - 32) / 2;
            }
            else
            {
                return 44;
            }
        }

        private void SetPreview()
        {
            nfloat previewBaseWidth = ViewWidth - 88 - 64;
            nfloat previewWidth = previewBaseWidth / 3;
            _viewPreview = new UIView() { BackgroundColor = UIColor.White };

            if (_isMultiPhase)
            {
                _expectedReadingCount = _imageModelList.Count;
                for (int i = 0; i < _expectedReadingCount; i++)
                {
                    ImageModel item = _imageModelList[i];
                    if (i == 0)
                    {
                        _viewPreviewOne = CreatePhotoPreview(new CGRect(GetPreviewXLoc(_expectedReadingCount, previewWidth)
                            , 16, previewWidth, previewWidth), i);
                        _viewPreviewOne.Tag = item.Tag;
                        _viewPreviewOne.AddGestureRecognizer(new UITapGestureRecognizer(() => { PreviewAction(_viewPreviewOne.Tag); }));
                        _viewPreview.AddSubview(_viewPreviewOne);
                    }
                    else if (i == 1)
                    {
                        _viewPreviewTwo = CreatePhotoPreview(new CGRect(_viewPreviewOne.Frame.GetMaxX() + 32, 16, previewWidth, previewWidth), i);
                        _viewPreviewTwo.Tag = item.Tag;
                        _viewPreviewTwo.AddGestureRecognizer(new UITapGestureRecognizer(() => { PreviewAction(_viewPreviewTwo.Tag); }));
                        _viewPreview.AddSubview(_viewPreviewTwo);
                    }
                    else
                    {
                        _viewPreviewThree = CreatePhotoPreview(new CGRect(_viewPreviewTwo.Frame.GetMaxX() + 32, 16, previewWidth, previewWidth), i);
                        _viewPreviewThree.Tag = item.Tag;
                        _viewPreviewThree.AddGestureRecognizer(new UITapGestureRecognizer(() => { PreviewAction(_viewPreviewThree.Tag); }));
                        _viewPreview.AddSubview(_viewPreviewThree);
                    }
                }
            }

            nfloat btnYLoc = (_isMultiPhase ? _viewPreviewOne.Frame.GetMaxY() : 0) + 16.0F;
            _btnSubmit = new CustomUIButtonV2()
            {
                Frame = new CGRect(16, btnYLoc, ViewWidth - 32, 48),
                BackgroundColor = MyTNBColor.FreshGreen,
                Tag = 1004
            };
            _btnSubmit.SetTitle(GetCommonI18NValue(SSMRConstants.I18N_Submit), UIControlState.Normal);
            _btnSubmit.TouchUpInside += OnSubmit;
            _viewPreview.AddSubview(_btnSubmit);
            nfloat containerHeight = _btnSubmit.Frame.GetMaxY() + (DeviceHelper.IsIphoneXUpResolution() ? 36 : 16);
            _viewPreview.Frame = new CGRect(0, ViewHeight - containerHeight, ViewWidth, containerHeight);
            View.AddSubview(_viewPreview);
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
                { view.Layer.BorderColor = hasImg ? MyTNBColor.FreshGreen.CGColor : MyTNBColor.WhiteTwo.CGColor; }
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
                        SetDescription(GetI18NValue(data.Image == null ? _multiPhaseDescription : SSMRConstants.I18N_EditDescription));
                        Title = GetI18NValue(data.Image == null ? SSMRConstants.I18N_NavTitleTakePhoto : SSMRConstants.I18N_NavTitleAdjustPhoto);
                        HideInfoIcon(data.Image != null);
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
                _viewDelete.Hidden = true;
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
            view.Layer.BorderColor = (index == 1 ? MyTNBColor.WaterBlue : MyTNBColor.WhiteTwo).CGColor;
            UILabel label = new UILabel(new CGRect(0, (frame.Height - 24) / 2, frame.Width, 24))
            {
                TextAlignment = UITextAlignment.Center,
                Font = MyTNBFont.MuseoSans16_500,
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
            _viewCamera = new UIView(new CGRect(0, _txtViewDescription.Frame.GetMaxY() + 16
               , ViewWidth, ViewHeight - _txtViewDescription.Frame.GetMaxY() - 16 - _viewPreview.Frame.Height))
            { ClipsToBounds = true };
            _viewDelete = GetDeleteSection(_viewCamera);
            _viewCameraActions = GetCameraActions(_viewCamera);
            _viewOverlay = GetOverlay(_viewCamera);
            _viewCamera.AddSubviews(new UIView[] { _viewOverlay, _viewDelete, _viewCameraActions });
            View.AddSubview(_viewCamera);
        }

        #region Box Overlay
        private UIView GetOverlay(UIView viewBase)
        {
            nfloat baseHeight = viewBase.Frame.Height;
            nfloat baseWidth = viewBase.Frame.Width;
            nfloat boxHeight = viewBase.Frame.Height - _viewCameraActions.Frame.Height - (baseHeight * 0.028F * 2);
            UIView view = new UIView(new CGRect(new CGPoint(0, 0), viewBase.Frame.Size))
            { BackgroundColor = UIColor.Clear, UserInteractionEnabled = false };
            UIView viewTop = new UIView(new CGRect(0, 0, ViewWidth, (baseHeight * 0.028F) + 1))
            { BackgroundColor = UIColor.Black.ColorWithAlpha(0.60F), UserInteractionEnabled = false };
            UIView viewLeft = new UIView(new CGRect(0, viewTop.Frame.GetMaxY(), (baseWidth * 0.22F) + 1, boxHeight))
            { BackgroundColor = UIColor.Black.ColorWithAlpha(0.60F), UserInteractionEnabled = false };

            UIView viewClear = new UIView(new CGRect(viewLeft.Frame.GetMaxX() - 1, viewTop.Frame.GetMaxY() - 1, (baseWidth - (baseWidth * 0.22F * 2)) + 2, boxHeight + 2))
            { BackgroundColor = UIColor.Clear, UserInteractionEnabled = false };
            viewClear.Layer.BorderColor = MyTNBColor.WaterBlue.CGColor;
            viewClear.Layer.BorderWidth = 1.0F;
            viewClear.Layer.CornerRadius = 4.0F;

            UIView viewRight = new UIView(new CGRect(viewClear.Frame.GetMaxX() - 1, viewTop.Frame.GetMaxY(), (baseWidth * 0.22F) + 1, boxHeight))
            { BackgroundColor = UIColor.Black.ColorWithAlpha(0.60F), UserInteractionEnabled = false };
            UIView viewBottom = new UIView(new CGRect(0, viewLeft.Frame.GetMaxY(), ViewWidth, (baseHeight - viewLeft.Frame.GetMaxY()) + 1))
            { BackgroundColor = UIColor.Black.ColorWithAlpha(0.60F), UserInteractionEnabled = false };
            view.AddSubviews(new UIView[] { viewTop, viewLeft, viewClear, viewRight, viewBottom });
            view.BringSubviewToFront(viewClear);
            return view;
        }
        #endregion

        private UIView GetDeleteSection(UIView viewBase)
        {
            UIView view = new UIView(new CGRect(0, viewBase.Frame.Height - 53, ViewWidth, 53))
            {
                BackgroundColor = UIColor.FromWhiteAlpha(1, 0.37F),
                Hidden = true
            };
            UIImageView imgDelete = new UIImageView(new CGRect((ViewWidth - 24) / 2, 14, 24, 24))
            {
                Image = UIImage.FromBundle(SSMRConstants.IMG_Delete)
            };
            view.AddGestureRecognizer(new UITapGestureRecognizer(() =>
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
                SetDescription(_isMultiPhase ? _multiPhaseDescription : GetI18NValue(SSMRConstants.I18N_SingleTakePhotoDescription));
                Title = GetI18NValue(SSMRConstants.I18N_NavTitleTakePhoto);
                HideInfoIcon(false);
            }));
            view.AddSubview(imgDelete);
            return view;
        }

        private UIView GetCameraActions(UIView viewBase)
        {
            nfloat size = ViewWidth * 0.15F;
            UIView view = new UIView() { BackgroundColor = UIColor.Clear };

            _zoomSlider = new CustomUISlider(new CGRect(16, 0, ViewWidth - 32, 3))
            {
                MinimumTrackTintColor = UIColor.White
            };
            _zoomSlider.SetThumbImage(UIImage.FromBundle(SSMRConstants.IMG_CameraThumb), UIControlState.Normal);
            _zoomSlider.ValueChanged += (sender, e) =>
            {
                Debug.WriteLine("zoomSlider ValueChanged: " + ((UISlider)sender).Value);
                nfloat zoomFactor = (nfloat)((UISlider)sender).Value;
                if (zoomFactor > _zoomSlider.MaxValue) { zoomFactor = _zoomSlider.MaxValue; }
                if (zoomFactor < _zoomSlider.MinValue) { zoomFactor = _zoomSlider.MinValue; }
                _captureDevice.LockForConfiguration(out NSError nsError);
                _captureDevice.VideoZoomFactor = zoomFactor;
                _captureDevice.UnlockForConfiguration();
            };

            _viewGallery = new UIView(new CGRect(16, _zoomSlider.Frame.GetMaxY() + 22, size, size)) { ClipsToBounds = true };
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

            _viewCapture = new UIView(new CGRect((ViewWidth - size) / 2
               , _zoomSlider.Frame.GetMaxY() + 22, size, size))
            { BackgroundColor = UIColor.FromWhiteAlpha(1, 0.80F) };
            _viewCapture.Layer.CornerRadius = _viewCapture.Frame.Width / 2;
            _viewCapture.Layer.BorderWidth = 3.0F;
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
                    Title = GetI18NValue(SSMRConstants.I18N_NavTitleAdjustPhoto);
                    HideInfoIcon(true);
                }
                ToggleCTA();
            };
            UIImagePickerController imgPicker = new UIImagePickerController
            {
                Delegate = pickerDelegate,
                SourceType = UIImagePickerControllerSourceType.PhotoLibrary
            };
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

                    if (i + 1 == count)
                    {
                        SetPreviewColors(parent, true, false);
                        AddMainPreview(image);

                        SetDescription(GetI18NValue(SSMRConstants.I18N_EditDescription));
                        Title = GetI18NValue(SSMRConstants.I18N_NavTitleAdjustPhoto);
                        HideInfoIcon(true);
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
                UIImageView imgLoading = new UIImageView(new CGRect((ViewWidth - 156) / 2, ViewHeight * 0.16F, 156, 146))
                { Image = UIImage.FromBundle(SSMRConstants.IMG_OCRReading) };

                UILabel lblDescription = new UILabel(new CGRect(20, imgLoading.Frame.GetMaxY() + 24, ViewWidth - 40, 48))
                {
                    TextAlignment = UITextAlignment.Center,
                    Font = MyTNBFont.MuseoSans16_300,
                    TextColor = MyTNBColor.Grey,
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
            Title = GetI18NValue(SSMRConstants.I18N_NavTitleTakePhoto);
            HideInfoIcon(false);
        }

        private void UpdateViewGallery()
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
                    IsHighResolutionCaptureEnabled = false
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
                    settings.IsHighResolutionPhotoEnabled = false;
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
                Title = GetI18NValue(SSMRConstants.I18N_NavTitleAdjustPhoto);
                HideInfoIcon(true);
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
                    BackgroundColor = MyTNBColor.WhiteTwo
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
            _viewCamera.BringSubviewToFront(_viewDelete);
            _viewDelete.Hidden = false;
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
                NSData imgData = img.AsJPEG(0.0F);
                if (imgData != null)
                {
                    fileSize = imgData.Length / 1000;
                    string base64 = imgData.GetBase64EncodedString(NSDataBase64EncodingOptions.None);
                    return base64 ?? string.Empty;
                }
            }
            return string.Empty;
        }

        private void OnSubmit(object sender, EventArgs e)
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
                    if (_viewLoading != null) { _viewLoading.Hidden = true; }
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
                OCRReadingCache.Instance.AddOCRReading(response);
                return response;
            });
        }
        #endregion
    }
}