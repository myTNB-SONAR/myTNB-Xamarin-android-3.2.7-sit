using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Icu.Text;
using Android.Support.V4.Graphics.Drawable;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Utils;
using Square.Picasso;
using System;
using System.Collections.Generic;
using Java.Util;
using Android.Preferences;
using Android.App;
using System.IO;

namespace myTNB_Android.Src.UpdatePersonalDetailStepTwo.Adapter
{
 
    public class your_ic_adaptercs : BaseRecyclerAdapter<AttachedImage>
    {
        private int countFileName = 1;

        public event EventHandler<int> RemoveClickEvent;

        public event EventHandler<int> AddClickEvent;

        public event EventHandler<int> SelectClickEvent;

        public your_ic_adaptercs(bool notify) : base(notify)
        {

        }

        public your_ic_adaptercs(List<AttachedImage> itemList) : base(itemList)
        {
        }

        public your_ic_adaptercs(List<AttachedImage> itemList, bool notify) : base(itemList, notify)
        {
        }

        public List<AttachedImage> GetAllImages()
        {
            List<AttachedImage> attachList = new List<AttachedImage>();
            foreach (AttachedImage image in itemList)
            {
                if (image.ViewType == Constants.VIEW_TYPE_REAL_RECORD)
                {
                    attachList.Add(image);
                }
            }
            return attachList;
        }

        public void OnSelectEvent(int position)
        {
            if (SelectClickEvent != null)
            {
                SelectClickEvent(this, position);
            }
        }


        public override int GetItemViewType(int position)
        {
            return itemList[position].ViewType;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AttachedImage image = GetItemObject(position);
            if (holder is FeedbackGeneralEnquiryStepOneImageViewHolder)
            {
                // the actual image

                var viewHolder = holder as FeedbackGeneralEnquiryStepOneImageViewHolder;
                Picasso.With(viewHolder.ItemView.Context)
                    .Load(new Java.IO.File(image.Path))
                    .Fit()
                    .Into(viewHolder.imageView
                            , delegate
                            {
                                Bitmap imageBitmap = ((BitmapDrawable)viewHolder.imageView.Drawable).Bitmap;
                                if (imageBitmap != null && !imageBitmap.IsRecycled)
                                {
                                    RoundedBitmapDrawable imageDrawable = RoundedBitmapDrawableFactory.Create(viewHolder.ItemView.Context.Resources, imageBitmap);
                                    imageDrawable.CornerRadius = 5f;
                                    viewHolder.imageView.SetImageDrawable(imageDrawable);

                                    if (image.Name == null)
                                    {
                                        SimpleDateFormat dateFormatter = new SimpleDateFormat("yyyyMMdd");
                                        Calendar calendar = Calendar.GetInstance(Locale.Default);

                                        var name = Resource.String.feedback_image_name_convention + dateFormatter.Format(calendar.TimeInMillis) + countFileName + ".jpeg";
                                        countFileName++;

                                        viewHolder.filename.Text = name;
                                    }
                                    else
                                    {
                                        viewHolder.filename.Text = image.Name;
                                    }


                                }

                            }
                            , delegate
                            {

                            });

                ;
            }
            else
            {
                // the dummy view
                var viewHolder = holder as FeedbackGeneralEnquiryStepOneDummyViewHolder;
                if (image.IsLoading)
                {
                    viewHolder.btnAdd.Visibility = ViewStates.Gone;
                    viewHolder.progressBar.Visibility = ViewStates.Visible;
                }
                else
                {
                    viewHolder.btnAdd.Visibility = ViewStates.Visible;
                    viewHolder.progressBar.Visibility = ViewStates.Gone;
                }
            }
        }

        private void RemoveClick(int position)
        {
            if (RemoveClickEvent != null)
            {
                RemoveClickEvent(this, position);
            }
        }

        private void AddClick(int position)
        {
            if (AddClickEvent != null)
            {
                AddClickEvent(this, position);
            }
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == Constants.VIEW_TYPE_REAL_RECORD)
            {
                return new FeedbackGeneralEnquiryStepOneImageViewHolder(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.FeedbackGeneralEnquiryStepOneImageRow, parent, false), RemoveClick);
            }
            else
            {
                return new FeedbackGeneralEnquiryStepOneDummyViewHolder(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.FeedbackGeneralEnquiryStepOneImageEmptyRow, parent, false), AddClick);
            }

        }





        class FeedbackGeneralEnquiryStepOneImageViewHolder : BaseRecyclerViewHolder
        {

            [BindView(Resource.Id.imageView)]

            internal ImageView imageView;

            [BindView(Resource.Id.btnClose)]

            internal ImageView btnClose;

            [BindView(Resource.Id.filename)]
            internal TextView filename;

            public FeedbackGeneralEnquiryStepOneImageViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                btnClose.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }

        class FeedbackGeneralEnquiryStepOneDummyViewHolder : BaseRecyclerViewHolder
        {

            [BindView(Resource.Id.btnAdd)]
            internal ImageView btnAdd;

            [BindView(Resource.Id.progressBar)]
            internal ProgressBar progressBar;


            public FeedbackGeneralEnquiryStepOneDummyViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                itemView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }

    }
}