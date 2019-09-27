using System;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using myTNB.SSMR;
using UIKit;

namespace myTNB
{
    public class SSMRComponent : BaseComponent
    {
        private readonly UIView _parentView;
        CustomUIView _containerView;
        UIView _iconLabelView;
        UIImageView _iconView;
        public UILabel _labelViewHistory;
        UILabel _description;
        public UIButton _smrButton;
        nfloat _descWidth = 0f;

        public SSMRComponent(UIView parentView)
        {
            _parentView = parentView;
        }

        private void CreateComponent()
        {
            nfloat iconPadding = GetScaledWidth(12f);
            nfloat imageWidth = GetScaledWidth(32f);
            nfloat imageHeight = GetScaledHeight(32f);
            nfloat buttonHeight = GetScaledHeight(40f);
            nfloat width = _parentView.Frame.Width;

            _containerView = new CustomUIView(new CGRect(BaseMarginWidth16, 0, width - (BaseMarginWidth16 * 2), GetScaledHeight(116f)))
            {
                BackgroundColor = UIColor.White
            };
            _containerView.Layer.CornerRadius = GetScaledHeight(4.0f);

            _iconView = new UIImageView(new CGRect(BaseMarginWidth16, GetScaledHeight(8f), imageWidth, imageHeight))
            {
                Image = UIImage.FromBundle(SSMRConstants.IMG_SMRMediumIcon)
            };
            _iconLabelView = new UIView(new CGRect(0, BaseMarginHeight16, _containerView.Frame.Width, GetScaledHeight(48f)))
            {
                BackgroundColor = UIColor.Clear
            };
            _containerView.AddSubview(_iconLabelView);
            _descWidth = _iconLabelView.Frame.Width - (_iconView.Frame.GetMaxX() + BaseMarginWidth16 + iconPadding);
            _description = new UILabel(new CGRect(_iconView.Frame.GetMaxX() + iconPadding, 0, _descWidth, GetScaledHeight(48)))
            {
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_12_300,
                TextColor = MyTNBColor.GreyishBrown,
                Lines = 0,
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            _labelViewHistory = new UILabel(new CGRect(_iconView.Frame.GetMaxX() + iconPadding, _description.Frame.GetMaxY(), _descWidth, GetScaledHeight(20f)))
            {
                UserInteractionEnabled = true,
                BackgroundColor = UIColor.Clear,
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                Lines = 0,
                LineBreakMode = UILineBreakMode.TailTruncation,
                Hidden = true
            };
            _smrButton = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(BaseMarginWidth16, _description.Frame.GetMaxY() + BaseMarginHeight16, _containerView.Frame.Width - (BaseMarginWidth16 * 2), buttonHeight)
            };
            _smrButton.Layer.CornerRadius = 4;
            _smrButton.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            _smrButton.Layer.BorderWidth = 1;
            _smrButton.Font = TNBFont.MuseoSans_16_500;
            _smrButton.SetTitleColor(MyTNBColor.FreshGreen, UIControlState.Normal);
            _smrButton.Enabled = true;

            _iconLabelView.AddSubviews(new UIView { _iconView, _description });
            _iconLabelView.AddSubview(_labelViewHistory);
            _containerView.AddSubview(_iconLabelView);
            _containerView.AddSubview(_smrButton);
        }

