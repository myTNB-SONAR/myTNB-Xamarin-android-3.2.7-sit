using Android.Content;


using Android.Text.Method;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.TextField;
using myTNB_Android.Src.Rating.Model;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using static myTNB_Android.Src.Rating.Request.SubmitRateUsRequest;

namespace myTNB_Android.Src.Rating.Adapter
{
    public class RateUsQuestionsAdapter : RecyclerView.Adapter
    {

        private Context mContext;
        private List<RateUsQuestion> questions = new List<RateUsQuestion>();
        public event EventHandler<int> RatingUpdate;
        public int QuestionCount = 1;
        public int SelectedRating = 1;

        //private List<InputAnswerDetails> inputAnswers = new List<InputAnswerDetails>();

        public override int ItemCount => questions.Count();

        public RateUsQuestionsAdapter(Context ctx, List<RateUsQuestion> items, int selectedRating)
        {
            this.mContext = ctx;
            this.questions = items;
            this.SelectedRating = selectedRating;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                RateUsQuestionViewHolder vh = holder as RateUsQuestionViewHolder;
                RateUsQuestion question = questions[position];
                if (question != null)
                {

                    vh.txtTitleInfo.Text = QuestionCount + ". " + question.Question;
                    QuestionCount += 1;
                    if (question.QuestionType.Equals(Constants.QUESTION_TYPE_RATING))
                    {
                        vh.ratingBar.Visibility = ViewStates.Visible;
                        vh.txtContentInfo.Visibility = ViewStates.Visible;
                        vh.txtInputLayoutComments.Visibility = ViewStates.Gone;
                        vh.txtComments.Visibility = ViewStates.Gone;

                        if (position == 0)
                        {
                            if (SelectedRating != 0 && SelectedRating < 6)
                            {
                                vh.txtContentInfo.Text = question.InputOptionValueList[SelectedRating - 1].InputOptionValues;
                                question.InputRating = SelectedRating.ToString();
                                question.IsQuestionAnswered = true;
                                vh.ratingBar.Rating = SelectedRating;
                            }
                            else
                            {
                                question.IsQuestionAnswered = false;
                                vh.txtContentInfo.Text = "";
                            }
                        }
                        else
                        {
                            question.IsQuestionAnswered = false;
                            vh.txtContentInfo.Text = "";
                        }
                        //vh.ratingBar.Rating = SelectedRating;

                        vh.ratingBar.RatingBarChange += (o, e) =>
                        {
                            vh.ratingBar.Rating = e.Rating;
                            int Rating = ((int)e.Rating);
                            if (Rating == 0)
                            {
                                question.IsQuestionAnswered = false;
                                vh.txtContentInfo.Text = "";
                            }
                            else if (Rating == 1)
                            {
                                question.IsQuestionAnswered = true;
                                vh.txtContentInfo.Text = question.InputOptionValueList[0].InputOptionValues;
                            }
                            else if (Rating == 2)
                            {
                                question.IsQuestionAnswered = true;
                                vh.txtContentInfo.Text = question.InputOptionValueList[1].InputOptionValues;
                            }
                            else if (Rating == 3)
                            {
                                question.IsQuestionAnswered = true;
                                vh.txtContentInfo.Text = question.InputOptionValueList[2].InputOptionValues;
                            }
                            else if (Rating == 4)
                            {
                                question.IsQuestionAnswered = true;
                                vh.txtContentInfo.Text = question.InputOptionValueList[3].InputOptionValues;
                            }
                            else if (Rating == 5)
                            {
                                question.IsQuestionAnswered = true;
                                vh.txtContentInfo.Text = question.InputOptionValueList[4].InputOptionValues;
                            }
                            question.InputRating = Rating.ToString();
                            OnRatingUpdate(vh, position);
                        };
                    }
                    else if (question.QuestionType.Equals(Constants.QUESTION_TYPE_COMMENTS))
                    {
                        vh.ratingBar.Visibility = ViewStates.Gone;
                        vh.txtContentInfo.Visibility = ViewStates.Gone;
                        vh.txtInputLayoutComments.Visibility = ViewStates.Visible;
                        vh.txtComments.Visibility = ViewStates.Visible;
                        vh.txtInputLayoutComments.Error = string.Format(Utility.GetLocalizedCommonLabel("charactersLeft"), "250");
                        vh.txtComments.TextChanged += delegate
                        {
                            string feedback = vh.txtComments.Text;

                            int char_count = feedback.Length;
                            if (char_count > 0)
                            {
                                int char_left = Constants.FEEDBACK_CHAR_LIMIT - char_count;
                                vh.txtInputLayoutComments.Error = string.Format(Utility.GetLocalizedCommonLabel("charactersLeft"), char_left);
                                questions[position].IsQuestionAnswered = true;
                                question.InputAnswer = feedback;
                            }
                            else
                            {
                                vh.txtInputLayoutComments.Error = string.Format(Utility.GetLocalizedCommonLabel("charactersLeft"), "250");
                                questions[position].IsQuestionAnswered = false;
                            }
                            OnRatingUpdate(vh, position);
                        };
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        void OnRatingUpdate(RateUsQuestionViewHolder sender, int position)
        {
            RatingUpdate(sender, position);
        }

        public bool IsAllQuestionAnswered()
        {
            bool flag = true;
            try
            {
                foreach (RateUsQuestion item in questions)
                {
                    if (!item.IsQuestionAnswered && item.IsMandatory)
                    {
                        flag = false;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return flag;
        }

        public List<InputAnswerDetails> GetInputAnswers()
        {
            List<InputAnswerDetails> inputAnswers = new List<InputAnswerDetails>();
            try
            {
                if (IsAllQuestionAnswered())
                {
                    foreach (RateUsQuestion ques in questions)
                    {
                        InputAnswerDetails item = new InputAnswerDetails();
                        item.WLTYQuestionId = ques.WLTYQuestionId;
                        item.RatingInput = string.IsNullOrEmpty(ques.InputRating) == true ? "" : ques.InputRating;
                        item.MultilineInput = string.IsNullOrEmpty(ques.InputAnswer) == true ? "" : ques.InputAnswer;
                        inputAnswers.Add(item);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return inputAnswers;
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.RateUsQuestionItemView;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new RateUsQuestionViewHolder(itemView, OnRatingUpdate);
        }

        public class RateUsQuestionViewHolder : RecyclerView.ViewHolder, EditText.IOnTouchListener
        {

            public TextView txtTitleInfo;
            public TextView txtContentInfo;
            public TextInputLayout txtInputLayoutComments;
            public EditText txtComments;
            public RatingBar ratingBar;
            public int ratingFromServer = 1;

            public RateUsQuestionViewHolder(View itemView, Action<RateUsQuestionViewHolder, int> listener) : base(itemView)
            {
                txtTitleInfo = itemView.FindViewById<TextView>(Resource.Id.txtTitleInfo);
                txtContentInfo = itemView.FindViewById<TextView>(Resource.Id.txtContentInfo);
                txtInputLayoutComments = itemView.FindViewById<TextInputLayout>(Resource.Id.txtInputLayoutComments);
                txtComments = itemView.FindViewById<EditText>(Resource.Id.txtComments);
                ratingBar = itemView.FindViewById<RatingBar>(Resource.Id.ratingBar);

                txtComments.TextChanged += (s, e) => listener((this), base.LayoutPosition);
                ratingBar.RatingBarChange += (s, e) => listener((this), base.LayoutPosition);

                TextViewUtils.SetMuseoSans300Typeface(txtInputLayoutComments);
                TextViewUtils.SetMuseoSans300Typeface(txtComments, txtContentInfo);
                TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo);

                txtComments.MovementMethod = new ScrollingMovementMethod();
                txtComments.SetOnTouchListener(this);
                txtInputLayoutComments.Hint = Utility.GetLocalizedCommonLabel("comments");
                txtComments.Hint = "";
                txtComments.AddTextChangedListener(new InputFilterFormField(txtComments, txtInputLayoutComments));
            }

            public bool OnTouch(View v, MotionEvent e)
            {
                if (v.Id == Resource.Id.txtComments)
                {
                    v.Parent.RequestDisallowInterceptTouchEvent(true);
                    switch (e.Action & MotionEventActions.Mask)
                    {
                        case MotionEventActions.Up:
                            v.Parent.RequestDisallowInterceptTouchEvent(false);
                            break;
                    }
                }
                return false;
            }
        }
    }
}