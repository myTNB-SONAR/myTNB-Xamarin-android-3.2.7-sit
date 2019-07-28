using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB.Home.Components
{
    public class AccountStatusComponent
    {
        readonly UIView _parentView;
        UIView _precedingView;
        UIView _statusView;
        UIView _lineView;
        UITextView _txtStatus;
        UILabel _lblToolTip;
        UITapGestureRecognizer _tapGestureRecognizer = new UITapGestureRecognizer();
        string _statusMsg = string.Empty;
        string _tooltipText = string.Empty;

        public AccountStatusComponent(UIView parentView, UIView precedingView)
        {
            _parentView = parentView;
            _precedingView = precedingView;
        }

        public void CreateComponent()
        {
            _statusView = new UIView(new CGRect(16, _precedingView.Frame.GetMaxY() + 12f
                , _parentView.Frame.Width - 32, 75));

            _lineView = new UIView(new CGRect(0, 0, (float)_statusView.Frame.Width, 1))
            {
                BackgroundColor = MyTNBColor.SelectionSemiTransparent
            };
            _statusView.AddSubviews(new UIView { _lineView });

            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(_statusMsg
                , ref htmlBodyError, MyTNBFont.FONTNAME_300, 12f);
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = UIColor.White,
                ParagraphStyle = new NSMutableParagraphStyle
                {
                    Alignment = UITextAlignment.Center
                }
            }, new NSRange(0, htmlBody.Length));

            _txtStatus = new UITextView(new CGRect(0, _lineView.Frame.GetMaxY() + 8f
                , (float)_statusView.Frame.Width, 25))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody
            };

            _statusView.AddSubview(_txtStatus);

            _lblToolTip = new UILabel(new CGRect(0, _txtStatus.Frame.GetMaxY() + 4f, (float)_statusView.Frame.Width, 16))
            {
                TextAlignment = UITextAlignment.Center,
                Font = MyTNBFont.MuseoSans12_500,
                TextColor = MyTNBColor.SunGlow,
                Text = _tooltipText,
                UserInteractionEnabled = true
            };
            _lblToolTip.AddGestureRecognizer(_tapGestureRecognizer);
            _statusView.AddSubview(_lblToolTip);

            _parentView.AddSubview(_statusView);
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _statusView;
        }

        public UIView GetView()
        {
            return _statusView;
        }

        public void SetStatusLabel(string label)
        {
            if (!string.IsNullOrWhiteSpace(label))
            {
                _statusMsg = label;
            }
        }

        public void SetToolTipLabel(string label)
        {
            if (!string.IsNullOrWhiteSpace(label))
            {
                _tooltipText = label;
            }
        }

        public void SetFrameByPrecedingView(float yLocation)
        {
            var newFrame = _statusView.Frame;
            newFrame.Y = yLocation + 12f;
            _statusView.Frame = newFrame;
        }

        public void SetEvent(UITapGestureRecognizer tapEvent)
        {
            _tapGestureRecognizer = tapEvent;
        }
    }
}
