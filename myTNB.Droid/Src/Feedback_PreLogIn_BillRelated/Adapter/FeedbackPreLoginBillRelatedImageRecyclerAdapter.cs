﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Adapter;
using CheeseBind;
using myTNB_Android.Src.Utils;

using Square.Picasso;
using Java.IO;
using myTNB_Android.Src.Base.Models;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Graphics.Drawable;

namespace myTNB_Android.Src.Feedback_PreLogIn_BillRelated.Adapter
{
    public class FeedbackPreLoginBillRelatedImageRecyclerAdapter : BaseRecyclerAdapter<AttachedImage>
    {

        public event EventHandler<int> RemoveClickEvent;
        public event EventHandler<int> AddClickEvent;

        public FeedbackPreLoginBillRelatedImageRecyclerAdapter(bool notify) : base(notify)
        {
        }

        public FeedbackPreLoginBillRelatedImageRecyclerAdapter(List<AttachedImage> itemList) : base(itemList)
        {
        }

        public FeedbackPreLoginBillRelatedImageRecyclerAdapter(List<AttachedImage> itemList, bool notify) : base(itemList, notify)
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

        public override int GetItemViewType(int position)
        {
            return itemList[position].ViewType;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AttachedImage image = GetItemObject(position);
            if (holder is FeedbackPreLoginImageViewHolder)
            {
                // the actual image
                var viewHolder = holder as FeedbackPreLoginImageViewHolder;
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

                                }
                            }
                            , delegate
                            {

                            });
            }
            else
            {
                // the dummy view
                var viewHolder = holder as FeedbackPreLoginDummyViewHolder;
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
                RemoveClickEvent(this , position);
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
                return new FeedbackPreLoginImageViewHolder(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.FeedbackBillRelatedImageRow, parent, false) , RemoveClick);
            }
            else
            {
                return new FeedbackPreLoginDummyViewHolder(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.FeedbackBillRelatedImageEmptyRow, parent, false) , AddClick);
            }
            
        }

        class FeedbackPreLoginImageViewHolder : BaseRecyclerViewHolder
        {

            [BindView(Resource.Id.imageView)]
            internal ImageView imageView;

            [BindView(Resource.Id.btnClose)]
            internal ImageView btnClose;

            public FeedbackPreLoginImageViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                btnClose.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }

        class FeedbackPreLoginDummyViewHolder : BaseRecyclerViewHolder
        {

            [BindView(Resource.Id.btnAdd)]
            internal ImageView btnAdd;

            [BindView(Resource.Id.progressBar)]
            internal ProgressBar progressBar;


            public FeedbackPreLoginDummyViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                itemView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
    }
}