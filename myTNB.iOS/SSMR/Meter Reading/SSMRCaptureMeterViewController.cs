using CoreGraphics;
using Foundation;
using myTNB.SSMR;
using System;
using System.Diagnostics;
using UIKit;
using AVFoundation;
using System.Threading.Tasks;
using CoreMedia;

namespace myTNB
{
    public partial class SSMRCaptureMeterViewController : CustomUIViewController
    {
        public SSMRCaptureMeterViewController(IntPtr handle) : base(handle)
        {
        }

        public bool IsThreePhase = true;

        private UILabel _lblDescription;
        private UIView _viewPreview, _viewCamera, _viewCapture;
        private UIView viewPreviewOne;
        private UISlider zoomSlider;

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
            AuthorizeCameraUse();
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
            UIImage backImg = UIImage.FromBundle(SSMRConstants.IMG_BackIcon);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                ViewHelper.DismissControllersAndSelectTab(this, 0, true);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
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
            viewPreviewOne = new UIView(new CGRect(44, 16, previewWidth, previewWidth)) { ClipsToBounds = true };
            viewPreviewOne.Layer.CornerRadius = 4.0F;
            viewPreviewOne.Layer.BorderWidth = 2.0F;
            viewPreviewOne.Layer.BorderColor = MyTNBColor.WaterBlue.CGColor;

            _viewPreview.AddSubview(viewPreviewOne);
            if (IsThreePhase)
            {
                UIView viewPreviewTwo = new UIView(new CGRect(viewPreviewOne.Frame.GetMaxX() + 32
                    , 16, previewWidth, previewWidth))
                { ClipsToBounds = true };
                viewPreviewTwo.Layer.CornerRadius = 4.0F;
                viewPreviewTwo.Layer.BorderWidth = 2.0F;
                viewPreviewTwo.Layer.BorderColor = MyTNBColor.WhiteTwo.CGColor;

                UIView viewPreviewThree = new UIView(new CGRect(viewPreviewTwo.Frame.GetMaxX() + 32
                    , 16, previewWidth, previewWidth))
                { ClipsToBounds = true };
                viewPreviewThree.Layer.CornerRadius = 4.0F;
                viewPreviewThree.Layer.BorderWidth = 2.0F;
                viewPreviewThree.Layer.BorderColor = MyTNBColor.WhiteTwo.CGColor;

                _viewPreview.AddSubviews(new UIView[] { viewPreviewTwo, viewPreviewThree });
            }

            UIButton btnSubmit = new CustomUIButtonV2()
            {
                Frame = new CGRect(16, viewPreviewOne.Frame.GetMaxY() + 16, ViewWidth - 32, 48),
                BackgroundColor = MyTNBColor.FreshGreen
            };
            btnSubmit.SetTitle(GetCommonI18NValue(SSMRConstants.I18N_Submit), UIControlState.Normal);
            btnSubmit.TouchUpInside += (sender, e) =>
            {
            };
            _viewPreview.AddSubview(btnSubmit);
            nfloat containerHeight = btnSubmit.Frame.GetMaxY() + (DeviceHelper.IsIphoneXUpResolution() ? 36 : 16);
            _viewPreview.Frame = new CGRect(0, ViewHeight - containerHeight, ViewWidth, containerHeight);
            View.AddSubview(_viewPreview);
        }

