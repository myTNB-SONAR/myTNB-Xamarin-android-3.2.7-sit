using System;
using UIKit;
using myTNB.Dashboard.DashboardComponents;
using CoreGraphics;
using myTNB.Home.Feedback;
using System.Threading.Tasks;
using myTNB.Model;
using System.Collections.Generic;
using Foundation;
using System.Diagnostics;

namespace myTNB
{
    public partial class FeedbackViewController : UIViewController
    {
        public FeedbackViewController(IntPtr handle) : base(handle)
        {
        }

        TitleBarComponent _titleBarComponent;
        SubmittedFeedbackResponseModel _submittedFeedback = new SubmittedFeedbackResponseModel();
        string _email = string.Empty;

        public bool isFromPreLogin;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NSNotificationCenter.DefaultCenter.AddObserver((Foundation.NSString)"LanguageDidChange", LanguageDidChange);
            if (isFromPreLogin == true)
            {
                feedbackTableView.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height - (114 - 64));
                AddBackButton();
                this.Title = "Feedback_Title".Translate();
            }
            else
            {
                feedbackTableView.Frame = new CGRect(0, DeviceHelper.IsIphoneXUpResolution()
                    ? 88 : 64, View.Frame.Width, View.Frame.Height - (114));
                SetNavigationBar();
            }
        }

        public void LanguageDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> FEEDBACK LanguageDidChange");
            _titleBarComponent?.SetTitle("Feedback_Title".Translate());
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            _email = string.Empty;
            if (!DataManager.DataManager.SharedInstance.IsPreloginFeedback)
            {
                if (DataManager.DataManager.SharedInstance.UserEntity != null
                    && DataManager.DataManager.SharedInstance.UserEntity.Count > 0)
                {
                    _email = DataManager.DataManager.SharedInstance.UserEntity[0]?.email;
                }
            }

            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        GetSubmittedFeedbackList().ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                if (_submittedFeedback == null || _submittedFeedback?.d == null
                                   || _submittedFeedback?.d?.data == null)
                                {
                                    _submittedFeedback = new SubmittedFeedbackResponseModel
                                    {
                                        d = new SubmittedFeedbackModel()
                                    };
                                    _submittedFeedback.d.data = new List<SubmittedFeedbackDataModel>();
                                }
                                var viewHeaderHeight = (View.Frame.Width * 126) / 320;
                                UIView viewHeader = new UIView(new CGRect(0, 0, View.Frame.Width, viewHeaderHeight));
                                UIImageView imgViewBackgroundPhoto = new UIImageView(viewHeader.Frame)
                                {
                                    Image = UIImage.FromBundle("Feedback-Header")
                                };
                                viewHeader.AddSubview(imgViewBackgroundPhoto);
                                feedbackTableView.TableHeaderView = viewHeader;

                                feedbackTableView.BackgroundColor = MyTNBColor.LightGrayBG;
                                feedbackTableView.RowHeight = 80f;
                                feedbackTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
                                feedbackTableView.Source = new FeedbackDataSource(this, _submittedFeedback?.d?.data
                                    , isFromPreLogin, DataManager.DataManager.SharedInstance.IsBcrmAvailable);
                                feedbackTableView.ReloadData();
                                feedbackTableView.TableFooterView = new UIView();
                                feedbackTableView.ScrollEnabled = feedbackTableView.ContentSize.Height > feedbackTableView.Frame.Height;
                                ActivityIndicator.Hide();
                            });
                        });
                    }
                    else
                    {
                        AlertHandler.DisplayNoDataAlert(this);
                    }
                });
            });
        }

        internal void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                this.DismissViewController(true, null);
            });
            this.NavigationItem.LeftBarButtonItem = btnBack;
        }

        internal void SetNavigationBar()
        {
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true); ;
            UIView headerView = gradientViewComponent.GetUI();
            _titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = _titleBarComponent.GetUI();
            _titleBarComponent.SetTitle("Feedback_Title".Translate());
            _titleBarComponent.SetNotificationVisibility(true);
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        internal void DisplaySubmittedFeedback()
        {
            if (_submittedFeedback != null && _submittedFeedback?.d != null
                && _submittedFeedback?.d?.data != null && _submittedFeedback?.d?.didSucceed == true)
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("Feedback", null);
                SubmittedFeedbackViewController submittedFeedbackVC =
                    storyBoard.InstantiateViewController("SubmittedFeedbackViewController") as SubmittedFeedbackViewController;
                if (submittedFeedbackVC != null)
                {
                    submittedFeedbackVC.SubmittedFeedback = _submittedFeedback;
                    var navController = new UINavigationController(submittedFeedbackVC);
                    PresentViewController(navController, true, null);
                }
            }
        }

        Task GetSubmittedFeedbackList()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    email = _email,
                    deviceId = DataManager.DataManager.SharedInstance.UDID
                };
                _submittedFeedback = serviceManager.GetSubmittedFeedbackList("GetSubmittedFeedbackList", requestParameter);
            });
        }
    }
}