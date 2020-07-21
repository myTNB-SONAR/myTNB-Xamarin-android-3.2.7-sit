using CoreGraphics;
using Foundation;
using myTNB.SitecoreCMS;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using myTNB.WhatsNew;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UIKit;

namespace myTNB
{
    public partial class WhatsNewViewController : CustomUIViewController
    {
        internal UIScrollView _loadingScrollView, _topBarScrollView, _mainScrollView;
        internal UIView _skeletonLoadingView, _emptyView, _refreshScreenView;
        private UIView _tutorialContainer;
        private int _selectedCategoryIndex;
        private bool _isViewDidLoad, _hotspotIsOn;

        private List<WhatsNewModel> _categoryList;
        private List<WhatsNewModel> _whatsNewList;

        public WhatsNewViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = WhatsNewConstants.Pagename_WhatsNew;
            base.ViewDidLoad();
            _isViewDidLoad = true;
            NotifCenterUtility.AddObserver((NSString)"OnReceiveWhatsNewNotification", OnReceiveWhatsNew);
            NotifCenterUtility.AddObserver(UIApplication.WillEnterForegroundNotification, OnEnterForeground);
            View.BackgroundColor = MyTNBColor.VeryLightPinkEight;
            InitiateView();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (!DataManager.DataManager.SharedInstance.IsWhatsNewLoading)
            {
                CheckForUpdates();
            }
            else
            {
                ResetViews();
                NavigationController.NavigationBar.Hidden = false;
                SetSkeletonLoading();
            }
            _isViewDidLoad = false;
        }

