using Android.Content;
using Android.Graphics;
using Android.Preferences;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.TextField;
using myTNB.AndroidApp.Src.AddAccount.Activity;
using myTNB.AndroidApp.Src.AddAccount.Models;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Common.Activity;
using myTNB.AndroidApp.Src.Common.Model;
using myTNB.AndroidApp.Src.CompoundView;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace myTNB.AndroidApp.Src.AddAccount
{
    public class AdditionalAccountListAdapter : RecyclerView.Adapter
    {
        private BaseAppCompatActivity mActicity;
        private Android.App.Activity mActivity;
        private List<NewAccount> accountList = new List<NewAccount>();
        private List<NewAccount> orgAccountList = new List<NewAccount>();
        public event EventHandler<int> AdditionalItemClick;

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

        public AdditionalAccountListAdapter(BaseAppCompatActivity activity, List<NewAccount> data)
        {
            this.mActicity = activity;
            this.accountList.AddRange(data);
            this.orgAccountList.AddRange(data);
        }

        public override int ItemCount => accountList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            AdditionalAccountViewHolder vh = holder as AdditionalAccountViewHolder;
            NewAccount item = accountList[position];
            vh.PopulateData(item);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.AdditionalAccountListItemViewOwnerTenant;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new AdditionalAccountViewHolder(itemView, OnClick);
        }

        void OnClick(int position)
        {
            if (AdditionalItemClick != null)
                AdditionalItemClick(this, position);
        }

    }

    public class AdditionalAccountViewHolder : RecyclerView.ViewHolder
    {
        public EditText AccountLabel { get; private set; }
        public TextView AccountNumber { get; private set; }
        public TextView AccountAddress { get; private set; }
        public ImageView DeleteView { get; private set; }
        public TextInputLayout textInputLayoutAccountLabel { get; private set; }
        private MobileNumberInputComponent mobileNumberInputComponent;

        private string phone_no;
        private bool flag = false;

        private Context context;
        private readonly string EG_ACCOUNT_LABEL = "";

        private NewAccount item = null;

        public AdditionalAccountViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            textInputLayoutAccountLabel = itemView.FindViewById<TextInputLayout>(Resource.Id.account_label_layout);
            AccountLabel = itemView.FindViewById<EditText>(Resource.Id.text_account_label);
            AccountNumber = itemView.FindViewById<TextView>(Resource.Id.text_account_number);
            AccountAddress = itemView.FindViewById<TextView>(Resource.Id.text_account_address);
            DeleteView = itemView.FindViewById<ImageView>(Resource.Id.delete);
            DeleteView.Click += (sender, e) => listener(base.LayoutPosition);

            textInputLayoutAccountLabel.Hint = Utility.GetLocalizedCommonLabel("acctNickname");
            textInputLayoutAccountLabel.SetHintTextAppearance(TextViewUtils.IsLargeFonts
                    ? Resource.Style.TextInputLayout_TextAppearance_Large
                    : Resource.Style.TextInputLayout_TextAppearance_Small);
            textInputLayoutAccountLabel.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                ? Resource.Style.TextInputLayoutFeedbackCountLarge
                : Resource.Style.TextInputLayoutFeedbackCount);
            context = itemView.Context;
            AccountLabel.AddTextChangedListener(new InputFilterFormField(AccountLabel, textInputLayoutAccountLabel));

            TextViewUtils.SetTextSize16(AccountLabel);
            TextViewUtils.SetTextSize14(AccountNumber);
            TextViewUtils.SetTextSize12(AccountAddress);


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

        public void PopulateData(NewAccount item)
        {
            this.item = item;
            int noISDONly;
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
                AccountLabel.Text = this.item.accountLabel;
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