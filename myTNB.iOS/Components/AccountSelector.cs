using System;
using CoreGraphics;
using UIKit;

namespace myTNB.Components
{
    public class AccountSelector
    {
        public AccountSelector() { }

        protected const string IMG_Dropdown = "IC-Header-Dropdown";
        protected const string Empty = "---";

        private CustomUIView _mainView, _viewContainer;
        private UILabel _lblTitle;
        private UIImageView _imgDropDown;
        private nfloat _width;

        private void CreateUI()
        {
            _width = UIScreen.MainScreen.Bounds.Width;
            nfloat height = ScaleUtility.GetScaledHeight(24);
            _mainView = new CustomUIView(new CGRect(0, 0, _width, height));
            _viewContainer = new CustomUIView(new CGRect(0, 0, _width, height));

            _lblTitle = new UILabel(new CGRect(0, 0, _width, height))
            {
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Font = MyTNBFont.MuseoSans16_500V2
            };

            nfloat imgWidth = ScaleUtility.GetScaledWidth(16);
            _imgDropDown = new UIImageView(new CGRect(0, (height - imgWidth) / 2, imgWidth, imgWidth))
            { Image = UIImage.FromBundle(IMG_Dropdown) };

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
                    val = Empty;
                }
                _lblTitle.Text = val;
                CGSize size = CustomUILabel.GetLabelSize(_lblTitle, _lblTitle.Frame.Width, _lblTitle.Frame.Height);
                CGRect lblFrame = _lblTitle.Frame;
                lblFrame.Width = size.Width;
                _lblTitle.Frame = lblFrame;

                CGRect imgFrame = _imgDropDown.Frame;
                imgFrame.X = _lblTitle.Frame.GetMaxX() + ScaleUtility.GetScaledWidth(8F);
                _imgDropDown.Frame = imgFrame;

                CGRect containerFrame = _viewContainer.Frame;
                containerFrame.Width = _imgDropDown.Frame.GetMaxX();
                containerFrame.X = (_width - containerFrame.Width) / 2;
                _viewContainer.Frame = containerFrame;
            }
        }
    }
}
