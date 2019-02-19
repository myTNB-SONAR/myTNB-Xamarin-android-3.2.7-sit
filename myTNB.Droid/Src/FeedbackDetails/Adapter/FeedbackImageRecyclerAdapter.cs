using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.Base.Models;
using static myTNB_Android.Src.Base.Models.SubmittedFeedbackDetails;
using Android.Support.V7.Widget;
using CheeseBind;
using Square.Picasso;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Graphics.Drawable;

namespace myTNB_Android.Src.FeedbackDetails.Adapter
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

   

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new FeedbackPreLoginImageViewHolder(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.FeedbackDetailImageRow, parent, false) , OnSelectEvent);
        }

        class FeedbackPreLoginImageViewHolder : BaseRecyclerViewHolder
        {

            [BindView(Resource.Id.imageView)]
            internal ImageView imageView;


            public FeedbackPreLoginImageViewHolder(View itemView , Action<int> listener) : base(itemView)
            {
                imageView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }

    }
}