using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using myTNB.AndroidApp.Src.ApplicationStatus.ApplicationStatusListing.Models;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.ApplicationStatus.ApplicationStatusFilter.Adapter
{
    public class ApplicationStatusFilterAdapter : RecyclerView.Adapter
    {
        private BaseActivityCustom mActicity;
        List<ApplicationStatusCodeModel> mStatusCodeList = new List<ApplicationStatusCodeModel>();
        List<ApplicationStatusTypeModel> mTypeList = new List<ApplicationStatusTypeModel>();
        List<ApplicationStatusStringSelectionModel> mDisplayStringList = new List<ApplicationStatusStringSelectionModel>();
        public event EventHandler<int> ItemClick;
        int mRequestCode = -1;
        int countNumber = 0;
        bool mMultiSelectCapable = false;

        public void Clear()
        {
            if (mRequestCode == Constants.APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE)
            {
                mTypeList.Clear();
            }
            else if (mRequestCode == Constants.APPLICATION_STATUS_FILTER_STATUS_REQUEST_CODE)
            {
                mStatusCodeList.Clear();
            }
            else
            {
                mDisplayStringList.Clear();
            }
            countNumber = 0;
            this.NotifyDataSetChanged();
        }

        public List<ApplicationStatusCodeModel> GetSelectedStatusCodeList()
        {
            return mStatusCodeList.FindAll(x => x.isChecked);
        }

        public List<ApplicationStatusTypeModel> GetSelectedTypeCodeList()
        {
            return mTypeList.FindAll(x => x.isChecked);
        }

        public List<ApplicationStatusStringSelectionModel> GetSelectedStringList()
        {
            return mDisplayStringList.FindAll(x => x.isChecked);
        }

        public ApplicationStatusFilterAdapter(BaseActivityCustom activity
            , int requestCode
            , bool multiSelectCapable
            , List<ApplicationStatusCodeModel> codeData = null
            , List<ApplicationStatusTypeModel> typeData = null
            , List<ApplicationStatusStringSelectionModel> stringData = null)
        {
            this.mActicity = activity;
            this.mRequestCode = requestCode;
            this.mMultiSelectCapable = multiSelectCapable;

            if (mRequestCode == Constants.APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE)
            {
                mTypeList = new List<ApplicationStatusTypeModel>();
                mTypeList.AddRange(typeData);
                countNumber = mTypeList.Count;
            }
            else if (mRequestCode == Constants.APPLICATION_STATUS_FILTER_STATUS_REQUEST_CODE)
            {
                mStatusCodeList = new List<ApplicationStatusCodeModel>();
                mStatusCodeList.AddRange(codeData);
                countNumber = mStatusCodeList.Count;
            }
            else
            {
                mDisplayStringList = new List<ApplicationStatusStringSelectionModel>();
                mDisplayStringList.AddRange(stringData);
                countNumber = mDisplayStringList.Count;
            }
        }

        public override int ItemCount => countNumber;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ApplicationStatusFilterViewHolder vh = holder as ApplicationStatusFilterViewHolder;

            if (mRequestCode == Constants.APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE)
            {
                var selectedTypeList = mTypeList[position];
                vh.PopulateTypeData(selectedTypeList, this.mMultiSelectCapable);
            }
            else if (mRequestCode == Constants.APPLICATION_STATUS_FILTER_STATUS_REQUEST_CODE)
            {
                var selectedStatusList = mStatusCodeList[position];
                vh.PopulateStatusData(selectedStatusList, this.mMultiSelectCapable);
            }
            else
            {
                var selectedStringList = mDisplayStringList[position];
                vh.PopulateStringData(selectedStringList, this.mMultiSelectCapable);
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
                else if (mRequestCode == Constants.APPLICATION_STATUS_FILTER_STATUS_REQUEST_CODE)
                {
                    var selectedCodeList = mStatusCodeList[position];
                    bool previousSelectedFlag = selectedCodeList.isChecked;
                    previousSelectedFlag = !previousSelectedFlag;

                    foreach (var item in mStatusCodeList)
                    {
                        item.isChecked = false;
                    }
                    mStatusCodeList[position].isChecked = previousSelectedFlag;
                }
                else
                {
                    var selectedStringList = mDisplayStringList[position];
                    bool previousSelectedFlag = selectedStringList.isChecked;
                    previousSelectedFlag = !previousSelectedFlag;

                    foreach (var item in mDisplayStringList)
                    {
                        item.isChecked = false;
                    }
                    mDisplayStringList[position].isChecked = previousSelectedFlag;
                }

                this.NotifyDataSetChanged();

                if (ItemClick != null)
                {
                    ItemClick(this, position);
                }
            }
        }
    }

    public class ApplicationStatusFilterViewHolder : RecyclerView.ViewHolder
    {
        public TextView txtFilterName { get; private set; }
        public TextView whyAccountsNotHere { get; private set; }

        public ImageView imgApplicationFilter { get; private set; }
        public CheckBox chkApplicationFilter { get; private set; }
        private Context context;

        public ApplicationStatusFilterViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            context = itemView.Context;
            txtFilterName = itemView.FindViewById<TextView>(Resource.Id.txtFilterName);
            whyAccountsNotHere = itemView.FindViewById<TextView>(Resource.Id.whyAccountsNotHere);
            imgApplicationFilter = itemView.FindViewById<ImageView>(Resource.Id.imgApplicationFilter);
            chkApplicationFilter = itemView.FindViewById<CheckBox>(Resource.Id.chkApplicationFilter);
            txtFilterName.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.context, Resource.Color.tunaGrey)));
            TextViewUtils.SetMuseoSans300Typeface(txtFilterName);
            txtFilterName.Click += (sender, e) => listener(base.LayoutPosition);
            chkApplicationFilter.Click += (sender, e) => listener(base.LayoutPosition);
            TextViewUtils.SetTextSize12(whyAccountsNotHere);
            TextViewUtils.SetTextSize16(txtFilterName);
        }

        public void PopulateStatusData(ApplicationStatusCodeModel item, bool isMultipleSelectCapable)
        {
            try
            {
                txtFilterName.Text = item.Status;
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

        public void PopulateTypeData(ApplicationStatusTypeModel item, bool isMultipleSelectCapable)
        {
            try
            {
                txtFilterName.Text = item.Type;

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

        public void PopulateStringData(ApplicationStatusStringSelectionModel item, bool isMultipleSelectCapable)
        {
            try
            {
                txtFilterName.Text = item.Value;
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