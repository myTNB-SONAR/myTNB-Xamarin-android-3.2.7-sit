using System;
using CoreGraphics;
using UIKit;

namespace myTNB.Home.Bill
{
    public class BillSectionViewCell : UITableViewCell
    {
        private UIView _view, _viewFilter, _viewParent;
        private UILabel _lblTitle;
        private UIImageView _imgFilter;
        private nfloat _cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        private nfloat _baseHMargin = ScaleUtility.GetScaledWidth(16);

        public BillSectionViewCell(IntPtr handle) : base(handle)
        {
            nfloat scaled16 = ScaleUtility.GetScaledWidth(16);
            _view = new UIView(new CGRect(0, 0, _cellWidth, ScaleUtility.GetScaledHeight(60)));
            _view.BackgroundColor = MyTNBColor.LightGrayBG;
            _lblTitle = new UILabel(new CGRect(scaled16, ScaleUtility.GetScaledHeight(16), _view.Frame.Width, ScaleUtility.GetScaledHeight(24)))
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue
            };

            _viewFilter = new UIView(new CGRect(_cellWidth - ScaleUtility.GetScaledWidth(32), ScaleUtility.GetScaledHeight(20)
                , scaled16, scaled16));
            _imgFilter = new UIImageView(new CGRect(0, 0, scaled16, scaled16))
            {
                Image = UIImage.FromBundle("IC-Action-Unfiltered")
            };
            _viewFilter.AddSubview(_imgFilter);

            _view.AddSubview(_lblTitle);
            _view.AddSubview(_viewFilter);

            AddSubview(_view);
            if (_view != null)
            {
                _view.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
                _view.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
                _view.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
                _view.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            }
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public string SectionTitle
        {
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                {
                    value = string.Empty;
                }
                _lblTitle.Text = value;
            }
        }

        public Action filterAction
        {
            set
            {
                if (value != null)
                {
                    _viewFilter.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                    {
                        value.Invoke();
                    }));
                }
            }
        }

        public void SetFilterImage(bool isFiltered)
        {
            _imgFilter.Image = UIImage.FromBundle(isFiltered ? "IC-Action-Filtered" : "IC-Action-Unfiltered");
        }
    }
}