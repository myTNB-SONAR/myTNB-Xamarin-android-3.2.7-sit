using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using myTNB.Dashboard.DashboardComponents;
using myTNB.DataManager;
using myTNB.Enums;
using myTNB.Model;
using UIKit;

namespace myTNB
{
    public partial class RatingViewController : UIViewController
    {
        UIView _headerView;

        string _message = string.Empty;
        nfloat _contentHeight;
        List<FeedbackQuestionModel> displayedQuestions;

        public int Rating = 5;
        public string TransId;

        public RatingViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetNavigationBar();
            SetSubViews();

            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidShowNotification, (NSNotification obj) =>
            {
                var userInfo = obj.UserInfo;
                NSValue keyboardFrame = userInfo.ValueForKey(UIKeyboard.FrameEndUserInfoKey) as NSValue;
                CGRect keyboardRectangle = keyboardFrame.CGRectValue;
                tableViewRating.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height - (keyboardRectangle.Height));
            });
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.DidHideNotification, (NSNotification obj) =>
            {
                SetDefaultTableFrame();
            });
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
            {
                InvokeOnMainThread(async () =>
                {
                    if (NetworkUtility.isReachable)
                    {
                        await LoadQuestions();
                    }
                });
            });
        }

        void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            _headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(_headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle("Rating_Title".Translate());
            titleBarComponent.SetNotificationVisibility(true);
            titleBarComponent.SetBackVisibility(false);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                this.DismissViewController(true, null);
            }));
            _headerView.AddSubview(titleBarView);
            View.AddSubview(_headerView);
        }

        void SetSubViews()
        {
            SetDefaultTableFrame();
            //tableViewRating.RowHeight = 200; //140;
            tableViewRating.RowHeight = UITableView.AutomaticDimension;
            tableViewRating.EstimatedRowHeight = 140;
            tableViewRating.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            //tableViewRating.AllowsSelection = false;
        }

        private void SetDefaultTableFrame()
        {
            nfloat svHeight = View.Frame.Height - _headerView.Frame.Height;
            _contentHeight = svHeight;
            tableViewRating.Frame = new CGRect(0, _headerView.Frame.Height, View.Frame.Width, svHeight);
        }

        private void AddSubmitButton()
        {
            UIView viewFooter = new UIView(new CGRect(0, 0, tableViewRating.Frame.Width, 48 + 32));

            UIButton btnCTA = new UIButton(UIButtonType.Custom);
            btnCTA.Frame = new CGRect(18, 10, tableViewRating.Frame.Width - 36, DeviceHelper.GetScaledHeight(48));
            btnCTA.Layer.CornerRadius = 4;
            btnCTA.Layer.BorderColor = myTNBColor.FreshGreen().CGColor;
            btnCTA.BackgroundColor = myTNBColor.FreshGreen();
            btnCTA.Layer.BorderWidth = 1;
            btnCTA.SetTitle("Common_Submit".Translate(), UIControlState.Normal);
            btnCTA.Font = myTNBFont.MuseoSans16_500();
            btnCTA.SetTitleColor(UIColor.White, UIControlState.Normal);
            btnCTA.TouchUpInside += (sender, e) =>
            {
                if (!AreRepliesComplete(displayedQuestions))
                {
                    // TODO: RRA, disable submit button
                    Debug.WriteLine("Rate us mandatory fields incomplete");
                    return;
                }

                NetworkUtility.CheckConnectivity().ContinueWith(networkTask =>
               {
                   InvokeOnMainThread(() =>
                   {
                       if (NetworkUtility.isReachable)
                       {
                           SubmitRatings(displayedQuestions);
                       }
                       else
                       {
                           ErrorHandler.DisplayNoDataAlert(this);
                       }
                   });
               });
            };
            viewFooter.AddSubview(btnCTA);
            tableViewRating.TableFooterView = viewFooter;
        }

        /// <summary>
        /// Checks if the replies complete.
        /// </summary>
        /// <returns><c>true</c>, if replies complete was ared, <c>false</c> otherwise.</returns>
        /// <param name="questions">Questions.</param>
        private bool AreRepliesComplete(List<FeedbackQuestionModel> questions)
        {
#if DEBUG
            foreach (var item in questions)
            {
                Debug.WriteLine("question: {0} answer: {1}", item.Question, item.Answer);
            }
#endif
            var emptyMandatory = questions.FindAll(item => item.Active && item.Mandatory && string.IsNullOrEmpty(item.Answer));
            var emptyCount = emptyMandatory?.Count ?? 0;
            Debug.WriteLine("mandatory empty fields: " + emptyMandatory?.Count.ToString());
            return emptyCount == 0;
        }


        /// <summary>
        /// Loads the questions.
        /// </summary>
        /// <returns>The questions.</returns>
        private async Task LoadQuestions()
        {
            ActivityIndicator.Show();
            var response = await ServiceCall.GetRateUsQuestions(QuestionCategoryEnum.Payment);

            if (response.didSucceed)
            {
                displayedQuestions = response.FeedbackQuestions?.FindAll(x => x.Active);
                tableViewRating.Source = new RatingDataSource(displayedQuestions, Rating);
                tableViewRating.ReloadData();
                AddSubmitButton();
            }
            else
            {
                displayedQuestions = new List<FeedbackQuestionModel>();
            }

            ActivityIndicator.Hide();
        }


        /// <summary>
        /// Displaies the dashboard.
        /// </summary>
        private void OnRatingCompleted()
        {
            var vc = this.Storyboard.InstantiateViewController("RatingResultsViewController") as RatingResultsViewController;
            if (vc != null)
            {
                this.NavigationController?.PushViewController(vc, true);
                return;
            }
        }

        Task SubmitExperienceRating()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    email = DataManager.DataManager.SharedInstance.UserEntity[0].email,
                    rating = Rating.ToString(),
                    message = _message,
                    ratingFor = "Common_Pay".Translate().ToUpper()
                };
                BaseResponseModel response = serviceManager.BaseServiceCall("SubmitExperienceRating", requestParameter);
            });
        }

        /// <summary>
        /// Submits the ratings.
        /// </summary>
        /// <param name="questions">Questions.</param>
        private void SubmitRatings(List<FeedbackQuestionModel> questions)
        {
            var answerDetails = new List<InputAnswerDetailsModel>();

            foreach (var item in questions)
            {
                var answer = new InputAnswerDetailsModel(item);
                answerDetails.Add(answer);
            }

            var inputAnswer = new InputAnswerModel
            {
                ReferenceId = !string.IsNullOrEmpty(TransId) ? TransId : string.Empty,
                Email = DataManager.DataManager.SharedInstance.UserEntity[0].email,
                DeviceId = DataManager.DataManager.SharedInstance.UDID,
                InputAnswerDetails = answerDetails
            };

            ServiceCall.SubmitRateUs(inputAnswer).ContinueWith(task =>
            {
                InvokeOnMainThread(OnRatingCompleted);
            });
        }
    }
}