using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using myTNB.Model.Usage;
using UIKit;

namespace myTNB
{
    public class TariffLegendComponent : BaseComponent
    {
        CustomUIView _containerView;
        UIView _parentView;
        List<LegendItemModel> _tariffLegendList = new List<LegendItemModel>();
        nfloat _totalHeight;
        public Func<string, string> GetI18NValue;

        public TariffLegendComponent(UIView parentView, List<LegendItemModel> tariffLegendList)
        {
            _parentView = parentView;
            _tariffLegendList = tariffLegendList;
        }

        private void CreateComponent()
        {
            nfloat width = _parentView.Frame.Width;
            nfloat height = _parentView.Frame.Height;
            _containerView = new CustomUIView(new CGRect(0, 0, width, height))
            {
                BackgroundColor = UIColor.Clear
            };
            for (int i = 0; i < _tariffLegendList.Count; i++)
            {
                _containerView.AddSubview(LegendItemView(i));
            }
            if (_tariffLegendList.Count > 0)
            {
                CreateNote();
            }
            AdjustContainerHeight();
        }

        public CustomUIView GetUI()
        {
            CreateComponent();
            return _containerView;
        }

        private void CreateNote()
        {
            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(GetI18NValue(UsageConstants.I18N_TariffLegendNote)
                , ref htmlBodyError, TNBFont.FONTNAME_300, (float)GetScaledHeight(10F));
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

            UITextView note = new UITextView(new CGRect(GetScaledWidth(24f), _totalHeight + GetScaledHeight(5F), _parentView.Frame.Width - (GetScaledWidth(24f) * 2), GetScaledHeight(60F)))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                UserInteractionEnabled = false,
                TextContainerInset = UIEdgeInsets.Zero
            };
            CGSize cGSize = note.SizeThatFits(new CGSize(note.Frame.Width, GetScaledHeight(500F)));
            ViewHelper.AdjustFrameSetHeight(note, cGSize.Height);
            _totalHeight = note.Frame.GetMaxY();
            _containerView.AddSubview(note);
        }

        private UIView LegendItemView(int index)
        {
            nfloat viewHeight = GetScaledHeight(14f) + GetScaledHeight(11f);
            nfloat viewXPos = GetScaledWidth(24f);
            nfloat viewYPos = viewHeight * index;
            nfloat viewWidth = _parentView.Frame.Width;

            UIView view = new UIView(new CGRect(viewXPos, viewYPos, viewWidth, viewHeight))
            {
                BackgroundColor = UIColor.Clear
            };

            nfloat coulourViewWidth = GetScaledWidth(14f);
            nfloat coulourViewHeight = GetScaledWidth(14f);
            UIView colourView = new UIView(new CGRect(0, 0, coulourViewWidth, coulourViewHeight))
            {
                BackgroundColor = _tariffLegendList[index].Colour
            };
            colourView.Layer.CornerRadius = GetScaledHeight(7f);
            view.AddSubview(colourView);

            nfloat labelViewMargin = GetScaledWidth(24f);
            nfloat labelViewXPos = colourView.Frame.GetMaxX() + GetScaledWidth(12f);
            nfloat labelViewWidth = viewWidth - viewXPos - labelViewXPos - labelViewMargin;
            nfloat labelWidth = labelViewWidth / 2;
            nfloat labelHeight = GetScaledHeight(14f);
            UIView labelView = new UIView(new CGRect(labelViewXPos, 0, labelViewWidth, labelHeight))
            {
                BackgroundColor = UIColor.Clear
            };
            view.AddSubview(labelView);

            UILabel rangeLabel = new UILabel(new CGRect(0, 0, labelWidth, labelHeight))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_10_300,
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Left,
                Text = _tariffLegendList[index].BlockRange
            };
            labelView.AddSubview(rangeLabel);

            UILabel priceLabel = new UILabel(new CGRect(labelWidth, 0, labelWidth, labelHeight))
            {
                BackgroundColor = UIColor.Clear,
                Font = TNBFont.MuseoSans_10_300,
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Right,
                Text = _tariffLegendList[index].BlockPrice
            };
            labelView.AddSubview(priceLabel);
            _totalHeight = view.Frame.GetMaxY();
            return view;
        }

        private void AdjustContainerHeight()
        {
            ViewHelper.AdjustFrameSetHeight(_containerView, _totalHeight);
        }
    }
}