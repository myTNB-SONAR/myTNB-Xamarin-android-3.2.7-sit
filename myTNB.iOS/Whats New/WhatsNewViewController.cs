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
                SetSkeletonLoading();
            }
            _isViewDidLoad = false;
        }

        private void OnEnterForeground(NSNotification notification)
        {
            var baseRootVc = UIApplication.SharedApplication.KeyWindow?.RootViewController;
            var topVc = AppDelegate.GetTopViewController(baseRootVc);
            if (topVc != null)
            {
                if (topVc is RewardsViewController)
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
                ResetViews();
                WhatsNewEntity whatsNewEntity = new WhatsNewEntity();
                _whatsNewList = whatsNewEntity.GetAllItems();
                if (_whatsNewList != null && _whatsNewList.Count > 0)
                {
                    _categoryList = new List<WhatsNewModel>();
                    _categoryList = _whatsNewList.GroupBy(x => x.CategoryID).Select(x => x.First()).ToList();

                    if (_categoryList.Count > 1)
                    {
                        WhatsNewModel viewAllModel = new WhatsNewModel()
                        {
                            CategoryID = "1001",
                            CategoryName = "View All"
                        };
                        _categoryList.Insert(0, viewAllModel);
                        CreateCategoryTopBar();
                    }
                    _selectedCategoryIndex = 0;
                    AddWhatsNewScrollView();
                    _hotspotIsOn = !DeviceHelper.IsIphoneXUpResolution() && DeviceHelper.GetStatusBarHeight() > 20;
                    //CheckTutorialOverlay();
                }
                else
                {
                    SetEmptyView();
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
                Text = GetI18NValue(WhatsNewConstants.I18N_EmptyDesc)
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
                Text = "What's New",
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
        private void CreateCategoryTopBar()
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
            SetCategoryTopBarValues();
        }

        private void SetCategoryTopBarValues()
        {
            nfloat xPos = 0;
            nfloat labelHeight = GetScaledHeight(14F);
            nfloat padding = GetScaledWidth(10F);

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
            }
            _topBarScrollView.ContentSize = new CGSize(xPos, _topBarScrollView.Frame.Height);
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

            View.AddSubview(_mainScrollView);
            SetWhatsNewTableViewForCategory();
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
                    bool hasUpdate = await SitecoreServices.Instance.WhatsNewHasUpdates();
                    InvokeOnMainThread(() =>
                    {
                        if (hasUpdate)
                        {
                            DataManager.DataManager.SharedInstance.IsWhatsNewLoading = true;
                            ResetViews();
                            SetSkeletonLoading();
                            InvokeInBackground(async () =>
                            {
                                await SitecoreServices.Instance.LoadWhatsNew(true);
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
                                        OnReloadTableAction(_selectedCategoryIndex);
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

        }

        public void OnItemSelection(WhatsNewModel whatsNew, int index)
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
                OnUpdateReadWhatsNew(whatsNew, index);
            }
        }

        private void OnUpdateReadWhatsNew(WhatsNewModel whatsNew, int index)
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
                    OnReloadTableAction(index);
                }
            }
        }

        private void OnReloadTableAction(int index)
        {
            if (_whatsNewList != null && _whatsNewList.Count > 0 && index > -1 && index < _whatsNewList.Count)
            {
                WhatsNewModel whatsNew = _whatsNewList[index];
                var catIndx = _categoryList.FindIndex(x => x.CategoryID.Equals(whatsNew.CategoryID));

                if (catIndx > -1 && catIndx < _mainScrollView.Subviews.Count())
                {
                    UIView viewContainer = _mainScrollView.Subviews[catIndx];
                    if (viewContainer != null && viewContainer.Subviews.Count() > 0)
                    {
                        if (viewContainer.Subviews[0] is UITableView table)
                        {
                            var filteredList = catIndx == 0 ? _whatsNewList : FilterWhatsNew(catIndx);
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

                UIView viewAllView = _mainScrollView.Subviews[0];
                if (viewAllView != null && viewAllView.Subviews.Count() > 0)
                {
                    if (viewAllView.Subviews[0] is UITableView table)
                    {
                        table.ClearsContextBeforeDrawing = true;
                        table.Source = new WhatsNewDataSource(this, _whatsNewList, GetI18NValue);
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
        #endregion

        #region TUTORIAL OVERLAY
        public void CheckTutorialOverlay()
        {

        }
        #endregion
    }
}
