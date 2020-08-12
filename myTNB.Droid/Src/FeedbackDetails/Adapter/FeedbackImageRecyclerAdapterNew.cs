using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;
using Android.Graphics.Drawables;


using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Adapter;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Utils;
using Square.Picasso;
using System;
using System.Collections.Generic;
using AndroidX.RecyclerView.Widget;
using AndroidX.Core.Graphics.Drawable;

namespace myTNB_Android.Src.FeedbackDetails.Adapter
{
 
    public class FeedbackImageRecyclerAdapterNew : BaseRecyclerAdapter<AttachedImage>
    {
        public event EventHandler<int> SelectClickEvent;

        public FeedbackImageRecyclerAdapterNew(bool notify) : base(notify)
        {
        }

        public FeedbackImageRecyclerAdapterNew(List<AttachedImage> itemList) : base(itemList)
        {
        }

        public FeedbackImageRecyclerAdapterNew(List<AttachedImage> itemList, bool notify) : base(itemList, notify)
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
                                    viewHolder.fileName.Text = image.Name;

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
            return new FeedbackPreLoginImageViewHolder(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.FeedbackDetailImageRowNew, parent, false), OnSelectEvent);
        }

        class FeedbackPreLoginImageViewHolder : BaseRecyclerViewHolder
        {

            [BindView(Resource.Id.imageView)]
            internal ImageView imageView;

            [BindView(Resource.Id.txtstep1of2)]
            internal TextView fileName;



            public FeedbackPreLoginImageViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                imageView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }

    }
}