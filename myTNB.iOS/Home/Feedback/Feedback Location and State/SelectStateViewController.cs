using System;
using UIKit;
using CoreGraphics;
using myTNB.Home.Feedback;
using myTNB.Model;
using System.Collections.Generic;

namespace myTNB
{
    public partial class SelectStateViewController : UIViewController
    {
        public List<StatesForFeedbackDataModel> _statesForFeedbackList = new List<StatesForFeedbackDataModel>();

        public SelectStateViewController(IntPtr handle) : base(handle)
        {
        }
        public Action OnSelect { get; set; }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            StateTableView.Source = new SelectStateDataSource(this, _statesForFeedbackList);
            StateTableView.RowHeight = 56f;
            StateTableView.TableFooterView = new UIView(new CGRect(0, 0, 0, 0));

            StateTableView.ScrollEnabled = StateTableView.ContentSize.Height >= StateTableView.Frame.Size.Height;

            this.NavigationItem.HidesBackButton = true;
            AddBackButton();
        }

        internal void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                this.NavigationController.PopViewController(true);
            });
            this.NavigationItem.LeftBarButtonItem = btnBack;
        }


    }
}