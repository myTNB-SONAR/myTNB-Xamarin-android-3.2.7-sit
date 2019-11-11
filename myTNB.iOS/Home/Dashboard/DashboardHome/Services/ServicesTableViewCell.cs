﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
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
        private CustomUIView _moreLessContainer;

        public Action<int> ReloadCell;
        public bool IsLoading, IsRefreshScreen;

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
            //if (_view != null)
            //{
            //    _view.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            //    _view.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            //    _view.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            //    _view.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            //}
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public void AddCards(List<ServiceItemModel> services, Dictionary<string, Action> actionsDictionary, bool isLoading)
        {
            _actionsDictionary = actionsDictionary;
            rowFactor = -1;
            xLoc = 0;
            for (int i = _view.Subviews.Length; i-- > 0;)
            {
                _view.Subviews[i].RemoveFromSuperview();
            }
            if (isLoading || services == null || services.Count == 0)
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
                    ShowInitialItems(services);
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
                Text = GetI18NValue(isShowMore ? DashboardHomeConstants.I18N_ShowMore : DashboardHomeConstants.I18N_ShowLess)
            };
            moreLessView.AddSubview(moreAcctsLabel);
            UIImageView arrowUpDown = new UIImageView(new CGRect(moreAcctsLabel.Frame.GetMaxX(), 0, GetScaledWidth(16F), GetScaledHeight(16F)))
            {
                Image = UIImage.FromBundle(isShowMore ? DashboardHomeConstants.Img_ArrowDownBlue : DashboardHomeConstants.Img_ArrowUpBlue)
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
                SetCardAction(ref card, serviceList[i]);
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

            nfloat imgSize = GetScaledWidth(28F);
            nfloat imgYLoc = GetScaledHeight(12F);
            UIImageView imgView = new UIImageView(new CGRect(GetXLocationToCenterObject(imgSize, view), imgYLoc, imgSize, imgSize))
            {
                Image = UIImage.FromBundle(GetImage(serviceItem))
            };

            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(GetServiceName(serviceItem)
                , ref htmlBodyError, TNBFont.FONTNAME_500, (float)GetScaledHeight(10F));
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = ServiceIsDisabled(serviceItem) ? MyTNBColor.VeryLightPinkSeven : MyTNBColor.WaterBlue,
                ParagraphStyle = new NSMutableParagraphStyle
                {
                    Alignment = UITextAlignment.Center,
                    LineSpacing = 3.0f
                }
            }, new NSRange(0, htmlBody.Length));

            UITextView txtTitle = new UITextView(new CGRect(0, imgView.Frame.GetMaxY(), cardWidth, GetScaledHeight(28F)))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                UserInteractionEnabled = false
            };
            CGSize cGSize = txtTitle.SizeThatFits(new CGSize(cardWidth, 1000F));
            ViewHelper.AdjustFrameSetHeight(txtTitle, cGSize.Height);
            view.AddSubviews(new UIView[] { imgView, txtTitle });

            if (ShowIndicator(serviceItem.ServiceId) && serviceItem.ServiceType != ServiceEnum.SUBMITFEEDBACK)
            {
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
                    Text = LanguageUtility.GetCommonI18NValue(Constants.Common_New)
                };
                indicatorView.AddSubview(newLbl);

                CGSize newLblSize = newLbl.SizeThatFits(new CGSize(1000F, GetScaledHeight(10F)));
                ViewHelper.AdjustFrameSetWidth(newLbl, newLblSize.Width);
                ViewHelper.AdjustFrameSetWidth(indicatorView, newLblSize.Width + GetScaledWidth(8F));

                view.AddSubview(indicatorView);
            }

            return view;
        }

        private string GetServiceName(ServiceItemModel serviceItem)
        {
            string name = string.Empty;
            if (serviceItem != null)
            {
                switch (serviceItem.ServiceType)
                {
                    case ServiceEnum.VIEWBILL:
                        name = DashboardHomeConstants.I18N_ViewEBill;
                        if (_dashboardHomeHelper.HasNormalAccounts && _dashboardHomeHelper.HasREAccounts)
                        {
                            name = DashboardHomeConstants.I18N_ViewEBillAndAdvice;
                        }
                        else if (_dashboardHomeHelper.HasREAccounts)
                        {
                            if (_dashboardHomeHelper.HasMultipleREAccounts)
                            {
                                name = DashboardHomeConstants.I18N_ViewAdvices;
                            }
                            else
                            {
                                name = DashboardHomeConstants.I18N_ViewAdvice;
                            }
                        }
                        else if (_dashboardHomeHelper.HasNormalAccounts)
                        {
                            if (_dashboardHomeHelper.HasMultipleNormalAccounts)
                            {
                                name = DashboardHomeConstants.I18N_ViewEBills;
                            }
                            else
                            {
                                name = DashboardHomeConstants.I18N_ViewEBill;
                            }
                        }
                        break;
                    case ServiceEnum.PAYBILL:
                        name = DashboardHomeConstants.I18N_PayBill;
                        if (_dashboardHomeHelper.HasNormalAccounts)
                        {
                            if (_dashboardHomeHelper.HasMultipleNormalAccounts)
                            {
                                name = DashboardHomeConstants.I18N_PayBills;
                            }
                            else
                            {
                                name = DashboardHomeConstants.I18N_PayBill;
                            }
                        }
                        break;
                    case ServiceEnum.SUBMITFEEDBACK:
                        name = DashboardHomeConstants.I18N_SubmitFeedback;
                        break;
                    case ServiceEnum.SELFMETERREADING:
                        name = DashboardHomeConstants.I18N_SelfMeterReading;
                        break;
                    default:
                        name = serviceItem.ServiceName;
                        break;
                }
            }

            return GetI18NValue(name) ?? name;
        }

        private void SetIndicatorFlag(string key)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key))
            {
                var sharedPreference = NSUserDefaults.StandardUserDefaults;
                sharedPreference.SetBool(true, "Service Id - " + key);
            }
        }

        private bool ShowIndicator(string key)
        {
            bool res = false;
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key))
            {
                var sharedPreference = NSUserDefaults.StandardUserDefaults;
                res = sharedPreference.BoolForKey("Service Id - " + key);
            }
            return !res;
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

        private string GetImage(ServiceItemModel serviceItem)
        {
            string imageName = string.Empty;

            if (serviceItem != null)
            {
                if (!string.IsNullOrEmpty(serviceItem.ServiceId) && !string.IsNullOrWhiteSpace(serviceItem.ServiceId))
                {
                    if (ServicesConstants.ImageDictionary.ContainsKey(serviceItem.ServiceId))
                    {
                        imageName = ServicesConstants.ImageDictionary[serviceItem.ServiceId]
                            + (ServiceIsDisabled(serviceItem) ? "-Disabled" : string.Empty);
                    }
                }
            }
            return imageName;
        }

        private bool ServiceIsDisabled(ServiceItemModel serviceItem)
        {
            bool res = false;
            if (serviceItem != null)
            {
                switch (serviceItem.ServiceType)
                {
                    case ServiceEnum.PAYBILL:
                        if (IsRefreshScreen)
                        {
                            res = true;
                        }
                        else
                        {
                            if (_dashboardHomeHelper.IsEmptyAccount)
                            {
                                res = true;
                            }
                            else
                            {
                                res = !(_dashboardHomeHelper.HasNormalAccounts || _dashboardHomeHelper.HasSmartMeterAccounts);
                            }
                        }
                        break;
                    case ServiceEnum.VIEWBILL:
                        if (IsRefreshScreen)
                        {
                            res = true;
                        }
                        else
                        {
                            res = _dashboardHomeHelper.IsEmptyAccount;
                        }
                        break;
                    default:
                        res = false;
                        break;
                }
            }
            return res;
        }

        private void SetCardAction(ref UIView view, ServiceItemModel serviceItem)
        {
            if (serviceItem != null)
            {
                if (!ServiceIsDisabled(serviceItem))
                {
                    Action action = _actionsDictionary.ContainsKey(serviceItem.ServiceId) ? _actionsDictionary[serviceItem.ServiceId] : null;
                    view.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                    {
                        if (action != null)
                        {
                            SetIndicatorFlag(serviceItem.ServiceId);
                            action.Invoke();
                        }
                    }));
                }
            }
        }
    }
}