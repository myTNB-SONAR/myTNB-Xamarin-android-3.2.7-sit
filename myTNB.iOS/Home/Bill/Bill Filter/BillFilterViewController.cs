using CoreGraphics;
using myTNB.Home.Bill;
using System;
using System.Collections.Generic;
using UIKit;

namespace myTNB
{
    public partial class BillFilterViewController : CustomUIViewController
    {
        UILabel _typeValueLabel;
        public List<string> FilterTypes = new List<string>();
        public int FilterIndex;
        public Action<int> ApplyFilter;

        public BillFilterViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = BillConstants.Pagename_BillFilter;
            NavigationController.SetNavigationBarHidden(false, true);
            base.ViewDidLoad();
            CreateSubvies();
            SetNavigation();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _typeValueLabel.Text = FilterTypes[FilterIndex];
        }

        private void CreateSubvies()
        {
            View.BackgroundColor = MyTNBColor.LightGrayBG;
            nfloat lblWidth = ViewWidth - (BaseMarginWidth16 * 2);
            UILabel titleLabel = new UILabel(new CGRect(BaseMarginWidth16, BaseMarginHeight16, lblWidth, 0))
            {
                Font = TNBFont.MuseoSans_16_500,
                TextColor = MyTNBColor.WaterBlue,
                Lines = 0,
                Text = GetI18NValue(BillConstants.I18N_FilterDescription)
            };
            CGSize lblSize = titleLabel.SizeThatFits(new CGSize(lblWidth, 1000F));
            ViewHelper.AdjustFrameSetHeight(titleLabel, lblSize.Height);
            View.AddSubview(titleLabel);

            UIView selectorParentView = new UIView(new CGRect(0, titleLabel.Frame.GetMaxY() + GetScaledHeight(8F), ViewWidth, GetScaledWidth(83F)))
            {
                BackgroundColor = UIColor.White
            };
            View.AddSubview(selectorParentView);

            UIView selectorView = new UIView(new CGRect(BaseMarginWidth16, BaseMarginHeight16, selectorParentView.Frame.Width - (BaseMarginWidth16 * 2), GetScaledHeight(51F)))
            {
                BackgroundColor = UIColor.Clear
            };
            selectorParentView.AddSubview(selectorView);
            selectorView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("GenericSelector", null);
                GenericSelectorViewController viewController = (GenericSelectorViewController)storyBoard
                    .InstantiateViewController("GenericSelectorViewController");
                viewController.Title = GetI18NValue(BillConstants.I18N_SelectFilter);
                viewController.Items = FilterTypes;
                viewController.OnSelect = OnSelectAction;
                viewController.SelectedIndex = FilterIndex;
                viewController.IsRootPage = true;
                NavigationController.PushViewController(viewController, true);
            }));

            UILabel filterByLbl = new UILabel(new CGRect(0, 0, selectorView.Frame.Width, GetScaledHeight(12F)))
            {
                Font = TNBFont.MuseoSans_9_300,
                TextColor = MyTNBColor.SilverChalice,
                TextAlignment = UITextAlignment.Left,
                Text = GetI18NValue(BillConstants.I18N_FilterBy).ToUpper()
            };
            selectorView.AddSubview(filterByLbl);

            _typeValueLabel = new UILabel(new CGRect(0, GetScaledHeight(12F), selectorView.Frame.Width, GetScaledHeight(24F)))
            {
                Font = TNBFont.MuseoSans_16_300,
                TextColor = MyTNBColor.CharcoalGrey,
                TextAlignment = UITextAlignment.Left,
                Text = FilterTypes[FilterIndex]
            };
            selectorView.AddSubview(_typeValueLabel);

            nfloat iconWidth = GetScaledWidth(24F);
            nfloat iconHeight = GetScaledWidth(24F);
            UIImageView dropdownImg = new UIImageView(new CGRect(selectorView.Frame.Width - (iconWidth + GetScaledWidth(6F)), GetScaledHeight(12F), iconWidth, iconHeight))
            {
                Image = UIImage.FromBundle(BillConstants.IMG_DropdownIcon)
            };
            selectorView.AddSubview(dropdownImg);

            UIView lineView = new UIView(new CGRect(0, GetScaledHeight(36F), selectorView.Frame.Width, GetScaledHeight(1F)))
            {
                BackgroundColor = MyTNBColor.VeryLightPinkThree
            };
            selectorView.AddSubview(lineView);

            nfloat footerRatio = 80.0f / 320.0f;
            nfloat footerHeight = ViewWidth * footerRatio;
            nfloat footerYPos = ViewHeight - footerHeight;
            if (DeviceHelper.IsIphoneXUpResolution())
            {
                footerHeight += 20f;
            }
            UIView footerContainer = new UIView(new CGRect(0, footerYPos, ViewWidth, footerHeight))
            {
                BackgroundColor = UIColor.White
            };
            UIButton applyBtn = new UIButton(UIButtonType.Custom)
            {
                Frame = new CGRect(BaseMarginWidth16, BaseMarginHeight16, footerContainer.Frame.Width - (BaseMarginWidth16 * 2), GetScaledHeight(48F))
            };
            applyBtn.Layer.CornerRadius = GetScaledHeight(4f);
            applyBtn.Layer.BackgroundColor = MyTNBColor.FreshGreen.CGColor;
            applyBtn.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            applyBtn.Layer.BorderWidth = GetScaledHeight(1f);
            applyBtn.SetTitle(GetI18NValue(BillConstants.I18N_ApplyFilter), UIControlState.Normal);
            applyBtn.Font = TNBFont.MuseoSans_16_500;
            applyBtn.TouchUpInside += (sender, e) =>
            {
                if (ApplyFilter != null)
                {
                    DismissViewController(true, null);
                    ApplyFilter.Invoke(FilterIndex);
                }
            };
            footerContainer.AddSubview(applyBtn);
            View.AddSubview(footerContainer);
        }

        private void SetNavigation()
        {
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle(BillConstants.IMG_BackIcon)
            , UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
            Title = GetI18NValue(BillConstants.I18N_NavTitle);
        }

        void OnSelectAction(int index)
        {
            FilterIndex = index;
        }
    }
}