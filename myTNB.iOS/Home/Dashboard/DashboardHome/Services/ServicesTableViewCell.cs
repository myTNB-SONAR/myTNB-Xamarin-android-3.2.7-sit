using System;
using System.Collections.Generic;
using CoreGraphics;
using myTNB.Home.Dashboard.DashboardHome.Services;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public class ServicesTableViewCell : UITableViewCell
    {
        private nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        private UIView _view;
        private int rowFactor = -1;
        private nfloat xLoc;
        private Dictionary<string, Action> _actionsDictionary;
        public UILabel _titleLabel;
        public ServicesTableViewCell(IntPtr handle) : base(handle)
        {
            _titleLabel = new UILabel(new CGRect(ScaleUtility.BaseMarginWidth16, 0, cellWidth - 32, 20f))
            {
                Font = TNBFont.MuseoSans_14_500,
                TextColor = MyTNBColor.PowerBlue
            };
            AddSubview(_titleLabel);
            _view = new UIView(new CGRect(ScaleUtility.BaseMarginWidth16, _titleLabel.Frame.GetMaxY() + 8f
                , cellWidth - (ScaleUtility.BaseMarginWidth16 * 2), 60.0F))
            {
                BackgroundColor = UIColor.Clear
            };
            AddSubview(_view);
            BackgroundColor = UIColor.Clear;
            if (_view != null)
            {
                _view.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
                _view.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
                _view.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
                _view.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            }
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public void AddCards(ServicesResponseModel services, Dictionary<string, Action> actionsDictionary)
        {
            _actionsDictionary = actionsDictionary;
            rowFactor = -1;
            xLoc = 0;
            for (int i = _view.Subviews.Length; i-- > 0;)
            {
                _view.Subviews[i].RemoveFromSuperview();
            }

            bool hasData = services != null && services.d != null && services.d.IsSuccess
                && services.d.data != null && services.d.data.services != null
                && services.d.data.services.Count > 0;
            if (hasData)
            {
                AddContentData(services.d.data.services);
            }
            else
            {
                AddShimmer();
            }
        }

        private void AddShimmer()
        {
            nfloat height = 0F;
            for (int i = 0; i < 6; i++)
            {
                UIView card = GetShimmerCards(i);
                _view.AddSubview(card);
                if (i == 5)
                {
                    height = card.Frame.GetMaxY() + 12;
                }
            }
            CGRect newFrame = _view.Frame;
            newFrame.Height = height;
            _view.Frame = newFrame;
        }

        private void AddContentData(List<ServiceItemModel> serviceList)
        {
            nfloat height = 0F;
            for (int i = 0; i < serviceList.Count; i++)
            {
                if (RemoveServiceItem(serviceList[i].ServiceId)) { continue; }
                UIView card = GetCard(serviceList[i], i);
                SetCardAction(ref card, serviceList[i].ServiceId);
                _view.AddSubview(card);
                if (i == serviceList.Count - 1)
                {
                    height = card.Frame.GetMaxY() + 12;
                }
            }
            CGRect newFrame = _view.Frame;
            newFrame.Height = height;
            _view.Frame = newFrame;
        }

        private bool RemoveServiceItem(string serviceID)
        {
            return false;//!SSMRAccounts.HasSSMREligibleAccount && (serviceID == "1001" || serviceID == "1002");
        }

        private UIView GetCard(ServiceItemModel serviceItem, int index, Action action = null)
        {
            nfloat cardWidth = (_view.Frame.Width - ScaleUtility.GetScaledWidth(12)) / 3;
            nfloat cardHeight = cardWidth * 0.9545F;
            nfloat margin = ScaleUtility.GetScaledWidth(6);
            nfloat yLoc = (cardHeight + margin);
            yLoc *= GetFactor(index);
            if (rowFactor == GetFactor(index))
            {
                xLoc += cardWidth + margin;
            }
            else
            {
                rowFactor = (int)GetFactor(index);
                xLoc = 0;
            }

            UIView view = new UIView(new CGRect(xLoc, yLoc, cardWidth, cardHeight)) { BackgroundColor = UIColor.White };
            AddCardShadow(ref view);

            nfloat imgSize = ScaleUtility.GetScaledWidth(28);
            nfloat imgYLoc = ScaleUtility.GetScaledHeight(12);
            UIImageView imgView = new UIImageView(new CGRect((view.Frame.Width - imgSize) / 2, imgYLoc, imgSize, imgSize))
            {
                Image = UIImage.FromBundle(GetImage(serviceItem.ServiceId))
            };

            nfloat xLblLoc = 16.0F;
            nfloat ylblLoc = imgView.Frame.GetMaxY() + ScaleUtility.GetScaledHeight(8);
            UILabel lblTitle = new UILabel(new CGRect(xLblLoc, ylblLoc, cardWidth - (xLblLoc * 2), cardHeight * 0.3F))
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.PowerBlue,
                Font = TNBFont.MuseoSans_10_500,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = serviceItem.ServiceName
            };
            view.AddSubviews(new UIView[] { imgView, lblTitle });
            return view;
        }

        private UIView GetShimmerCards(int index)
        {
            nfloat cardWidth = (_view.Frame.Width - ScaleUtility.GetScaledWidth(12)) / 3;
            nfloat cardHeight = cardWidth * 0.9545F;
            nfloat margin = ScaleUtility.GetScaledWidth(6);
            nfloat yLoc = (cardHeight + margin);
            yLoc *= GetFactor(index);
            if (rowFactor == GetFactor(index))
            {
                xLoc += cardWidth + margin;
            }
            else
            {
                rowFactor = (int)GetFactor(index);
                xLoc = 0;
            }

            CustomShimmerView shimmeringView = new CustomShimmerView();
            UIView viewParent = new UIView(new CGRect(xLoc, yLoc, cardWidth, cardHeight)) { BackgroundColor = UIColor.White };
            AddCardShadow(ref viewParent);
            UIView viewShimmerParent = new UIView(new CGRect(0, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
            UIView viewShimmerContent = new UIView(new CGRect(0, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
            viewParent.AddSubviews(new UIView[] { viewShimmerParent, viewShimmerContent });

            nfloat viewImgWidth = viewShimmerContent.Frame.Width * 0.27F;
            UIView viewImg = new UIView(new CGRect((viewShimmerContent.Frame.Width - viewImgWidth) / 2
                , 16, viewImgWidth, viewImgWidth))
            { BackgroundColor = MyTNBColor.PowderBlue };
            viewImg.Layer.CornerRadius = viewImgWidth / 2;
            UIView viewLbl = new UIView(new CGRect(12, viewImgWidth + 32
                , viewShimmerContent.Frame.Width - 24, 14))
            { BackgroundColor = MyTNBColor.PowderBlue };

            viewShimmerContent.AddSubviews(new UIView[] { viewImg, viewLbl });

            viewShimmerParent.AddSubview(shimmeringView);
            shimmeringView.ContentView = viewShimmerContent;
            shimmeringView.Shimmering = true;
            shimmeringView.SetValues();

            return viewParent;
        }

        private nfloat GetFactor(int index)
        {
            return index < 1 ? 0 : (nfloat)Math.Floor((decimal)index / 3);
        }

        private void AddCardShadow(ref UIView view)
        {
            view.Layer.CornerRadius = 5.0F;
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.BabyBlue60.CGColor;
            view.Layer.ShadowOpacity = 0.5F;
            view.Layer.ShadowOffset = new CGSize(-4, 8);
            view.Layer.ShadowRadius = 8;
            view.Layer.ShadowPath = UIBezierPath.FromRect(view.Bounds).CGPath;
        }

        private string GetImage(string serviceID)
        {
            return ServicesConstants.ImageDictionary.ContainsKey(serviceID) ? ServicesConstants.ImageDictionary[serviceID] : string.Empty;
        }

        private void SetCardAction(ref UIView view, string id)
        {
            Action action = _actionsDictionary.ContainsKey(id) ? _actionsDictionary[id] : null;
            view.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                if (action != null)
                {
                    action.Invoke();
                }
            }));
        }
    }
}