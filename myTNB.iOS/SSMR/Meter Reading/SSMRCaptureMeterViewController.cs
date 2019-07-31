using CoreGraphics;
using Foundation;
using myTNB.SSMR;
using System;
using System.Diagnostics;
using UIKit;
using AVFoundation;
using myTNB.SSMR.MeterReading;

namespace myTNB
{
    public partial class SSMRCaptureMeterViewController : CustomUIViewController
    {
        public SSMRCaptureMeterViewController(IntPtr handle) : base(handle)
        {
        }

        public bool IsThreePhase = false;

        private UILabel _lblDescription;
        private UIView _viewPreview, _viewCamera, _viewCapture;
        private UIView _viewPreviewOne, _viewPreviewTwo, _viewPreviewThree;
        private UIView _viewDelete, _viewCameraActions, _viewMainPreviewParent;
        private UIImageView _imgViewMainPreview;
        private CustomUISlider _zoomSlider;
        private UIButton _btnSubmit;

        private AVCaptureSession _captureSession;
        private AVCaptureDevice _captureDevice;
        private AVCaptureDeviceInput _input;
        private AVCapturePhotoOutput _output;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ConfigureNavigationBar();
            SetDescription();
            SetPreview();
            SetCamera();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            SetupLiveCameraStream();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
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
            Title = "Take Photo";//GetI18NValue(SSMRConstants.I18N_NavTitle);
        }

        private void SetDescription()
        {
            _lblDescription = new UILabel(new CGRect(16, 16, ViewWidth - 32, 38))
            {
                TextAlignment = UITextAlignment.Left,
                Font = MyTNBFont.MuseoSans14_300,
                TextColor = MyTNBColor.CharcoalGrey,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = "Get ready, you'll have 10 seconds between each value being flashed."
            };
            CGSize newLblSize = GetLabelSize(_lblDescription, _lblDescription.Frame.Width, ViewHeight / 2);
            CGRect newFrame = _lblDescription.Frame;
            newFrame.Height = newLblSize.Height < 38 ? 38 : newLblSize.Height;
            _lblDescription.Frame = newFrame;
            View.AddSubview(_lblDescription);
        }

        private void SetPreview()
        {
            nfloat previewBaseWidth = ViewWidth - 88 - 64;
            nfloat previewWidth = previewBaseWidth / 3;
            _viewPreview = new UIView() { BackgroundColor = UIColor.White };

            if (IsThreePhase)
            {
                _viewPreviewOne = CreatePhotoPreview(new CGRect(44, 16, previewWidth, previewWidth));
                _viewPreview.AddSubview(_viewPreviewOne);

                _viewPreviewTwo = CreatePhotoPreview(new CGRect(_viewPreviewOne.Frame.GetMaxX() + 32, 16, previewWidth, previewWidth));
                _viewPreview.AddSubview(_viewPreviewTwo);

                _viewPreviewThree = CreatePhotoPreview(new CGRect(_viewPreviewTwo.Frame.GetMaxX() + 32, 16, previewWidth, previewWidth));
                _viewPreview.AddSubview(_viewPreviewThree);
            }

            nfloat btnYLoc = (IsThreePhase ? _viewPreviewOne.Frame.GetMaxY() : 0) + 16.0F;

            _btnSubmit = new CustomUIButtonV2()
            {
                Frame = new CGRect(16, btnYLoc, ViewWidth - 32, 48),
                BackgroundColor = MyTNBColor.FreshGreen
            };
            _btnSubmit.SetTitle(GetCommonI18NValue(SSMRConstants.I18N_Submit), UIControlState.Normal);
            _btnSubmit.TouchUpInside += OnSubmit;
            _viewPreview.AddSubview(_btnSubmit);
            nfloat containerHeight = _btnSubmit.Frame.GetMaxY() + (DeviceHelper.IsIphoneXUpResolution() ? 36 : 16);
            _viewPreview.Frame = new CGRect(0, ViewHeight - containerHeight, ViewWidth, containerHeight);
            View.AddSubview(_viewPreview);
        }

        private UIView CreatePhotoPreview(CGRect frame)
        {
            UIView view = new UIView(frame) { ClipsToBounds = true };
            view.Layer.CornerRadius = 4.0F;
            view.Layer.BorderWidth = 2.0F;
            view.Layer.BorderColor = MyTNBColor.WhiteTwo.CGColor;
            return view;
        }

