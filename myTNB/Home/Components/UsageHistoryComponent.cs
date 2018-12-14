using CoreGraphics;
using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class UsageHistoryComponent
    {
        UIView _parentView;
        UIView _viewUsageHistory;
        UILabel _lblDateRange;
        UIView _viewLeft;
        UIView _viewRight;

        public UsageHistoryComponent(UIView view)
        {
            _parentView = view;
        }

        internal void CreateComponent()
        {
            _viewUsageHistory = new UIView(new CGRect(0, 0, _parentView.Frame.Width, 18));

            _lblDateRange = new UILabel(new CGRect(0, 0, _viewUsageHistory.Frame.Width, 18));
            _lblDateRange.Font = myTNBFont.MuseoSans14_500();
            _lblDateRange.TextAlignment = UITextAlignment.Center;
            _lblDateRange.TextColor = myTNBColor.SunGlow();
            _lblDateRange.Text = "-".ToUpper();
            _viewUsageHistory.AddSubview(_lblDateRange);
        }

        public UIView GetUI()
        {
            CreateComponent();
            CreateNavigationArrows();
            return _viewUsageHistory;
        }

        /// <summary>
        /// Sets the date range.
        /// </summary>
        /// <param name="dateRange">Date range.</param>
        public void SetDateRange(string dateRange)
        {
            _lblDateRange.Text = dateRange;
        }

        /// <summary>
        /// Sets the type of the frame by meter.
        /// </summary>
        /// <param name="isNormalMeter">If set to <c>true</c> is normal meter.</param>
        public void SetFrameByMeterType(bool isNormalMeter)
        {
            int yLocation = 0;

            if (isNormalMeter)
            {
                yLocation = 0;//!DeviceHelper.IsIphoneXUpResolution() ? 80 : 104;
            }
            else
            {
                yLocation = !DeviceHelper.IsIphoneXUpResolution() ? 32 : 40; //109 : 133;
            }

            var newFrame = _viewUsageHistory.Frame;
            newFrame.Y = yLocation;
            _viewUsageHistory.Frame = newFrame;
        }

        public void SetFrameCustomLocationY(int yLocation)
        {
            var newFrame = _viewUsageHistory.Frame;
            newFrame.Y = yLocation;
            _viewUsageHistory.Frame = newFrame;
        }

        /// <summary>
        /// Creates the navigation arrows.
        /// </summary>
        internal void CreateNavigationArrows()
        {
            double width = 122; // selector width
            double xLocation = (_parentView.Frame.Width / 2) - (width / 2) - 22;
            _viewLeft = new UIView(new CGRect(xLocation, 0, 16, 16));
            UIImageView imgViewLeft = new UIImageView(new CGRect(0, 0, 16, 16));
            imgViewLeft.Image = UIImage.FromBundle("Arrow-Left");
            imgViewLeft.Alpha = .60f;
            _viewLeft.AddSubview(imgViewLeft);
            _viewUsageHistory.AddSubview(_viewLeft);

            var rightX = _viewUsageHistory.Frame.Width - _viewLeft.Frame.X - _viewLeft.Frame.Width;
            _viewRight = new UIView(new CGRect(rightX, 0, 16, 16));
            UIImageView imgViewRight = new UIImageView(new CGRect(0, 0, 16, 16));
            imgViewRight.Image = UIImage.FromBundle("Arrow-Right");
            imgViewRight.Alpha = .60f;
            _viewRight.AddSubview(imgViewRight);
            _viewUsageHistory.AddSubview(_viewRight);
        }

        /// <summary>
        /// Toggles the navigation visibility.
        /// </summary>
        /// <param name="isHidden">If set to <c>true</c> is hidden.</param>
        public void ToggleNavigationVisibility(bool isHidden)
        {
            ToggleLeftNavigationVisibility(isHidden);
            ToggleRightNavigationVisibility(isHidden);
        }

        /// <summary>
        /// Toggles the left navigation visibility.
        /// </summary>
        /// <param name="isHidden">If set to <c>true</c> is hidden.</param>
        public void ToggleLeftNavigationVisibility(bool isHidden)
        {
            _viewLeft.Hidden = isHidden;
        }

        /// <summary>
        /// Toggles the right navigation visibility.
        /// </summary>
        /// <param name="isHidden">If set to <c>true</c> is hidden.</param>
        public void ToggleRightNavigationVisibility(bool isHidden)
        {
            _viewRight.Hidden = isHidden;
        }

        /// <summary>
        /// Adds the left navigation event.
        /// </summary>
        /// <param name="gesture">Gesture.</param>
        public void AddLeftNavigationEvent(UITapGestureRecognizer gesture)
        {
            _viewLeft.AddGestureRecognizer(gesture);
        }

        /// <summary>
        /// Adds the right navigation event.
        /// </summary>
        /// <param name="gesture">Gesture.</param>
        public void AddRightNavigationEvent(UITapGestureRecognizer gesture)
        {
            _viewRight.AddGestureRecognizer(gesture);
        }
    }
}
