using System;
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
using Android.Support.Design.Widget;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using Android.Util;
using Android.Views.InputMethods;
using Java.Lang;
using static myTNB_Android.Src.MultipleAccountPayment.Adapter.SelectAccountListAdapter;
using Java.Text;
using Android.Text;
using Android.Text.Method;

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

        //public override int GetItemViewType(int position)
        //{
        //    return base.GetItemViewType(position);
        //}

        public void AddAccounts(List<MPAccount> _accountList)
        {
            accountList = accountList.Concat(_accountList).ToList();
            //this.NotifyDataSetChanged();
            this.NotifyItemRangeInserted(accountList.Count, _accountList.Count);
        }

        public override int ItemCount => accountList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            SelectAccountListViewHolder vh = holder as SelectAccountListViewHolder;
            TextViewUtils.SetMuseoSans300Typeface(vh.AccountLabel, vh.AccountNumber, vh.AccountAddress);
            TextViewUtils.SetMuseoSans300Typeface(vh.AmountLabel);
            TextViewUtils.SetMuseoSans300Typeface(vh.Amount);
            MPAccount item = accountList[position];
            vh.AccountNumber.Text = item.accountNumber;
            vh.AccountAddress.Text = item.accountAddress;
            vh.AccountLabel.Text = item.accountLabel;
            vh.Amount.Text = payableFormatter.Format(item.amount);
            if(item.amount < 1)
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
                        //if (!string.IsNullOrEmpty(vh.Amount.Text))
                        //{
                            ValidateHolder(item, position, vh);
                        //}
                    }
                }
            };
            vh.SelectAccountView.Checked = item.isSelected;
        }

        public void ValidateHolder(MPAccount item,int position, SelectAccountListViewHolder vh)
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
                    vh.SelectAccountView.Checked = false;
                    CheckChanged(this, position);
                }
                ///<summary>
                /// Uncomment this block of code to enable check for amount should be greater that Due Amount 
                /// </summary>
                /// 
                //else if (newAmount < item.orgAmount)
                //{
                //    vh.Amount.Text = item.orgAmount.ToString();
                //    vh.AmountLabel.Error = string.Format(mActicity.GetString(Resource.String.error_less_than_original_amount), item.amount.ToString());
                //    vh.AmountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);
                //    vh.Amount.RequestFocus();
                //    item.isValidAmount = false;
                //    item.isSelected = false;
                //    vh.SelectAccountView.Checked = false;
                //}
                else
                {
                    vh.AmountLabel.Error = "";
                    vh.AmountLabel.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomHint);
                    item.isValidAmount = true;
                    item.amount = newAmount;
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
                vh.SelectAccountView.Checked = false;
                CheckChanged(this, position);
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
            MPAccount item = accountList[position];
            if (GetSelectedAccounts().Count == 5 && sender.SelectAccountView.Checked)
            {
                bool isChecked = false;
                item.isSelected = isChecked;
                sender.SelectAccountView.Checked = isChecked;
                CheckChanged(this, -1);
            }
            else
            {
                bool isChecked = sender.SelectAccountView.Checked;
                item.isSelected = isChecked;
                sender.SelectAccountView.Checked = isChecked;
                //CheckChanged(this, position);
                ValidateHolder(item, position, sender);
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

            public SelectAccountListViewHolder(View itemView, Action<SelectAccountListViewHolder, int> listener) : base(itemView)
            {
                AccountLabel = itemView.FindViewById<TextView>(Resource.Id.text_account_lable);
                AccountNumber = itemView.FindViewById<TextView>(Resource.Id.text_account_number);
                AccountAddress = itemView.FindViewById<TextView>(Resource.Id.text_account_address);
                AmountLabel = itemView.FindViewById<TextInputLayout>(Resource.Id.account_amount_layout);
                Amount = itemView.FindViewById<EditText>(Resource.Id.account_amount_edittext);

                SelectAccountView = itemView.FindViewById<CheckBox>(Resource.Id.select_account);
                SelectAccountView.Click += (s, e) => listener((this), base.LayoutPosition);

                TextViewUtils.SetMuseoSans300Typeface(AccountLabel, AccountNumber, AccountAddress);
                TextViewUtils.SetMuseoSans300Typeface(AmountLabel);
                TextViewUtils.SetMuseoSans300Typeface(Amount);

                Amount.AddTextChangedListener(new RestrictTextChangeListener(Amount, AmountLabel, 2));

                //Amount.SetFilters(new IInputFilter[] { new DecimalFilter(2) });

                //Amount.SetFilters(new IInputFilter[] {new MoneyValueFilter(1,2)});

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
            //NotifyDataSetChanged();
        }

        public List<MPAccount> GetSelectedAccounts()
        {
            List<MPAccount> selectedStores = new List<MPAccount>();
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
            return selectedStores;
        }

        public bool IsAllAmountValid()
        {
            bool flag = true;
            foreach(MPAccount item in GetSelectedAccounts())
            {
                if (!item.isValidAmount)
                {
                    flag = false;
                }
            }
            return flag;
        }

    }
}