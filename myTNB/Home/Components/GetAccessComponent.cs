using CoreGraphics;
using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class GetAccessComponent
    {
        UIView _parentView;
        UIView _viewGetAccess;
        UILabel _lblTitle;
        UILabel _lblSubtitle;
        public UIButton _btnGetAccess;
        public GetAccessComponent(UIView view)
        {
            _parentView = view;
        }

        internal void CreateComponent()
        {
            int mainYLocation = 102;
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                mainYLocation = 136;
            }
            _viewGetAccess = new UIView(new CGRect(0, mainYLocation, _parentView.Frame.Width, 244));

            int yLocation = -20;
            if (_parentView.Frame.Width == 320)
            {
                yLocation = 10;
            }
            else if (_parentView.Frame.Width == 414)
            {
                yLocation = -40;
            }

            var imgWidth = _viewGetAccess.Frame.Width * .51;
            var xLocation = _viewGetAccess.Frame.Width / 2 - imgWidth / 2;
            UIImageView imgViewEmpty = new UIImageView(new CGRect(xLocation, yLocation, imgWidth, imgWidth));
            imgViewEmpty.Image = UIImage.FromBundle("Get-Access");
            imgViewEmpty.ContentMode = UIViewContentMode.ScaleAspectFit;
            _viewGetAccess.AddSubview(imgViewEmpty);

            _lblTitle = new UILabel(new CGRect(10, imgViewEmpty.Frame.GetMaxY() + 1, _viewGetAccess.Frame.Width - 20, 16));
            _lblTitle.TextAlignment = UITextAlignment.Center;
            _lblTitle.Font = myTNBFont.MuseoSans12_300();
            _lblTitle.Text = "View usage history";
            _lblTitle.TextColor = UIColor.White;
            _viewGetAccess.AddSubview(_lblTitle);

            _lblSubtitle = new UILabel(new CGRect(0, _lblTitle.Frame.GetMaxY(), _viewGetAccess.Frame.Width, 28));
            _lblSubtitle.TextAlignment = UITextAlignment.Center;
            //_lblSubtitle.Text = "You will require permission from the owner to view usage and transaction details.";
            _lblSubtitle.Text = "Only electricity account owners\r\nmay view usage and transaction details.";
            _lblSubtitle.Font = myTNBFont.MuseoSans9();
            _lblSubtitle.Lines = 2;
            _lblSubtitle.TextColor = UIColor.White;
            _lblSubtitle.LineBreakMode = UILineBreakMode.WordWrap;
            _viewGetAccess.AddSubview(_lblSubtitle);

            _btnGetAccess = new UIButton(UIButtonType.Custom);
            _btnGetAccess.Frame = new CGRect(90, _viewGetAccess.Frame.Height - 71, _viewGetAccess.Frame.Width - 180, 48);
            _btnGetAccess.Layer.CornerRadius = 4;
            _btnGetAccess.Layer.BorderColor = UIColor.White.CGColor;
            _btnGetAccess.Layer.BorderWidth = 1;
            _btnGetAccess.SetTitle("Get access", UIControlState.Normal);
            _btnGetAccess.Font = myTNBFont.MuseoSans16();
            _btnGetAccess.SetTitleColor(UIColor.White, UIControlState.Normal);
            //_viewGetAccess.AddSubview(_btnGetAccess);
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _viewGetAccess;
        }

        public void SetTitle(string title)
        {
            _lblTitle.Text = title;
        }

        public void SetSubtitle(string subtitle)
        {
            _lblSubtitle.Text = subtitle;
        }

        public void SetAddAccountButtonTitle(string title)
        {
            _btnGetAccess.SetTitle(title, UIControlState.Normal);
        }
    }
}