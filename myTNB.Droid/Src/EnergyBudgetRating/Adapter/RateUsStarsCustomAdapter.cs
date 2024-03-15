using Android.Content;


using Android.Text.Method;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.TextField;
using myTNB.AndroidApp.Src.Base;
using myTNB.AndroidApp.Src.EnergyBudgetRating.Model;
using myTNB.AndroidApp.Src.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using static myTNB.AndroidApp.Src.Rating.Request.SubmitRateUsRequest;

namespace myTNB.AndroidApp.Src.EnergyBudgetRating.Adapter
{
    public class RateUsStarsCustomAdapter : RecyclerView.Adapter
    {

        private Context mContext;
        private List<ImproveSelectModel> questions = new List<ImproveSelectModel>();
        public event EventHandler<int> RatingUpdate;
        public int QuestionCount = 1;
        public int SelectedRating = 1;

        //private List<InputAnswerDetails> inputAnswers = new List<InputAnswerDetails>();

        public override int ItemCount => questions.Count();

        public RateUsStarsCustomAdapter(Context ctx, List<ImproveSelectModel> items, int selectedRating)
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
                ImproveSelectModel question = questions[position];
                if (question != null)
                {
                    if (question.IconCategories.Equals("1"))
                    {
                        if (!question.IsSelected)
                        {
                            vh.ImageStar.SetImageResource(Resource.Drawable.star_outline_grey_icon);
                        }
                        else
                        {
                            vh.ImageStar.SetImageResource(Resource.Drawable.star_outline_yellow_icon);
                        }
                    }
                    else if (question.IconCategories.Equals("2"))
                    {
                        if (!question.IsSelected)
                        {
                            vh.ImageStar.SetImageResource(Resource.Drawable.star_outline_grey_icon);
                        }
                        else
                        {
                            vh.ImageStar.SetImageResource(Resource.Drawable.star_outline_yellow_icon);
                        }
                    }
                    else if (question.IconCategories.Equals("3"))
                    {
                        if (!question.IsSelected)
                        {
                            vh.ImageStar.SetImageResource(Resource.Drawable.star_outline_grey_icon);
                        }
                        else
                        {
                            vh.ImageStar.SetImageResource(Resource.Drawable.star_outline_yellow_icon);
                        }
                    }
                    else if (question.IconCategories.Equals("4"))
                    {
                        if (!question.IsSelected)
                        {
                            vh.ImageStar.SetImageResource(Resource.Drawable.star_outline_grey_icon);
                        }
                        else
                        {
                            vh.ImageStar.SetImageResource(Resource.Drawable.star_outline_yellow_icon);
                        }
                    }
                    else if(question.IconCategories.Equals("5"))
                    {
                        if (!question.IsSelected)
                        {
                            vh.ImageStar.SetImageResource(Resource.Drawable.star_outline_grey_icon);
                        }
                        else
                        {
                            vh.ImageStar.SetImageResource(Resource.Drawable.star_outline_yellow_icon);
                        }
                    }

                    vh.ImageStar.Click += (o, e) =>
                    {
                        OnRatingUpdate(vh, position);
                    };
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

        /*public bool IsAllQuestionAnswered()
        {
            bool flag = true;
            try
            {
                foreach (RateUsStar item in questions)
                {
                    if (item.InputRating.Equals("0"))
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
        }*/

        /*public List<InputAnswerDetails> GetInputAnswers()
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
        }*/


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.CustomStarRatingVIew;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new RateUsStarsViewHolder(itemView, OnRatingUpdate);
        }

        public class RateUsStarsViewHolder : RecyclerView.ViewHolder //EditText.IOnTouchListener
        {

            public ImageView ImageStar;

            public RateUsStarsViewHolder(View itemView, Action<RateUsStarsViewHolder, int> listener) : base(itemView)
            {
                ImageStar = itemView.FindViewById<ImageView>(Resource.Id.ratingBar_custom);

                //ImageStar.Click += (s, e) => listener((this), base.LayoutPosition);
            }            
        }
    }
}