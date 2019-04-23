using CoreGraphics;

using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class NoDataConnectionComponent
    {
        UIView _parentView;
        UIView _viewNoDataConnection;
        UILabel _lblNoDataTitle;
        UILabel _lblNoDataSubtitle;
        public UIButton _btnRefresh;

        public NoDataConnectionComponent(UIView view)
        {
            _parentView = view;
        }

        internal void CreateComponent()
        {
            int yLocation = 181;
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                yLocation = 205;
            }
            float viewHeight = (float)_parentView.Frame.Height - (317f + 40f);
            _viewNoDataConnection = new UIView(new CGRect(0, yLocation, _parentView.Frame.Width, viewHeight));

            UIView viewContent = new UIView(new CGRect(0, (viewHeight - 95) / 2, _viewNoDataConnection.Frame.Width, 95));

            _lblNoDataTitle = new UILabel(new CGRect(0, 0, _viewNoDataConnection.Frame.Width, 16));
            _lblNoDataTitle.TextAlignment = UITextAlignment.Center;
            _lblNoDataTitle.Font = MyTNBFont.MuseoSans14_500;
            _lblNoDataTitle.Text = "Component_CannotLoadChart".Translate();
            _lblNoDataTitle.TextColor = UIColor.White;
            viewContent.AddSubview(_lblNoDataTitle);

            _lblNoDataSubtitle = new UILabel(new CGRect(0, 16, _viewNoDataConnection.Frame.Width, 14));
            _lblNoDataSubtitle.TextAlignment = UITextAlignment.Center;
            _lblNoDataSubtitle.Font = MyTNBFont.MuseoSans13_300;
            _lblNoDataSubtitle.Text = "Error_CheckInternetMessage".Translate();
            _lblNoDataSubtitle.TextColor = UIColor.White;
            viewContent.AddSubview(_lblNoDataSubtitle);

            _btnRefresh = new UIButton(UIButtonType.Custom);
            _btnRefresh.Frame = new CGRect(90, 47, _viewNoDataConnection.Frame.Width - 180, 48);
            _btnRefresh.Layer.CornerRadius = 4;
            _btnRefresh.Layer.BorderColor = UIColor.White.CGColor;
            _btnRefresh.Layer.BorderWidth = 1;
            _btnRefresh.SetTitle("Component_TapToRefresh".Translate(), UIControlState.Normal);
            _btnRefresh.Font = MyTNBFont.MuseoSans18_500;
            _btnRefresh.SetTitleColor(UIColor.White, UIControlState.Normal);
            viewContent.AddSubview(_btnRefresh);

            _viewNoDataConnection.AddSubview(viewContent);
        }

        public UIView GetUI()
        {
            CreateComponent();
            return _viewNoDataConnection;
        }

        public void SetNoDataConnectionTitle(string title)
        {
            _lblNoDataTitle.Text = title;
        }

        public void SetNoDataConnectionSubtitle(string subtitle)
        {
            _lblNoDataSubtitle.Text = subtitle;
        }

        public void SetRefreshButtonTitle(string title)
        {
            _btnRefresh.SetTitle(title, UIControlState.Normal);
        }
    }
}