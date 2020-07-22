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
using myTNB.Feedback;

namespace myTNB
{
    public partial class FeedbackViewController : CustomUIViewController
    {
        public FeedbackViewController(IntPtr handle) : base(handle) { }

        public bool isFromPreLogin;
        private TitleBarComponent _titleBarComponent;
        private SubmittedFeedbackResponseModel _submittedFeedback = new SubmittedFeedbackResponseModel();
        private string _email = string.Empty;

        //cell1
        public UIView _viewTitleSection;
        public UIView Frame;
        public UILabel lblTitle;         public UILabel lblSubtTitle;         public UIView viewLine;         public UILabel lblCount;         public UIImageView imgViewIcon;

        public override void ViewDidLoad()
        {
            //PageName = FeedbackConstants.Pagename_FeedbackList;
            PageName = "SubmitEnquiry";

            base.ViewDidLoad();
            if (isFromPreLogin == true)
            {
                feedbackTableView.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height - (114 - 64));
                AddBackButton();
                //Title = GetI18NValue(FeedbackConstants.I18N_Title);
                Title = GetI18NValue("enquireTitle");

            }
            else
            {
                feedbackTableView.Frame = new CGRect(0, DeviceHelper.IsIphoneXUpResolution()
                    ? 88 : 64, View.Frame.Width, View.Frame.Height - (114));
                SetNavigationBar();
            }
        }

        protected override void LanguageDidChange(NSNotification notification)
        {
            Debug.WriteLine("DEBUG >>> FEEDBACK LanguageDidChange");
            base.LanguageDidChange(notification);
            //_titleBarComponent?.SetTitle(GetI18NValue(FeedbackConstants.I18N_Title));
            _titleBarComponent?.SetTitle(GetI18NValue("enquireTitle"));

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
                                nfloat viewHeaderHeight = (View.Frame.Width * 126) / 320;
                                UIView viewHeader = new UIView(new CGRect(0, 0, View.Frame.Width, viewHeaderHeight));
                                UIImageView imgViewBackgroundPhoto = new UIImageView(viewHeader.Frame)
                                {
                                    Image = UIImage.FromBundle("Feedback-Header")
                                };
                                viewHeader.AddSubview(imgViewBackgroundPhoto);
                                //feedbackTableView.TableHeaderView = viewHeader;

                                feedbackTableView.BackgroundColor = MyTNBColor.LightGrayBG;
                                feedbackTableView.RowHeight = 80f;
                                feedbackTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
                                feedbackTableView.Source = new FeedbackDataSource(this, _submittedFeedback?.d?.data
                                    , DataManager.DataManager.SharedInstance.IsBcrmAvailable);
                                feedbackTableView.ReloadData();
                                feedbackTableView.TableFooterView = new UIView();
                                feedbackTableView.ScrollEnabled = feedbackTableView.ContentSize.Height > feedbackTableView.Frame.Height;
                                ActivityIndicator.Hide();
                            });
                        });
                    }
                    else
                    {
                        DisplayNoDataAlert();
                    }
                });
            });
        }

        private void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
        }

        private void SetNavigationBar()
        {
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            UIView headerView = gradientViewComponent.GetUI();
            _titleBarComponent = new TitleBarComponent(headerView);
            UIView titleBarView = _titleBarComponent.GetUI();
            //_titleBarComponent.SetTitle(GetI18NValue(FeedbackConstants.I18N_Title));
            _titleBarComponent.SetTitle(GetI18NValue("enquiryTitle"));
            _titleBarComponent.SetPrimaryVisibility(true);
            headerView.AddSubview(titleBarView);
            View.AddSubview(headerView);
        }

        internal void DisplaySubmittedFeedback(string title)
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
                    submittedFeedbackVC.Title = title;
                    UINavigationController navController = new UINavigationController(submittedFeedbackVC);
                    navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                    PresentViewController(navController, true, null);
                }
            }
            else
            {
                DisplayServiceError(_submittedFeedback?.d?.DisplayMessage ?? string.Empty);
            }
        }

        internal void DisplayFeedbackEntry(string id)
        {
            if (!DataManager.DataManager.SharedInstance.IsLoggedIn()) //Check user from prelogin or login
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("Feedback", null);
                FeedbackEntryViewController feedbackEntryViewController =
                 storyBoard.InstantiateViewController("FeedbackEntryViewController") as FeedbackEntryViewController;
                feedbackEntryViewController.FeedbackID = id;
                feedbackEntryViewController.IsLoggedIn = DataManager.DataManager.SharedInstance.IsLoggedIn();//!isFromPreLogin;
                UINavigationController navController = new UINavigationController(feedbackEntryViewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
            }
            else
            {
                UIStoryboard storyBoard = UIStoryboard.FromName("Feedback", null);
                EnquiryViewController enquiryViewController =
                 storyBoard.InstantiateViewController("EnquiryViewController") as EnquiryViewController;
                enquiryViewController.FeedbackID = id;
                enquiryViewController.IsLoggedIn = DataManager.DataManager.SharedInstance.IsLoggedIn();//!isFromPreLogin;
                UINavigationController navController = new UINavigationController(enquiryViewController);
                navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                PresentViewController(navController, true, null);
            }
        }

        private Task GetSubmittedFeedbackList()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    serviceManager.usrInf,
                    serviceManager.deviceInf
                };
                _submittedFeedback = serviceManager.OnExecuteAPIV6<SubmittedFeedbackResponseModel>(FeedbackConstants.Service_GetSubmittedFeedbackList, requestParameter);
            });
        }
    }
}