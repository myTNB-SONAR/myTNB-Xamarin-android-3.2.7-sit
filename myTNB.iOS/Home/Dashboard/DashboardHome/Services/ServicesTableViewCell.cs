using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreGraphics;
using myTNB.Home.Dashboard.DashboardHome.Services;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public class ServicesTableViewCell : CustomUITableViewCell
    {
        private DashboardHomeHelper _dashboardHomeHelper = new DashboardHomeHelper();
        private nfloat cellWidth = UIApplication.SharedApplication.KeyWindow.Frame.Width;
        private nfloat cellHeight = UIApplication.SharedApplication.KeyWindow.Frame.Height;
        private UIView _view;
        private int rowFactor = -1;
        private nfloat xLoc;
        private Dictionary<string, Action> _actionsDictionary;
        private nfloat _cardHeight = ScaleUtility.GetScaledHeight(84F);
        public Action<int> ReloadCell;
        private CustomUIView _moreLessContainer;
        public bool IsLoading;
        public ServicesTableViewCell(IntPtr handle) : base(handle)
        {
            _view = new UIView(new CGRect(BaseMarginWidth16, 0, cellWidth - (BaseMarginWidth16 * 2), _cardHeight))
            {
                ClipsToBounds = false,
                BackgroundColor = UIColor.White
            };
            _view.Layer.CornerRadius = GetScaledHeight(5F);
            //AddCardShadow(ref _view);
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

        public void AddCards(ServicesResponseModel services, Dictionary<string, Action> actionsDictionary, bool isLoading)
        {
            _actionsDictionary = actionsDictionary;
            rowFactor = -1;
            xLoc = 0;
            for (int i = _view.Subviews.Length; i-- > 0;)
            {
                _view.Subviews[i].RemoveFromSuperview();
            }
            if (isLoading || services == null || services.d == null || services.d.data == null || services.d.data.services == null)
            {
                ViewHelper.AdjustFrameSetHeight(_view, _cardHeight);
                AddShimmer();
            }
            else
            {
                if (DataManager.DataManager.SharedInstance.ActiveServicesList.Count > 0)
                {
                    AddContentData(DataManager.DataManager.SharedInstance.ActiveServicesList);
                }
                else
                {
                    ShowInitialItems(services.d.data.services);
                }
            }
        }

        private void ShowInitialItems(List<ServiceItemModel> serviceItems)
        {
            DataManager.DataManager.SharedInstance.ActiveServicesList = new List<ServiceItemModel>();
            for (int i = 0; i < serviceItems.Count; i++)
            {
                if (i < 3)
                {
                    DataManager.DataManager.SharedInstance.ActiveServicesList.Add(serviceItems[i]);
                }
            }
            AddContentData(DataManager.DataManager.SharedInstance.ActiveServicesList);
        }

        private void AddShimmer()
        {
            for (int i = 0; i < 3; i++)
            {
                UIView card = GetShimmerCards(i);
                _view.AddSubview(card);
            }
        }

        private void AddShowLessView(bool isShowMore)
        {
            if (_moreLessContainer != null)
            {
                _moreLessContainer.RemoveFromSuperview();
            }
            nfloat yPos = _view.Frame.Height - GetScaledHeight(41F);
            nfloat width = _view.Frame.Width;
            _moreLessContainer = new CustomUIView(new CGRect(0, yPos, _view.Frame.Width, GetScaledHeight(41F)))
            {
                BackgroundColor = UIColor.Clear
            };

            _moreLessContainer.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                rowFactor = -1;
                xLoc = 0;
                for (int i = _view.Subviews.Length; i-- > 0;)
                {
                    _view.Subviews[i].RemoveFromSuperview();
                }
                if (isShowMore)
                {
                    OnShowMoreAction();
                }
                else
                {
                    OnShowLessAction();
                }
            }));

            UIView lineView = new UIView(new CGRect(0, 0, width, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkSix
            };
            _moreLessContainer.AddSubview(lineView);

            CustomUIView moreLessView = new CustomUIView(new CGRect(0, GetScaledHeight(12F), 0, GetScaledHeight(16F)))
            {
                BackgroundColor = UIColor.Clear
            };

            UILabel moreAcctsLabel = new UILabel(new CGRect(0, 0, 0, GetScaledHeight(16F)))
            {
                Font = TNBFont.MuseoSans_12_500,
                TextColor = MyTNBColor.WaterBlue,
                Text = isShowMore ? "Show More" : "Show Less"
            };
            moreLessView.AddSubview(moreAcctsLabel);
            UIImageView arrowUpDown = new UIImageView(new CGRect(moreAcctsLabel.Frame.GetMaxX(), 0, GetScaledWidth(16F), GetScaledHeight(16F)))
            {
                Image = UIImage.FromBundle(isShowMore ? "Arrow-Down-Blue-Small" : "Arrow-Up-Blue-Small")
            };
            moreLessView.AddSubview(arrowUpDown);

            CGSize lblSize = moreAcctsLabel.SizeThatFits(new CGSize(1000F, 1000F));
            ViewHelper.AdjustFrameSetWidth(moreAcctsLabel, lblSize.Width);
            ViewHelper.AdjustFrameSetX(arrowUpDown, moreAcctsLabel.Frame.GetMaxX() + GetScaledWidth(4F));

            ViewHelper.AdjustFrameSetWidth(moreLessView, moreAcctsLabel.Frame.Width + GetScaledWidth(4F) + arrowUpDown.Frame.Width);
            ViewHelper.AdjustFrameSetX(moreLessView, GetXLocationToCenterObject(moreLessView.Frame.Width, _moreLessContainer));

            _moreLessContainer.AddSubview(moreLessView);
            _view.AddSubview(_moreLessContainer);
        }

        private void OnShowMoreAction()
        {
            DataManager.DataManager.SharedInstance.ActiveServicesList = new List<ServiceItemModel>();
            DataManager.DataManager.SharedInstance.ActiveServicesList = DataManager.DataManager.SharedInstance.ServicesList;
            AddContentData(DataManager.DataManager.SharedInstance.ActiveServicesList);
            ReloadCell?.Invoke(DashboardHomeConstants.CellIndex_Help);
        }

        private void OnShowLessAction()
        {
            ShowInitialItems(DataManager.DataManager.SharedInstance.ServicesList);
            ReloadCell?.Invoke(DashboardHomeConstants.CellIndex_Help);
        }

        private nfloat GetViewHeight(List<ServiceItemModel> serviceList, bool isMorethanThreeItems)
        {
            nfloat totalHeight = 0;
            if (serviceList != null &&
                serviceList.Count > 0)
            {
                var multiplier = Math.Ceiling((double)serviceList.Count / 3);
                totalHeight += _cardHeight * (nfloat)multiplier;
                if (isMorethanThreeItems)
                {
                    totalHeight += GetScaledHeight(41F);
                }
            }
            return totalHeight;
        }

        private void AddContentData(List<ServiceItemModel> serviceList)
        {
            bool isMoreThanThreeItems = DataManager.DataManager.SharedInstance.ServicesList.Count > 3;

            for (int i = 0; i < serviceList.Count; i++)
            {
                if (RemoveServiceItem(serviceList[i].ServiceId)) { continue; }
                UIView card = GetCard(serviceList[i], i);
                SetCardAction(ref card, serviceList[i].ServiceId);
                _view.AddSubview(card);
            }
            ViewHelper.AdjustFrameSetHeight(_view, GetViewHeight(serviceList, isMoreThanThreeItems));
            if (isMoreThanThreeItems)
            {
                bool isShowMore = DataManager.DataManager.SharedInstance.ActiveServicesList.Count < DataManager.DataManager.SharedInstance.ServicesList.Count;
                AddShowLessView(isShowMore);
            }
        }

        private bool RemoveServiceItem(string serviceID)
        {
            return false;//!SSMRAccounts.HasSSMREligibleAccount && (serviceID == "1001" || serviceID == "1002");
        }

        private UIView GetCard(ServiceItemModel serviceItem, int index, Action action = null)
        {
            nfloat cardWidth = _view.Frame.Width / 3;
            nfloat cardHeight = GetScaledHeight(84F);
            nfloat yLoc = cardHeight;
            yLoc *= GetFactor(index);
            if (rowFactor == GetFactor(index))
            {
                xLoc += cardWidth;
            }
            else
            {
                rowFactor = (int)GetFactor(index);
                xLoc = 0;
            }

            UIView view = new UIView(new CGRect(xLoc, yLoc, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };

            nfloat imgSize = GetScaledWidth(27F);
            nfloat imgYLoc = GetScaledHeight(12F);
            UIImageView imgView = new UIImageView(new CGRect(GetXLocationToCenterObject(imgSize, view), imgYLoc, imgSize, imgSize))
            {
                Image = UIImage.FromBundle(GetImage(serviceItem.ServiceId))
            };

            nfloat xLblLoc = GetScaledWidth(14F);
            nfloat ylblLoc = GetYLocationFromFrame(imgView.Frame, GetScaledHeight(5F));
            UILabel lblTitle = new UILabel(new CGRect(xLblLoc, ylblLoc, cardWidth - (xLblLoc * 2), GetScaledHeight(28F)))
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.WaterBlue,
                Font = TNBFont.MuseoSans_10_500,
                Lines = 2,
                LineBreakMode = UILineBreakMode.WordWrap,
                Text = serviceItem.ServiceName
            };

            UIView indicatorView = new UIView(new CGRect(GetScaledWidth(51F), GetScaledHeight(13F), 0, GetScaledHeight(14F)))
            {
                BackgroundColor = MyTNBColor.SunflowerYellow
            };
            indicatorView.Layer.CornerRadius = GetScaledHeight(8F);
            UILabel newLbl = new UILabel(new CGRect(GetScaledWidth(4F), GetScaledHeight(2F), 0, GetScaledHeight(10F)))
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = MyTNBColor.GreyishBrown,
                Font = TNBFont.MuseoSans_8_500,
                Text = "New"
            };
            indicatorView.AddSubview(newLbl);

            CGSize newLblSize = newLbl.SizeThatFits(new CGSize(1000F, GetScaledHeight(10F)));
            ViewHelper.AdjustFrameSetWidth(newLbl, newLblSize.Width);
            ViewHelper.AdjustFrameSetWidth(indicatorView, newLblSize.Width + GetScaledWidth(8F));

            view.AddSubviews(new UIView[] { imgView, lblTitle, indicatorView });
            return view;
        }

        private UIView GetShimmerCards(int index)
        {
            nfloat cardWidth = (_view.Frame.Width - GetScaledWidth(24F)) / 3;
            nfloat cardHeight = GetScaledHeight(84F);
            nfloat yLoc = cardHeight;
            yLoc *= GetFactor(index);
            if (rowFactor == GetFactor(index))
            {
                xLoc += cardWidth + GetScaledWidth(12F);
            }
            else
            {
                rowFactor = (int)GetFactor(index);
                xLoc = 0;
            }

            CustomShimmerView shimmeringView = new CustomShimmerView();
            UIView viewParent = new UIView(new CGRect(xLoc, yLoc, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
            UIView viewShimmerParent = new UIView(new CGRect(0, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
            UIView viewShimmerContent = new UIView(new CGRect(0, 0, cardWidth, cardHeight)) { BackgroundColor = UIColor.Clear };
            viewParent.AddSubviews(new UIView[] { viewShimmerParent, viewShimmerContent });

            nfloat viewImgWidth = GetScaledWidth(24F);
            UIView viewImg = new UIView(new CGRect(GetXLocationToCenterObject(viewImgWidth, viewShimmerContent)
                , GetScaledHeight(12F), viewImgWidth, viewImgWidth))
            { BackgroundColor = MyTNBColor.PaleGreyThree };
            viewImg.Layer.CornerRadius = viewImgWidth / 2;
            UIView viewLbl1 = new UIView(new CGRect(GetScaledWidth(12F), GetYLocationFromFrame(viewImg.Frame, 8F)
                , viewShimmerContent.Frame.Width - GetScaledWidth(24F), GetScaledHeight(12F)))
            { BackgroundColor = MyTNBColor.PaleGreyThree };
            viewLbl1.Layer.CornerRadius = GetScaledHeight(2F);
            UIView viewLbl2 = new UIView(new CGRect(GetScaledWidth(12F), GetYLocationFromFrame(viewLbl1.Frame, 4F)
                , viewShimmerContent.Frame.Width - GetScaledWidth(24F), GetScaledHeight(12F)))
            { BackgroundColor = MyTNBColor.PaleGreyThree };
            viewLbl2.Layer.CornerRadius = GetScaledHeight(2F);
            viewShimmerContent.AddSubviews(new UIView[] { viewImg, viewLbl1, viewLbl2 });

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
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowColor = MyTNBColor.VeryLightPinkFive.CGColor;
            view.Layer.ShadowOpacity = 0.8F;
            view.Layer.ShadowOffset = new CGSize(0, -8);
            view.Layer.ShadowRadius = 4;
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
