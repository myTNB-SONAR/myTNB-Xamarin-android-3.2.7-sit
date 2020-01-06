using System;
using CoreGraphics;
using Foundation;
using myTNB.SSMR;
using UIKit;

namespace myTNB
{
    public class SSMRHistoryTutorialOverlay : BaseComponent
    {
        UIView _parentView, _containerView;
        public Func<string, string> GetI18NValue;
        public Action OnDismissAction;
        public nfloat TopViewYPos, HeaderHeight;

        public SSMRHistoryTutorialOverlay(UIView parent)
        {
            _parentView = parent;
        }

        private void CreateView()
        {
            nfloat width = _parentView.Frame.Width;
            nfloat height = _parentView.Frame.Height;
            _containerView = new UIView(new CGRect(0, 0, width, height))
            {
                BackgroundColor = UIColor.Clear,
                UserInteractionEnabled = true
            };

            UITapGestureRecognizer doubleTap = new UITapGestureRecognizer(OnDoubleTapAction);
            doubleTap.NumberOfTapsRequired = 2;
            _containerView.AddGestureRecognizer(doubleTap);

            _containerView.AddSubview(GetHistoryView());
        }

        public UIView GetView()
        {
            CreateView();
            return _containerView;
        }

        private void OnDoubleTapAction()
        {
            OnDismissAction?.Invoke();
        }

        private UIView GetHistoryView()
        {
            UIView parentView = new UIView(_parentView.Bounds)
            {
                BackgroundColor = UIColor.Clear
            };
            nfloat width = parentView.Frame.Width;
            nfloat height = parentView.Frame.Height;
            UIView topView = new UIView(new CGRect(0, 0, width, TopViewYPos + GetScaledHeight(15F)))
            {
                BackgroundColor = MyTNBColor.Black60
            };
            UIView topLine = new UIView(new CGRect(0, topView.Frame.GetMaxY() - GetScaledHeight(1F), width, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            topView.AddSubview(topLine);
            UIView verticalLine = new UIView(new CGRect(GetScaledWidth(35.5F), topView.Frame.GetMaxY() - GetScaledHeight(176F), GetScaledWidth(1F), GetScaledHeight(176F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            topView.AddSubview(verticalLine);
            UIView circle = new UIView(new CGRect(verticalLine.Frame.GetMinX() - GetScaledWidth(4F) + GetScaledWidth(.5F), verticalLine.Frame.GetMinY(), GetScaledWidth(8F), GetScaledHeight(8F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            circle.Layer.CornerRadius = GetScaledWidth(8F) / 2;
            topView.AddSubview(circle);
            nfloat textYPos = circle.Frame.GetMinY() + (circle.Frame.Height / 2) - (GetScaledHeight(20F) / 2);
            nfloat textXPos = GetXLocationFromFrame(circle.Frame, 12F);
            nfloat textPadding = GetScaledWidth(49F);
            nfloat textWidth = width - (textXPos + textPadding);
            UILabel title = new UILabel(new CGRect(textXPos, textYPos, textWidth, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.ButterScotch,
                TextAlignment = UITextAlignment.Left,
                Text = GetI18NValue(SSMRConstants.I18N_TutorialHeaderTitle)
            };
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(GetI18NValue(SSMRConstants.I18N_TutorialHeaderDesc)
                , ref htmlBodyError, TNBFont.FONTNAME_300, (float)GetScaledHeight(14F));
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = UIColor.White,
                ParagraphStyle = new NSMutableParagraphStyle
                {
                    Alignment = UITextAlignment.Left,
                    LineSpacing = 3.0f
                }
            }, new NSRange(0, htmlBody.Length));

            UITextView description = new UITextView(new CGRect(textXPos, GetYLocationFromFrame(title.Frame, 8F), textWidth, GetScaledHeight(60F)))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                UserInteractionEnabled = false
            };
            description.TextContainer.LineFragmentPadding = 0F;
            CGSize cGSize = description.SizeThatFits(new CGSize(textWidth, GetScaledHeight(80F)));
            ViewHelper.AdjustFrameSetHeight(description, cGSize.Height);
            UIButton btnGotIt = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(textXPos, GetYLocationFromFrame(description.Frame, 16F), GetScaledWidth(142F), GetScaledHeight(48F)),
                Font = TNBFont.MuseoSans_16_500,
                BackgroundColor = UIColor.White,
                UserInteractionEnabled = true
            };
            btnGotIt.SetTitleColor(MyTNBColor.WaterBlue, UIControlState.Normal);
            btnGotIt.SetTitle(GetCommonI18NValue(Constants.Common_GotIt), UIControlState.Normal);
            btnGotIt.Layer.CornerRadius = GetScaledHeight(4F);
            btnGotIt.Layer.BorderColor = UIColor.White.CGColor;
            btnGotIt.TouchUpInside += (sender, e) =>
            {
                OnDismissAction?.Invoke();
            };
            topView.AddSubviews(new UIView { title, description });
            topView.AddSubview(btnGotIt);
            nfloat bottomViewYPos = topView.Frame.GetMaxY() + HeaderHeight - GetScaledHeight(15F);
            UIView bottomView = new UIView(new CGRect(0, bottomViewYPos, width, height - bottomViewYPos))
            {
                BackgroundColor = MyTNBColor.Black60
            };
            UIView bottomLine = new UIView(new CGRect(0, 0, width, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            bottomView.AddSubview(bottomLine);
            parentView.AddSubview(topView);
            parentView.AddSubviews(bottomView);
            return parentView;
        }
    }
}
