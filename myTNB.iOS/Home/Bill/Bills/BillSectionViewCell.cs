﻿using System;
using System.Diagnostics;
using CoreGraphics;
using UIKit;

namespace myTNB.Home.Bill
{
    public class BillSectionViewCell : UITableViewCell
    {
        private UIView _view;
        private UILabel _lblTitle;
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

            UIView viewFilter = new UIView(new CGRect(_cellWidth - ScaleUtility.GetScaledWidth(32), ScaleUtility.GetScaledHeight(20)
                , scaled16, scaled16));
            UIImageView imgFilter = new UIImageView(new CGRect(0, 0, scaled16, scaled16))
            {
                Image = UIImage.FromBundle("IC-Action-Filter")
            };
            viewFilter.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                Debug.WriteLine("Filter");
            }));
            viewFilter.AddSubview(imgFilter);

            _view.AddSubview(_lblTitle);
            _view.AddSubview(viewFilter);

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

        public bool IsLoading
        {
            set
            {
                if (value)
                {
                    CustomShimmerView shimmeringView = new CustomShimmerView();
                    UIView viewParent = new UIView(new CGRect(new CGPoint(0, 0), _view.Frame.Size)) { BackgroundColor = UIColor.Clear, Tag = 99 };
                    UIView viewShimmerParent = new UIView(new CGRect(new CGPoint(0, 0), _view.Frame.Size)) { BackgroundColor = UIColor.Clear };
                    UIView viewShimmerContent = new UIView(new CGRect(new CGPoint(0, 0), _view.Frame.Size)) { BackgroundColor = UIColor.Clear };
                    viewParent.AddSubviews(new UIView[] { viewShimmerParent, viewShimmerContent });

                    UIView viewGroupedData = new UIView(new CGRect(_baseHMargin, _view.Frame.GetMaxY() - ScaleUtility.GetScaledHeight(12)
                        , ScaleUtility.GetScaledWidth(100), ScaleUtility.GetScaledHeight(12)))
                    {
                        BackgroundColor = MyTNBColor.PaleGrey,
                    };
                    viewShimmerContent.AddSubviews(new UIView[] { viewGroupedData });

                    viewShimmerParent.AddSubview(shimmeringView);
                    shimmeringView.ContentView = viewShimmerContent;
                    shimmeringView.Shimmering = true;
                    shimmeringView.SetValues();

                    _view.AddSubview(viewParent);
                }
                else
                {
                    UIView view = _view.ViewWithTag(99);
                    if (view != null)
                    {
                        view.RemoveFromSuperview();
                    }
                }
            }
        }
    }
}