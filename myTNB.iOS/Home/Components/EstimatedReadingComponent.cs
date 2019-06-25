using CoreGraphics;
using UIKit;
namespace myTNB.Dashboard.DashboardComponents
{
    public class EstimatedReadingComponent
    {
        readonly UIView _parentView;
        UILabel _lblTitle;
        public EstimatedReadingComponent(UIView view)
        {
            _parentView = view;
        }

        internal void CreateComponent()
        {
            _lblTitle = new UILabel(new CGRect(20, _parentView.Frame.Height - 74, _parentView.Frame.Width - 40, 14))
            {
                Font = MyTNBFont.MuseoSans9,
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Text = "Component_EstimatedReading".Translate()
            };
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