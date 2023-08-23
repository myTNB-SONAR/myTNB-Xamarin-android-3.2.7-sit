using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using myTNB_Android.Src.ApplicationStatus.SearchApplicationStatus.Models;
using myTNB_Android.Src.ApplicationStatus.SearchApplicationStatus.SearchApplicationStatusSelection.MVP;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.ApplicationStatus.SearchApplicationStatus.SearchApplicationStatusSelection.Adapter
{
    public class SearchApplicationStatusAdapter : RecyclerView.Adapter
    {
        public SearchApplicationStatusSelectionActivity mActivity;
        List<TypeModel> mTypeList = new List<TypeModel>();
        List<SearchByModel> mSearchByList = new List<SearchByModel>();
        List<SMRTypeModel> mSMRTypeList = new List<SMRTypeModel>();
        public event EventHandler<int> ItemClick;
        int mRequestCode = -1;
        int countNumber = 0;
        bool smrFlag;

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
            else if (mRequestCode == Constants.APPLICATION_STATUS_FILTER_SMRTYPE_REQUEST_CODE)
            {
                mSMRTypeList.Clear();
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

        public List<SMRTypeModel> GetSMRTypeByList()
        {
            return mSMRTypeList;
        }

        public SearchApplicationStatusAdapter(SearchApplicationStatusSelectionActivity activity
            , int requestCode
            , List<TypeModel> typeData = null
            , List<SearchByModel> searchByData = null
            , List<SMRTypeModel> smrData = null
            , bool smrFlag = false)
        {
            this.mActivity = activity;
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
                    mSearchByList.Add(new SearchByModel()
                    {
                        SearchTypeId = "NUMBER-CTA-ANDROID"
                    });
                }
                countNumber = mSearchByList.Count;
            }
            else if (mRequestCode == Constants.APPLICATION_STATUS_FILTER_SMRTYPE_REQUEST_CODE)
            {
                mSMRTypeList = new List<SMRTypeModel>();
                if (smrData != null && smrData.Count > 0)
                {
                    mSMRTypeList.AddRange(smrData);
                }
                countNumber = mSMRTypeList.Count;
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
            else if (mRequestCode == Constants.APPLICATION_STATUS_FILTER_SMRTYPE_REQUEST_CODE)
            {
                var selectedSMRTypeList = mSMRTypeList[position];
                vh.PopulateSMRTypeData(selectedSMRTypeList);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.ApplicationStatusFilterListItemLayout;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new ApplicationStatusFilterViewHolder(this.mActivity, this.mSearchByList, itemView, smrFlag, OnClick);
        }

        void OnClick(int position)
        {
            if (position != -1)
            {
                if (mRequestCode == Constants.APPLICATION_STATUS_FILTER_TYPE_REQUEST_CODE)
                {
                    var selectedTypeList = mTypeList[position];
                    bool previousSelectedFlag = selectedTypeList.isChecked;
                    if (!previousSelectedFlag)
                        previousSelectedFlag = !previousSelectedFlag;

                    foreach (var item in mTypeList)
                    {
                        item.isChecked = false;
                    }
                    mTypeList[position].isChecked = previousSelectedFlag;

                    this.NotifyDataSetChanged();

                    if (ItemClick != null)
                        ItemClick(this, position);
                }
                else if (mRequestCode == Constants.APPLICATION_STATUS_FILTER_SEARCH_BY_REQUEST_CODE)
                {
                    if (position == mSearchByList.Count - 1)
                    {

                    }
                    else
                    {
                        var selectedSearchByList = mSearchByList[position];
                        bool previousSelectedFlag = selectedSearchByList.isChecked;
                        if (!previousSelectedFlag)
                            previousSelectedFlag = !previousSelectedFlag;

                        foreach (var item in mSearchByList)
                        {
                            item.isChecked = false;
                        }
                        mSearchByList[position].isChecked = previousSelectedFlag;

                        this.NotifyDataSetChanged();

                        if (ItemClick != null)
                            ItemClick(this, position);
                    }
                }
                else if (mRequestCode == Constants.APPLICATION_STATUS_FILTER_SMRTYPE_REQUEST_CODE)
                {
                    var selectedTypeList = mSMRTypeList[position];
                    bool previousSelectedFlag = selectedTypeList.isChecked;
                   
                    if (!previousSelectedFlag)
                        previousSelectedFlag = !previousSelectedFlag;

                    foreach (var item in mSMRTypeList)
                    {
                        item.isChecked = false;
                    }


                    selectedTypeList.isChecked = previousSelectedFlag;

                    this.NotifyDataSetChanged();

                    if (ItemClick != null)
                        ItemClick(this, position);
                }
            }
        }
    }


    public class ApplicationStatusFilterViewHolder : RecyclerView.ViewHolder
    {
        public TextView txtFilterName { get; private set; }
        public ImageView imgApplicationFilter { get; private set; }
        public CheckBox chkApplicationFilter { get; private set; }
        public LinearLayout filterSelection { get; private set; }
        public LinearLayout ctaSelection { get; private set; }
        public TextView whyAccountsNotHere { get; private set; }
        private Context context;

        public ApplicationStatusFilterViewHolder(SearchApplicationStatusSelectionActivity activity
            , List<SearchByModel> mSearchByList
            , View itemView
            , bool smrFlag
            , Action<int> listener) : base(itemView)
        {
            context = itemView.Context;
            txtFilterName = itemView.FindViewById<TextView>(Resource.Id.txtFilterName);
            imgApplicationFilter = itemView.FindViewById<ImageView>(Resource.Id.imgApplicationFilter);
            chkApplicationFilter = itemView.FindViewById<CheckBox>(Resource.Id.chkApplicationFilter);
            filterSelection = itemView.FindViewById<LinearLayout>(Resource.Id.filterSelection);
            ctaSelection = itemView.FindViewById<LinearLayout>(Resource.Id.ctaSelection);
            whyAccountsNotHere = itemView.FindViewById<TextView>(Resource.Id.whyAccountsNotHere);
            txtFilterName.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.context, Resource.Color.tunaGrey)));
            TextViewUtils.SetMuseoSans300Typeface(txtFilterName);
            TextViewUtils.SetMuseoSans500Typeface(whyAccountsNotHere);
            txtFilterName.Click += (sender, e) => listener(base.LayoutPosition);
            chkApplicationFilter.Click += (sender, e) => listener(base.LayoutPosition);
            whyAccountsNotHere.Click += (sender, e) => ShowWhereIsMyAcc(activity, mSearchByList, smrFlag);
        }

        public async void ShowWhereIsMyAcc(Android.App.Activity context, List<SearchByModel> mSearchByList, bool smrFlag)
        {
            if (smrFlag = true)
            {
                MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                .SetTitle(Utility.GetLocalizedLabel("SearchByNumber", "whereToGetTheseNumberSMRTitle"))
                .SetMessage(Utility.GetLocalizedLabel("SearchByNumber", "whereToGetTheseNumberSMRMessage"))
                .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                .Build();
                whereisMyacc.Show();
            }
            else
            {
                MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(context, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
               .SetTitle(Utility.GetLocalizedLabel("SearchByNumber", "whereToGetTheseNumberTitle"))
               .SetMessage(Utility.GetLocalizedLabel("SearchByNumber", "whereToGetTheseNumberMessage"))
               .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
               .Build();
                whereisMyacc.Show();
            }
        }
        public static Bitmap Base64ToBitmap(string base64String)
        {
            Bitmap convertedBitmap = null;
            try
            {
                byte[] imageAsBytes = Android.Util.Base64.Decode(base64String, Base64Flags.Default);
                convertedBitmap = BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
            }
            catch (Exception e)
            {
                convertedBitmap = null;
                Utility.LoggingNonFatalError(e);
            }

            return convertedBitmap;
        }
        public void PopulateSearchByData(SearchByModel item)
        {
            try
            {
                if (item.SearchTypeId == "NUMBER-CTA-ANDROID")
                {
                    ctaSelection.Visibility = ViewStates.Visible;
                    filterSelection.Visibility = ViewStates.Gone;

                    whyAccountsNotHere.Text = Utility.GetLocalizedLabel("SearchByNumber", "whereToGetTheseNumber");
                    TextViewUtils.SetTextSize12(whyAccountsNotHere);
                }
                else
                {
                    ctaSelection.Visibility = ViewStates.Gone;
                    filterSelection.Visibility = ViewStates.Visible;
                    txtFilterName.Text = item.SearchTypeDisplay;
                    TextViewUtils.SetTextSize16(txtFilterName);
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
                txtFilterName.Text = item.ApplicationTypeDisplay;
                TextViewUtils.SetTextSize16(txtFilterName);
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

        public void PopulateSMRTypeData(SMRTypeModel item)
        {
            try
            {
                if (item.Type.Equals("info"))
                {
                    ctaSelection.Visibility = ViewStates.Visible;
                    filterSelection.Visibility = ViewStates.Gone;

                    whyAccountsNotHere.Text = Utility.GetLocalizedLabel("SearchByNumber", "whereToGetTheseNumberSMRInfoBar");
                    TextViewUtils.SetTextSize12(whyAccountsNotHere);
                }
                else
                {
                    ctaSelection.Visibility = ViewStates.Gone;
                    filterSelection.Visibility = ViewStates.Visible;
                    TextViewUtils.SetTextSize16(txtFilterName);


                    txtFilterName.Text = item.Type.ToString();
                    TextViewUtils.SetTextSize16(txtFilterName);
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

               
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