        protected override void LanguageDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> What's New LanguageDidChange");
            base.LanguageDidChange(notification);
            Title = GetI18NValue(WhatsNewConstants.I18N_Title);
        }

        private void OnEnterForeground(NSNotification notification)
        {
            var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
            var topVc = AppDelegate.GetTopViewController(baseRootVc);
            if (topVc != null)
            {
                if (topVc is WhatsNewViewController)
                {
                    OnChangeStatusBarFrame(null);
                }
            }
        }

        private void OnChangeStatusBarFrame(NSNotification notification)
        {
            if (DeviceHelper.IsIphoneXUpResolution())
                return;

            SetFrames();
            _hotspotIsOn = DeviceHelper.GetStatusBarHeight() > 20;
            if (_tutorialContainer != null)
            {
                _tutorialContainer.RemoveFromSuperview();
            }
            CheckTutorialOverlay();
        }

        private void SetNavigationBar()
        {
            NavigationItem.HidesBackButton = true;
            Title = GetI18NValue(WhatsNewConstants.I18N_Title);
        }

        #region INITIALIZATION METHODS
        private void InitiateView()
        {
            SetNavigationBar();
            SetSkeletonLoading();
            if (!DataManager.DataManager.SharedInstance.IsWhatsNewLoading)
            {
                if (WhatsNewCache.WhatsNewIsAvailable)
                {
                    ProcessWhatsNew();
                }
                else
                {
                    SetRefreshScreen();
                }
            }
        }

        private void OnReceiveWhatsNew(NSNotification notification)
        {
            Debug.WriteLine("OnReceiveWhatsNewNotification");
            if (WhatsNewCache.WhatsNewIsAvailable)
            {
                ProcessWhatsNew();
            }
            else
            {
                SetRefreshScreen();
            }
        }

        private void ProcessWhatsNew()
        {
            WhatsNewServices.FilterExpiredWhatsNew();
            ValidateWhatsNew();
        }

        private void ValidateWhatsNew()
        {
            InvokeOnMainThread(async () =>
            {
                NotifCenterUtility.PostNotificationName("WhatsNewFetchUpdate", new NSObject());
                ResetViews();
                NavigationController.NavigationBar.Hidden = false;
                WhatsNewEntity whatsNewEntity = new WhatsNewEntity();
                _whatsNewList = whatsNewEntity.GetAllItems();
                if (_whatsNewList != null && _whatsNewList.Count > 0)
                {
                    _whatsNewList = _whatsNewList.FindAll(x => !x.Donot_Show_In_WhatsNew);
                    _categoryList = new List<WhatsNewModel>();
                    _categoryList = _whatsNewList.GroupBy(x => x.CategoryID).Select(x => x.First()).ToList();

                    if (_categoryList.Count > 1)
                    {
                        WhatsNewModel viewAllModel = new WhatsNewModel()
                        {
                            CategoryID = "1001",
                            CategoryName = GetI18NValue(WhatsNewConstants.I18N_ViewAll)
                        };
                        _categoryList.Insert(0, viewAllModel);
                    }
                    _selectedCategoryIndex = 0;
                    AddWhatsNewScrollView();
                    if (CategoryMenuIsVisible())
                    {
                        CreateCategoryTopBar(0);
                    }
                    _hotspotIsOn = !DeviceHelper.IsIphoneXUpResolution() && DeviceHelper.GetStatusBarHeight() > 20;
                    CheckTutorialOverlay();
                }
                else
                {
                    if (WhatsNewCache.IsSitecoreRefresh)
                    {
                        SetRefreshScreen();
                    }
                    else
                    {
                        SetEmptyView();
                    }
                }
            });
        }

        private void ResetViews()
        {
            if (_skeletonLoadingView != null)
            {
                _skeletonLoadingView.RemoveFromSuperview();
                _skeletonLoadingView = null;
            }
            if (_topBarScrollView != null)
            {
                _topBarScrollView.RemoveFromSuperview();
                _topBarScrollView = null;
            }
            if (_mainScrollView != null)
            {
                _mainScrollView.RemoveFromSuperview();
                _mainScrollView = null;
            }
            if (_emptyView != null)
            {
                _emptyView.RemoveFromSuperview();
                _emptyView = null;
            }
            if (_refreshScreenView != null)
            {
                _refreshScreenView.RemoveFromSuperview();
                _refreshScreenView = null;
            }
        }

        private void SetSkeletonLoading()
        {
            ResetViews();
            _hotspotIsOn = !DeviceHelper.IsIphoneXUpResolution() && DeviceHelper.GetStatusBarHeight() > 20;
            var addtl = _hotspotIsOn ? 20F : 0F;
            _skeletonLoadingView = new UIView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height - addtl, ViewWidth, ViewHeight))
            {
                BackgroundColor = UIColor.White
            };
            View.AddSubview(_skeletonLoadingView);

            UIScrollView scrollViewCategories = new UIScrollView(new CGRect(0, 0, ViewWidth, GetScaledHeight(44F)))
            {
                BackgroundColor = UIColor.White
            };
            scrollViewCategories.Layer.CornerRadius = GetScaledHeight(5F);
            scrollViewCategories.Layer.MasksToBounds = false;
            scrollViewCategories.Layer.ShadowColor = MyTNBColor.BabyBlue60.CGColor;
            scrollViewCategories.Layer.ShadowOpacity = 0.5F;
            scrollViewCategories.Layer.ShadowOffset = new CGSize(-4, 8);
            scrollViewCategories.Layer.ShadowRadius = 8;
            scrollViewCategories.Layer.ShadowPath = UIBezierPath.FromRect(scrollViewCategories.Bounds).CGPath;
            scrollViewCategories.ShowsHorizontalScrollIndicator = false;
            _skeletonLoadingView.AddSubview(scrollViewCategories);

            CustomShimmerView shimmeringView = new CustomShimmerView();
            UIView viewShimmerParent = new UIView(new CGRect(0, 0, ViewWidth, _skeletonLoadingView.Frame.Height))
            { BackgroundColor = UIColor.Clear };
            UIView viewShimmerContent = new UIView(new CGRect(0, 0, ViewWidth, _skeletonLoadingView.Frame.Height))
            { BackgroundColor = UIColor.Clear };

            nfloat xPos = 0;
            nfloat labelHeight = GetScaledHeight(14F);
            nfloat padding = GetScaledWidth(10F);
            for (int i = 0; i < 5; i++)
            {
                UIView categoryView = new UIView(scrollViewCategories.Bounds);
                categoryView.BackgroundColor = UIColor.White;

                UIView catItemView = new UIView(new CGRect(padding, GetYLocationToCenterObject(labelHeight, categoryView), GetScaledWidth(52F), labelHeight))
                {
                    BackgroundColor = MyTNBColor.PaleGreyThree
                };
                catItemView.Layer.CornerRadius = GetScaledHeight(2F);
                ViewHelper.AdjustFrameSetWidth(categoryView, catItemView.Frame.Width + (padding * 2));
                ViewHelper.AdjustFrameSetX(categoryView, xPos);
                xPos = categoryView.Frame.GetMaxX();
                categoryView.AddSubview(catItemView);
                scrollViewCategories.AddSubview(categoryView);

                viewShimmerContent.AddSubview(categoryView);
            }
            scrollViewCategories.AddSubview(viewShimmerParent);

            UIView lineView = new UIView(new CGRect(padding, scrollViewCategories.Frame.Height - GetScaledHeight(2F), GetScaledWidth(52F), GetScaledHeight(2F)))
            {
                BackgroundColor = MyTNBColor.WaterBlue
            };
            lineView.Layer.CornerRadius = GetScaledHeight(1F);
            scrollViewCategories.AddSubview(lineView);

            nfloat topPadding = GetScaledWidth(17F);
            nfloat yPos = scrollViewCategories.Frame.GetMaxY() + topPadding;
            for (int i = 0; i < 5; i++)
            {
                UIView itemView = new UIView(new CGRect(BaseMarginWidth16, yPos, ViewWidth - (BaseMarginWidth16 * 2), GetScaledHeight(160F)))
                {
                    BackgroundColor = UIColor.White,
                    ClipsToBounds = true
                };
                itemView.Layer.CornerRadius = GetScaledHeight(5F);
                itemView.Layer.MasksToBounds = false;
                itemView.Layer.ShadowColor = MyTNBColor.BabyBlue60.CGColor;
                itemView.Layer.ShadowOpacity = 0.5F;
                itemView.Layer.ShadowOffset = new CGSize(-4, 8);
                itemView.Layer.ShadowRadius = 8;
                itemView.Layer.ShadowPath = UIBezierPath.FromRect(itemView.Bounds).CGPath;

                UIView whatsNewImgView = new UIImageView(new CGRect(0, 0, itemView.Frame.Width, GetScaledHeight(112F)))
                {
                    BackgroundColor = MyTNBColor.PaleGreyThree,
                    ClipsToBounds = false
                };
                itemView.AddSubview(whatsNewImgView);

                UIView titleView = new UIView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(whatsNewImgView.Frame, 16F), itemView.Frame.Width - (BaseMarginWidth16 * 2), GetScaledHeight(16F)))
                {
                    BackgroundColor = MyTNBColor.PaleGreyThree
                };
                titleView.Layer.CornerRadius = GetScaledHeight(2F);
                itemView.AddSubview(titleView);

                yPos = itemView.Frame.GetMaxY() + topPadding;
                _skeletonLoadingView.AddSubview(itemView);

                viewShimmerContent.AddSubview(itemView);
            }

            viewShimmerParent.AddSubview(shimmeringView);
            shimmeringView.ContentView = viewShimmerContent;
            shimmeringView.Shimmering = true;
            shimmeringView.SetValues();
        }

        private void SetEmptyView()
        {
            ResetViews();
            _emptyView = new UIView(new CGRect(0, DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height, ViewWidth, ViewHeight))
            {
                BackgroundColor = UIColor.White
            };
            View.AddSubview(_emptyView);
            nfloat iconWidth = GetScaledWidth(102F);
            nfloat iconHeight = GetScaledHeight(94F);
            UIImageView emptyIcon = new UIImageView(new CGRect(GetXLocationToCenterObject(iconWidth), DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height + GetScaledHeight(88F), iconWidth, iconHeight))
            {
                Image = UIImage.FromBundle(WhatsNewConstants.Img_EmptyIcon)
            };
            UITextView emptyDesc = new UITextView(new CGRect(GetScaledWidth(32F), GetYLocationFromFrame(emptyIcon.Frame, 24F), ViewWidth - (GetScaledWidth(32F) * 2), GetScaledHeight(70F)))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                Font = TNBFont.MuseoSans_14_300,
                TextColor = MyTNBColor.Grey,
                TextAlignment = UITextAlignment.Center,
                Text = GetI18NValue(WhatsNewConstants.I18N_NoPromotions)
            };
            emptyDesc.TextContainer.LineFragmentPadding = 0F;

            _emptyView.AddSubviews(new UIView { emptyIcon, emptyDesc });
        }

        private void SetRefreshScreen()
        {
            ResetViews();
            NavigationController.NavigationBar.Hidden = true;

            _refreshScreenView = new UIView(new CGRect(0, 0, ViewWidth, ViewHeight))
            {
                BackgroundColor = UIColor.Clear
            };

            UIImageView refreshIcon = new UIImageView(new CGRect(0, 0, ViewWidth, ViewWidth * 0.70F))
            {
                Image = UIImage.FromBundle(WhatsNewConstants.Img_Refresh),
                BackgroundColor = UIColor.Clear
            };

            _refreshScreenView.AddSubview(refreshIcon);

            UILabel title = new UILabel(new CGRect(BaseMarginWidth16, DeviceHelper.GetStatusBarHeight() + GetScaledHeight(8F), ViewWidth - (BaseMarginWidth16 * 2), GetScaledHeight(24F)))
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = UIColor.White,
                Text = GetI18NValue(WhatsNewConstants.I18N_Title),
                TextAlignment = UITextAlignment.Center
            };
            _refreshScreenView.AddSubview(title);

            NSError htmlBodyError = null;
            NSAttributedString htmlBody = TextHelper.ConvertToHtmlWithFont(GetCommonI18NValue(Constants.Common_RefreshMessage)
                , ref htmlBodyError, TNBFont.FONTNAME_300, (float)GetScaledHeight(16F));
            NSMutableAttributedString mutableHTMLBody = new NSMutableAttributedString(htmlBody);
            mutableHTMLBody.AddAttributes(new UIStringAttributes
            {
                ForegroundColor = MyTNBColor.Grey,
                ParagraphStyle = new NSMutableParagraphStyle
                {
                    Alignment = UITextAlignment.Center,
                    LineSpacing = 3.0f
                }
            }, new NSRange(0, htmlBody.Length));

            nfloat descwidth = ViewWidth - (BaseMarginWidth16 * 2);
            UITextView desc = new UITextView(new CGRect(BaseMarginWidth16, GetYLocationFromFrame(refreshIcon.Frame, 16F), descwidth, 0))
            {
                BackgroundColor = UIColor.Clear,
                Editable = false,
                ScrollEnabled = false,
                AttributedText = mutableHTMLBody,
                UserInteractionEnabled = false,
                TextAlignment = UITextAlignment.Center

            };
            desc.TextContainer.LineFragmentPadding = 0F;
            CGSize cGSize = desc.SizeThatFits(new CGSize(descwidth, GetScaledHeight(500F)));
            ViewHelper.AdjustFrameSetWidth(desc, cGSize.Width);
            ViewHelper.AdjustFrameSetHeight(desc, cGSize.Height);

            _refreshScreenView.AddSubview(desc);

            CustomUIButtonV2 btnRefresh = new CustomUIButtonV2()
            {
                Frame = new CGRect(BaseMargin, GetYLocationFromFrame(desc.Frame, 16), BaseMarginedWidth, GetScaledHeight(48)),
                BackgroundColor = MyTNBColor.FreshGreen,
                PageName = PageName,
                EventName = WhatsNewConstants.Event_Refresh,
                Hidden = false
            };
            btnRefresh.SetTitle(GetCommonI18NValue(Constants.Common_RefreshNow), UIControlState.Normal);
            btnRefresh.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnRefresh.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                RefreshButtonOnTap();
            }));
            _refreshScreenView.AddSubview(btnRefresh);
            View.AddSubview(_refreshScreenView);
        }
        #endregion

        #region CATEGORY TOP BAR MENU
        private void CreateCategoryTopBar(nfloat addtlPadding)
        {
            _hotspotIsOn = !DeviceHelper.IsIphoneXUpResolution() && DeviceHelper.GetStatusBarHeight() > 20;
            var addtl = _hotspotIsOn ? 20F : 0F;
            _topBarScrollView = new UIScrollView(new CGRect(0
                , DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height - addtl
                , ViewWidth, GetScaledHeight(44F)))
            {
                BackgroundColor = UIColor.White,
                Tag = 1001
            };
            _topBarScrollView.ShowsHorizontalScrollIndicator = false;
            View.AddSubview(_topBarScrollView);
            SetCategoryTopBarValues(addtlPadding);
        }

        private void SetCategoryTopBarValues(nfloat addtlPadding)
        {
            nfloat xPos = 0;
            nfloat labelHeight = GetScaledHeight(16F);
            nfloat padding = GetScaledWidth(10F) + addtlPadding;
            CustomUIView lastView = new CustomUIView();

            for (int i = 0; i < _categoryList.Count; i++)
            {
                CustomUIView categoryView = new CustomUIView(_topBarScrollView.Bounds)
                {
                    BackgroundColor = UIColor.White,
                    Tag = i
                };
                categoryView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    OnSelectCategoryAction((int)categoryView.Tag);
                }));
                UILabel categoryLabel = new UILabel(new CGRect(padding, GetYLocationToCenterObject(labelHeight, categoryView), 0, labelHeight))
                {
                    Font = TNBFont.MuseoSans_14_500,
                    TextColor = i == 0 ? MyTNBColor.WaterBlue : MyTNBColor.WarmGrey,
                    TextAlignment = UITextAlignment.Center,
                    Text = _categoryList[i].CategoryName,
                    Tag = WhatsNewConstants.Tag_CategoryLabel
                };

                CGSize labelNewSize = categoryLabel.SizeThatFits(new CGSize(500F, labelHeight));
                ViewHelper.AdjustFrameSetWidth(categoryLabel, labelNewSize.Width);
                ViewHelper.AdjustFrameSetWidth(categoryView, categoryLabel.Frame.Width + (padding * 2));
                ViewHelper.AdjustFrameSetX(categoryView, xPos);

                UIView lineView = new UIView(new CGRect(padding
                    , _topBarScrollView.Frame.Height - GetScaledHeight(2F), labelNewSize.Width, GetScaledHeight(2)))
                {
                    BackgroundColor = MyTNBColor.WaterBlue,
                    Tag = WhatsNewConstants.Tag_SelectedCategory,
                    Hidden = i != 0
                };
                lineView.Layer.CornerRadius = GetScaledHeight(1F);
                categoryView.AddSubview(lineView);

                xPos = categoryView.Frame.GetMaxX();
                categoryView.AddSubview(categoryLabel);
                _topBarScrollView.AddSubview(categoryView);
                lastView = categoryView;
            }
            _topBarScrollView.ContentSize = new CGSize(xPos, _topBarScrollView.Frame.Height);
            AdjustCategoryMenuUI(lastView);
        }

        private void AdjustCategoryMenuUI(CustomUIView lastView)
        {
            if (lastView != null)
            {
                if (Convert.ToInt32(lastView.Frame.GetMaxX()) < Convert.ToInt32(ViewWidth))
                {
                    nfloat diff = ViewWidth - lastView.Frame.GetMaxX();
                    nfloat xtraPadding = diff / (_categoryList.Count * 2);

                    if (_topBarScrollView != null)
                    {
                        _topBarScrollView.RemoveFromSuperview();
                        _topBarScrollView = null;
                    }
                    CreateCategoryTopBar(xtraPadding);
                }
            }
        }

        private void OnSelectCategoryAction(int index)
        {
            _selectedCategoryIndex = index;
            _mainScrollView.SetContentOffset(new CGPoint(_mainScrollView.Frame.Width * _selectedCategoryIndex, 0), true);
        }

        private void CategorySelectionUpdate(int index)
        {
            foreach (UIView catView in _topBarScrollView.Subviews)
            {
                if (catView != null)
                {
                    UILabel label = catView.ViewWithTag(WhatsNewConstants.Tag_CategoryLabel) as UILabel;
                    if (label != null)
                    {
                        label.TextColor = catView.Tag == index ? MyTNBColor.WaterBlue : MyTNBColor.WarmGrey;
                    }
                    UIView line = catView.ViewWithTag(WhatsNewConstants.Tag_SelectedCategory) as UIView;
                    if (line != null)
                    {
                        line.Hidden = catView.Tag != index;
                    }
                }

                try
                {
                    if (catView != null && catView.GetType() != typeof(UIImageView))
                    {
                        if (catView.Tag == _selectedCategoryIndex)
                        {
                            nfloat maxXPos = catView.Frame.X - _topBarScrollView.ContentOffset.X + catView.Frame.Width;
                            nfloat minXPos = catView.Frame.X;
                            if (maxXPos > ViewWidth)
                            {
                                nfloat addtl = 0;
                                UIView nView = _topBarScrollView.ViewWithTag(_selectedCategoryIndex + 1) as UIView;
                                if (nView != null && nView.GetType() != typeof(UIImageView))
                                {
                                    addtl = nView.Frame.Width * 0.30F;
                                }
                                _topBarScrollView.SetContentOffset(new CGPoint(catView.Frame.X + catView.Frame.Width - ViewWidth + addtl, 0), true);
                            }
                            else if (minXPos < _topBarScrollView.ContentOffset.X)
                            {
                                nfloat addtl = 0;
                                UIView pView = _topBarScrollView.ViewWithTag(_selectedCategoryIndex - 1) as UIView;
                                if (pView != null && pView.GetType() != typeof(UIImageView))
                                {
                                    addtl = pView.Frame.Width * 0.30F;
                                }
                                _topBarScrollView.SetContentOffset(new CGPoint(minXPos - addtl, 0), true);
                            }
                        }
                    }
                }

                catch (MonoTouchException m) { Debug.WriteLine("Error: " + m.Message); }
                catch (Exception e)
                {
                    Debug.WriteLine("Error: " + e.Message);
                }
            }
        }
        #endregion

        #region WHAT'S NEW SCROLL VIEW
        private void AddWhatsNewScrollView()
        {
            _hotspotIsOn = !DeviceHelper.IsIphoneXUpResolution() && DeviceHelper.GetStatusBarHeight() > 20;
            var addtl = _hotspotIsOn ? 20F : 0F;
            var categoryHeight = _categoryList != null && _categoryList.Count > 1 ? GetScaledHeight(44F) : 0;
            _mainScrollView = new UIScrollView(new CGRect(0
                , DeviceHelper.GetStatusBarHeight() + NavigationController.NavigationBar.Frame.Height + categoryHeight - addtl
                , ViewWidth, ViewHeight - categoryHeight + addtl))
            {
                Delegate = new ScrollViewDelegate(this),
                PagingEnabled = true,
                ShowsHorizontalScrollIndicator = false,
                ShowsVerticalScrollIndicator = false,
                ClipsToBounds = true,
                BackgroundColor = UIColor.Clear,
                Hidden = false,
                Bounces = false
            };
            SetWhatsNewTableViewForCategory();
            View.AddSubview(_mainScrollView);
            if (DeviceHelper.IsIOS10AndBelow)
            {
                _mainScrollView.SetContentOffset(new CGPoint(_mainScrollView.Frame.Width * _selectedCategoryIndex
                    , NavigationController.NavigationBar.Frame.Height + DeviceHelper.GetStatusBarHeight()), true);
            }
        }

        private void SetWhatsNewTableViewForCategory()
        {
            nfloat width = _mainScrollView.Frame.Width;
            for (int i = 0; i < _categoryList.Count; i++)
            {
                UIView viewContainer = new UIView(_mainScrollView.Bounds)
                {
                    BackgroundColor = UIColor.Clear,
                    Tag = WhatsNewConstants.Tag_ViewContainer * (i + 1)
                };

                UITableView whatsNewTableView = new UITableView(viewContainer.Bounds)
                {
                    BackgroundColor = UIColor.Clear,
                    Tag = WhatsNewConstants.Tag_TableView,
                    SeparatorStyle = UITableViewCellSeparatorStyle.None
                };

                whatsNewTableView.RegisterClassForCellReuse(typeof(WhatsNewCell), WhatsNewConstants.Cell_WhatsNew);
                viewContainer.AddSubview(whatsNewTableView);

                _mainScrollView.AddSubview(viewContainer);

                ViewHelper.AdjustFrameSetX(viewContainer, i * width);

                var filteredList = i == 0 ? _whatsNewList : FilterWhatsNew(i);
                whatsNewTableView.Source = new WhatsNewDataSource(this, filteredList, GetI18NValue);
                whatsNewTableView.ReloadData();
            }
            _mainScrollView.ContentSize = new CGSize(_mainScrollView.Frame.Width * _categoryList.Count, _mainScrollView.Frame.Height);
        }

        private List<WhatsNewModel> FilterWhatsNew(int index)
        {
            List<WhatsNewModel> filteredWhatsNew = new List<WhatsNewModel>();
            var activeCatId = _categoryList[index].CategoryID;
            filteredWhatsNew = _whatsNewList.FindAll(x => x.CategoryID.Equals(activeCatId)).ToList();
            return filteredWhatsNew;
        }

        private void ScrollViewHasPaginated()
        {
            CategorySelectionUpdate(_selectedCategoryIndex);
        }

        private class ScrollViewDelegate : UIScrollViewDelegate
        {
            WhatsNewViewController _controller;
            public ScrollViewDelegate(WhatsNewViewController controller)
            {
                _controller = controller;
            }
            public override void Scrolled(UIScrollView scrollView)
            {
                int newPageIndex = (int)Math.Round(_controller._mainScrollView.ContentOffset.X / _controller._mainScrollView.Frame.Width);
                if (newPageIndex == _controller._selectedCategoryIndex)
                    return;

                _controller._selectedCategoryIndex = newPageIndex;
                _controller.ScrollViewHasPaginated();
            }
        }
        #endregion

        #region SERVICE LOGIC METHODS
        private void CheckForUpdates()
        {
            if (NetworkUtility.isReachable)
            {
                InvokeInBackground(async () =>
                {
                    bool hasUpdate = await SitecoreServices.Instance.WhatsNewHasUpdates() || WhatsNewCache.RefreshWhatsNew;
                    InvokeOnMainThread(() =>
                    {
                        if (hasUpdate)
                        {
                            WhatsNewCache.RefreshWhatsNew = false;
                            DataManager.DataManager.SharedInstance.IsWhatsNewLoading = true;
                            ResetViews();
                            NavigationController.NavigationBar.Hidden = false;
                            SetSkeletonLoading();
                            InvokeInBackground(async () =>
                            {
                                await SitecoreServices.Instance.LoadWhatsNew(true);
                                InvokeOnMainThread(() =>
                                {
                                    DataManager.DataManager.SharedInstance.IsWhatsNewLoading = false;
                                    if (WhatsNewCache.WhatsNewIsAvailable)
                                    {
                                        ProcessWhatsNew();
                                    }
                                    else
                                    {
                                        SetRefreshScreen();
                                    }
                                });
                            });
                        }
                        else
                        {
                            if (WhatsNewCache.WhatsNewIsAvailable)
                            {
                                bool needsUpdate = WhatsNewServices.FilterExpiredWhatsNew();
                                if (needsUpdate)
                                {
                                    ValidateWhatsNew();
                                }
                                else
                                {
                                    if (!_isViewDidLoad)
                                    {
                                        NotifCenterUtility.PostNotificationName("WhatsNewFetchUpdate", new NSObject());
                                        RefreshTable();
                                        _hotspotIsOn = !DeviceHelper.IsIphoneXUpResolution() && DeviceHelper.GetStatusBarHeight() > 20;
                                        CheckTutorialOverlay();
                                    }
                                }
                            }
                            else
                            {
                                SetRefreshScreen();
                            }
                        }
                    });
                });
            }
            else
            {
                AlertHandler.DisplayNoDataAlert(this);
            }
        }
        #endregion

        #region ACTION METHODS
        private void RefreshButtonOnTap()
        {
            if (NetworkUtility.isReachable)
            {
                ResetViews();
                NavigationController.NavigationBar.Hidden = false;
                SetSkeletonLoading();

                InvokeInBackground(async () =>
                {
                    DataManager.DataManager.SharedInstance.IsWhatsNewLoading = true;
                    await SitecoreServices.Instance.LoadWhatsNew(true);
                    DataManager.DataManager.SharedInstance.IsWhatsNewLoading = false;
                    InvokeOnMainThread(() =>
                    {
                        if (WhatsNewCache.WhatsNewIsAvailable)
                        {
                            ProcessWhatsNew();
                        }
                        else
                        {
                            SetRefreshScreen();
                        }
                    });
                });
            }
            else
            {
                AlertHandler.DisplayNoDataAlert(this);
            }
        }

        public void OnItemSelection(WhatsNewModel whatsNew)
        {
            if (whatsNew != null)
            {
                WhatsNewDetailsViewController whatsNewDetailView = new WhatsNewDetailsViewController();
                whatsNewDetailView.WhatsNewModel = whatsNew;
                UINavigationController navController = new UINavigationController(whatsNewDetailView)
                {
                    ModalPresentationStyle = UIModalPresentationStyle.FullScreen
                };
                PresentViewController(navController, true, null);
                WhatsNewServices.SetIsRead(whatsNew.ID);
                OnUpdateReadWhatsNew(whatsNew);
            }
        }

        private void OnUpdateReadWhatsNew(WhatsNewModel whatsNew)
        {
            if (whatsNew == null)
                return;

            var entity = WhatsNewEntity.GetItem(whatsNew.ID);
            if (entity != null)
            {
                var entityModel = whatsNew.ToEntity();
                entityModel.IsRead = true;
                WhatsNewEntity whatsNewEntity = new WhatsNewEntity();
                whatsNewEntity.UpdateItem(entityModel);

                int indx = _whatsNewList.FindIndex(x => x.ID == entityModel.ID);
                if (indx > -1 && indx < _whatsNewList.Count)
                {
                    _whatsNewList[indx].IsRead = entityModel.IsRead;
                    RefreshTable();
                }
            }
        }

        private void RefreshTable()
        {
            if (_whatsNewList != null && _whatsNewList.Count > 0)
            {
                for (int c = 0; c < _categoryList.Count; c++)
                {
                    UIView viewContainer = _mainScrollView.Subviews[c];
                    if (viewContainer != null && viewContainer.Subviews.Count() > 0)
                    {
                        if (viewContainer.Subviews[0] is UITableView table)
                        {
                            var filteredList = c == 0 ? _whatsNewList : FilterWhatsNew(c);
                            table.ClearsContextBeforeDrawing = true;
                            table.Source = new WhatsNewDataSource(this, filteredList, GetI18NValue);
                            UIView.PerformWithoutAnimation(() =>
                            {
                                table.BeginUpdates();
                                table.ReloadData();
                                table.EndUpdates();
                            });
                        }
                    }
                }
            }
        }
        #endregion

        #region TUTORIAL OVERLAY
        public void CheckTutorialOverlay()
        {
            if (!WhatsNewCache.WhatsNewIsAvailable) { return; }

            var sharedPreference = NSUserDefaults.StandardUserDefaults;
            var tutorialOverlayHasShown = sharedPreference.BoolForKey(WhatsNewConstants.Pref_WhatsNewTutorialOverlay);

            if (tutorialOverlayHasShown) { return; }

            if (!DataManager.DataManager.SharedInstance.IsWhatsNewLoading && _whatsNewList != null && _whatsNewList.Count > 0)
            {
                InvokeOnMainThread(() =>
                {
                    var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
                    var topVc = AppDelegate.GetTopViewController(baseRootVc);
                    if (topVc != null)
                    {
                        if (topVc is WhatsNewViewController)
                        {
                            ShowTutorialOverlay();
                        }
                        else
                        {
                            if (_tutorialContainer != null)
                            {
                                _tutorialContainer.RemoveFromSuperview();
                            }
                        }
                    }
                });
            }
        }

        private void ShowTutorialOverlay()
        {
            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;

            if (_tutorialContainer != null && _tutorialContainer.IsDescendantOfView(currentWindow)) { return; }

            nfloat width = currentWindow.Frame.Width;
            nfloat height = currentWindow.Frame.Height;

            _tutorialContainer = new UIView(new CGRect(0, 0, width, height))
            {
                BackgroundColor = UIColor.Clear,
                Tag = 1001
            };

            WhatsNewTutorialOverlay tutorialView = new WhatsNewTutorialOverlay(_tutorialContainer, this, _hotspotIsOn)
            {
                OnDismissAction = HideTutorialOverlay,
                GetI18NValue = GetI18NValue
            };
            var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
            var topVc = AppDelegate.GetTopViewController(baseRootVc);
            if (topVc != null)
            {
                if (topVc is WhatsNewViewController && _tutorialContainer != null && !_tutorialContainer.IsDescendantOfView(currentWindow))
                {
                    foreach (UIView view in currentWindow.Subviews)
                    {
                        if (view.Tag == 1001)
                        {
                            view.RemoveFromSuperview();
                            break;
                        }
                    }

                    _tutorialContainer.AddSubview(tutorialView.GetView());
                    currentWindow.AddSubview(_tutorialContainer);
                }
                else
                {
                    if (_tutorialContainer != null)
                    {
                        _tutorialContainer.RemoveFromSuperview();
                    }
                }
            }
        }

        private void HideTutorialOverlay()
        {
            if (_tutorialContainer != null)
            {
                _tutorialContainer.Alpha = 1F;
                _tutorialContainer.Transform = CGAffineTransform.MakeIdentity();
                UIView.Animate(0.3, 0, UIViewAnimationOptions.CurveEaseInOut, () =>
                {
                    _tutorialContainer.Alpha = 0F;
                }, _tutorialContainer.RemoveFromSuperview);

                var sharedPreference = NSUserDefaults.StandardUserDefaults;
                sharedPreference.SetBool(true, WhatsNewConstants.Pref_WhatsNewTutorialOverlay);
                sharedPreference.Synchronize();
            }
        }

        public nfloat GetFirstWhatsNewYPos()
        {
            nfloat yPos = 0;
            if (_mainScrollView != null)
            {
                yPos = _mainScrollView.Frame.Y + GetScaledHeight(17F);
            }
            return yPos;
        }

        public nfloat GetNavigationMaxYPos()
        {
            try
            {
                return NavigationController.NavigationBar.Frame.GetMaxY();
            }
            catch (MonoTouchException m)
            {
                Debug.WriteLine("Error in services: " + m.Message);
                return 0;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in services: " + e.Message);
                return 0;
            }
        }

        public bool CategoryMenuIsVisible()
        {
            return _categoryList != null && _categoryList.Count > 2;
        }
        #endregion
    }
}
