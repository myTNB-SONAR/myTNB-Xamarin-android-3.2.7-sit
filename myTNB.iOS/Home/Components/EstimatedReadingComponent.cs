using CoreGraphics;

using UIKit;
namespace myTNB.Dashboard.DashboardComponents
{
    public class EstimatedReadingComponent
    {
        UIView _parentView;
        UILabel _lblTitle;
        public EstimatedReadingComponent(UIView view)
        {
            _parentView = view;
        }

        internal void CreateComponent()
        {
            _lblTitle = new UILabel(new CGRect(20, _parentView.Frame.Height - 74, _parentView.Frame.Width - 40, 14));
            _lblTitle.Font = myTNBFont.MuseoSans9();
            _lblTitle.TextColor = UIColor.White;
            _lblTitle.TextAlignment = UITextAlignment.Center;
            _lblTitle.Text = "Component_EstimatedReading".Translate();
        }

        public UILabel GetUI()
        {
            CreateComponent();
            return _lblTitle;
        }

        public void SetTitle(string title)
        {
            _lblTitle.Text = title;
        }
    }
}