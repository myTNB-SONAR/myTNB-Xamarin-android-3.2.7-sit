
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using myTNB.AndroidApp.Src.QuickActionArrange.Model;
using myTNB.AndroidApp.Src.Utils;


namespace myTNB.AndroidApp.Src.QuickActionArrange.Adapter
{
    public class AddIconAdapter : RecyclerView.Adapter
    {
        private Android.App.Activity mActivity;
        public event EventHandler<int> ItemClick;
        public event EventHandler<int> IconClick;
        private List<int> IconList = new List<int>();
        private IItemClickListener listener;

        public override int ItemCount => IconList.Count;

        public void Clear()
        {
            this.IconList.Clear();
            this.NotifyDataSetChanged();
        }

        public void Add(int newData)
        {
            this.IconList.Add(newData);
            this.NotifyItemInserted(IconList.IndexOf(newData));
        }

        public void AddAll(List<int> allData)
        {
            this.IconList.AddRange(allData);
            this.NotifyDataSetChanged();
        }

        public void RemoveItem(int Data)
        {
            this.IconList.RemoveAt(Data);
            this.NotifyItemRemoved(Data);
        }

        public int GetItemCount()
        {
            return IconList.Count;
        }

        public AddIconAdapter(Android.App.Activity activity, List<int> data)
        {
            this.mActivity = activity;
            this.IconList = data;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AddIconAdapterViewHolder vh = holder as AddIconAdapterViewHolder;

            try
            {

                vh.btnNewIcon.SetImageResource(Resource.Drawable.ic_plusround_black);
                vh.containerNewIcon.Click += (sender, e) =>
                {
                    listener?.OnItemClickAddIcon();
                    Log.Debug("ClickEvent", "Button clicked!");
                };
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetItemClickListener(IItemClickListener clickListener)
        {
            this.listener = clickListener;
        }

        public interface IItemClickListener
        {
            void OnItemClickAddIcon();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.QuickActionAddIconItemLayout;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new AddIconAdapterViewHolder(itemView);
        }

        public class AddIconAdapterViewHolder : RecyclerView.ViewHolder
        {
            public ImageView btnNewIcon { get; private set; }
            public LinearLayout containerNewIcon { get; private set; }
            private Context context;

            public AddIconAdapterViewHolder(View itemView) : base(itemView)
            {
                btnNewIcon = itemView.FindViewById<ImageView>(Resource.Id.imageActionIcon);
                containerNewIcon = itemView.FindViewById<LinearLayout>(Resource.Id.btnNewIcon);
                context = itemView.Context;
            }
        }
    }

}