        private void SetCamera()
        {
            _viewCamera = new UIView(new CGRect(0, _lblDescription.Frame.GetMaxY() + 16
               , ViewWidth, ViewHeight - _lblDescription.Frame.GetMaxY() - 16 - _viewPreview.Frame.Height))
            {
                //BackgroundColor = UIColor.Red
            };
            UIView viewDelete = GetDeleteSection(_viewCamera);
            UIView viewCameraActions = GetCameraActions(_viewCamera);
            _viewCamera.AddSubviews(new UIView[] { viewDelete, viewCameraActions });
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
            }));
            view.AddSubview(imgDelete);
            return view;
        }

        private UIView GetCameraActions(UIView viewBase)
        {
            nfloat size = ViewWidth * 0.15F;
            UIView view = new UIView() { BackgroundColor = UIColor.Clear };

            //Need image thumb
            zoomSlider = new UISlider(new CGRect(16, 0, ViewWidth - 32, 20)) { };
            zoomSlider.MinimumTrackTintColor = UIColor.White;
            zoomSlider.SetThumbImage(UIImage.FromBundle("Camera-Thumb"), UIControlState.Normal);
            zoomSlider.ValueChanged += (sender, e) =>
            {
                Debug.WriteLine("zoomSlider ValueChanged: " + ((UISlider)sender).Value);
                nfloat zoomFactor = (nfloat)((UISlider)sender).Value;
                NSError nsError;
                _captureDevice.LockForConfiguration(out nsError);
                _captureDevice.VideoZoomFactor = zoomFactor;
                _captureDevice.UnlockForConfiguration();
            };

            UIView viewGallery = new UIView(new CGRect(16, zoomSlider.Frame.GetMaxY() + 22, size, size));
            viewGallery.Layer.CornerRadius = 4.0F;
            viewGallery.Layer.BorderWidth = 2.0F;
            viewGallery.Layer.BorderColor = UIColor.White.CGColor;
            viewGallery.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                Debug.WriteLine("viewGallery tapped");
            }));

            _viewCapture = new UIView(new CGRect((ViewWidth - size) / 2
               , zoomSlider.Frame.GetMaxY() + 22, size, size))
            { BackgroundColor = UIColor.FromWhiteAlpha(1, 0.80F) };
            _viewCapture.Layer.CornerRadius = _viewCapture.Frame.Width / 2;
            _viewCapture.Layer.BorderWidth = 3.0F;
            _viewCapture.Layer.BorderColor = UIColor.White.CGColor;

            view.AddSubviews(new UIView[] { viewGallery, _viewCapture, zoomSlider });
            CGRect viewFrame = new CGRect(0, viewBase.Frame.Height - _viewCapture.Frame.GetMaxY() - 16
                , ViewWidth, _viewCapture.Frame.GetMaxY() + 16);
            view.Frame = viewFrame;
            return view;
        }

        private async Task AuthorizeCameraUse()
        {
            AVAuthorizationStatus authorizationStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);
            if (authorizationStatus != AVAuthorizationStatus.Authorized)
            {
                await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);
            }
        }

        public void SetupLiveCameraStream()
        {
            _captureDevice = AVCaptureDevice.GetDefaultDevice(AVMediaTypes.Video);

            zoomSlider.MinValue = (float)_captureDevice.MinAvailableVideoZoomFactor;
            zoomSlider.MaxValue = (float)_captureDevice.MaxAvailableVideoZoomFactor;

            NSError nsError;
            try
            {
                _input = new AVCaptureDeviceInput(_captureDevice, out nsError);

                _output = new AVCapturePhotoOutput();
                _output.IsHighResolutionCaptureEnabled = true;

                _captureSession = new AVCaptureSession();
                _captureSession.AddInput(_input);
                _captureSession.AddOutput(_output);

                AVCaptureVideoPreviewLayer videoPreviewLayer = new AVCaptureVideoPreviewLayer(_captureSession)
                {
                    Frame = new CGRect(0, 0, _viewCamera.Frame.Width, _viewCamera.Frame.Height),
                    VideoGravity = AVLayerVideoGravity.ResizeAspectFill,
                    ZPosition = -1
                };
                _viewCamera.Layer.AddSublayer(videoPreviewLayer);
                _captureSession.StartRunning();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in camera: " + e.Message);
            }

            _viewCapture.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                Debug.WriteLine("viewCapture tapped");
                if (_output != null)
                {
                    AVCapturePhotoSettings settings = AVCapturePhotoSettings.Create();
                    settings.IsHighResolutionPhotoEnabled = true;
                    settings.IsAutoStillImageStabilizationEnabled = true;
                    settings.FlashMode = AVCaptureFlashMode.Auto;
                    _output.CapturePhoto(settings, new CapturePhotoDelegate(viewPreviewOne));
                }
            }));
        }

        public class CapturePhotoDelegate : AVCapturePhotoCaptureDelegate
        {
            private UIView viewPreviewOne;
            public CapturePhotoDelegate(UIView viewPreviewOne)
            {
                this.viewPreviewOne = viewPreviewOne;
            }
            public override void DidFinishCapture(AVCapturePhotoOutput captureOutput, AVCaptureResolvedPhotoSettings resolvedSettings, NSError error)
            {
            }

            public override void DidFinishProcessingPhoto(AVCapturePhotoOutput captureOutput
                , CMSampleBuffer photoSampleBuffer, CMSampleBuffer previewPhotoSampleBuffer
                , AVCaptureResolvedPhotoSettings resolvedSettings, AVCaptureBracketedStillImageSettings bracketSettings
                , NSError error)
            {
                if (photoSampleBuffer == null)
                {
                    return;
                }
                NSData imgData = AVCapturePhotoOutput.GetJpegPhotoDataRepresentation(photoSampleBuffer, previewPhotoSampleBuffer);
                UIImage image = UIImage.LoadFromData(imgData, 1.0F);
                UIImageView imgView = new UIImageView(new CGRect(0, 0, viewPreviewOne.Frame.Width, viewPreviewOne.Frame.Height))
                {
                    Image = image
                };
                viewPreviewOne.AddSubview(imgView);
            }
        }
    }
}