using Android.Content;
using Android.Views;
using Android.Widget;
using CheeseBind;
using System.Collections.Generic;

namespace myTNB.Android.Src.Base.Adapter
{
    /// <summary>
    /// The class that abstracts the implementation of the adding, removing , updating and handling for the preparation of listview.
    /// </summary>
    /// <typeparam name="T">generic object type</typeparam>
    public abstract class BaseCustomAdapter<T> : BaseAdapter
    {
        protected Context context;
        protected List<T> itemList;
        protected bool notify;

        public BaseCustomAdapter(Context context)
        {
            this.context = context;
            this.itemList = new List<T>();
        }

        public BaseCustomAdapter(Context context, bool notify) : this(context)
        {
            this.notify = notify;
            this.itemList = new List<T>();
        }

        public BaseCustomAdapter(Context context, List<T> itemList) : this(context)
        {
            this.itemList = itemList;
        }

        public BaseCustomAdapter(Context context, List<T> itemList, bool notify) : this(context, itemList)
        {
            this.itemList = itemList;
            this.notify = notify;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public void Add(T item)
        {
            itemList.Add(item);
            if (notify)
            {
                NotifyDataSetChanged();
            }
        }

        public void AddAll(List<T> items)
        {
            itemList.AddRange(items);
            if (notify)
            {
                NotifyDataSetChanged();
            }
        }

        public void Clear()
        {
            itemList.Clear();
            if (notify)
            {
                NotifyDataSetChanged();
            }
        }

        public void Update(int index, T item)
        {
            itemList[index] = item;
            if (notify)
            {
                NotifyDataSetChanged();
            }
        }

        public void Remove(int position)
        {
            itemList.RemoveAt(position);
            if (notify)
            {
                NotifyDataSetChanged();
            }
        }

        public T GetItemObject(int position)
        {
            return itemList[position];
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {
            get
            {
                return itemList.Count;
            }
        }

    }

    public class BaseAdapterViewHolder : Java.Lang.Object
    {
        protected View itemView;

        public BaseAdapterViewHolder(View itemView)
        {
            this.itemView = itemView;
            Cheeseknife.Bind(this, itemView);
        }
    }
}