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

            _lblAddress = new UILabel(new CGRect(42, _parentView.Frame.Height - 56
                , _parentView.Frame.Width - 84, 72))
            {
                Font = MyTNBFont.MuseoSans12,
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                Lines = 0,
                LineBreakMode = UILineBreakMode.TailTruncation,
                Text = TNBGlobal.EMPTY_ADDRESS
            };
            _parentView.AddSubview(_lblAddress);
        }

        public UILabel GetUI()
        {
            CreateCommponent();
            return _lblAddress;
        }

        public void SetAddress(string address)
        {
            if (!string.IsNullOrEmpty(address))
            {
                _lblAddress.Text = address;
            }
        }

        /// <summary>
        /// Sets the frame by preceding view.
        /// </summary>
        /// <param name="precedingViewFrame">Preceding view frame.</param>
        public void SetFrameByPrecedingView(float yLocation)
        {
            var newFrame = _lblAddress.Frame;
            newFrame.Y = yLocation;
            _lblAddress.Frame = newFrame;
        }
    }
}