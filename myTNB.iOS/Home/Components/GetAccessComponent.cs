using CoreGraphics;
using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class GetAccessComponent
    {
        readonly UIView _parentView;
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
            _viewGetAccess = new UIView(new CGRect(0, mainYLocation, _parentView.Frame.Width, DeviceHelper.GetScaledHeight(254)));

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
            var xLocation = _viewGetAccess.Frame.Width / 2 - DeviceHelper.GetScaledWidth((float)imgWidth) / 2;
            UIImageView imgViewEmpty = new UIImageView(new CGRect(xLocation, yLocation, DeviceHelper.GetScaledWidth((float)imgWidth)
                , DeviceHelper.GetScaledHeight((float)imgWidth)))
            {
                Image = UIImage.FromBundle("Get-Access"),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };
            _viewGetAccess.AddSubview(imgViewEmpty);

            _lblTitle = new UILabel(new CGRect(10, imgViewEmpty.Frame.GetMaxY() + 1, _viewGetAccess.Frame.Width - 20, 16))
            {
                TextAlignment = UITextAlignment.Center,
                Font = MyTNBFont.MuseoSans14_500,
                Text = "Component_ViewUsageHistory".Translate(),
                TextColor = UIColor.White
            };
            _viewGetAccess.AddSubview(_lblTitle);

            _lblSubtitle = new UILabel(new CGRect(0, _lblTitle.Frame.GetMaxY(), _viewGetAccess.Frame.Width, 28))
            {
                TextAlignment = UITextAlignment.Center,
                Text = "Component_GetAccessMessage".Translate(),
                Font = MyTNBFont.MuseoSans11_300,
                Lines = 2,
                TextColor = UIColor.White,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            _viewGetAccess.AddSubview(_lblSubtitle);

            _btnGetAccess = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(90, _viewGetAccess.Frame.Height - 71, _viewGetAccess.Frame.Width - 180, 48)
            };
            _btnGetAccess.Layer.CornerRadius = 4;
            _btnGetAccess.Layer.BorderColor = UIColor.White.CGColor;
            _btnGetAccess.Layer.BorderWidth = 1;
            _btnGetAccess.SetTitle("Common_GetAccess".Translate(), UIControlState.Normal);
            _btnGetAccess.Font = MyTNBFont.MuseoSans16;
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