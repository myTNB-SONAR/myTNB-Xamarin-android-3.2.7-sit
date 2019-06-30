﻿using AFollestad.MaterialDialogs;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Text;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace myTNB_Android.Src.MultipleAccountPayment.Adapter
{
    public class SelectAccountListAdapter : RecyclerView.Adapter
    {

        private BaseAppCompatActivity mActicity;
        private List<MPAccount> accountList = new List<MPAccount>();
        private List<MPAccount> orgAccountList = new List<MPAccount>();
        public event EventHandler<int> CheckChanged;
        private SparseBooleanArray selectedItems;

        private DecimalFormat payableFormatter = new DecimalFormat("###############0.00");

        public SelectAccountListAdapter(BaseToolbarAppCompatActivity activity, List<MPAccount> data)
        {
            this.mActicity = activity;
            this.accountList.AddRange(data);
            this.orgAccountList.AddRange(data);
            this.selectedItems = new SparseBooleanArray();
        }

        public void AddAccounts(List<MPAccount> _accountList)
        {
            try
            {
                accountList = accountList.Concat(_accountList).ToList();
                this.NotifyItemRangeInserted(accountList.Count, _accountList.Count);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override int ItemCount => accountList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            SelectAccountListViewHolder vh = holder as SelectAccountListViewHolder;
            try
            {
                TextViewUtils.SetMuseoSans300Typeface(vh.AccountLabel, vh.AccountNumber, vh.AccountAddress);
                TextViewUtils.SetMuseoSans300Typeface(vh.AmountLabel);
                TextViewUtils.SetMuseoSans300Typeface(vh.Amount, vh.MandatoryPaymentContent);
                TextViewUtils.SetMuseoSans500Typeface(vh.MandatoryPaymentTite);
                MPAccount item = accountList[position];
                vh.AccountNumber.Text = item.accountNumber;
                vh.AccountAddress.Text = item.accountAddress;
                vh.AccountLabel.Text = item.accountLabel;
                vh.Amount.Text = payableFormatter.Format(item.amount);

                if(item.OpenChargeTotal != 0)
                {
                    vh.MandatoryPaymentContent.Text = payableFormatter.Format(item.OpenChargeTotal);
                }
                else
                {
                    vh.MandatoryPaymentDetailView.Visibility = ViewStates.Gone;
                }

                if (item.amount < 1)
                {
                    item.isValidAmount = false;
                }
                else
                {
                    item.isValidAmount = true;
                }
                vh.AmountLabel.Error = "";
                vh.AmountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomHint);
                vh.Amount.AfterTextChanged += (sender, args) =>
                {
                    if (vh.Amount.HasFocus)
                    {
                        if (vh.SelectAccountView.Checked)
                        {
                            ValidateHolder(item, position, vh, false);
                        }
                    }
                };
                vh.SelectAccountView.Checked = item.isSelected;
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ValidateHolder(MPAccount item, int position, SelectAccountListViewHolder vh, bool tooltipShow)
        {
            try
            {
                if (!string.IsNullOrEmpty(vh.Amount.Text))
                {
                    double newAmount = double.Parse(vh.Amount.Text);
                    if (newAmount < 1)
                    {
                        vh.AmountLabel.Error = mActicity.GetString(Resource.String.error_invalid_amount);
                        vh.AmountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                        vh.Amount.RequestFocus();
                        item.isValidAmount = false;
                        item.isSelected = false;
                        item.tooltipPopUp = false;
                        vh.SelectAccountView.Checked = false;
                        CheckChanged(this, position);
                    }

                    else
                    {
                        vh.AmountLabel.Error = "";
                        vh.AmountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomHint);
                        item.isValidAmount = true;
                        item.amount = newAmount;
                        item.tooltipPopUp = tooltipShow;
                        CheckChanged(this, position);
                    }
                }
                else
                {
                    vh.AmountLabel.Error = mActicity.GetString(Resource.String.error_invalid_amount);
                    vh.AmountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                    vh.Amount.RequestFocus();
                    item.isValidAmount = false;
                    item.isSelected = false;
                    item.tooltipPopUp = false;
                    vh.SelectAccountView.Checked = false;
                    CheckChanged(this, position);
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.SelectAccountListItemView;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new SelectAccountListViewHolder(itemView, OnClick);
        }

        void OnClick(SelectAccountListViewHolder sender, int position)
        {
            try
            {
                MPAccount item = accountList[position];
                if (GetSelectedAccounts().Count == 5 && sender.SelectAccountView.Checked)
                {
                    bool isChecked = false;
                    item.isSelected = isChecked;
                    item.tooltipPopUp = false;
                    sender.SelectAccountView.Checked = isChecked;
                    CheckChanged(this, -1);
                }
                else
                {
                    bool isChecked = sender.SelectAccountView.Checked;
                    item.isSelected = isChecked;
                    sender.SelectAccountView.Checked = isChecked;
                    ValidateHolder(item, position, sender, true);
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public class SelectAccountListViewHolder : RecyclerView.ViewHolder
        {
            public TextView AccountLabel { get; private set; }
            public TextView AccountNumber { get; private set; }
            public TextView AccountAddress { get; private set; }
            public TextInputLayout AmountLabel { get; private set; }
            public EditText Amount { get; private set; }
            public CheckBox SelectAccountView { get; private set; }
            public LinearLayout MandatoryPaymentDetailView { get; private set; }
            public TextView MandatoryPaymentTite { get; private set; }
            public TextView MandatoryPaymentContent { get; private set; }

            public SelectAccountListViewHolder(View itemView, Action<SelectAccountListViewHolder, int> listener) : base(itemView)
            {
                AccountLabel = itemView.FindViewById<TextView>(Resource.Id.text_account_lable);
                AccountNumber = itemView.FindViewById<TextView>(Resource.Id.text_account_number);
                AccountAddress = itemView.FindViewById<TextView>(Resource.Id.text_account_address);
                AmountLabel = itemView.FindViewById<TextInputLayout>(Resource.Id.account_amount_layout);
                Amount = itemView.FindViewById<EditText>(Resource.Id.account_amount_edittext);
                MandatoryPaymentDetailView = itemView.FindViewById<LinearLayout>(Resource.Id.mandatory_payment_detail);
                MandatoryPaymentTite = itemView.FindViewById<TextView>(Resource.Id.mandatory_payment_title);
                MandatoryPaymentContent = itemView.FindViewById<TextView>(Resource.Id.mandatory_payment_content);


                SelectAccountView = itemView.FindViewById<CheckBox>(Resource.Id.select_account);
                SelectAccountView.Click += (s, e) => listener((this), base.LayoutPosition);

                TextViewUtils.SetMuseoSans300Typeface(AccountLabel, AccountNumber, AccountAddress);
                TextViewUtils.SetMuseoSans300Typeface(AmountLabel);
                TextViewUtils.SetMuseoSans300Typeface(Amount, MandatoryPaymentContent);
                TextViewUtils.SetMuseoSans500Typeface(MandatoryPaymentTite);

                Amount.AddTextChangedListener(new RestrictTextChangeListener(Amount, AmountLabel, 2));
            }

        }

        public void ToggleSelection(int pos)
        {
            if (selectedItems.Get(pos, false))
            {
                selectedItems.Delete(pos);
            }
            else
            {
                selectedItems.Put(pos, true);
            }
        }

        public List<MPAccount> GetSelectedAccounts()
        {
            List<MPAccount> selectedStores = new List<MPAccount>();
            try
            {
                if (accountList != null)
                {
                    for (int i = 0; i < accountList.Count; i++)
                    {
                        if (accountList[i].isSelected)
                        {
                            MPAccount item = accountList[i];
                            selectedStores.Add(item);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return selectedStores;
        }

        public bool IsAllAmountValid()
        {
            bool flag = true;
            try
            {
                foreach (MPAccount item in GetSelectedAccounts())
                {
                    if (!item.isValidAmount)
                    {
                        flag = false;
                    }
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return flag;
        }

    }
}