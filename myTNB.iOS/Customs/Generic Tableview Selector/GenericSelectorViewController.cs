using System;
using System.Collections.Generic;
using CoreGraphics;
using UIKit;

namespace myTNB
{
    public partial class GenericSelectorViewController : CustomUIViewController
    {
        public Action<int> OnSelect;
        public List<string> Items;
        public int SelectedIndex = -1;
        public bool IsRootPage;
        public bool HasSectionTitle { set; get; }
        public bool HasCTA { set; get; }
        public string SectionTitle { set; get; } = string.Empty;
        public string CTATitle { set; private get; } = string.Empty;

        internal CustomUIButtonV2 _btnCTA;
        internal int DefaultIndex;

        public GenericSelectorViewController(IntPtr handle) : base(handle) { }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            DefaultIndex = SelectedIndex;
            View.BackgroundColor = MyTNBColor.SectionGrey;
            // Perform any additional setup after loading the view, typically from a nib.
            SetNavigationBar();
            SetTableView();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        private void SetNavigationBar()
        {
            NavigationItem.HidesBackButton = true;
            UIBarButtonItem btnBack = new UIBarButtonItem(UIImage.FromBundle(Constants.IMG_Back)
                , UIBarButtonItemStyle.Done, (sender, e) =>
            {
                if (IsRootPage && NavigationController != null)
                {
                    NavigationController.PopViewController(true);
                }
                else
                {
                    DismissViewController(true, null);
                }
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        private void SetTableView()
        {
            nfloat tableHeight = ViewHeight - (HasCTA ? ScaleUtility.GetScaledHeight(80) : 0);

            UITableView genericTableView = new UITableView
            {
                Frame = new CGRect(new CGPoint(0, 0), new CGSize(ViewWidth, tableHeight)),
                Source = new GenericSelectorDataSource(this),
                SeparatorStyle = UITableViewCellSeparatorStyle.None,
                BackgroundColor = MyTNBColor.SectionGrey
            };
            genericTableView.RegisterClassForCellReuse(typeof(UITableViewCell), "genericViewCell");
            genericTableView.ReloadData();
            View.AddSubview(genericTableView);
            if (HasCTA)
            {
                _btnCTA = new CustomUIButtonV2
                {
                    Frame = new CGRect(BaseMarginWidth16
                       , genericTableView.Frame.GetMaxY() + GetScaledWidth(16)
                       , BaseMarginedWidth, GetScaledHeight(48)),
                    BackgroundColor = MyTNBColor.SilverChalice,
                    Enabled = false
                };
                _btnCTA.SetTitle(CTATitle, UIControlState.Normal);
                _btnCTA.SetTitleColor(UIColor.White, UIControlState.Normal);
                _btnCTA.Layer.BorderColor = MyTNBColor.SilverChalice.CGColor;
                _btnCTA.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    if (OnSelect != null)
                    {
                        OnSelect.Invoke(SelectedIndex);
                    }
                }));
                View.AddSubview(_btnCTA);
            }
        }

        internal void SetCTAState()
        {
            bool isActive = DefaultIndex != SelectedIndex;
            if (_btnCTA != null)
            {
                _btnCTA.Enabled = isActive;
                _btnCTA.BackgroundColor = isActive ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice;
                _btnCTA.Layer.BorderColor = (isActive ? MyTNBColor.FreshGreen : MyTNBColor.SilverChalice).CGColor;
            }
        }
    }
}