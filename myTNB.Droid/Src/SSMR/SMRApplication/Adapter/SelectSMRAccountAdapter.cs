using System;
using System.Collections.Generic;

using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.SSMR.SMRApplication.MVP;
using myTNB.AndroidApp.Src.Utils;

namespace myTNB.AndroidApp.Src.SSMR.SMRApplication.Adapter
{
    public class SelectSMRAccountAdapter : RecyclerView.Adapter
    {
        List<SMRAccount> accountList = new List<SMRAccount>();
        SelectSMRAccountContract.IView mView;
        public SelectSMRAccountAdapter(SelectSMRAccountContract.IView view, List<SMRAccount> data)
        {
            mView = view;
            accountList = data;
        }

        public override int ItemCount => this.accountList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            SelectSMRAccountViewHolder selectSMRAccountViewHolder = holder as SelectSMRAccountViewHolder;
            selectSMRAccountViewHolder.accountName.Text = this.accountList[position].accountName;
            selectSMRAccountViewHolder.accountSelected.Checked = this.accountList[position].accountSelected;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            int id = Resource.Layout.SelectSMRAccountItemLayout;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            SelectSMRAccountViewHolder selectSMRAccountViewHolder = new SelectSMRAccountViewHolder(itemView);
            itemView.SetOnClickListener(new OnAccountSelectListener(mView, selectSMRAccountViewHolder));
            return selectSMRAccountViewHolder;
        }

        public class SelectSMRAccountViewHolder : RecyclerView.ViewHolder
        {
            public TextView accountName;
            public CheckBox accountSelected;
            public SelectSMRAccountViewHolder(View itemView) : base(itemView)
            {
                accountName = itemView.FindViewById(Resource.Id.account_name_value) as TextView;
                accountSelected = itemView.FindViewById(Resource.Id.select_smr_account) as CheckBox;
                TextViewUtils.SetMuseoSans300Typeface(accountName);
                TextViewUtils.SetTextSize16(accountName);
            }
        }

        public class OnAccountSelectListener : Java.Lang.Object, View.IOnClickListener
        {
            RecyclerView.ViewHolder mAdapter;
            SelectSMRAccountContract.IView mView;
            public OnAccountSelectListener(SelectSMRAccountContract.IView view, RecyclerView.ViewHolder adapter)
            {
                mView = view;
                mAdapter = adapter;
            }
            public void OnClick(View v)
            {
                int pos = mAdapter.AdapterPosition;
                mView.UpdateSelectedAccount(pos);
            }
        }
    }
}
