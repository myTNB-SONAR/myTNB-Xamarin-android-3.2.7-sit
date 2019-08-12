using CoreGraphics;
using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class AddressComponent
    {
        UIView _parentView;
        UILabel _lblAddress;

        public AddressComponent(UIView view)
        {
            _parentView = view;
        }

        internal void CreateCommponent()
        {

            _lblAddress = new UILabel(new CGRect(16, _parentView.Frame.Height - 56
                , _parentView.Frame.Width - 32, 72))
            {
                Font = MyTNBFont.MuseoSans12,
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                Lines = 0,
                LineBreakMode = UILineBreakMode.TailTruncation,
                Text = TNBGlobal.EMPTY_ADDRESS,
                BackgroundColor = UIColor.Clear
            };
            _parentView.AddSubview(_lblAddress);
        }

        public UILabel GetUI()
        {
            CreateCommponent();
            return _lblAddress;
        }

        public UILabel GetView()
        {
            return _lblAddress;
        }

        public void SetAddress(string address)
        {
            if (!string.IsNullOrEmpty(address))
            {
                _lblAddress.Text = address;
                CGSize addressNewSize = _lblAddress.SizeThatFits(new CGSize(_parentView.Frame.Width - 84, 1000f));
                CGRect frame = _lblAddress.Frame;
                frame.Height = addressNewSize.Height;
                _lblAddress.Frame = frame;
            }
        }

        /// <summary>
        /// Sets the frame by preceding view.
        /// </summary>
        /// <param name="yLocation">Preceding view frame.</param>
        public void SetFrameByPrecedingView(float yLocation)
        {
            var newFrame = _lblAddress.Frame;
            newFrame.Y = yLocation;
            _lblAddress.Frame = newFrame;
        }
    }
}