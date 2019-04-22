using System;
using UIKit;
using System.Collections.Generic;
using myTNB.Model;
using CoreGraphics;
using myTNB.Home.Feedback;

namespace myTNB
{
    public partial class FeedbackTypeViewController : UIViewController
    {
        public List<OtherFeedbackTypeDataModel> _feedbackTypeList = new List<OtherFeedbackTypeDataModel>();

        public FeedbackTypeViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            FeedbackTypeTableView.Source = new SelectFeedbackTypeDataSource(this, _feedbackTypeList);
            FeedbackTypeTableView.RowHeight = 56f;
            FeedbackTypeTableView.TableFooterView = new UIView(new CGRect(0, 0, 0, 0));

            FeedbackTypeTableView.ScrollEnabled = FeedbackTypeTableView.ContentSize.Height >= FeedbackTypeTableView.Frame.Size.Height;

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