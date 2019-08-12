using System;
using CoreGraphics;
using myTNB.Model;
using myTNB.SSMR;
using UIKit;

namespace myTNB
{
    public class SSMRReadingHistoryHeaderComponent
    {
        private readonly UIView _parentView;
        UIView _containerView;
        UIImageView _imageView;
        UILabel _labelTitle;
        UITextView _txtDesc;
        UIButton _btnSubmit;
        public Action OnButtonTap;

        string _buttonText;
        string _imageIconString;
        bool _isBtnHidden;

        nfloat _padding = 16f;

        public SSMRReadingHistoryHeaderComponent(UIView parentView)
        {
            _parentView = parentView;
        }

        private void CreateComponent()
        {
            _containerView = new UIView(new CGRect(0, 0, _parentView.Frame.Width, 300f))
            {
                BackgroundColor = UIColor.Clear
            };
            _imageView = new UIImageView(new CGRect(DeviceHelper.GetCenterXWithObjWidth(DeviceHelper.GetScaledWidth(131.0f), _containerView), 25, DeviceHelper.GetScaledWidth(131.0f), DeviceHelper.GetScaledHeight(144.0f)))
            {
                Image = UIImage.FromBundle(_imageIconString),
                ContentMode = UIViewContentMode.ScaleAspectFit,
                BackgroundColor = UIColor.Clear
            };
            _labelTitle = new UILabel(new CGRect(_padding, _imageView.Frame.GetMaxY() + _padding, _parentView.Frame.Width - (_padding * 2), 24f))
            {
                Font = MyTNBFont.MuseoSans16_500,
                TextColor = MyTNBColor.WaterBlue,
                TextAlignment = UITextAlignment.Center,
                Lines = 0,
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            _txtDesc = new UITextView(new CGRect(_padding, _labelTitle.Frame.GetMaxY() + 8f, _parentView.Frame.Width - (_padding * 2), 60f))
            {
                Font = MyTNBFont.MuseoSans14_300,
                TextColor = MyTNBColor.GreyishBrownTwo,
                TextAlignment = UITextAlignment.Center,
                Editable = false,
                BackgroundColor = UIColor.Clear
            };
            _btnSubmit = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(_padding, _txtDesc.Frame.GetMaxY() + 17f, _parentView.Frame.Width - (_padding * 2), DeviceHelper.GetScaledHeight(48f)),
                Hidden = _isBtnHidden,
                BackgroundColor = MyTNBColor.FreshGreen,
                Font = MyTNBFont.MuseoSans16_500
            };
            _btnSubmit.Layer.CornerRadius = 4;
            _btnSubmit.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            _btnSubmit.Layer.BorderWidth = 1;
            _btnSubmit.SetTitle(_buttonText, UIControlState.Normal);
            _btnSubmit.SetTitleColor(UIColor.White, UIControlState.Normal);
            _btnSubmit.TouchUpInside += (sender, e) =>
            {
                OnButtonTap?.Invoke();
            };
            _containerView.AddSubviews(new UIView { _imageView, _labelTitle, _txtDesc });
            _containerView.AddSubview(_btnSubmit);
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _containerView;
        }

        public UIView GetView()
        {
            return _containerView;
        }

        public void SetImageIconText(MeterReadingHistoryModel model)
        {
            if (model != null)
            {
                var ctaChar = model.DashboardCTAType.ToLower();
                if (ctaChar == DashboardHomeConstants.CTA_ShowReadingHistory)
                {
                    _imageIconString = model.IsCurrentPeriodSubmitted ? SSMRConstants.IMG_SMROpenIcon : SSMRConstants.IMG_SMRClosedIcon;
                }
                else if (ctaChar == DashboardHomeConstants.CTA_ShowSubmitReading)
                {
                    _imageIconString = SSMRConstants.IMG_SMROpenIcon;
                }
            }
        }

        public void SetSubmitButtonHidden(MeterReadingHistoryModel model)
        {
            if (model != null)
            {
                var ctaChar = model.DashboardCTAType.ToLower();
                if (ctaChar == DashboardHomeConstants.CTA_ShowReadingHistory)
                {
                    _isBtnHidden = true;
                }
                else if (ctaChar == DashboardHomeConstants.CTA_ShowSubmitReading)
                {
                    _isBtnHidden = model.IsCurrentPeriodSubmitted;
                }
                _buttonText = model.DashboardCTAText ?? string.Empty;
            }
        }

        public void SetTitle(string text)
        {
            _labelTitle.Text = text ?? string.Empty;
            CGSize labelNewSize = CustomUILabel.GetLabelSize(_labelTitle, _parentView.Frame.Width - (_padding * 2), 1000f);
            CGRect frame = _labelTitle.Frame;
            frame.Height = labelNewSize.Height;
            _labelTitle.Frame = frame;
        }

        public void SetDescription(string text)
        {
            _txtDesc.Text = text ?? string.Empty;
            CGSize labelNewSize = _txtDesc.SizeThatFits(new CGSize(_parentView.Frame.Width - (_padding * 2), 1000f));
            CGRect frame = _txtDesc.Frame;
            frame.Height = labelNewSize.Height;
            _txtDesc.Frame = frame;

            AdjustViewFrames();
        }

        private void AdjustViewFrames()
        {
            if (!_isBtnHidden)
            {
                CGRect frame = _btnSubmit.Frame;
                frame.Y = _txtDesc.Frame.GetMaxY() + 17f;
                _btnSubmit.Frame = frame;
            }
            CGRect containerFrame = _containerView.Frame;
            containerFrame.Height = _isBtnHidden ? _txtDesc.Frame.GetMaxY() + _padding : _btnSubmit.Frame.GetMaxY() + _padding;
            _containerView.Frame = containerFrame;
        }
    }
}
