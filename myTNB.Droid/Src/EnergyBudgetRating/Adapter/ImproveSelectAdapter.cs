using Android.Content;


using Android.Text.Method;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
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
    public class ImproveSelectAdapter : RecyclerView.Adapter
    {

        private Context mContext;
        private List<ImproveSelectModel> questions = new List<ImproveSelectModel>();
        public event EventHandler<int> SelectUpdate;
        public int QuestionCount = 1;
        public int SelectedRating = 1;

        //private List<InputAnswerDetails> inputAnswers = new List<InputAnswerDetails>();

        public override int ItemCount => questions.Count();

        public ImproveSelectAdapter(Context ctx, List<ImproveSelectModel> items)
        {
            this.mContext = ctx;
            this.questions = items;
            //this.SelectedRating = selectedRating;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ImproveSelectViewHolder vh = holder as ImproveSelectViewHolder;
                ImproveSelectModel question = questions[position];
                if (question != null)
                {
                    if (question.ModelCategories.Equals("feedback_two"))
                    {
                        if (question.IconCategories.Equals("1"))
                        {
                            vh.txtTitleInfo.Text = Utility.GetLocalizedLabel("FeedBackEB", "txtSimplicity");
                            if (!question.IsSelected)
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.simplify_icon);
                            }
                            else
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.active_simplify_icon);
                            }
                        }
                        else if (question.IconCategories.Equals("2"))
                        {
                            vh.txtTitleInfo.Text = Utility.GetLocalizedLabel("FeedBackEB", "txtContent");
                            if (!question.IsSelected)
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.content_relevant_icon);
                            }
                            else
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.active_content_relevant_icon);
                            }
                        }
                        else if (question.IconCategories.Equals("3"))
                        {
                            vh.txtTitleInfo.Text = Utility.GetLocalizedLabel("FeedBackEB", "txtNavigation");
                            if (!question.IsSelected)
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.navigation_icon);
                            }
                            else
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.active_navigation_icon);
                            }
                        }
                        else
                        {
                            vh.txtTitleInfo.Text = Utility.GetLocalizedLabel("FeedBackEB", "txtDesign");
                            if (!question.IsSelected)
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.design_icon);
                            }
                            else
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.active_design_icon);
                            }
                        }
                    }
                    else
                    {
                        if (question.IconCategories.Equals("1"))
                        {
                            vh.txtTitleInfo.Text = Utility.GetLocalizedLabel("FeedBackEBNotification", "saveOnEBusage");
                            if (!question.IsSelected)
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.saved_EB_usage_ratingEB_icon);
                            }
                            else
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.active_saved_EB_usage_ratingEB_icon);
                            }
                        }
                        else if (question.IconCategories.Equals("2"))
                        {
                            vh.txtTitleInfo.Text = Utility.GetLocalizedLabel("FeedBackEBNotification", "EBcannotTrusted");
                            if (!question.IsSelected)
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.cannot_function_ratingEB_icon);
                            }
                            else
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.active_cannot_function_ratingEB_icon);
                            }
                        }
                        else if (question.IconCategories.Equals("3"))
                        {
                            vh.txtTitleInfo.Text = Utility.GetLocalizedLabel("FeedBackEBNotification", "NotUsingEBagain");
                            if (!question.IsSelected)
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.will_use_again_ratingEB_icon);
                            }
                            else
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.active_will_use_again_ratingEB_icon);
                            }
                        }
                        else
                        {
                            vh.txtTitleInfo.Text = Utility.GetLocalizedLabel("FeedBackEBNotification", "EBtipNotHelpful");
                            if (!question.IsSelected)
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.saving_tip_ratingEB_icon);
                            }
                            else
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.active_saving_tip_ratingEB_icon);
                            }
                        }
                    }
                    vh.ImageIcon.Click += (o, e) =>
                    {
                        //vh.ratingBar.Rating = e.Rating;
                        //int Rating = ((int)e.Rating);
                        if (question.IsSelected == false)
                        {
                            question.IsSelected = true;
                        }
                        else if (question.IsSelected == true)
                        {
                            question.IsSelected = false;
                        }
                        if (question.ModelCategories.Equals("feedback_two"))
                        {
                            if (question.IconCategories.Equals("1"))
                            {
                                if (!question.IsSelected)
                                {
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.simplify_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                                }
                                else
                                {
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.active_simplify_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.powerBlue));
                                }
                            }
                            else if (question.IconCategories.Equals("2"))
                            {
                                if (!question.IsSelected)
                                {
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.content_relevant_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                                }
                                else
                                {
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.active_content_relevant_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.powerBlue));
                                }
                            }
                            else if (question.IconCategories.Equals("3"))
                            {
                                if (!question.IsSelected)
                                {
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.navigation_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                                }
                                else
                                {
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.active_navigation_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.powerBlue));
                                }
                            }
                            else
                            {
                                if (!question.IsSelected)
                                {
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.design_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                                }
                                else
                                {
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.active_design_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.powerBlue));
                                }
                            }
                        }
                        else
                        {
                            if (question.IconCategories.Equals("1"))
                            {
                                if (!question.IsSelected)
                                {
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.saved_EB_usage_ratingEB_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                                }
                                else
                                {
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.active_saved_EB_usage_ratingEB_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.powerBlue));
                                }
                            }
                            else if (question.IconCategories.Equals("2"))
                            {
                                if (!question.IsSelected)
                                {
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.cannot_function_ratingEB_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                                }
                                else
                                {
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.active_cannot_function_ratingEB_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.powerBlue));
                                }
                            }
                            else if (question.IconCategories.Equals("3"))
                            {
                                if (!question.IsSelected)
                                {
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.will_use_again_ratingEB_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                                }
                                else
                                {
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.active_will_use_again_ratingEB_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.powerBlue));
                                }
                            }
                            else
                            {
                                if (!question.IsSelected)
                                {
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.saving_tip_ratingEB_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                                }
                                else
                                {
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.active_saving_tip_ratingEB_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.powerBlue));
                                }
                            }
                        }
                        //question.InputRating = Rating.ToString();
                        OnSelectUpdate(vh, position);
                    };
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        void OnSelectUpdate(ImproveSelectViewHolder sender, int position)
        {
            SelectUpdate(sender, position);
        }

        public bool IsAllQuestionAnswered()
        {
            bool flag = false;
            try
            {
                foreach (ImproveSelectModel item in questions)
                {
                    if (item.IsSelected)
                    {
                        flag = true;
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

        /*public List<InputAnswerDetails> GetInputAnswers()
        {
            List<InputAnswerDetails> inputAnswers = new List<InputAnswerDetails>();
            try
            {
                if (IsAllQuestionAnswered())
                {
                    foreach (improveSelectModels ques in questions)
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
            var id = Resource.Layout.ImproveSelected_EnergyBudget;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new ImproveSelectViewHolder(itemView, OnSelectUpdate);
        }

        public class ImproveSelectViewHolder : RecyclerView.ViewHolder //EditText.IOnTouchListener
        {

            public TextView txtTitleInfo;
            public ImageView ImageIcon;

            public ImproveSelectViewHolder(View itemView, Action<ImproveSelectViewHolder, int> listener) : base(itemView)
            {
                txtTitleInfo = itemView.FindViewById<TextView>(Resource.Id.bodyInfo);
                ImageIcon = itemView.FindViewById<ImageView>(Resource.Id.img_display_select);

                ImageIcon.Click += (s, e) => listener((this), base.LayoutPosition);

                TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo);
                TextViewUtils.SetTextSize10(txtTitleInfo);

            }
        }
    }
}