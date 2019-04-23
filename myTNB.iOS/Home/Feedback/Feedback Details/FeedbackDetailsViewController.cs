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
            GetNickName();
            feedbackDetailsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            feedbackDetailsTableView.Source = new FeedbackDetailsDataSource(this, FeedbackDetails);
            feedbackDetailsTableView.ReloadData();
        }

        /// <summary>
        /// Gets the nickname of the account number to be appended for display.
        /// </summary>
        internal void GetNickName()
        {
            if (FeedbackDetails.FeedbackCategoryId == "1" && !string.IsNullOrEmpty(FeedbackDetails.AccountNum))
            {
                var index = DataManager.DataManager.SharedInstance.AccountRecordsList?.d?.FindIndex(x => x.accNum == FeedbackDetails.AccountNum) ?? -1;

                if (index >= 0)
                {
                    FeedbackDetails.AccountNum = string.Format("{0} - {1}", FeedbackDetails?.AccountNum
                        , DataManager.DataManager.SharedInstance?.AccountRecordsList?.d[index]?.accDesc);
                }
            }
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
            viewContainer = new UIView(UIScreen.MainScreen.Bounds)
            {
                BackgroundColor = UIColor.Black
            };

            UILabel lblFileName = new UILabel(new CGRect(0, DeviceHelper.IsIphoneXUpResolution() ? 44 : 0, viewContainer.Frame.Width, 24))
            {
                BackgroundColor = UIColor.Black,
                Font = MyTNBFont.MuseoSans16,
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Center,
                Text = fileName
            };

            UIView viewClose = new UIView(new CGRect(viewContainer.Frame.Width - 70, DeviceHelper.IsIphoneXUpResolution() ? 44 : 0, 60, 24));
            UILabel lblClose = new UILabel(new CGRect(0, 0, 60, 24))
            {
                BackgroundColor = UIColor.Black,
                Font = MyTNBFont.MuseoSans16,
                TextColor = UIColor.White,
                TextAlignment = UITextAlignment.Right,
                Text = "Common_Close".Translate()
            };
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
                    if (image.Size.Height < View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 44 : 24))
                    {
                        imgHeight = (float)image.Size.Height;
                    }
                    else
                    {
                        imgHeight = (float)View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 44 : 24);
                    }
                }
                else
                {
                    imgWidth = (float)View.Frame.Width;
                    float ratio = (float)(image.Size.Width / image.Size.Height);
                    imgHeight = imgWidth / ratio;
                    if (imgHeight > View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 44 : 24))
                    {
                        imgHeight = (float)View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 44 : 24);
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
                    if (imgHeight > View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 44 : 24))
                    {
                        imgHeight = (float)View.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 44 : 24);
                    }
                }
            }

            UIImageView imgView = new UIImageView(new CGRect((viewContainer.Frame.Width / 2) - (imgWidth / 2)
                , ((viewContainer.Frame.Height - (DeviceHelper.IsIphoneXUpResolution() ? 44 : 24)) / 2) - (imgHeight / 2)
                , imgWidth, imgHeight))
            {
                Image = image
            };

            viewContainer.AddSubviews(new UIView[] { lblFileName, viewClose, imgView });

            UIWindow currentWindow = UIApplication.SharedApplication.KeyWindow;
            currentWindow.AddSubview(viewContainer);
            viewContainer.Hidden = false;
            UIApplication.SharedApplication.StatusBarHidden = true;
        }
    }
}