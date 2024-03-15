using Android.Graphics;
using Android.Graphics.Drawables;


using Android.Views;
using Android.Widget;
using AndroidX.Core.Graphics.Drawable;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using myTNB.AndroidApp.Src.Base.Adapter;
using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.Utils;
using Square.Picasso;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.FeedbackDetails.Adapter
{
    public class FeedbackImageRecyclerAdapter : BaseRecyclerAdapter<AttachedImage>
    {
        public event EventHandler<int> SelectClickEvent;

        public FeedbackImageRecyclerAdapter(bool notify) : base(notify)
        {
        }

        public FeedbackImageRecyclerAdapter(List<AttachedImage> itemList) : base(itemList)
        {
        }

        public FeedbackImageRecyclerAdapter(List<AttachedImage> itemList, bool notify) : base(itemList, notify)
        {
        }

        public void OnSelectEvent(int position)
        {
            if (SelectClickEvent != null)
            {
                SelectClickEvent(this, position);
            }
        }


        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                AttachedImage image = GetItemObject(position);
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
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }



        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new FeedbackPreLoginImageViewHolder(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.FeedbackDetailImageRow, parent, false), OnSelectEvent);
        }

        class FeedbackPreLoginImageViewHolder : BaseRecyclerViewHolder
        {

            [BindView(Resource.Id.imageView)]
            internal ImageView imageView;


            public FeedbackPreLoginImageViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                imageView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }

    }
}