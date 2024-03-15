using Android.Content;


using Android.Text.Method;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.TextField;
using myTNB.AndroidApp.Src.ServiceDistruptionRating.Model;
using myTNB.AndroidApp.Src.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using static myTNB.AndroidApp.Src.Rating.Request.SubmitRateUsRequest;

namespace myTNB.AndroidApp.Src.ServiceDistruptionRating.Adapter
{
    public class ImproveSelectAdapter : RecyclerView.Adapter
    {

        private Context mContext;
        private List<QuestionModel> questions = new List<QuestionModel>();
        public event EventHandler<int> SelectUpdate;

        public override int ItemCount => questions.Count();

        public ImproveSelectAdapter(Context ctx, List<QuestionModel> items)
        {
            this.mContext = ctx;
            this.questions = items;
            //this.SelectedRating = selectedRating;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                QuestionModelViewHolder vh = holder as QuestionModelViewHolder;
                QuestionModel question = questions[position];
                if (question != null)
                {
                    if (question.ModelCategories.Equals("FeedbackYes"))
                    {
                        if (question.IconPosition.Equals(1))
                        {
                            vh.txtTitleInfo.Text = question.IconCategories;
                            if (!question.IsSelected)
                            {
                                vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.silver_chalice_button_outline);
                                vh.ImageIcon.SetImageResource(Resource.Drawable.hourglass_icon);
                                vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                            }
                            else
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.active_hourglass_icon);
                            }
                        }
                        else if (question.IconPosition.Equals(2))
                        {
                            vh.txtTitleInfo.Text = question.IconCategories;
                            if (!question.IsSelected)
                            {
                                vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.silver_chalice_button_outline);
                                vh.ImageIcon.SetImageResource(Resource.Drawable.information_icon);
                                vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                            }
                            else
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.active_information_icon);
                            }
                        }
                        else if (question.IconPosition.Equals(3))
                        {
                            vh.txtTitleInfo.Text = question.IconCategories;
                            if (!question.IsSelected)
                            {
                                vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.silver_chalice_button_outline);
                                vh.ImageIcon.SetImageResource(Resource.Drawable.hand_icon);
                                vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                            }
                            else
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.active_hand_icon);
                            }
                        }
                        else
                        {
                            vh.txtTitleInfo.Text = question.IconCategories;
                            if (!question.IsSelected)
                            {
                                vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.silver_chalice_button_outline);
                                vh.ImageIcon.SetImageResource(Resource.Drawable.phonecall_icon);
                                vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                            }
                            else
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.active_phonecall_icon);
                            }
                        }
                    }
                    else if (question.ModelCategories.Equals("FeedbackNo"))
                    {
                        if (question.IconPosition.Equals(1))
                        {
                            vh.txtTitleInfo.Text = question.IconCategories;
                            if (!question.IsSelected)
                            {
                                vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.silver_chalice_button_outline);
                                vh.ImageIcon.SetImageResource(Resource.Drawable.hourglass_icon);
                                vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                            }
                            else
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.active_hourglass_icon);
                            }
                        }
                        else if (question.IconPosition.Equals(2))
                        {
                            vh.txtTitleInfo.Text = question.IconCategories;
                            if (!question.IsSelected)
                            {
                                vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.silver_chalice_button_outline);
                                vh.ImageIcon.SetImageResource(Resource.Drawable.information_icon);
                                vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                            }
                            else
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.active_information_icon);
                            }
                        }
                        else if (question.IconPosition.Equals(3))
                        {
                            vh.txtTitleInfo.Text = question.IconCategories;
                            if (!question.IsSelected)
                            {
                                vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.silver_chalice_button_outline);
                                vh.ImageIcon.SetImageResource(Resource.Drawable.hand_icon);
                                vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                            }
                            else
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.active_hand_icon);
                            }
                        }
                        else
                        {
                            vh.txtTitleInfo.Text = question.IconCategories;
                            if (!question.IsSelected)
                            {
                                vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.silver_chalice_button_outline);
                                vh.ImageIcon.SetImageResource(Resource.Drawable.phonecall_icon);
                                vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                            }
                            else
                            {
                                vh.ImageIcon.SetImageResource(Resource.Drawable.active_phonecall_icon);
                            }
                        }
                    }

                    vh.outerLayout.Click += (o, e) =>
                    {
                        if (question.IsSelected == false)
                        {
                            question.IsSelected = true;
                        }
                        else if (question.IsSelected == true)
                        {
                            question.IsSelected = false;
                        }
                        if (question.ModelCategories.Equals("FeedbackYes"))
                        {
                            if (question.IconPosition.Equals(1))
                            {
                                if (!question.IsSelected)
                                {
                                    vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.silver_chalice_button_outline);
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.hourglass_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                                }
                                else
                                {
                                    vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.blue_out_line_thin);
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.active_hourglass_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.powerBlue));
                                }
                            }
                            else if (question.IconPosition.Equals(2))
                            {
                                if (!question.IsSelected)
                                {
                                    vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.silver_chalice_button_outline);
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.information_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                                }
                                else
                                {
                                    vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.blue_out_line_thin);
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.active_information_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.powerBlue));
                                }
                            }
                            else if (question.IconPosition.Equals(3))
                            {
                                if (!question.IsSelected)
                                {
                                    vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.silver_chalice_button_outline);
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.hand_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                                }
                                else
                                {
                                    vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.blue_out_line_thin);
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.active_hand_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.powerBlue));
                                }
                            }
                            else
                            {
                                if (!question.IsSelected)
                                {
                                    vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.silver_chalice_button_outline);
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.phonecall_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                                }
                                else
                                {
                                    vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.blue_out_line_thin);
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.active_phonecall_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.powerBlue));
                                }
                            }
                        }
                        else if (question.ModelCategories.Equals("FeedbackNo"))
                        {
                            if (question.IconPosition.Equals(1))
                            {
                                if (!question.IsSelected)
                                {
                                    vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.silver_chalice_button_outline);
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.hourglass_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                                }
                                else
                                {
                                    vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.blue_out_line_thin);
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.active_hourglass_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.powerBlue));
                                }
                            }
                            else if (question.IconPosition.Equals(2))
                            {
                                if (!question.IsSelected)
                                {
                                    vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.silver_chalice_button_outline);
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.information_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                                }
                                else
                                {
                                    vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.blue_out_line_thin);
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.active_information_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.powerBlue));
                                }
                            }
                            else if (question.IconPosition.Equals(3))
                            {
                                if (!question.IsSelected)
                                {
                                    vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.silver_chalice_button_outline);
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.hand_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                                }
                                else
                                {
                                    vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.blue_out_line_thin);
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.active_hand_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.powerBlue));
                                }
                            }
                            else
                            {
                                if (!question.IsSelected)
                                {
                                    vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.silver_chalice_button_outline);
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.phonecall_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.silverchalice));
                                }
                                else
                                {
                                    vh.outerLayout.Background = ContextCompat.GetDrawable(this.mContext, Resource.Drawable.blue_out_line_thin);
                                    vh.ImageIcon.SetImageResource(Resource.Drawable.active_phonecall_icon);
                                    vh.txtTitleInfo.SetTextColor(ContextCompat.GetColorStateList(this.mContext, Resource.Color.powerBlue));
                                }
                            }
                        }
                        OnSelectUpdate(vh, position);
                    };
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        void OnSelectUpdate(QuestionModelViewHolder sender, int position)
        {
            SelectUpdate(sender, position);
        }

        public bool IsAllQuestionAnswered()
        {
            bool flag = false;
            try
            {
                foreach (QuestionModel item in questions)
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

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.ImproveSelectedServiceDistruption;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new QuestionModelViewHolder(itemView, OnSelectUpdate);
        }

        public class QuestionModelViewHolder : RecyclerView.ViewHolder //EditText.IOnTouchListener
        {
            public TextView txtTitleInfo;
            public ImageView ImageIcon;
            public LinearLayout outerLayout;

            public QuestionModelViewHolder(View itemView, Action<QuestionModelViewHolder, int> listener) : base(itemView)
            {
                txtTitleInfo = itemView.FindViewById<TextView>(Resource.Id.bodyInfo);
                ImageIcon = itemView.FindViewById<ImageView>(Resource.Id.img_display_select);
                outerLayout = itemView.FindViewById<LinearLayout>(Resource.Id.outerLayout);

                outerLayout.Click += (s, e) => listener((this), base.LayoutPosition);

                TextViewUtils.SetMuseoSans500Typeface(txtTitleInfo);
                TextViewUtils.SetTextSize10(txtTitleInfo);

            }
        }
    }
}