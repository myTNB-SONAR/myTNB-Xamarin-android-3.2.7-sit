﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.Utils;
using Android.Support.Design.Widget;

namespace myTNB_Android.Src.AddAccount
{
    public class AdditionalAccountListAdapter : RecyclerView.Adapter
    {
        private BaseAppCompatActivity mActicity;
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
            var id = Resource.Layout.AdditionalAccountListItemView;
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
            TextViewUtils.SetMuseoSans300Typeface(AccountNumber, AccountAddress, AccountLabel);
            TextViewUtils.SetMuseoSans300Typeface(textInputLayoutAccountLabel);
            context = itemView.Context;
            //string hnt = context.GetString(Resource.String.add_account_form_account_nickname);
            //AccountLabel.Hint = hnt.ToUpper();
            AccountLabel.AddTextChangedListener(new InputFilterFormField(AccountLabel, textInputLayoutAccountLabel));

            AccountLabel.FocusChange += (sender, e) => {
                textInputLayoutAccountLabel.Error = null;
                if (e.HasFocus)
                {
                    if (item != null)
                    {
                    item.accountLabel = AccountLabel.Text.Trim();
                    if (!string.IsNullOrEmpty(item.accountLabel))
                    {
                        if (!Utility.isAlphaNumeric(item.accountLabel))
                        {
                            textInputLayoutAccountLabel.Error = context.GetString(Resource.String.invalid_charac);
                        }
                        else
                        {
                            textInputLayoutAccountLabel.Error = "e.g. My House, Parent's House";
                        }
                    }
                    else
                    {
                        textInputLayoutAccountLabel.Error = "e.g. My House, Parent's House";
                    }
                }
                } else {
                    if (item != null) {
                        item.accountLabel = AccountLabel.Text.Trim();
                        if (!string.IsNullOrEmpty(item.accountLabel))
                        {
                            if (!Utility.isAlphaNumeric(item.accountLabel))
                            {
                                textInputLayoutAccountLabel.Error = context.GetString(Resource.String.invalid_charac);
                            }
                        }    
                    }

                }
            };
        }



        public void PopulateData(NewAccount item) {
            this.item = item;
            try
            {
                AccountNumber.Text = this.item.accountNumber;
                //TextViewUtils.SetMuseoSans300Typeface(AccountNumber);
                AccountAddress.Text = this.item.accountAddress;
                //TextViewUtils.SetMuseoSans300Typeface(AccountAddress);
                if (this.item.accountLabel.Equals(EG_ACCOUNT_LABEL))
                {
                    AccountLabel.Hint = this.item.accountLabel;
                }
                else
                {
                    AccountLabel.Text = this.item.accountLabel;
                }
                AccountLabel.Text = this.item.accountLabel;
                //TextViewUtils.SetMuseoSans500Typeface(AccountLabel);
                AccountLabel.AfterTextChanged += (sender, args) =>
                {
                    textInputLayoutAccountLabel.Error = null;
                    item.accountLabel = AccountLabel.Text.Trim();
                    if (!string.IsNullOrEmpty(item.accountLabel))
                    {
                        if (!Utility.isAlphaNumeric(item.accountLabel))
                        {
                            textInputLayoutAccountLabel.Error = context.GetString(Resource.String.invalid_charac);
                        }
                        else
                        {
                            textInputLayoutAccountLabel.Error = "e.g. My House, Parent's House";
                        }
                    }
                    else
                    {
                        textInputLayoutAccountLabel.Error = "e.g. My House, Parent's House";
                    }
                };
            } catch(Exception e) {
                Utility.LoggingNonFatalError(e);
            }
        }

    }
}