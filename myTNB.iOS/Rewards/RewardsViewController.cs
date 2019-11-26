using CoreGraphics;
using Foundation;
using myTNB.SitecoreCMS;
using myTNB.SitecoreCMS.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UIKit;

namespace myTNB
{
    public partial class RewardsViewController : CustomUIViewController
    {
        internal UIScrollView _loadingScrollView, _topBarScrollView;
        private List<RewardsModel> _rewardsList;

        public RewardsViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = RewardsConstants.PageName;
            base.ViewDidLoad();
            View.BackgroundColor = MyTNBColor.SectionGrey;
            SetNavigationBar();
        }

        private void SetNavigationBar()
        {
            NavigationItem.HidesBackButton = true;
            //NavigationItem.Title = GetI18NValue(RewardsConstants.I18N_Rewards);
            NavigationItem.Title = "Rewards";

            UIBarButtonItem btnSavedRewards = new UIBarButtonItem(UIImage.FromBundle(RewardsConstants.Img_HeartIcon), UIBarButtonItemStyle.Done, (sender, e) =>
            {
                Debug.WriteLine("btnSavedRewards");
            });
            NavigationItem.RightBarButtonItem = btnSavedRewards;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                if (NetworkUtility.isReachable)
                {
                    InvokeOnMainThread(() =>
                    {
                        CreateLoadingCategoryTopBar();
                        InvokeInBackground(async () =>
                        {
                            await SitecoreServices.Instance.LoadRewards();
                            InvokeOnMainThread(() =>
                            {
                                NSUserDefaults userDefaults = NSUserDefaults.StandardUserDefaults;
                                var rewardsData = userDefaults.StringForKey("SiteCoreRewardsData");
                                if (rewardsData.IsValid())
                                {
                                    try
                                    {
                                        _rewardsList = JsonConvert.DeserializeObject<List<RewardsModel>>(rewardsData);
                                        if (_rewardsList != null && _rewardsList.Count > 0)
                                        {
                                            CreateCategoryTopBar();
                                            foreach (var obj in _rewardsList)
                                            {
                                                Debug.WriteLine(obj.CategoryName);
                                            }
                                        }
                                        else
                                        {
                                            // Empty rewards handling here....
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.WriteLine("Rewards data load Error: " + e.Message);
                                        // Error on rewards handling here....
                                    }
                                }
                                else
                                {
                                    // Empty rewards handling here....
                                }
                            });
                        });
                    });
                }
                else
                {
                    AlertHandler.DisplayNoDataAlert(this);
                }
            });
        }

        private void CreateLoadingCategoryTopBar()
        {
            if (_loadingScrollView != null)
            {
                _loadingScrollView.RemoveFromSuperview();
            }
            _loadingScrollView = new UIScrollView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height, ViewWidth, GetScaledHeight(44F)))
            {
                BackgroundColor = UIColor.White
            };
            _loadingScrollView.ShowsHorizontalScrollIndicator = false;
            View.AddSubview(_loadingScrollView);

            CustomShimmerView shimmeringView = new CustomShimmerView();
            UIView viewShimmerParent = new UIView(new CGRect(0, 0, ViewWidth, _loadingScrollView.Frame.Height))
            { BackgroundColor = UIColor.Clear };
            UIView viewShimmerContent = new UIView(new CGRect(0, 0, ViewWidth, _loadingScrollView.Frame.Height))
            { BackgroundColor = UIColor.Clear };

            nfloat xPos = 0;
            nfloat labelHeight = GetScaledHeight(14F);
            nfloat padding = GetScaledWidth(10F);
            for (int i = 0; i < 5; i++)
            {
                UIView categoryView = new UIView(_loadingScrollView.Bounds);
                categoryView.BackgroundColor = UIColor.White;

                UIView itemView = new UIView(new CGRect(padding, GetYLocationToCenterObject(labelHeight, categoryView), GetScaledWidth(52F), labelHeight))
                {
                    BackgroundColor = MyTNBColor.PaleGrey25
                };
                ViewHelper.AdjustFrameSetWidth(categoryView, itemView.Frame.Width + (padding * 2));
                ViewHelper.AdjustFrameSetX(categoryView, xPos);
                xPos = categoryView.Frame.GetMaxX();
                categoryView.AddSubview(itemView);
                _loadingScrollView.AddSubview(categoryView);

                viewShimmerContent.AddSubview(categoryView);
            }

            viewShimmerParent.AddSubview(shimmeringView);
            shimmeringView.ContentView = viewShimmerContent;
            shimmeringView.Shimmering = true;
            shimmeringView.SetValues();

            _loadingScrollView.AddSubview(viewShimmerParent);

            UIView lineView = new UIView(new CGRect(padding, _loadingScrollView.Frame.Height - GetScaledHeight(2F), GetScaledWidth(52F), GetScaledHeight(2)))
            {
                BackgroundColor = MyTNBColor.WaterBlue
            };
            lineView.Layer.CornerRadius = GetScaledHeight(5F);
            _loadingScrollView.AddSubview(lineView);
        }

        private void CreateCategoryTopBar()
        {
            if (_loadingScrollView != null)
            {
                _loadingScrollView.RemoveFromSuperview();
            }
            if (_topBarScrollView != null)
            {
                _topBarScrollView.RemoveFromSuperview();
            }
            _topBarScrollView = new UIScrollView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height, ViewWidth, GetScaledHeight(44F)))
            {
                BackgroundColor = UIColor.White
            };
            _topBarScrollView.ShowsHorizontalScrollIndicator = false;
            View.AddSubview(_topBarScrollView);
            SetCategoryTopBarValues();
        }

        private void SetCategoryTopBarValues()
        {
            nfloat xPos = 0;
            nfloat labelHeight = GetScaledHeight(14F);
            nfloat padding = GetScaledWidth(10F);
            for (int i = 0; i < _rewardsList.Count; i++)
            {
                CustomUIView categoryView = new CustomUIView(_topBarScrollView.Bounds);
                categoryView.BackgroundColor = UIColor.White;

                UILabel categoryLabel = new UILabel(new CGRect(padding, GetYLocationToCenterObject(labelHeight, categoryView), 0, labelHeight))
                {
                    Font = TNBFont.MuseoSans_14_500,
                    TextColor = MyTNBColor.WaterBlue,
                    TextAlignment = UITextAlignment.Center,
                    Text = _rewardsList[i].CategoryName
                };

                CGSize labelNewSize = categoryLabel.SizeThatFits(new CGSize(500F, labelHeight));
                ViewHelper.AdjustFrameSetWidth(categoryLabel, labelNewSize.Width);
                ViewHelper.AdjustFrameSetWidth(categoryView, categoryLabel.Frame.Width + (padding * 2));
                ViewHelper.AdjustFrameSetX(categoryView, xPos);

                xPos = categoryView.Frame.GetMaxX();
                categoryView.AddSubview(categoryLabel);
                _topBarScrollView.AddSubview(categoryView);
            }
            _topBarScrollView.ContentSize = new CGSize(xPos, _topBarScrollView.Frame.Height);
        }
    }
}