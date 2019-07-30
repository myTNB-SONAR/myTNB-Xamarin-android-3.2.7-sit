using System;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.SSMR.SMRApplication.Adapter
{
    public class SelectSMRAccountAdapter : RecyclerView.Adapter
    {
        List<SMRAccount> accountList = new List<SMRAccount>();
        public SelectSMRAccountAdapter(BaseToolbarAppCompatActivity activity, List<SMRAccount> data)
        {
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
            return new SelectSMRAccountViewHolder(itemView);
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
            }
        }
    }
}
