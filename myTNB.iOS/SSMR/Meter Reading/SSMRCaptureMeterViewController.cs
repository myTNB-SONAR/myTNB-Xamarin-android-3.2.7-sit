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

        private UILabel _lblDescription;
        private UIView _viewPreview, _viewCamera, _viewCapture;
        private UIView _viewPreviewOne, _viewPreviewTwo, _viewPreviewThree;
        private UIView _viewDelete, _viewCameraActions, _viewMainPreviewParent
            , _viewGallery, _viewOverlay, _viewLoading;
        private UIImageView _imgViewMainPreview;
        private CustomUISlider _zoomSlider;
        private UIButton _btnSubmit;
        private UIImage _capturedImage;

        private AVCaptureSession _captureSession;
        private AVCaptureDevice _captureDevice;
        private AVCaptureDeviceInput _input;
        private AVCapturePhotoOutput _output;

        private bool _isMultiPhase;
        private List<ImageModel> _imageModelList;

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
            _isMultiPhase = ReadingDictionary != null && ReadingDictionary.Count > 1;
            SetImageList();
            ConfigureNavigationBar();
            SetDescription();
            SetPreview();
            SetCamera();
            Debug.WriteLine("Viewdidload");
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
                PHPhotoLibrary.RequestAuthorization((status) => { });
            }
            else
            {
                UpdateViewGallery();
            }
        }

        private void ConfigureNavigationBar()
        {
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle(SSMRConstants.IMG_BackIcon)
                , UIBarButtonItemStyle.Done, (sender, e) =>
            {
                ViewHelper.DismissControllersAndSelectTab(this, 0, true);
            });
            UIBarButtonItem btnInfo = new UIBarButtonItem(UIImage.FromBundle(SSMRConstants.IMG_Info)
                , UIBarButtonItemStyle.Done, (sender, e) =>
            {
                Debug.WriteLine("Info Tapped");
            });
            NavigationItem.LeftBarButtonItem = btnBack;
            NavigationItem.RightBarButtonItem = btnInfo;
            Title = GetI18NValue(SSMRConstants.I18N_NavTitleTakePhoto);
        }

        private void SetImageList()
        {
            if (_isMultiPhase)
            {
                _imageModelList = new List<ImageModel>();
                List<string> keys = ReadingDictionary.Keys.ToList();
                for (int i = 0; i < keys.Count; i++)
                {
                    ImageModel imgModel = new ImageModel
                    {
                        NeedsPhoto = !ReadingDictionary[keys[i]],
                        ReadingUnit = keys[i],
                        Tag = 1001 + i
                    };
                    _imageModelList.Add(imgModel);
                }
            }
        }

        private void SetDescription(string description = "")
        {
            if (_lblDescription == null)
            {
                _lblDescription = new UILabel(new CGRect(16, 16, ViewWidth - 32, 38))
                {
                    TextAlignment = UITextAlignment.Left,
                    Font = MyTNBFont.MuseoSans14_300,
                    TextColor = MyTNBColor.CharcoalGrey,
                    Lines = 0,
                    LineBreakMode = UILineBreakMode.WordWrap
                };
                View.AddSubview(_lblDescription);
            }
            if (string.IsNullOrEmpty(description) || string.IsNullOrWhiteSpace(description))
            {
                description = GetI18NValue(_isMultiPhase ? SSMRConstants.I18N_MultiTakePhotoDescription
                    : SSMRConstants.I18N_SingleTakePhotoDescription);
            }
            _lblDescription.Text = description;
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
                int expectedReadingCount = ReadingDictionary.Count(x => !x.Value);
                for (int i = 0; i < expectedReadingCount; i++)
                {
                    if (i == 0)
                    {
                        _viewPreviewOne = CreatePhotoPreview(new CGRect(GetPreviewXLoc(expectedReadingCount, previewWidth)
                            , 16, previewWidth, previewWidth), i);
                        _viewPreviewOne.AddGestureRecognizer(new UITapGestureRecognizer(() => { PreviewAction(_viewPreviewOne.Tag); }));
                        _viewPreview.AddSubview(_viewPreviewOne);
                    }
                    else if (i == 1)
                    {
                        _viewPreviewTwo = CreatePhotoPreview(new CGRect(_viewPreviewOne.Frame.GetMaxX() + 32, 16, previewWidth, previewWidth), i);
                        _viewPreviewTwo.AddGestureRecognizer(new UITapGestureRecognizer(() => { PreviewAction(_viewPreviewTwo.Tag); }));
                        _viewPreview.AddSubview(_viewPreviewTwo);
                    }
                    else
                    {
                        _viewPreviewThree = CreatePhotoPreview(new CGRect(_viewPreviewTwo.Frame.GetMaxX() + 32, 16, previewWidth, previewWidth), i);
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

        private void PreviewAction(nint tag)
        {
            Debug.WriteLine("PreviewAction: " + tag);
            for (int i = 0; i < _viewPreview.Subviews.Length; i++)
            {
                UIView view = _viewPreview.Subviews[i];
                if (view == null || view.Tag == 1004) { continue; }
                UIImageView imgView = view.ViewWithTag(99) as UIImageView;
                if (imgView != null && imgView.Hidden)
                {
                    if (view.Tag == tag) { continue; }
                    view.Layer.BorderColor = MyTNBColor.WhiteTwo.CGColor;
                }
                if (imgView != null && !imgView.Hidden) { view.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor; }
                if (view.Tag == tag && imgView != null && !imgView.Hidden)
                {
                    view.Layer.BorderColor = MyTNBColor.WaterBlue.CGColor;
                    AddMainPreview(imgView.Image);
                }
            }
            /*UIView view = _viewPreview.ViewWithTag(tag);
            if (view != null)
            {
                UIImageView imgView = view.ViewWithTag(99) as UIImageView;
                if (imgView == null || imgView.Hidden) { return; }
                Debug.WriteLine("PreviewAction with img: " + tag);

                AddMainPreview(imgView.Image);
                view.Layer.BorderColor = MyTNBColor.WaterBlue.CGColor;
            }*/
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
                Text = index.ToString()
            };
            UIImageView imgView = new UIImageView(new CGRect(new CGPoint(0, 0), frame.Size))
            {
                Hidden = true,
                Tag = 99
            };
            view.AddSubviews(new UIView[] { label, imgView });
            view.Tag = 1000 + index;
            return view;
        }

        private void SetCamera()
        {
            _viewCamera = new UIView(new CGRect(0, _lblDescription.Frame.GetMaxY() + 16
               , ViewWidth, ViewHeight - _lblDescription.Frame.GetMaxY() - 16 - _viewPreview.Frame.Height))
            { ClipsToBounds = true };
            _viewDelete = GetDeleteSection(_viewCamera);
            _viewCameraActions = GetCameraActions(_viewCamera);
            _viewOverlay = GetOverlay(_viewCamera);
            _viewCamera.AddSubviews(new UIView[] { _viewOverlay, _viewDelete, _viewCameraActions });
            View.AddSubview(_viewCamera);
        }

        private UIView GetOverlay(UIView viewBase)
        {
            nfloat baseHeight = viewBase.Frame.Height;
            UIView view = new UIView(new CGRect(new CGPoint(0, 0), viewBase.Frame.Size))
            { BackgroundColor = UIColor.Clear, UserInteractionEnabled = false };
            UIView viewTop = new UIView(new CGRect(0, 0, ViewWidth, baseHeight * 0.2F))
            { BackgroundColor = UIColor.Black.ColorWithAlpha(0.60F), UserInteractionEnabled = false };
            UIView viewLeft = new UIView(new CGRect(0, viewTop.Frame.GetMaxY(), 18, baseHeight * 0.3F))
            { BackgroundColor = UIColor.Black.ColorWithAlpha(0.60F), UserInteractionEnabled = false };
            UIView viewRight = new UIView(new CGRect(ViewWidth - 18, viewTop.Frame.GetMaxY(), 18, baseHeight * 0.3F))
            { BackgroundColor = UIColor.Black.ColorWithAlpha(0.60F), UserInteractionEnabled = false };
            UIView viewBottom = new UIView(new CGRect(0, viewLeft.Frame.GetMaxY(), ViewWidth, baseHeight - viewLeft.Frame.GetMaxY()))
            { BackgroundColor = UIColor.Black.ColorWithAlpha(0.60F), UserInteractionEnabled = false };
            view.AddSubviews(new UIView[] { viewTop, viewLeft, viewRight, viewBottom });
            return view;
        }

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
                _viewMainPreviewParent.Hidden = true;
                _imgViewMainPreview.Image = null;
                _viewDelete.Hidden = true;
                _viewCameraActions.Hidden = false;

                _viewCamera.SendSubviewToBack(_viewOverlay);
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
                NSError nsError;
                _captureDevice.LockForConfiguration(out nsError);
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
                    }
                };
                UIImagePickerController imgPicker = new UIImagePickerController
                {
                    Delegate = pickerDelegate,
                    SourceType = UIImagePickerControllerSourceType.PhotoLibrary
                };
                PresentViewController(imgPicker, true, null);

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

        private void UpdateImagePreview(UIImage image)
        {
            int count = _imageModelList.Count;
            for (int i = 0; i < count; i++)
            {
                ImageModel model = _imageModelList[i];
                if (model.NeedsPhoto)
                {
                    UIView parent = _viewPreview.ViewWithTag(model.Tag);

                    UIImageView imgView = parent.ViewWithTag(99) as UIImageView;
                    parent.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;

                    imgView.Hidden = false;
                    imgView.Image = image;
                    model.Image = image;
                    model.NeedsPhoto = false;

                    _imageModelList[i] = model;

                    if (i + 1 < count)
                    {
                        UIView nextParent = _viewPreview.ViewWithTag(_imageModelList[i + 1].Tag);
                        nextParent.Layer.BorderColor = MyTNBColor.WaterBlue.CGColor;
                    }

                    if (i + 1 == count)
                    {
                        parent.Layer.BorderColor = MyTNBColor.WaterBlue.CGColor;
                        AddMainPreview(image);
                    }
                    break;
                }
            }
        }

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

            _zoomSlider.MinValue = (float)_captureDevice.MinAvailableVideoZoomFactor;
            _zoomSlider.MaxValue = (float)_captureDevice.MaxAvailableVideoZoomFactor;

            NSError nsError;
            try
            {
                _input = new AVCaptureDeviceInput(_captureDevice, out nsError);

                _output = new AVCapturePhotoOutput
                {
                    IsHighResolutionCaptureEnabled = false
                };

                _captureSession = new AVCaptureSession();
                _captureSession.AddInput(_input);
                _captureSession.AddOutput(_output);

                AVCaptureVideoPreviewLayer videoPreviewLayer = new AVCaptureVideoPreviewLayer(_captureSession)
                {
                    Frame = new CGRect(0, 0, _viewCamera.Frame.Width, _viewCamera.Frame.Height),
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
            }
        }

        private void AddMainPreview(UIImage previewImg)
        {
            if (_viewMainPreviewParent == null || _imgViewMainPreview == null)
            {
                _viewMainPreviewParent = new UIView(new CGRect(0, 0, ViewWidth, _viewCamera.Frame.Height))
                {
                    ClipsToBounds = true,
                    BackgroundColor = UIColor.DarkGray
                };
                _imgViewMainPreview = new UIImageView(new CGRect(new CGPoint(0, 0), _viewMainPreviewParent.Frame.Size))
                {
                    UserInteractionEnabled = true,
                    MultipleTouchEnabled = true,
                    ClipsToBounds = true
                };
                _imgViewMainPreview.AddGestureRecognizer(new UIPinchGestureRecognizer((sender) => { PinchZoomAction(sender); }));
                _imgViewMainPreview.AddGestureRecognizer(new UIPanGestureRecognizer((sender) => { PanAction(sender); }));

                _viewMainPreviewParent.AddSubview(_imgViewMainPreview);
            }
            _imgViewMainPreview.Frame = new CGRect(new CGPoint(0, 0), _viewMainPreviewParent.Frame.Size);
            _imgViewMainPreview.Image = previewImg;
            _viewMainPreviewParent.Hidden = false;

            _viewCamera.AddSubview(_viewMainPreviewParent);
            _viewCamera.BringSubviewToFront(_viewOverlay);
            _viewCamera.BringSubviewToFront(_viewDelete);
            _viewDelete.Hidden = false;
            _viewCameraActions.Hidden = true;
        }

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
                CGPoint point = sender.TranslationInView(sender.View);
                sender.View.Transform = CGAffineTransform.Translate(sender.View.Transform, point.X, point.Y);
                sender.SetTranslation(new CGPoint(0, 0), sender.View);
            }
        }

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
                    var test = OCRReadingCache.Instance.GetOCRReadings();
                    Debug.WriteLine("ResponseList: ");
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
    }
}