        public virtual CustomUIView GetShimmerUI()
        {
            nfloat baseWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
            nfloat iconPadding = GetScaledWidth(12f);
            nfloat baseHeight = GetScaledHeight(128f);
            nfloat imageWidth = GetScaledWidth(32f);
            nfloat imageHeight = GetScaledHeight(32f);
            nfloat buttonHeight = GetScaledHeight(40f);
            CustomShimmerView shimmeringView = new CustomShimmerView();
            CustomUIView parentView = new CustomUIView(new CGRect(BaseMarginWidth16, 0
                , baseWidth - (BaseMarginWidth16 * 2), baseHeight))
            { BackgroundColor = UIColor.White };
            parentView.Layer.CornerRadius = GetScaledHeight(4f);
            UIView viewShimmerParent = new UIView(new CGRect(new CGPoint(0, 0), parentView.Frame.Size)) { BackgroundColor = UIColor.Clear };
            UIView viewShimmerContent = new UIView(new CGRect(new CGPoint(0, 0), parentView.Frame.Size)) { BackgroundColor = UIColor.Clear };
            parentView.AddSubviews(new UIView[] { viewShimmerParent, viewShimmerContent });

            UIView iconView = new UIView(new CGRect(BaseMarginWidth16, BaseMarginHeight16, imageWidth, imageHeight))
            {
                BackgroundColor = MyTNBColor.PaleGrey
            };
            iconView.Layer.CornerRadius = GetScaledHeight(16f);
            UIView labelView = new UIView(new CGRect(iconView.Frame.GetMaxX() + iconPadding, BaseMarginHeight16, parentView.Frame.Width - GetScaledWidth(28f) - iconView.Frame.GetMaxX(), GetScaledHeight(16)))
            {
                BackgroundColor = MyTNBColor.PaleGrey
            };
            labelView.Layer.CornerRadius = GetScaledHeight(2f);

            UIView labelView2 = new UIView(new CGRect(iconView.Frame.GetMaxX() + iconPadding, labelView.Frame.GetMaxY() + GetScaledHeight(8f), parentView.Frame.Width - GetScaledWidth(28f) - iconView.Frame.GetMaxX(), GetScaledHeight(16)))
            {
                BackgroundColor = MyTNBColor.PaleGrey
            };
            labelView2.Layer.CornerRadius = GetScaledHeight(2f);

            UIButton smrButton = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(BaseMarginWidth16, labelView2.Frame.GetMaxY() + BaseMarginHeight16, parentView.Frame.Width - (BaseMarginWidth16 * 2), buttonHeight)
            };
            smrButton.Layer.CornerRadius = 4;
            smrButton.Layer.BorderColor = MyTNBColor.SilverChalice.CGColor;
            smrButton.Layer.BorderWidth = 1;
            smrButton.Font = TNBFont.MuseoSans_16_500;
            smrButton.SetTitleColor(MyTNBColor.SilverChalice, UIControlState.Normal);
            smrButton.SetTitle(LanguageUtility.GetCommonI18NValue(Constants.I18N_ViewReadHistory), UIControlState.Normal);
            smrButton.Enabled = false;

            parentView.AddSubview(smrButton);
            viewShimmerContent.AddSubviews(new UIView { iconView, labelView, labelView2 });
            viewShimmerParent.AddSubview(shimmeringView);
            shimmeringView.ContentView = viewShimmerContent;
            shimmeringView.Shimmering = true;
            shimmeringView.SetValues();

            return parentView;
        }

        public CustomUIView GetUI()
        {
            CreateComponent();
            return _containerView;
        }

        public UIView GetView()
        {
            return _containerView;
        }

        public void SetDescription(string text)
        {
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(text
                , ref htmlBodyError, MyTNBFont.FONTNAME_300, (float)GetScaledHeight(12F));
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.CharcoalGrey
            }, new NSRange(0, htmlBody.Length));
            _description.AttributedText = mutableHTMLBody;
            CGSize descNewSize = _description.SizeThatFits(new CGSize(_descWidth, 1000f));
            ViewHelper.AdjustFrameSetHeight(_description, descNewSize.Height);

            AdjustViewFrames();
        }

        private void AdjustViewFrames(bool linkIsVisible = false)
        {
            ViewHelper.AdjustFrameSetHeight(_iconLabelView, linkIsVisible ? _labelViewHistory.Frame.GetMaxY() : _description.Frame.GetMaxY());
            ViewHelper.AdjustFrameSetY(_smrButton, _iconLabelView.Frame.GetMaxY() + BaseMarginHeight16);
            ViewHelper.AdjustFrameSetHeight(_containerView, _smrButton.Frame.GetMaxY() + BaseMarginHeight16);
            ViewHelper.AdjustFrameSetY(_iconView, GetYLocationToCenterObject(_iconView.Frame.Height, _iconLabelView));
        }

        public void SetSRMButtonEnable(bool isDisabled)
        {
            _smrButton.Enabled = !isDisabled;
            _smrButton.SetTitleColor(!isDisabled ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice, UIControlState.Normal);
            _smrButton.Layer.BorderColor = !isDisabled ? MyTNBColor.FreshGreen.CGColor : MyTNBColor.SilverChalice.CGColor;
        }

        public void SetButtonText(string text)
        {
            _smrButton.SetTitle(text, UIControlState.Normal);
        }

        public void ShowHistoryLink(bool showLink, string text)
        {
            _labelViewHistory.Hidden = !showLink;
            if (showLink)
            {
                _labelViewHistory.Text = text;
                CGSize labelNewSize = _labelViewHistory.SizeThatFits(new CGSize(_descWidth, 1000f));

                ViewHelper.AdjustFrameSetY(_labelViewHistory, _description.Frame.GetMaxY());
                ViewHelper.AdjustFrameSetHeight(_labelViewHistory, labelNewSize.Height);
            }
            AdjustViewFrames(showLink);
        }

        public nfloat GetContainerHeight()
        {
            nfloat height = 0;
            if (_containerView != null)
            {
                height = _containerView.Frame.Height;
            }
            return height;
        }
    }
}
