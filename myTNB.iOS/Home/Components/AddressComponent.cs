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
                                                , _parentView.Frame.Width - 84, 72));
            _lblAddress.Font = myTNBFont.MuseoSans12();
            _lblAddress.TextAlignment = UITextAlignment.Center;
            _lblAddress.TextColor = UIColor.White;
            _lblAddress.Lines = 0;
            _lblAddress.LineBreakMode = UILineBreakMode.TailTruncation;
            _lblAddress.Text = "- - -";
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