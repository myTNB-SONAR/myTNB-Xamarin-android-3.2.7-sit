using System;
using UIKit;
using myTNB.Home.Feedback;
using System.Threading.Tasks;
using myTNB.Model;
using CoreGraphics;
using myTNB.Feedback;

namespace myTNB
{
    public partial class SubmittedFeedbackViewController : CustomUIViewController
    {
        public SubmittedFeedbackViewController(IntPtr handle) : base(handle) { }

        public SubmittedFeedbackResponseModel SubmittedFeedback = new SubmittedFeedbackResponseModel();
        private SubmittedFeedbackDetailsResponseModel _feedbackDetails = new SubmittedFeedbackDetailsResponseModel();

        private UIImageView _imgNoFeedback;
        private UILabel _lblNoFeedback;

        public override void ViewDidLoad()
        {
            PageName = EnquiryConstants.Pagename_Enquiry;

            base.ViewDidLoad();
            AddBackButton();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (SubmittedFeedback.d.data.Count > 0)
            {
                SetTable();
                if (_imgNoFeedback != null && _lblNoFeedback != null)
                {
                    _imgNoFeedback.Hidden = true;
                    _lblNoFeedback.Hidden = true;
                }
                SubmittedFeedbackTableView.Hidden = false;
                SubmittedFeedbackTableView.ClearsContextBeforeDrawing = true;
            }
            else
            {
                if (_imgNoFeedback == null || _lblNoFeedback == null)
                {
                    _imgNoFeedback = new UIImageView(new CGRect((View.Frame.Width / 2) - 75, 185, 150, 150))
                    {
                        Image = UIImage.FromBundle("Feedback-Empty")
                    };
                    _lblNoFeedback = new UILabel(new CGRect(44, 352, View.Frame.Width - 88, 32))
                    {
                        TextAlignment = UITextAlignment.Center,
                        Lines = 2,
                        Text = GetI18NValue(FeedbackConstants.I18N_NoFeedback),
                        Font = MyTNBFont.MuseoSans12_300,
                        TextColor = MyTNBColor.SilverChalice
                    };
                    View.AddSubviews(new UIView[] { _imgNoFeedback, _lblNoFeedback });
                }
                SubmittedFeedbackTableView.Hidden = true;
                _imgNoFeedback.Hidden = false;
                _lblNoFeedback.Hidden = false;
            }
        }

        private void SetTable()
        {
            SubmittedFeedbackTableView.Source = new SubmittedFeedbackDataSource(this, SubmittedFeedback.d.data);
            SubmittedFeedbackTableView.ReloadData();
            SubmittedFeedbackTableView.TableFooterView = new UIView();
        }

        private void AddBackButton()
        {
            UIImage backImg = UIImage.FromBundle(Constants.IMG_Back);
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;

            Title = GetI18NValue(EnquiryConstants.viewSubmittedEnquiry);
        }

        //Call from on row select
        internal void ExecuteGetSubmittedFeedbackDetailsCall(SubmittedFeedbackDataModel feedback)
        {
            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(() =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        ActivityIndicator.Show();
                        GetSubmittedFeedbackDetails(feedback.ServiceReqNo).ContinueWith(task =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                if (!DataManager.DataManager.SharedInstance.IsLoggedIn())
                                {
                                    if (_feedbackDetails != null && _feedbackDetails.d != null
                                       && _feedbackDetails.d.data != null && _feedbackDetails.d.IsSuccess && _feedbackDetails.d.data.RelationshipWithCA == null)
                                    {
                                        _feedbackDetails.d.data.FeedbackCategoryId = feedback.FeedbackCategoryId;
                                        _feedbackDetails.d.data.FeedbackMessage = feedback.FeedbackMessage;
                                        _feedbackDetails.d.data.FeedbackCategoryName = feedback.FeedbackCategoryName;
                                        UIStoryboard storyBoard = UIStoryboard.FromName("FeedbackDetails", null);
                                        FeedbackDetailsViewController viewController =
                                            storyBoard.InstantiateViewController("FeedbackDetailsViewController")
                                                      as FeedbackDetailsViewController;
                                        viewController.FeedbackDetails = _feedbackDetails.d.data;
                                        viewController.Title = feedback.FeedbackNameInListView;
                                        UINavigationController navController = new UINavigationController(viewController);
                                        navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                                        PresentViewController(navController, true, null);
                                    }
                                    else
                                    {
                                        DisplayServiceError(_feedbackDetails.d.DisplayMessage);
                                    }
                                }
                                else
                                {
                                    if (_feedbackDetails != null && _feedbackDetails.d != null
                                     && _feedbackDetails.d.data != null && _feedbackDetails.d.IsSuccess && _feedbackDetails.d.data.RelationshipWithCA != null)
                                    {
                                        _feedbackDetails.d.data.FeedbackCategoryId = feedback.FeedbackCategoryId;
                                        _feedbackDetails.d.data.FeedbackMessage = feedback.FeedbackMessage;
                                        _feedbackDetails.d.data.FeedbackCategoryName = feedback.FeedbackCategoryName;
                                        UIStoryboard storyBoard = UIStoryboard.FromName("Enquiry", null);
                                        EnquiryDetailsViewController enquiryDetailsViewController =
                                         storyBoard.InstantiateViewController("EnquiryDetailsViewController")
                                         as EnquiryDetailsViewController;

                                        enquiryDetailsViewController._feedbackDetails = _feedbackDetails.d.data;
                                        enquiryDetailsViewController.Title = feedback.FeedbackNameInListView;

                                        UINavigationController navController = new UINavigationController(enquiryDetailsViewController);
                                        navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                                        PresentViewController(navController, true, null);
                                    }
                                    else if (_feedbackDetails != null && _feedbackDetails.d != null
                                    && _feedbackDetails.d.data != null && _feedbackDetails.d.IsSuccess && _feedbackDetails.d.data.RelationshipWithCA == null)
                                    {
                                        _feedbackDetails.d.data.FeedbackCategoryId = feedback.FeedbackCategoryId;
                                        _feedbackDetails.d.data.FeedbackMessage = feedback.FeedbackMessage;
                                        _feedbackDetails.d.data.FeedbackCategoryName = feedback.FeedbackCategoryName;
                                        UIStoryboard storyBoard = UIStoryboard.FromName("FeedbackDetails", null);
                                        FeedbackDetailsViewController viewController =
                                         storyBoard.InstantiateViewController("FeedbackDetailsViewController")
                                         as FeedbackDetailsViewController;

                                        viewController.FeedbackDetails = _feedbackDetails.d.data;
                                        viewController.Title = feedback.FeedbackNameInListView;

                                        UINavigationController navController = new UINavigationController(viewController);
                                        navController.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                                        PresentViewController(navController, true, null);
                                    }
                                    else
                                    {
                                        DisplayServiceError(_feedbackDetails.d.DisplayMessage);
                                    }
                                }
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

        private Task GetSubmittedFeedbackDetails(string serviceReq)
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    serviceManager.usrInf,
                    serviceReqNo = serviceReq
                };
                //_feedbackDetails = serviceManager.OnExecuteAPIV6<SubmittedFeedbackDetailsResponseModel>(FeedbackConstants.Service_GetSubmittedFeedbackDetails, requestParameter);
                _feedbackDetails = serviceManager.OnExecuteAPIV6<SubmittedFeedbackDetailsResponseModel>("GetSubmittedFeedbackWithContactDetails", requestParameter);

            });
        }
    }
}