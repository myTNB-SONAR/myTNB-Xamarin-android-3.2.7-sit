using Android.Views;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using System.Collections.Generic;

namespace myTNB_Android.Src.Base.Adapter
{
    /// <summary>
    /// The class that abstracts the implementation of the adding, removing , updating and handling for the preparation of recyclerview.
    /// </summary>
    /// <typeparam name="T">generic object type</typeparam>
    public abstract class BaseRecyclerAdapter<T> : RecyclerView.Adapter
    {

        protected List<T> itemList = new List<T>();
        protected bool notify;

        public BaseRecyclerAdapter(bool notify)
        {
            this.notify = notify;
        }

        public BaseRecyclerAdapter(List<T> itemList)
        {
            this.itemList.AddRange(itemList);
        }

        public BaseRecyclerAdapter(List<T> itemList, bool notify) : this(itemList)
        {
            this.notify = notify;
        }

        public override int ItemCount => itemList.Count;


        public void Insert(T item)
        {
            itemList.Insert(0, item);
            if (notify)
            {
                NotifyItemInserted(0);
            }
        }

        public void Add(T item)
        {

            itemList.Add(item);
            if (notify)
            {
                NotifyItemInserted(itemList.Count - 1);
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

        public void ClearAll()
        {
            itemList.Clear();
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
                NotifyItemRemoved(position);
            }

        }

        public void Update(int position, T item)
        {
            itemList[position] = item;
            if (notify)
            {
                NotifyItemChanged(position);
            }
        }

        public T GetItemObject(int position)
        {
            return itemList[position];
        }

        public class BaseRecyclerViewHolder : RecyclerView.ViewHolder
        {

            public BaseRecyclerViewHolder(View itemView) : base(itemView)
            {
                Cheeseknife.Bind(this, itemView);
            }
        }
    }
}