using System;
using CoreGraphics;
using UIKit;
using static myTNB.ScaleUtility;

namespace myTNB.Common
{
    internal class CommonInfoBar
    {
        private nfloat baseWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        private nfloat _baseMargin = 0;
        private nfloat _marginedWidth = 0;
        private nfloat _yLoc = 0;
        private string _message = string.Empty;
        private UILabel _lblDescription;

        internal CustomUIView View { private set; get; }
        internal Action OnTapAction { set; private get; }

        internal CommonInfoBar(string message, nfloat yLoc)
        {
            _message = message;
            _yLoc = yLoc;
            _baseMargin = ScaleUtility.GetScaledWidth(16);
            _marginedWidth = baseWidth - ScaleUtility.GetScaledWidth(32);
            ConstructInfoBar();
        }

        private void ConstructInfoBar()
        {
            View = new CustomUIView(new CGRect(0, _yLoc, baseWidth, GetScaledHeight(24)))
            {
                BackgroundColor = UIColor.White,
                PageName = "InfoBar",
                EventName = "DisplayPopup"
            };
            CustomUIView viewInfo = new CustomUIView(new CGRect(_baseMargin, 0, _marginedWidth, GetScaledHeight(24)))
            {
                BackgroundColor = MyTNBColor.IceBlue
            };
            UIImageView imgView = new UIImageView(new CGRect(GetScaledWidth(4)
                , GetScaledHeight(4), GetScaledWidth(16), GetScaledWidth(16)))
            {
                Image = UIImage.FromBundle("IC-Info-Blue")
            };
            _lblDescription = new UILabel(new CGRect(GetScaledWidth(28)
               , GetScaledHeight(4), View.Frame.Width - GetScaledWidth(44), GetScaledHeight(16)))
            {
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = _message
            };
            viewInfo.Layer.CornerRadius = GetScaledHeight(12);
            viewInfo.AddSubviews(new UIView[] { imgView, _lblDescription });
            View.AddSubview(viewInfo);
            View.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                OnTapAction?.Invoke();
            }));
        }

        internal string Message
        {
            set
            {
                if (value.IsValid())
                {
                    _lblDescription.Text = value;
                }
            }
        }
    }
}