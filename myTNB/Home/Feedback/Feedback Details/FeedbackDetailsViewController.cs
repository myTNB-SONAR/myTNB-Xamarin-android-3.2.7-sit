using System;
using UIKit;
using myTNB.Home.Feedback.FeedbackDetails;
using myTNB.Model;
using CoreGraphics;

namespace myTNB
{
    public partial class FeedbackDetailsViewController : UIViewController
    {
        public FeedbackDetailsViewController(IntPtr handle) : base(handle)
        {
        }

        public SubmittedFeedbackDetailsDataModel FeedbackDetails = new SubmittedFeedbackDetailsDataModel();
        UIView viewContainer;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            AddBackButton();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            feedbackDetailsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            feedbackDetailsTableView.Source = new FeedbackDetailsDataSource(this, FeedbackDetails);
            feedbackDetailsTableView.ReloadData();
        }

        void AddBackButton()
        {
            NavigationController.NavigationItem.HidesBackButton = true;
            Title = FeedbackDetails.FeedbackCategoryName;
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                this.DismissViewController(true, null);
            });
            this.NavigationItem.LeftBarButtonItem = btnBack;
        }

        internal void OnImageClick(UIImage image, string fileName)
        {
            viewContainer = new UIView(UIScreen.MainScreen.Bounds);
            viewContainer.BackgroundColor = UIColor.Black;

            UILabel lblFileName = new UILabel(new CGRect(0, DeviceHelper.IsIphoneX() ? 44 : 0, viewContainer.Frame.Width, 24));
            lblFileName.BackgroundColor = UIColor.Black;
            lblFileName.Font = myTNBFont.MuseoSans16();
            lblFileName.TextColor = UIColor.White;
            lblFileName.TextAlignment = UITextAlignment.Center;
            lblFileName.Text = fileName;

            UIView viewClose = new UIView(new CGRect(viewContainer.Frame.Width - 70, DeviceHelper.IsIphoneX() ? 44 : 0, 60, 24));
            UILabel lblClose = new UILabel(new CGRect(0, 0, 60, 24));
            lblClose.BackgroundColor = UIColor.Black;
            lblClose.Font = myTNBFont.MuseoSans16();
            lblClose.TextColor = UIColor.White;
            lblClose.TextAlignment = UITextAlignment.Right;
            lblClose.Text = "Close";
            viewClose.AddSubview(lblClose);
            viewClose.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                viewContainer.Hidden = true;
                viewContainer.RemoveFromSuperview();
                UIApplication.SharedApplication.StatusBarHidden = false;
            }));

            float imgWidth = 0;
            float imgHeight = 0;
            if (image.Size.Width < image.Size.Height)
            {
                if (image.Size.Width < View.Frame.Width)
                {
                    imgWidth = (float)image.Size.Width;
                    if (image.Size.Height < View.Frame.Height - (DeviceHelper.IsIphoneX() ? 44 : 24))
                    {
                        imgHeight = (float)image.Size.Height;
                    }
                    else
                    {
                        imgHeight = (float)View.Frame.Height - (DeviceHelper.IsIphoneX() ? 44 : 24);
                    }
                }
                else
                {
                    imgWidth = (float)View.Frame.Width;
                    float ratio = (float)(image.Size.Width / image.Size.Height);
                    imgHeight = imgWidth / ratio;
                    if (imgHeight > View.Frame.Height - (DeviceHelper.IsIphoneX() ? 44 : 24))
                    {
                        imgHeight = (float)View.Frame.Height - (DeviceHelper.IsIphoneX() ? 44 : 24);
                    }
                }
            }
            else
            {
                if (image.Size.Width < View.Frame.Width)
                {
                    imgWidth = (float)image.Size.Width;
                    imgHeight = (float)image.Size.Height;
                }
                else
                {
                    imgWidth = (float)View.Frame.Width;
                    float ratio = (float)(image.Size.Width / image.Size.Height);
                    imgHeight = imgWidth / ratio;
                    if (imgHeight > View.Frame.Height - (DeviceHelper.IsIphoneX() ? 44 : 24))
                    {
                        imgHeight = (float)View.Frame.Height - (DeviceHelper.IsIphoneX() ? 44 : 24);
                    }
                }
            }

            UIImageView imgView = new UIImageView(new CGRect((viewContainer.Frame.Width / 2) - (imgWidth / 2)
                                                             , ((viewContainer.Frame.Height - (DeviceHelper.IsIphoneX() ? 44 : 24)) / 2) - (imgHeight / 2)
                                                             , imgWidth
                                                             , imgHeight));
            imgView.Image = image;

            viewContainer.AddSubviews(new UIView[] { lblFileName, viewClose, imgView });

            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
            currentWindow.AddSubview(viewContainer);
            viewContainer.Hidden = false;
            UIApplication.SharedApplication.StatusBarHidden = true;
        }
    }
}