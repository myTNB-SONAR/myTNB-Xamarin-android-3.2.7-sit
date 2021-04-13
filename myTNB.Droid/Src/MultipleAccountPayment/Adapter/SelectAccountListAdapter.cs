using AFollestad.MaterialDialogs;
using Android.Content;
using Android.Graphics;
using Android.OS;



using Android.Text;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.TextField;
using Java.Text;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private bool IsShowMoreEnable = true;
        private static Action ShowMoreAction = null;

        private DecimalFormat payableFormatter = new DecimalFormat("###############0.00", new DecimalFormatSymbols(Java.Util.Locale.Us));

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
                NotifyDataSetChanged();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override int ItemCount => accountList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            holder.IsRecyclable = false;
            SelectAccountListViewHolder vh = holder as SelectAccountListViewHolder;
            try
            {
                TextViewUtils.SetMuseoSans300Typeface(vh.AccountNumber, vh.AccountAddress);
                TextViewUtils.SetMuseoSans300Typeface(vh.AmountLabel);
                TextViewUtils.SetMuseoSans300Typeface(vh.Amount);
                TextViewUtils.SetMuseoSans500Typeface(vh.AccountLabel);
                MPAccount item = accountList[position];
                vh.AccountNumber.Text = item.accountNumber;
                vh.AccountAddress.Text = item.accountAddress;
                if (string.IsNullOrEmpty(item.accountAddress))
                {
                    vh.AccountAddress.Visibility = ViewStates.Gone;
                }
                vh.AccountLabel.Text = item.accountLabel;

                if (item.amount <= 0f)
                {
                    vh.Amount.Text = "";
                }
                else
                {
                    vh.Amount.Text = payableFormatter.Format(item.amount);
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
                vh.AmountLabel.ErrorEnabled = false;
                vh.AmountLabel.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomHintLarge : Resource.Style.TextInputLayoutBottomHint);

                vh.Amount.AfterTextChanged += (sender, args) =>
                {
                    try
                    {
                        CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                        item.amount = double.Parse(vh.Amount.Text, currCult);
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }

                    if (vh.Amount.HasFocus)
                    {
                        if (vh.SelectAccountView.Checked)
                        {
                            ValidateHolder(item, position, vh, false);
                        }
                    }
                };
                vh.SelectAccountView.Checked = item.isSelected;
                if (vh.SelectAccountView.Checked)
                {
                    ValidateHolder(item, position, vh, false);
                }
                if (position == (ItemCount - 1) && IsShowMoreEnable)
                {
                    string htmlText = "<html><u>" + Utility.GetLocalizedLabel("SelectBills", "loadMore") + "</u></html>";
                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                    {
                        vh.ShowMore.TextFormatted = Html.FromHtml(htmlText, FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
                        vh.ShowMore.TextFormatted = Html.FromHtml(htmlText);
                    }
                    TextViewUtils.SetMuseoSans300Typeface(vh.ShowMore);
                    vh.ShowMore.Visibility = ViewStates.Visible;
                }
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
                    CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                    double newAmount = double.Parse(vh.Amount.Text, currCult);
                    if (newAmount < 1)
                    {
                        vh.AmountLabel.Error = Utility.GetLocalizedLabel("Error", "minimumPayAmount");
                        vh.AmountLabel.ErrorEnabled = true;
                        vh.AmountLabel.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);

                        vh.Amount.SetTextColor(new Color(ContextCompat.GetColor(mActicity, Resource.Color.tomato)));
                        vh.Amount.RequestFocus();
                        item.isValidAmount = false;
                        item.isSelected = false;
                        item.tooltipPopUp = false;
                        vh.SelectAccountView.Checked = false;
                        CheckChanged(this, -2);
                    }
                    else
                    {
                        vh.AmountLabel.Error = "";
                        vh.AmountLabel.ErrorEnabled = false;
                        vh.AmountLabel.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomHintLarge : Resource.Style.TextInputLayoutBottomHint);


                        vh.Amount.SetTextColor(new Color(ContextCompat.GetColor(mActicity, Resource.Color.tunaGrey)));
                        item.isValidAmount = true;
                        item.amount = newAmount;
                        item.tooltipPopUp = tooltipShow;
                        int selectedPosition = GetSelectedAccounts().FindIndex(x => x.accountNumber == item.accountNumber);
                        if (selectedPosition == -1)
                        {
                            CheckChanged(this, -2);
                        }
                        else
                        {
                            CheckChanged(this, selectedPosition);
                        }
                    }
                }
                else
                {
                    vh.AmountLabel.Error = Utility.GetLocalizedLabel("Error", "minimumPayAmount");
                    vh.AmountLabel.ErrorEnabled = true;
                    vh.AmountLabel.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutBottomErrorHintLarge : Resource.Style.TextInputLayoutBottomErrorHint);

                    vh.Amount.RequestFocus();
                    item.isValidAmount = false;
                    item.isSelected = false;
                    item.tooltipPopUp = false;
                    vh.SelectAccountView.Checked = false;
                    CheckChanged(this, -2);
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
            public TextView ShowMore { get; private set; }

            public SelectAccountListViewHolder(View itemView, Action<SelectAccountListViewHolder, int> listener) : base(itemView)
            {
                AccountLabel = itemView.FindViewById<TextView>(Resource.Id.text_account_lable);
                AccountNumber = itemView.FindViewById<TextView>(Resource.Id.text_account_number);
                AccountAddress = itemView.FindViewById<TextView>(Resource.Id.text_account_address);
                AmountLabel = itemView.FindViewById<TextInputLayout>(Resource.Id.account_amount_layout);
                Amount = itemView.FindViewById<EditText>(Resource.Id.account_amount_edittext);
                ShowMore = itemView.FindViewById<TextView>(Resource.Id.show_more_btn);

                SelectAccountView = itemView.FindViewById<CheckBox>(Resource.Id.select_account);
                SelectAccountView.Click += (s, e) => listener((this), base.LayoutPosition);
                ShowMore.Click += (s, e) => ShowMoreAction();

                TextViewUtils.SetMuseoSans300Typeface(AccountNumber, AccountAddress);
                TextViewUtils.SetMuseoSans300Typeface(AmountLabel);
                TextViewUtils.SetTextSize12(AccountAddress);
                TextViewUtils.SetTextSize14(AccountNumber);
                TextViewUtils.SetTextSize16(AccountLabel);
                AmountLabel.SetHintTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.AmountHint_TextInputLayout_TextAppearance_LightBackgroundLarge : Resource.Style.AmountHint_TextInputLayout_TextAppearance_LightBackground);

                Amount.AddTextChangedListener(new RestrictAmountChangeListener(Amount, AmountLabel, 2));
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

        public void EnableShowMoreButton(bool isEnable)
        {
            IsShowMoreEnable = isEnable;
        }

        public void SetShowMoreAction(Action showMoreAction)
        {
            ShowMoreAction = showMoreAction;
        }
    }
}
