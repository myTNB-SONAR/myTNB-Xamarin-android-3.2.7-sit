using CoreGraphics;
using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class InfoComponent
    {
        UIView _parentView;
        UIView _baseView;
        public UIImageView Icon;
        public UILabel TitleLabel;
        public UILabel SubTitleLabel;
        public UILabel ValueLabel;
        public UIImageView ValuePairIcon;
        CGRect _baseFrame;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:myTNB.Dashboard.DashboardComponents.InfoComponent"/> class.
        /// </summary>
        /// <param name="parentView">Parent view.</param>
        public InfoComponent(UIView parentView)
        {
            _parentView = parentView;
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:myTNB.Dashboard.DashboardComponents.InfoComponent"/> class.
        /// </summary>
        /// <param name="parentView">Parent view.</param>
        /// <param name="baseRect">Base rect frame.</param>
        public InfoComponent(UIView parentView, CGRect baseRect)
            : this(parentView)
        {
            _baseFrame = baseRect;
        }

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        private void Initialize()
        {
            int topMargin = 12;
            double titleWidth = _parentView.Frame.Width * 0.60;
            double valueWidth = _parentView.Frame.Width * 0.30;
            Icon = new UIImageView(new CGRect(7, topMargin + 5, 24, 24));

            TitleLabel = new UILabel(new CGRect(Icon.Frame.X + Icon.Frame.Width + 10, topMargin, titleWidth, 18))
            {
                Font = myTNBFont.MuseoSans14_300(),
                TextColor = UIColor.White
            };

            SubTitleLabel = new UILabel(new CGRect(Icon.Frame.X + Icon.Frame.Width + 10, TitleLabel.Frame.Y + TitleLabel.Frame.Height, titleWidth, 16))
            {
                Font = myTNBFont.MuseoSans12_300(),
                TextColor = new UIColor(red: 1.0f, green: 1.0f, blue: 1.0f, alpha: 0.7f)
            };

            ValueLabel = new UILabel(new CGRect(_parentView.Frame.Width - valueWidth, topMargin + 6, valueWidth, 20))
            {
                Font = myTNBFont.MuseoSans16_300(),
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Right
            };

            ValuePairIcon = new UIImageView(new CGRect(_parentView.Frame.Width - 51, topMargin + 9, 7, 13));
        }

        /// <summary>
        /// Creates the component.
        /// </summary>
        private void CreateComponent()
        {
            var baseFrame = _baseFrame != default(CGRect) ? _baseFrame : new CGRect(0, 0, _parentView.Frame.Width, 59);
            _baseView = new UIView(baseFrame);

            _baseView.AddSubview(Icon);
            _baseView.AddSubview(TitleLabel);
            _baseView.AddSubview(SubTitleLabel);
            _baseView.AddSubview(ValueLabel);
            _baseView.AddSubview(ValuePairIcon);

            UIView viewLine = new UIView(new CGRect(0, _baseView.Frame.Height - 1,
                                                    _baseView.Frame.Width, 1))
            {
                BackgroundColor = myTNBColor.SelectionSemiTransparent()
            };
            _baseView.AddSubview(viewLine);
        }

        /// <summary>
        /// Gets the user interface.
        /// </summary>
        /// <returns>The user interface.</returns>
        public UIView GetUI()
        {
            CreateComponent();
            return _baseView;
        }

        /// <summary>
        /// Sets the hidden.
        /// </summary>
        /// <param name="isHidden">If set to <c>true</c> is hidden.</param>
        public void SetHidden(bool isHidden)
        {
            _baseView.Hidden = isHidden;
        }

    }
}
