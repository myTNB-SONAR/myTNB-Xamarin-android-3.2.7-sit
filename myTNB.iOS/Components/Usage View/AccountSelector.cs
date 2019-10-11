using System;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public class AccountSelector : BaseComponent
    {
        public AccountSelector(CustomUIView parentView)
        {
            _parentView = parentView;
        }

        private CustomUIView _parentView, _mainView, _viewContainer;
        private UILabel _lblTitle;
        private UIImageView _imgDropDown;
        private nfloat _width;

        private void CreateUI()
        {
            _width = _parentView.Frame.Width;
            nfloat height = GetScaledHeight(24);
            _mainView = new CustomUIView(new CGRect(0, 0, _width, height));
            _viewContainer = new CustomUIView(new CGRect(0, 0, _width, height));

            _lblTitle = new UILabel(new CGRect(0, 0, _width, height))
            {
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Font = TNBFont.MuseoSans_16_500
            };

            nfloat imgWidth = GetScaledWidth(16);
            _imgDropDown = new UIImageView(new CGRect(0, (height - imgWidth) / 2, imgWidth, imgWidth))
            { Image = UIImage.FromBundle(Constants.IMG_Dropdown) };

            _viewContainer.AddSubviews(new UIView[] { _lblTitle, _imgDropDown });
            _mainView.AddSubview(_viewContainer);
        }

        public CustomUIView GetUI()
        {
            CreateUI();
            return _mainView;
        }

        public void SetAction(Action action)
        {
            if (action == null) { return; }
            _mainView.AddGestureRecognizer(new UITapGestureRecognizer(() => { action.Invoke(); }));
        }

        public string Title
        {
            set
            {
                string val = value;
                if (string.IsNullOrEmpty(val) || string.IsNullOrWhiteSpace(val))
                {
                    val = LanguageUtility.GetHintI18NValue(Constants.Hint_EmptyAcctSelector);
                }
                _lblTitle.Text = val;
                CGSize size = CustomUILabel.GetLabelSize(_lblTitle, _width - GetScaledWidth(56), _lblTitle.Frame.Height);
                CGRect lblFrame = _lblTitle.Frame;
                lblFrame.Width = size.Width;
                _lblTitle.Frame = lblFrame;

                CGRect imgFrame = _imgDropDown.Frame;
                imgFrame.X = _lblTitle.Frame.GetMaxX() + GetScaledWidth(8F);
                _imgDropDown.Frame = imgFrame;

                CGRect containerFrame = _viewContainer.Frame;
                containerFrame.Width = _imgDropDown.Frame.GetMaxX();
                containerFrame.X = (_width - containerFrame.Width) / 2;
                _viewContainer.Frame = containerFrame;
            }
        }
    }
}