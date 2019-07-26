using CoreGraphics;
using Foundation;
using myTNB.SSMR;
using System;
using System.Diagnostics;
using UIKit;

namespace myTNB
{
    public partial class SSMRCaptureMeterViewController : CustomUIViewController
    {
        public SSMRCaptureMeterViewController(IntPtr handle) : base(handle)
        {
        }

        public bool IsThreePhase = true;

        private UILabel _lblDescription;
        private UIView _viewPreview;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetDescription();
            SetPreview();
            SetCamera();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
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
            UIView viewPreviewOne = new UIView(new CGRect(44, 16, previewWidth, previewWidth)) { ClipsToBounds = true };
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
            UIView viewCamera = new UIView(new CGRect(0, _lblDescription.Frame.GetMaxY() + 16
                , ViewWidth, ViewHeight - _lblDescription.Frame.GetMaxY() - 16 - _viewPreview.Frame.Height))
            {
                BackgroundColor = UIColor.Red
            };
            UIView viewDelete = GetDeleteSection(viewCamera);
            UIView viewCameraActions = GetCameraActions(viewCamera);
            viewCamera.AddSubviews(new UIView[] { viewDelete, viewCameraActions });
            View.AddSubview(viewCamera);
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
            UISlider zoomSlider = new UISlider(new CGRect(16, 0, ViewWidth - 32, 20)) { };
            zoomSlider.MinimumTrackTintColor = UIColor.White;
            zoomSlider.ValueChanged += (sender, e) =>
            {
                Debug.WriteLine("zoomSlider ValueChanged");
            };

            UIView viewGallery = new UIView(new CGRect(16, zoomSlider.Frame.GetMaxY() + 22, size, size));
            viewGallery.Layer.CornerRadius = 4.0F;
            viewGallery.Layer.BorderWidth = 2.0F;
            viewGallery.Layer.BorderColor = UIColor.White.CGColor;
            viewGallery.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                Debug.WriteLine("viewGallery tapped");
            }));

            UIView viewCapture = new UIView(new CGRect((ViewWidth - size) / 2
                , zoomSlider.Frame.GetMaxY() + 22, size, size))
            { BackgroundColor = UIColor.FromWhiteAlpha(1, 0.80F) };
            viewCapture.Layer.CornerRadius = viewCapture.Frame.Width / 2;
            viewCapture.Layer.BorderWidth = 3.0F;
            viewCapture.Layer.BorderColor = UIColor.White.CGColor;
            viewCapture.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                Debug.WriteLine("viewCapture tapped");
            }));

            view.AddSubviews(new UIView[] { viewGallery, viewCapture, zoomSlider });
            CGRect viewFrame = new CGRect(0, viewBase.Frame.Height - viewCapture.Frame.GetMaxY() - 16
                , ViewWidth, viewCapture.Frame.GetMaxY() + 16);
            view.Frame = viewFrame;
            return view;
        }
    }
}