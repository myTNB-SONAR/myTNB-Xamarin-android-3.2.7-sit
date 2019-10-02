using System;
using UIKit;
using myTNB.Home.Feedback;
using System.Threading.Tasks;
using myTNB.Model;
using CoreGraphics;

namespace myTNB
{
    public partial class SubmittedFeedbackViewController : UIViewController
    {
        public SubmittedFeedbackViewController(IntPtr handle) : base(handle)
        {
        }

        public SubmittedFeedbackResponseModel SubmittedFeedback = new SubmittedFeedbackResponseModel();
        private SubmittedFeedbackDetailsResponseModel _feedbackDetails = new SubmittedFeedbackDetailsResponseModel();

        private UIImageView _imgNoFeedback;
        private UILabel _lblNoFeedback;

        public override void ViewDidLoad()
        {
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
                        Text = "Feedback_NoFeedbackSubmittedMessage".Translate(),
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
            UIImage backImg = UIImage.FromBundle("Back-White");
            UIBarButtonItem btnBack = new UIBarButtonItem(backImg, UIBarButtonItemStyle.Done, (sender, e) =>
            {
                DismissViewController(true, null);
            });
            NavigationItem.LeftBarButtonItem = btnBack;
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
                                if (_feedbackDetails != null && _feedbackDetails.d != null
                                   && _feedbackDetails.d.data != null && _feedbackDetails.d.isError.Equals("false"))
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
                                    AlertHandler.DisplayServiceError(this, _feedbackDetails?.d?.message);
                                }
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

        private Task GetSubmittedFeedbackDetails(string serviceReq)
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    serviceReqNo = serviceReq
                };
                _feedbackDetails = serviceManager.OnExecuteAPI<SubmittedFeedbackDetailsResponseModel>("GetSubmittedFeedbackDetails", requestParameter);
            });
        }
    }
}