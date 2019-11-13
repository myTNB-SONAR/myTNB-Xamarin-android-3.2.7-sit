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
using myTNB.Rating;
using UIKit;

namespace myTNB
{
    public partial class RatingViewController : CustomUIViewController
    {
        private UIView _headerView;
        private List<FeedbackQuestionModel> displayedQuestions;

        public int Rating = 5;
        public string TransId;
        public RatingViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            PageName = RatingConstants.Pagename_Rating;
            base.ViewDidLoad();
            SetNavigationBar();
            SetSubViews();
            NotifCenterUtility.AddObserver(UIKeyboard.DidShowNotification, (NSNotification obj) =>
            {
                NSDictionary userInfo = obj.UserInfo;
                NSValue keyboardFrame = userInfo.ValueForKey(UIKeyboard.FrameEndUserInfoKey) as NSValue;
                CGRect keyboardRectangle = keyboardFrame.CGRectValue;
                tableViewRating.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height - (keyboardRectangle.Height));
            });
            NotifCenterUtility.AddObserver(UIKeyboard.DidHideNotification, (NSNotification obj) =>
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

        private void SetNavigationBar()
        {
            NavigationController.NavigationBar.Hidden = true;
            GradientViewComponent gradientViewComponent = new GradientViewComponent(View, true, 64, true);
            _headerView = gradientViewComponent.GetUI();
            TitleBarComponent titleBarComponent = new TitleBarComponent(_headerView);
            UIView titleBarView = titleBarComponent.GetUI();
            titleBarComponent.SetTitle(GetI18NValue(RatingConstants.I18N_Title));
            titleBarComponent.SetPrimaryVisibility(true);
            titleBarComponent.SetBackVisibility(false);
            titleBarComponent.SetBackAction(new UITapGestureRecognizer(() =>
            {
                DismissViewController(true, null);
            }));
            _headerView.AddSubview(titleBarView);
            View.AddSubview(_headerView);
        }

        private void SetSubViews()
        {
            SetDefaultTableFrame();
            tableViewRating.RowHeight = UITableView.AutomaticDimension;
            tableViewRating.EstimatedRowHeight = 140;
            tableViewRating.SeparatorStyle = UITableViewCellSeparatorStyle.None;
        }

        private void SetDefaultTableFrame()
        {
            nfloat svHeight = View.Frame.Height - _headerView.Frame.Height;
            // _contentHeight = svHeight;
            tableViewRating.Frame = new CGRect(0, _headerView.Frame.Height, View.Frame.Width, svHeight);
        }

        private void AddSubmitButton()
        {
            UIView viewFooter = new UIView(new CGRect(0, 0, tableViewRating.Frame.Width, 48 + 32));

            UIButton btnCTA = new UIButton(UIButtonType.Custom);
            btnCTA.Frame = new CGRect(18, 10, tableViewRating.Frame.Width - 36, DeviceHelper.GetScaledHeight(48));
            btnCTA.Layer.CornerRadius = 4;
            btnCTA.Layer.BorderColor = MyTNBColor.FreshGreen.CGColor;
            btnCTA.BackgroundColor = MyTNBColor.FreshGreen;
            btnCTA.Layer.BorderWidth = 1;
            btnCTA.SetTitle(GetCommonI18NValue(Constants.Common_Submit), UIControlState.Normal);
            btnCTA.Font = MyTNBFont.MuseoSans16_500;
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
                           DisplayNoDataAlert();
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
            int emptyCount = 0;
            if (questions != null)
            {
                List<FeedbackQuestionModel> emptyMandatory = questions.FindAll(item => item.Active && item.Mandatory && string.IsNullOrEmpty(item.Answer));
                emptyCount = emptyMandatory?.Count ?? 0;
            }
            return emptyCount == 0;
        }

        /// <summary>
        /// Loads the questions.
        /// </summary>
        /// <returns>The questions.</returns>
        private async Task LoadQuestions()
        {
            ActivityIndicator.Show();
            FeedbackQuestionResponseModel response = await ServiceCall.GetRateUsQuestions(QuestionCategoryEnum.Payment);

            if (response != null && response.d != null && response.d.IsSuccess && response.d.data != null)
            {
                displayedQuestions = response.d.data.FindAll(x => x.Active);
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
            RatingResultsViewController vc = Storyboard.InstantiateViewController("RatingResultsViewController") as RatingResultsViewController;
            if (vc != null)
            {
                NavigationController?.PushViewController(vc, true);
                return;
            }
        }

        /// <summary>
        /// Submits the ratings.
        /// </summary>
        /// <param name="questions">Questions.</param>
        private void SubmitRatings(List<FeedbackQuestionModel> questions)
        {
            List<InputAnswerDetailsModel> answerDetails = new List<InputAnswerDetailsModel>();

            foreach (FeedbackQuestionModel item in questions)
            {
                InputAnswerDetailsModel answer = new InputAnswerDetailsModel(item);
                answerDetails.Add(answer);
            }

            InputAnswerModel inputAnswer = new InputAnswerModel
            {
                ReferenceId = !string.IsNullOrEmpty(TransId) ? TransId : string.Empty,
                Email = DataManager.DataManager.SharedInstance?.UserEntity[0]?.email ?? string.Empty,
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