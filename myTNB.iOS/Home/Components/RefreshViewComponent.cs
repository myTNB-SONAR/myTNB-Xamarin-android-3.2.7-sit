using System;
using CoreGraphics;
using Foundation;
using myTNB.Dashboard.DashboardComponents;
using UIKit;

namespace myTNB.Home.Components
{
    public class RefreshViewComponent
    {
        UIView _headerView;
        UIView _parentView;
        UIView _viewRefreshScreen;
        UIImage _imgRefreshIcon;
        UIImageView _imgViewRefreshIcon;
        UILabel _lblDescription;
        public UIButton _btnRefresh;
        public Action OnButtonTap;
        string _descriptionMessage;
        string _buttonText;
        bool _isBtnHidden;
        bool _isIconHidden;
        bool _isDescriptionHidden;

        public RefreshViewComponent(UIView view, UIView headerView = null)
        {
            _parentView = view;
            _headerView = headerView;
        }

        internal void CreateComponent(bool forGradientBG = false)
        {
            float viewMargin = 10f;
            float imageTopMargin = forGradientBG ? 14f : 75f;
            float imageWidth = DeviceHelper.GetScaledWidth(96f);
            float imageHeight = DeviceHelper.GetScaledHeight(96f);
            float lineTextHeight = 24f;

            nfloat yPos = (_headerView != null) ? _headerView.Frame.GetMaxY() + viewMargin : viewMargin;
            nfloat viewHeight = _parentView.Frame.Height - yPos - viewMargin;

            if (DeviceHelper.IsIphoneXUpResolution())
            {
                if (DeviceHelper.IsIphoneXOrXs())
                {
                    yPos += 35f;
                }
                else
                {
                    yPos += 50f;
                }
            }
            else if (DeviceHelper.IsIphone6UpResolution())
            {
                yPos += 20f;
            }

            _viewRefreshScreen = new UIView(new CGRect(viewMargin, yPos, _parentView.Frame.Width - (viewMargin * 2), viewHeight))
            {
                BackgroundColor = UIColor.Clear
            };

            float labelWidth = (float)(_viewRefreshScreen.Frame.Width - 28f);
            float buttonWidth = (float)(_viewRefreshScreen.Frame.Width - 28f);
            float buttonHeight = 48f;

            _imgViewRefreshIcon = new UIImageView()
            {
                Frame = new CGRect(DeviceHelper.GetCenterXWithObjWidth(imageWidth, _viewRefreshScreen), DeviceHelper.GetScaledHeightWithY(imageTopMargin), imageWidth, imageHeight),
                Image = _imgRefreshIcon,
                Hidden = _isIconHidden
            };

            var descMsg = _descriptionMessage ?? string.Empty;
            var btnText = _buttonText ?? "Error_RefreshBtnTitle".Translate();

            NSMutableParagraphStyle msgParagraphStyle = new NSMutableParagraphStyle
            {
                Alignment = UITextAlignment.Center,
                MinimumLineHeight = lineTextHeight,
                MaximumLineHeight = lineTextHeight
            };

            UIStringAttributes msgAttributes = new UIStringAttributes
            {
                Font = MyTNBFont.MuseoSans16_300,
                ForegroundColor = forGradientBG ? UIColor.White : MyTNBColor.Grey,
                BackgroundColor = UIColor.Clear,
                ParagraphStyle = msgParagraphStyle
            };

            var attributedText = new NSMutableAttributedString(descMsg);
            attributedText.AddAttributes(msgAttributes, new NSRange(0, descMsg.Length));

            _lblDescription = new UILabel()
            {
                AttributedText = attributedText,
                Lines = 0,
                Hidden = _isDescriptionHidden
            };

            CGSize cGSize = _lblDescription.SizeThatFits(new CGSize(labelWidth, 1000f));
            _lblDescription.Frame = new CGRect(14f, _imgViewRefreshIcon.Frame.GetMaxY() + 24f, labelWidth, cGSize.Height);

            _btnRefresh = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(14f, _lblDescription.Frame.GetMaxY() + 24f, buttonWidth, buttonHeight),
                Hidden = _isBtnHidden,
                BackgroundColor = forGradientBG ? UIColor.White : MyTNBColor.FreshGreen,
                Font = MyTNBFont.MuseoSans16_500
            };

            _btnRefresh.Layer.CornerRadius = 4;
            _btnRefresh.Layer.BorderColor = forGradientBG ? UIColor.White.CGColor : MyTNBColor.FreshGreen.CGColor;
            _btnRefresh.Layer.BorderWidth = 1;
            _btnRefresh.SetTitle(btnText, UIControlState.Normal);
            _btnRefresh.SetTitleColor(forGradientBG ? MyTNBColor.PowerBlue : UIColor.White, UIControlState.Normal);
            _btnRefresh.TouchUpInside += (sender, e) =>
            {
                OnButtonTap?.Invoke();
            };

            _viewRefreshScreen.AddSubviews(new UIView[] { _imgViewRefreshIcon, _lblDescription, _btnRefresh });
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _viewRefreshScreen;
        }

        public UIView GetUIForGradientBG()
        {
            CreateComponent(true);
            return _viewRefreshScreen;
        }

        public UIView GetView()
        {
            return _viewRefreshScreen;
        }

        public void SetDescription(string desc)
        {
            _descriptionMessage = desc;
        }

        public void SetButtonText(string text)
        {
            _buttonText = text;
        }

        public void SetIconImage(string img)
        {
            _imgRefreshIcon = UIImage.FromBundle(img);
        }

        public void SetIconImageHidden(bool flag)
        {
            _isIconHidden = flag;
        }

        public void SetDescriptionHidden(bool flag)
        {
            _isDescriptionHidden = flag;
        }

        public void SetRefreshButtonHidden(bool flag)
        {
            _isBtnHidden = flag;
        }
    }
}
