using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB
{
    public class UsageSSMRTutorialOverlay : BaseComponent
    {
        UIView _parentView, _containerView;
        public Func<string, string> GetI18NValue;
        public Action OnDismissAction;
        public nfloat NavigationHeight, SSMRCardYPos, SSMRCardHeight;

        public UsageSSMRTutorialOverlay(UIView parent)
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

            _containerView.AddSubview(GetSMRView());
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

        private UIView GetSMRView()
        {
            UIView parentView = new UIView(_parentView.Bounds)
            {
                BackgroundColor = UIColor.Clear
            };
            nfloat width = parentView.Frame.Width;
            nfloat height = parentView.Frame.Height;
            UIView topView = new UIView(new CGRect(0, 0, width, NavigationHeight + SSMRCardYPos))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            nfloat boxViewXPos = GetScaledWidth(16F);
            nfloat boxViewWidth = width - GetScaledWidth(32F);
            nfloat boxViewHeight = SSMRCardHeight;
            UIView boxView = new UIView(new CGRect(boxViewXPos - GetScaledWidth(1F), topView.Frame.GetMaxY() - GetScaledHeight(1F), boxViewWidth + GetScaledWidth(2F), boxViewHeight + GetScaledHeight(2F)))
            {
                BackgroundColor = UIColor.Clear
            };
            boxView.Layer.CornerRadius = GetScaledHeight(4F);
            boxView.Layer.BorderColor = MyTNBColor.ButterScotch.CGColor;
            boxView.Layer.BorderWidth = GetScaledWidth(1F);
            nfloat bottomViewYPos = boxView.Frame.GetMaxY() - GetScaledHeight(1F);
            UIView bottomView = new UIView(new CGRect(0, bottomViewYPos, width, height - bottomViewYPos))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            UIView leftView = new UIView(new CGRect(0, topView.Frame.GetMaxY(), GetScaledWidth(16F), boxView.Frame.Height - GetScaledHeight(2F)))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            UIView rightView = new UIView(new CGRect(boxView.Frame.GetMaxX() - GetScaledWidth(1F), topView.Frame.GetMaxY(), GetScaledWidth(16F) + GetScaledWidth(1F), boxView.Frame.Height - GetScaledHeight(2F)))
            {
                BackgroundColor = MyTNBColor.Black75
            };
            nfloat verticalLineHeight = GetScaledHeight(185F);
            nfloat verticalLineYPos = topView.Frame.GetMaxY() - verticalLineHeight;
            UIView verticalLine = new UIView(new CGRect(GetScaledWidth(35.5F), verticalLineYPos, GetScaledWidth(1F), verticalLineHeight))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            nfloat circleYPos = verticalLine.Frame.GetMinY();
            UIView circle = new UIView(new CGRect(verticalLine.Frame.GetMinX() - GetScaledWidth(4F) + GetScaledWidth(.5F), circleYPos, GetScaledWidth(8F), GetScaledHeight(8F)))
            {
                BackgroundColor = MyTNBColor.ButterScotch
            };
            circle.Layer.CornerRadius = GetScaledWidth(8F) / 2;
            topView.AddSubviews(new UIView { verticalLine, circle });
            nfloat textYPos = circle.Frame.GetMinY() + (circle.Frame.Height / 2) - (GetScaledHeight(20F) / 2);
            nfloat textXPos = GetXLocationFromFrame(circle.Frame, 12F);
            nfloat textPadding = GetScaledWidth(30F);
            nfloat textWidth = width - (textXPos + textPadding);
            UILabel title = new UILabel(new CGRect(textXPos, textYPos, textWidth, GetScaledHeight(20F)))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.ButterScotch,
                TextAlignment = UITextAlignment.Left,
                Text = GetI18NValue(UsageConstants.I18N_TutorialSMRTitle)
            };
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(GetI18NValue(UsageConstants.I18N_TutorialSMRDesc)
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
            parentView.AddSubview(topView);
            parentView.AddSubviews(new UIView { bottomView, leftView, rightView, boxView });
            return parentView;
        }
    }
}
