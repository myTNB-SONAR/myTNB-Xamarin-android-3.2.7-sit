using CoreGraphics;
using UIKit;

namespace myTNB.Dashboard.DashboardComponents
{
    public class SelectorComponent
    {
        UIView _parentView;
        public UISegmentedControl _selectorBar;

        public SelectorComponent(UIView view)
        {
            _parentView = view;
        }

        internal void CreateComponent()
        {
            UITextAttributes attr = new UITextAttributes();
            attr.Font = MyTNBFont.MuseoSans12;
            attr.TextColor = UIColor.White;
            UITextAttributes attrSelected = new UITextAttributes();
            attrSelected.Font = MyTNBFont.MuseoSans12;
            attrSelected.TextColor = MyTNBColor.NeonBlue;
            double width = 122;
            double xLocation = (_parentView.Frame.Width / 2) - (width / 2);
            _selectorBar = new UISegmentedControl(new CGRect(xLocation, 0, width, 26));
            _selectorBar.InsertSegment("Common_Day".Translate(), 0, false);
            _selectorBar.InsertSegment("Common_Month".Translate(), 1, false);
            _selectorBar.TintColor = UIColor.White;
            _selectorBar.SetTitleTextAttributes(attr, UIControlState.Normal);
            _selectorBar.SetTitleTextAttributes(attrSelected, UIControlState.Selected);
            _selectorBar.Layer.CornerRadius = 13.0f;
            _selectorBar.Layer.BorderColor = UIColor.White.CGColor;
            _selectorBar.Layer.BorderWidth = 1.0f;
            _selectorBar.Layer.MasksToBounds = true;
            _parentView.AddSubview(_selectorBar);
        }

        public UISegmentedControl GetUI()
        {
            CreateComponent();
            return _selectorBar;
        }

        /// <summary>
        /// Sets the hidden property of the selector bar.
        /// </summary>
        /// <param name="isHidden">If set to <c>true</c> is hidden.</param>
        public void SetHidden(bool isHidden)
        {
            _selectorBar.Hidden = isHidden;
        }
    }
}