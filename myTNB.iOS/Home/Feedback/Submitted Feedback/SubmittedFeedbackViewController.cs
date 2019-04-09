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
        SubmittedFeedbackDetailsResponseModel _feedbackDetails = new SubmittedFeedbackDetailsResponseModel();

        UIImageView _imgNoFeedback;
        UILabel _lblNoFeedback;
        string _email = string.Empty;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            AddBackButton();
            _email = DataManager.DataManager.SharedInstance.IsPreloginFeedback
                                ? string.Empty
                                : DataManager.DataManager.SharedInstance.UserEntity[0].email;
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
                    _imgNoFeedback = new UIImageView(new CGRect((View.Frame.Width / 2) - 75, 185, 150, 150));
                    _imgNoFeedback.Image = UIImage.FromBundle("Feedback-Empty");
                    _lblNoFeedback = new UILabel(new CGRect(44, 352, View.Frame.Width - 88, 32));
                    _lblNoFeedback.TextAlignment = UITextAlignment.Center;
                    _lblNoFeedback.Lines = 2;
                    _lblNoFeedback.Text = "You have not submittted\r\nany feedback.";
                    _lblNoFeedback.Font = myTNBFont.MuseoSans12_300();
                    _lblNoFeedback.TextColor = myTNBColor.SilverChalice();
                    View.AddSubviews(new UIView[] { _imgNoFeedback, _lblNoFeedback });
                }
                SubmittedFeedbackTableView.Hidden = true;
                _imgNoFeedback.Hidden = false;
                _lblNoFeedback.Hidden = false;
            }
        }

        internal void SetTable()
        {
            SubmittedFeedbackTableView.Source = new SubmittedFeedbackDataSource(this, SubmittedFeedback.d.data);
            SubmittedFeedbackTableView.ReloadData();
            SubmittedFeedbackTableView.TableFooterView = new UIView();
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
                                    var navController = new UINavigationController(viewController);
                                    PresentViewController(navController, true, null);
                                }
                                else
                                {
                                    ToastHelper.DisplayAlertView(this, "ErrorTitle".Translate(), _feedbackDetails?.d?.message);
                                }
                                ActivityIndicator.Hide();
                            });
                        });
                    }
                    else
                    {
                        Console.WriteLine("No Network");
                        var alert = UIAlertController.Create("ErrNoNetworkTitle".Translate(), "ErrNoNetworkMsg".Translate(), UIAlertControllerStyle.Alert);
                        alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                        PresentViewController(alert, animated: true, completionHandler: null);
                    }
                });
            });
        }

        Task GetSubmittedFeedbackDetails(string serviceReq)
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    serviceReqNo = serviceReq
                };
                _feedbackDetails = serviceManager.GetSubmittedFeedbackDetails("GetSubmittedFeedbackDetails", requestParameter);
            });
        }
    }
}