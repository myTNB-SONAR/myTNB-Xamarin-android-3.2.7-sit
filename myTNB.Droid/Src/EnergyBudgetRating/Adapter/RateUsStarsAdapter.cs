using Android.Content;


using Android.Text.Method;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.TextField;
using myTNB_Android.Src.EnergyBudgetRating.Model;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using static myTNB_Android.Src.Rating.Request.SubmitRateUsRequest;

namespace myTNB_Android.Src.EnergyBudgetRating.Adapter
{
    public class RateUsStarsAdapter : RecyclerView.Adapter
    {

        private Context mContext;
        private List<RateUsStar> questions = new List<RateUsStar>();
        public event EventHandler<int> RatingUpdate;
        public int QuestionCount = 1;
        public int SelectedRating = 1;

        //private List<InputAnswerDetails> inputAnswers = new List<InputAnswerDetails>();

        public override int ItemCount => questions.Count();

        public RateUsStarsAdapter(Context ctx, List<RateUsStar> items, int selectedRating)
        {
            this.mContext = ctx;
            this.questions = items;
            this.SelectedRating = selectedRating;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                RateUsStarsViewHolder vh = holder as RateUsStarsViewHolder;
                RateUsStar question = questions[position];
                if (question != null)
                {

                    //vh.txtTitleInfo.Text = QuestionCount + ". " + question.Question;
                    //QuestionCount += 1;
                    if (question.QuestionType.Equals(Constants.QUESTION_TYPE_RATING))
                    {
                        vh.ratingBar.Visibility = ViewStates.Visible;
                        vh.txtTitleInfo.Visibility = ViewStates.Visible;

                        if (position == 0)
                        {
                            if (SelectedRating != 0 && SelectedRating < 6)
                            {
                                vh.txtTitleInfo.Text = question.InputOptionValueList[SelectedRating - 1].InputOptionValues;
                                question.InputRating = SelectedRating.ToString();
                                question.IsQuestionAnswered = true;
                                vh.ratingBar.Rating = SelectedRating;
                            }
                            else
                            {
                                question.IsQuestionAnswered = false;
                                //vh.txtContentInfo.Text = "";
                            }
                        }
                        else
                        {
                            question.IsQuestionAnswered = false;
                            //vh.txtTitleInfo.Text = "";
                        }
                        //vh.ratingBar.Rating = SelectedRating;

                        vh.ratingBar.RatingBarChange += (o, e) =>
                        {
                            vh.ratingBar.Rating = e.Rating;
                            int Rating = ((int)e.Rating);
                            if (Rating == 0)
                            {
                                question.IsQuestionAnswered = false;
                                vh.txtTitleInfo.Text = "";
                            }
                            else if (Rating == 1)
                            {
                                question.IsQuestionAnswered = true;
                                //vh.txtTitleInfo.Text = question.InputOptionValueList[0].InputOptionValues;
                                vh.txtTitleInfo.Text = "I don't like this";
                            }
                            else if (Rating == 2)
                            {
                                question.IsQuestionAnswered = true;
                                //vh.txtTitleInfo.Text = question.InputOptionValueList[1].InputOptionValues;
                                vh.txtTitleInfo.Text = "I did not enjoy it";
                            }
                            else if (Rating == 3)
                            {
                                question.IsQuestionAnswered = true;
                                //vh.txtTitleInfo.Text = question.InputOptionValueList[2].InputOptionValues;
                                vh.txtTitleInfo.Text = "It was okay";
                            }
                            else if (Rating == 4)
                            {
                                question.IsQuestionAnswered = true;
                                //vh.txtTitleInfo.Text = question.InputOptionValueList[3].InputOptionValues;
                                vh.txtTitleInfo.Text = "It was good!";
                            }
                            else if (Rating == 5)
                            {
                                question.IsQuestionAnswered = true;
                                //vh.txtTitleInfo.Text = question.InputOptionValueList[4].InputOptionValues;
                                vh.txtTitleInfo.Text = "It was great!";
                            }
                            question.InputRating = Rating.ToString();
                            OnRatingUpdate(vh, position);
                        };
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        void OnRatingUpdate(RateUsStarsViewHolder sender, int position)
        {
            RatingUpdate(sender, position);
        }

        public bool IsAllQuestionAnswered()
        {
            bool flag = true;
            try
            {
                foreach (RateUsStar item in questions)
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
                    foreach (RateUsStar ques in questions)
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
            var id = Resource.Layout.RateUsQuestionItemView_EnergyBudget;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new RateUsStarsViewHolder(itemView, OnRatingUpdate);
        }

        public class RateUsStarsViewHolder : RecyclerView.ViewHolder //EditText.IOnTouchListener
        {

            public TextView txtTitleInfo;
            public ImageView ImageNumber;
            public RatingBar ratingBar;
            public int ratingFromServer = 1;

            public RateUsStarsViewHolder(View itemView, Action<RateUsStarsViewHolder, int> listener) : base(itemView)
            {
                txtTitleInfo = itemView.FindViewById<TextView>(Resource.Id.txtTitleInfo);
                ImageNumber = itemView.FindViewById<ImageView>(Resource.Id.img_display);
                ratingBar = itemView.FindViewById<RatingBar>(Resource.Id.ratingBar);

                ratingBar.RatingBarChange += (s, e) => listener((this), base.LayoutPosition);

                TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo);
                TextViewUtils.SetTextSize16(txtTitleInfo);

            }
            /*public bool OnTouch(View v, MotionEvent e)
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
            }*/
        }
    }
}