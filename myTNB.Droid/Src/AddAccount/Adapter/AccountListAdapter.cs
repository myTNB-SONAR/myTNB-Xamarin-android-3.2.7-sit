using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.TextField;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.AddAccount
{
    public class AccountListAdapter : RecyclerView.Adapter
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

        public List<NewAccount> GetAccountList()
        {
            return accountList;
        }

        public AccountListAdapter(BaseAppCompatActivity activity, List<NewAccount> data)
        {
            this.mActicity = activity;
            this.accountList.AddRange(data);
            this.orgAccountList.AddRange(data);
        }

        public override int ItemCount => accountList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AccountListViewHolder vh = holder as AccountListViewHolder;

            NewAccount item = accountList[position];
            vh.populateData(item);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.AccountListItemView;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new AccountListViewHolder(itemView, OnClick);
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }


    public class AccountListViewHolder : RecyclerView.ViewHolder
    {
        public EditText AccountLabel { get; private set; }
        public TextView AccountNumber { get; private set; }
        public TextView AccountAddress { get; private set; }
        public ImageView DeleteView { get; private set; }
        public TextInputLayout textInputLayoutAccountLabel { get; private set; }
        private Context context;

        private readonly string EG_ACCOUNT_LABEL = "";

        private NewAccount item = null;

        public AccountListViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            textInputLayoutAccountLabel = itemView.FindViewById<TextInputLayout>(Resource.Id.account_label_layout);
            AccountLabel = itemView.FindViewById<EditText>(Resource.Id.account_label_edittext);
            AccountNumber = itemView.FindViewById<TextView>(Resource.Id.text_account_number);
            AccountAddress = itemView.FindViewById<TextView>(Resource.Id.text_account_address);
            DeleteView = itemView.FindViewById<ImageView>(Resource.Id.delete);
            DeleteView.Click += (sender, e) => listener(base.LayoutPosition);
            TextViewUtils.SetMuseoSans300Typeface(AccountNumber, AccountAddress, AccountLabel);
            TextViewUtils.SetMuseoSans300Typeface(textInputLayoutAccountLabel);
            textInputLayoutAccountLabel.Hint = Utility.GetLocalizedCommonLabel("acctNickname");
            textInputLayoutAccountLabel.SetErrorTextAppearance(TextViewUtils.SelectedFontSize() == "L" ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);
            context = itemView.Context;
            AccountLabel.AddTextChangedListener(new InputFilterFormField(AccountLabel, textInputLayoutAccountLabel));
            AccountLabel.TextSize = TextViewUtils.GetFontSize(16);
            AccountNumber.TextSize = TextViewUtils.GetFontSize(14);
            AccountAddress.TextSize = TextViewUtils.GetFontSize(12);

            AccountLabel.FocusChange += (sender, e) =>
            {
                textInputLayoutAccountLabel.Error = null;
                if (e.HasFocus)
                {
                    if (item != null)
                    {
                        item.accountLabel = AccountLabel.Text.Trim();
                        if (!string.IsNullOrEmpty(item.accountLabel))
                        {
                            textInputLayoutAccountLabel.Error = Utility.GetLocalizedHintLabel("nickname");
                        }
                        else
                        {
                            textInputLayoutAccountLabel.Error = Utility.GetLocalizedHintLabel("nickname");
                        }
                    }
                }
                else
                {
                    if (item != null)
                    {
                        item.accountLabel = AccountLabel.Text.Trim();
                    }

                }
            };
        }


        public void populateData(NewAccount item)
        {
            this.item = item;
            try
            {
                AccountNumber.Text = this.item.accountNumber;
                AccountAddress.Text = this.item.accountAddress;
                if (this.item.accountLabel.Equals(EG_ACCOUNT_LABEL))
                {
                    AccountLabel.Hint = this.item.accountLabel;
                }
                else
                {
                    AccountLabel.Text = this.item.accountLabel;
                }
                TextViewUtils.SetMuseoSans500Typeface(AccountLabel);
                AccountLabel.AfterTextChanged += (sender, args) =>
                {
                    textInputLayoutAccountLabel.Error = null;
                    item.accountLabel = AccountLabel.Text.Trim();
                    if (!string.IsNullOrEmpty(item.accountLabel))
                    {
                        textInputLayoutAccountLabel.Error = Utility.GetLocalizedHintLabel("nickname");
                    }
                    else
                    {
                        textInputLayoutAccountLabel.Error = Utility.GetLocalizedHintLabel("nickname");
                    }
                };
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

    }
}
