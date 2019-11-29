using System;
using System.Globalization;
using CoreGraphics;
using UIKit;

namespace myTNB.Home.Bill
{
    public class BillHistoryViewCell : UITableViewCell
    {
        private CustomUIView _view;
        private UIView _viewGroupedDate, _viewLine;
        private UILabel _lblGroupedDate, _lblDate, _lblSource, _lblAmount;
        private UIImageView _imgArrow;
        private nfloat _cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        private nfloat _baseHMargin = ScaleUtility.GetScaledWidth(16);
        private nfloat _baseVMargin = ScaleUtility.GetScaledHeight(16);

        public BillHistoryViewCell(IntPtr handle) : base(handle)
        {
            _view = new CustomUIView(new CGRect(0, 0, _cellWidth, ScaleUtility.GetScaledHeight(68))) { ClipsToBounds = false };
            AddViews();
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
        private void AddViews()
        {
            _lblDate = new UILabel(new CGRect(_baseHMargin, _baseVMargin, ScaleUtility.GetScaledWidth(200), _baseVMargin))
            {
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_12_500
            };

            _lblSource = new UILabel(new CGRect(_baseHMargin, ScaleUtility.GetYLocationFromFrame(_lblDate.Frame, 4)
                , ScaleUtility.GetScaledWidth(200), _baseVMargin))
            {
                TextColor = MyTNBColor.Grey,
                TextAlignment = UITextAlignment.Left,
                Font = TNBFont.MuseoSans_12_300
            };

            _lblAmount = new UILabel(new CGRect(_cellWidth - ScaleUtility.GetScaledWidth(148), ScaleUtility.GetScaledHeight(26)
                , ScaleUtility.GetScaledWidth(100), _baseVMargin))
            {
                TextColor = MyTNBColor.FreshGreen,
                TextAlignment = UITextAlignment.Right,
                Font = TNBFont.MuseoSans_12_500
            };

            _imgArrow = new UIImageView(new CGRect(_cellWidth - ScaleUtility.GetScaledWidth(32), ScaleUtility.GetScaledHeight(26)
                , _baseHMargin, _baseHMargin))
            {
                Image = UIImage.FromBundle(BillConstants.IMG_ArrowExpand)
            };

            _viewLine = new UIView(new CGRect(_baseHMargin, 0, _cellWidth - (_baseHMargin * 2), ScaleUtility.GetScaledHeight(1)))
            { BackgroundColor = MyTNBColor.VeryLightPinkThree };

            _view.AddSubviews(new UIView[] { _lblDate, _lblSource, _lblAmount, _imgArrow, _viewLine });

            _viewGroupedDate = new UIView(new CGRect(ScaleUtility.GetScaledWidth(16), 0 - ScaleUtility.GetScaledHeight(12)
                  , ScaleUtility.GetScaledWidth(70), ScaleUtility.GetScaledHeight(24)))
            {
                ClipsToBounds = true,
                Hidden = true
            };
            _lblGroupedDate = new UILabel(new CGRect(new CGPoint(0, 0), _viewGroupedDate.Frame.Size))
            {
                BackgroundColor = MyTNBColor.WaterBlue,
                TextColor = UIColor.White,
                Font = TNBFont.MuseoSans_12_500,
                TextAlignment = UITextAlignment.Center
            };
            _viewGroupedDate.AddSubview(_lblGroupedDate);
            _viewGroupedDate.Layer.CornerRadius = ScaleUtility.GetScaledHeight(12);
            _viewGroupedDate.Layer.ZPosition = 99;

            _view.AddSubview(_viewGroupedDate);
        }
        public string Type
        {
            set
            {
                if (!IsValidInput(value))
                {
                    value = string.Empty;
                }
                _lblDate.Text = value;
            }
        }
        public string Source
        {
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                {
                    value = string.Empty;
                }
                _lblSource.Text = value;
                nfloat width = _lblSource.GetLabelWidth(_cellWidth - _lblAmount.Frame.Width - _baseHMargin - ScaleUtility.GetScaledWidth(48));
                _lblSource.Frame = new CGRect(_baseHMargin, ScaleUtility.GetYLocationFromFrame(_lblDate.Frame, 4), width, _baseVMargin);
            }
        }
        public string Amount
        {
            set
            {
                if (!IsValidInput(value))
                {
                    value = string.Empty;
                }
                double.TryParse(value, out double parsedValue);
                string amt = parsedValue.ToString("N2", CultureInfo.InvariantCulture);
                _lblAmount.Text = string.Format(BillConstants.Format_Default, TNBGlobal.UNIT_CURRENCY, amt);
                nfloat width = _lblAmount.GetLabelWidth(_cellWidth - ScaleUtility.GetScaledWidth(48));
                _lblAmount.Frame = new CGRect(_cellWidth - width - ScaleUtility.GetScaledWidth(48)
                    , ScaleUtility.GetScaledHeight(26), width, _baseVMargin);
            }
        }
        public bool IsArrowHidden
        {
            set
            {
                _imgArrow.Hidden = value;
            }
        }
        public bool IsPayment
        {
            set
            {
                _lblAmount.TextColor = value ? MyTNBColor.FreshGreen : MyTNBColor.CharcoalGrey;
            }
        }
        public string Date
        {
            set
            {
                if (!IsValidInput(value))
                {
                    value = string.Empty;
                }
                _lblGroupedDate.Text = value;
                _viewGroupedDate.Hidden = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
            }
        }
        private bool IsValidInput(string value)
        {
            return !string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value);
        }
        public void SetWidgetHeight(bool hasDate, bool isTop = true, bool isBottom = false)
        {
            if (hasDate)
            {
                _view.Frame = new CGRect(_view.Frame.Location, new CGSize(_view.Frame.Width, ScaleUtility.GetScaledHeight(isTop && isBottom ? 92 : 80)));
                nfloat dateYloc = _baseVMargin;
                nfloat amtYloc = ScaleUtility.GetScaledHeight(26);
                if (isTop && isBottom)
                {
                    dateYloc = ScaleUtility.GetScaledHeight(28);
                    amtYloc = ScaleUtility.GetScaledHeight((92 - 16) / 2);
                }
                else if (isTop)
                {
                    dateYloc = ScaleUtility.GetScaledHeight(28);
                    amtYloc = ScaleUtility.GetScaledHeight(33);
                }
                else
                {
                    _viewGroupedDate.Hidden = true;
                }

                _lblDate.Frame = new CGRect(new CGPoint(_lblDate.Frame.X, dateYloc), _lblDate.Frame.Size);
                _lblSource.Frame = new CGRect(new CGPoint(_lblSource.Frame.X, ScaleUtility.GetYLocationFromFrame(_lblDate.Frame, 4))
                    , _lblSource.Frame.Size);
                _lblAmount.Frame = new CGRect(new CGPoint(_lblAmount.Frame.X, amtYloc), _lblAmount.Frame.Size);
                _imgArrow.Frame = new CGRect(new CGPoint(_imgArrow.Frame.X, amtYloc), _imgArrow.Frame.Size);
            }
        }
        public bool IsLineHidden
        {
            set
            {
                _viewLine.Hidden = value;
            }
        }
        public bool IsGroupedDateHidden
        {
            set
            {
                _viewGroupedDate.Hidden = value;
                nfloat lineXloc = value ? _baseHMargin : 0;
                nfloat lineWidth = value ? _cellWidth - (_baseHMargin * 2) : _cellWidth;
                _viewLine.Frame = new CGRect(lineXloc, 0, lineWidth, ScaleUtility.GetScaledHeight(1));
            }
        }
    }
}