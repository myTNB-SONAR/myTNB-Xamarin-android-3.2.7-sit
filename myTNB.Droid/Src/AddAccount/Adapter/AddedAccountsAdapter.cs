using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace myTNB_Android.Src.AddAccount.Adapter
{
    public class AddedAccountsAdapter : RecyclerView.Adapter
    {
        private BaseAppCompatActivity mActicity;
        private List<NewAccount> accountList = new List<NewAccount>();
        private List<NewAccount> orgAccountList = new List<NewAccount>();
        public event EventHandler<int> ItemClick;

        public void Clear()
        {
            this.accountList.Clear();
            this.orgAccountList.Clear();
            this.NotifyDataSetChanged();
        }

        public void Add(NewAccount newData)
        {
            this.accountList.Add(newData);
            this.orgAccountList.Add(newData);
            this.NotifyItemInserted(accountList.IndexOf(newData));
        }

        public void AddAll(List<NewAccount> allData)
        {
            this.accountList.AddRange(allData);
            this.orgAccountList.AddRange(allData);
            this.NotifyDataSetChanged();
        }

        public AddedAccountsAdapter(BaseAppCompatActivity activity, List<NewAccount> data)
        {
            this.mActicity = activity;
            this.accountList.AddRange(data);
            this.orgAccountList.AddRange(data);
        }

        public override int ItemCount => accountList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AccountListViewHolder vh = holder as AccountListViewHolder;
            vh.bottomLine.Visibility = ViewStates.Visible;
            if (accountList != null && accountList.Count() > 0 && accountList.Count() == 1)
            {
                vh.bottomLine.Visibility = ViewStates.Gone;
            }

            NewAccount item = accountList[position];
            vh.AccountNumber.Text = item.accountNumber;
            TextViewUtils.SetMuseoSans300Typeface(vh.AccountNumber);
            vh.AccountAddress.Text = item.accountAddress;
            TextViewUtils.SetMuseoSans300Typeface(vh.AccountAddress);
            vh.AccountLabel.Text = item.accountLabel;
            TextViewUtils.SetMuseoSans500Typeface(vh.AccountLabel);


        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.AddedAccountListItemView;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new AccountListViewHolder(itemView);
        }

    }


    public class AccountListViewHolder : RecyclerView.ViewHolder
    {
        public TextView AccountLabel { get; private set; }
        public TextView AccountNumber { get; private set; }
        public TextView AccountAddress { get; private set; }
        public View bottomLine { get; private set; }

        public AccountListViewHolder(View itemView) : base(itemView)
        {
            AccountLabel = itemView.FindViewById<TextView>(Resource.Id.text_account_label);
            AccountNumber = itemView.FindViewById<TextView>(Resource.Id.text_account_number);
            AccountAddress = itemView.FindViewById<TextView>(Resource.Id.text_account_address);
            bottomLine = itemView.FindViewById<View>(Resource.Id.bottom_line);
            AccountLabel.TextSize = TextViewUtils.GetFontSize(16);
            AccountNumber.TextSize = TextViewUtils.GetFontSize(14);
            AccountAddress.TextSize = TextViewUtils.GetFontSize(12);
        }

    }
}