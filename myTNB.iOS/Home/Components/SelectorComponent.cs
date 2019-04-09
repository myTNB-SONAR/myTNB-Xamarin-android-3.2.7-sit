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
            attr.Font = myTNBFont.MuseoSans12();
            attr.TextColor = myTNBColor.SelectionSemiTransparent();
            UITextAttributes attrSelected = new UITextAttributes();
            attrSelected.Font = myTNBFont.MuseoSans12();
            attrSelected.TextColor = UIColor.White;
            double width = 122;
            double xLocation = (_parentView.Frame.Width / 2) - (width / 2);
            _selectorBar = new UISegmentedControl(new CGRect(xLocation, 0, width, 26));
            _selectorBar.InsertSegment("Common_Month".Translate(), 0, false);
            _selectorBar.InsertSegment("Common_Year".Translate(), 1, false);
            _selectorBar.TintColor = myTNBColor.SelectionSemiTransparent();
            _selectorBar.SetTitleTextAttributes(attr, UIControlState.Normal);
            _selectorBar.SetTitleTextAttributes(attrSelected, UIControlState.Selected);
            _selectorBar.Layer.CornerRadius = 13.0f;
            UIColor bColor = new UIColor(red: 1.0f, green: 1.0f, blue: 1.0f, alpha: 0.5f);
            _selectorBar.Layer.BorderColor = bColor.CGColor;
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