        private void SetCamera()
        {
            _viewCamera = new UIView(new CGRect(0, _lblDescription.Frame.GetMaxY() + 16
               , ViewWidth, ViewHeight - _lblDescription.Frame.GetMaxY() - 16 - _viewPreview.Frame.Height))
            { ClipsToBounds = true };
            _viewDelete = GetDeleteSection(_viewCamera);
            _viewCameraActions = GetCameraActions(_viewCamera);
            _viewCamera.AddSubviews(new UIView[] { _viewDelete, _viewCameraActions });
            View.AddSubview(_viewCamera);
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
                Image = UIImage.FromBundle("Notification-Delete")
            };
            view.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                Debug.WriteLine("Delete tapped");
                _viewMainPreviewParent.Hidden = true;
                _imgViewMainPreview.Image = null;
                _viewDelete.Hidden = true;
                _viewCameraActions.Hidden = false;
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
            _zoomSlider.SetThumbImage(UIImage.FromBundle("Camera-Thumb"), UIControlState.Normal);
            _zoomSlider.ValueChanged += (sender, e) =>
            {
                Debug.WriteLine("zoomSlider ValueChanged: " + ((UISlider)sender).Value);
                nfloat zoomFactor = (nfloat)((UISlider)sender).Value;
                NSError nsError;
                _captureDevice.LockForConfiguration(out nsError);
                _captureDevice.VideoZoomFactor = zoomFactor;
                _captureDevice.UnlockForConfiguration();
            };

            UIView viewGallery = new UIView(new CGRect(16, _zoomSlider.Frame.GetMaxY() + 22, size, size));
            viewGallery.Layer.CornerRadius = 4.0F;
            viewGallery.Layer.BorderWidth = 2.0F;
            viewGallery.Layer.BorderColor = UIColor.White.CGColor;
            viewGallery.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                Debug.WriteLine("viewGallery tapped");
            }));

            _viewCapture = new UIView(new CGRect((ViewWidth - size) / 2
               , _zoomSlider.Frame.GetMaxY() + 22, size, size))
            { BackgroundColor = UIColor.FromWhiteAlpha(1, 0.80F) };
            _viewCapture.Layer.CornerRadius = _viewCapture.Frame.Width / 2;
            _viewCapture.Layer.BorderWidth = 3.0F;
            _viewCapture.Layer.BorderColor = UIColor.White.CGColor;

            view.AddSubviews(new UIView[] { viewGallery, _viewCapture, _zoomSlider });
            CGRect viewFrame = new CGRect(0, viewBase.Frame.Height - _viewCapture.Frame.GetMaxY() - 16
                , ViewWidth, _viewCapture.Frame.GetMaxY() + 16);
            view.Frame = viewFrame;
            return view;
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

                _output = new AVCapturePhotoOutput();
                _output.IsHighResolutionCaptureEnabled = false;

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
                    settings.FlashMode = AVCaptureFlashMode.Auto;

                    _output.CapturePhoto(settings, capturePhotoDelegate);
                }
            }));
        }

        private UIImage _capturedImage;
        private void OnCapturePhoto(UIImage capturedImage)
        {
            _capturedImage = capturedImage;
            if (IsThreePhase)
            {

            }
            else
            {
                if (_viewMainPreviewParent == null || _imgViewMainPreview == null)
                {
                    _viewMainPreviewParent = new UIView(new CGRect(0, 0, ViewWidth, _viewCamera.Frame.Height))
                    {
                        ClipsToBounds = true,
                        BackgroundColor = UIColor.White
                    };
                    _imgViewMainPreview = new UIImageView(new CGRect(new CGPoint(0, 0), _viewMainPreviewParent.Frame.Size));
                    _imgViewMainPreview.UserInteractionEnabled = true;
                    _imgViewMainPreview.MultipleTouchEnabled = true;
                    _imgViewMainPreview.AddGestureRecognizer(new UIPinchGestureRecognizer((sender) => { PinchZoomAction(sender); }));
                    _imgViewMainPreview.AddGestureRecognizer(new UIPanGestureRecognizer((sender) => { PanAction(sender); }));

                    _viewMainPreviewParent.AddSubview(_imgViewMainPreview);
                }
                _imgViewMainPreview.Frame = new CGRect(new CGPoint(0, 0), _viewMainPreviewParent.Frame.Size);
                _imgViewMainPreview.Image = capturedImage;
                _viewMainPreviewParent.Hidden = false;

                _viewCamera.AddSubview(_viewMainPreviewParent);
                _viewCamera.BringSubviewToFront(_viewDelete);
                _viewDelete.Hidden = false;
                _viewCameraActions.Hidden = true;
            }
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

        private string GetBase64String(UIImage img)
        {
            if (img != null)
            {
                NSData imgData = img.AsJPEG();//0.0Lowest Compression
                string base64 = imgData.GetBase64EncodedString(NSDataBase64EncodingOptions.None);
                return base64 ?? string.Empty;
            }
            return string.Empty;
        }

        private UIImage _croppedImage;
        private void OnSubmit(object sender, EventArgs e)
        {
            CGRect cropRect = new CGRect(0, 0, ViewWidth, 100);
            CGImage subImage = _capturedImage.CGImage.WithImageInRect(cropRect);
            _croppedImage = UIImage.FromImage(subImage);
            GetBase64String(_croppedImage);
        }
    }
}