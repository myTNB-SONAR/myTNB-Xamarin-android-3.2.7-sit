using Android.Content;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.ApplicationStatus.SearchApplicationStatus.Models;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.ApplicationStatus.SearchApplicationStatus.SearchApplicationStatusSelection.Adapter
{
    public class SearchApplicationStatusAdapter : RecyclerView.Adapter
    {
        private BaseActivityCustom mActicity;
        List<TypeModel> mTypeList = new List<TypeModel>();
        List<SearchByModel> mSearchByList = new List<SearchByModel>();
        public event EventHandler<int> ItemClick;
        int mRequestCode = -1;
        int countNumber = 0;

        public void Clear()
        {
            if (mRequestCode == Constants.APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE)
            {
                mTypeList.Clear();
            }
            else if (mRequestCode == Constants.APPLICATION_STATUS_FILTER_SEARCH_BY_REQUEST_CODE)
            {
                mSearchByList.Clear();
            }
            countNumber = 0;
            this.NotifyDataSetChanged();
        }

        public List<TypeModel> GetTypeList()
        {
            return mTypeList;
        }

        public List<SearchByModel> GetSearchByList()
        {
            return mSearchByList;
        }

        public SearchApplicationStatusAdapter(BaseActivityCustom activity, int requestCode, List<TypeModel> typeData = null, List<SearchByModel> searchByData = null)
        {
            this.mActicity = activity;
            this.mRequestCode = requestCode;

            if (mRequestCode == Constants.APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE)
            {
                mTypeList = new List<TypeModel>();
                if (typeData != null && typeData.Count > 0)
                {
                    mTypeList.AddRange(typeData);
                }
                countNumber = mTypeList.Count;
            }
            else if (mRequestCode == Constants.APPLICATION_STATUS_FILTER_SEARCH_BY_REQUEST_CODE)
            {
                mSearchByList = new List<SearchByModel>();
                if (searchByData != null && searchByData.Count > 0)
                {
                    mSearchByList.AddRange(searchByData);
                }
                mSearchByList.AddRange(searchByData);
                countNumber = mSearchByList.Count;
            }
        }

        public override int ItemCount => countNumber;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ApplicationStatusFilterViewHolder vh = holder as ApplicationStatusFilterViewHolder;

            if (mRequestCode == Constants.APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE)
            {
                var selectedTypeList = mTypeList[position];
                vh.PopulateTypeData(selectedTypeList);
            }
            else if (mRequestCode == Constants.APPLICATION_STATUS_FILTER_SEARCH_BY_REQUEST_CODE)
            {
                var selectedSearchByList = mSearchByList[position];
                vh.PopulateSearchByData(selectedSearchByList);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.ApplicationStatusFilterListItemLayout;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new ApplicationStatusFilterViewHolder(itemView, OnClick);
        }

        void OnClick(int position)
        {
            if (position != -1)
            {
                if (mRequestCode == Constants.APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE)
                {
                    var selectedTypeList = mTypeList[position];
                    bool previousSelectedFlag = selectedTypeList.isChecked;
                    previousSelectedFlag = !previousSelectedFlag;
                    foreach (var item in mTypeList)
                    {
                        item.isChecked = false;
                    }
                    mTypeList[position].isChecked = previousSelectedFlag;
                }
                else if (mRequestCode == Constants.APPLICATION_STATUS_FILTER_SEARCH_BY_REQUEST_CODE)
                {
                    var selectedSearchByList = mSearchByList[position];
                    bool previousSelectedFlag = selectedSearchByList.isChecked;
                    previousSelectedFlag = !previousSelectedFlag;
                    foreach (var item in mSearchByList)
                    {
                        item.isChecked = false;
                    }
                    mSearchByList[position].isChecked = previousSelectedFlag;
                }

                this.NotifyDataSetChanged();

                if (ItemClick != null)
                    ItemClick(this, position);
            }
        }
    }


    public class ApplicationStatusFilterViewHolder : RecyclerView.ViewHolder
    {
        public TextView txtFilterName { get; private set; }
        public ImageView imgApplicationFilter { get; private set; }
        public CheckBox chkApplicationFilter { get; private set; }
        private Context context;

        public ApplicationStatusFilterViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            context = itemView.Context;
            txtFilterName = itemView.FindViewById<TextView>(Resource.Id.txtFilterName);
            imgApplicationFilter = itemView.FindViewById<ImageView>(Resource.Id.imgApplicationFilter);
            chkApplicationFilter = itemView.FindViewById<CheckBox>(Resource.Id.chkApplicationFilter);
            txtFilterName.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.context, Resource.Color.tunaGrey)));
            TextViewUtils.SetMuseoSans300Typeface(txtFilterName);
            txtFilterName.Click += (sender, e) => listener(base.LayoutPosition);
            chkApplicationFilter.Click += (sender, e) => listener(base.LayoutPosition);
        }


        public void PopulateSearchByData(SearchByModel item)
        {
            try
            {
                txtFilterName.Text = item.Title;
                imgApplicationFilter.Visibility = ViewStates.Gone;
                chkApplicationFilter.Visibility = ViewStates.Gone;
                txtFilterName.Clickable = true;
                chkApplicationFilter.Clickable = false;
                if (item.isChecked)
                {
                    imgApplicationFilter.Visibility = ViewStates.Visible;
                }

                txtFilterName.RequestLayout();
                imgApplicationFilter.RequestLayout();
                chkApplicationFilter.RequestLayout();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void PopulateTypeData(TypeModel item)
        {
            try
            {
                txtFilterName.Text = item.Title;
                imgApplicationFilter.Visibility = ViewStates.Gone;
                chkApplicationFilter.Visibility = ViewStates.Gone;
                txtFilterName.Clickable = true;
                chkApplicationFilter.Clickable = false;
                if (item.isChecked)
                {
                    imgApplicationFilter.Visibility = ViewStates.Visible;
                }

                txtFilterName.RequestLayout();
                imgApplicationFilter.RequestLayout();
                chkApplicationFilter.RequestLayout();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

    }
}
