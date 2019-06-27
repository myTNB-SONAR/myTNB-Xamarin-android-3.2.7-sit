using AFollestad.MaterialDialogs;
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

        private MaterialDialog mWhyThisAmtCardDialog;

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
            try
            {
                accountList = accountList.Concat(_accountList).ToList();
                //this.NotifyDataSetChanged();
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
                        //if (!string.IsNullOrEmpty(vh.Amount.Text))
                        //{
                            ValidateHolder(item, position, vh);
                        //}
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

        public void ValidateHolder(MPAccount item, int position, SelectAccountListViewHolder vh)
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
                        if(item.isSelected && !item.isTooltipShow && item.OpenChargeTotal != 0)
                        {
                            ShowTooltip(item);
                            item.isTooltipShow = true;
                        }
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

        public void ShowTooltip(MPAccount item)
        {
            try
            {
                mWhyThisAmtCardDialog = new MaterialDialog.Builder(mActicity)
                    .CustomView(Resource.Layout.CustomDialogDoubleButtonLayout, false)
                    .Cancelable(false)
                    .CanceledOnTouchOutside(false)
                    .Build();

                View dialogView = mWhyThisAmtCardDialog.Window.DecorView;
                dialogView.SetBackgroundResource(Android.Resource.Color.Transparent);

                TextView txtItemizedTitle = mWhyThisAmtCardDialog.FindViewById<TextView>(Resource.Id.txtTitle);
                TextView txtItemizedMessage = mWhyThisAmtCardDialog.FindViewById<TextView>(Resource.Id.txtMessage);
                TextView btnGotIt = mWhyThisAmtCardDialog.FindViewById<TextView>(Resource.Id.txtBtnSecond);
                TextView btnBringMeThere = mWhyThisAmtCardDialog.FindViewById<TextView>(Resource.Id.txtBtnFirst);
                txtItemizedMessage.MovementMethod = new ScrollingMovementMethod();
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    txtItemizedMessage.TextFormatted = string.IsNullOrEmpty(item.MandatoryChargesMessage) ? Html.FromHtml(mActicity.GetString(Resource.String.itemized_bill_third_message), FromHtmlOptions.ModeLegacy) : Html.FromHtml(item.MandatoryChargesMessage, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    txtItemizedMessage.TextFormatted = string.IsNullOrEmpty(item.MandatoryChargesMessage) ? Html.FromHtml(mActicity.GetString(Resource.String.itemized_bill_third_message)) : Html.FromHtml(item.MandatoryChargesMessage);
                }
                txtItemizedTitle.Text = string.IsNullOrEmpty(item.MandatoryChargesTitle) ? mActicity.GetString(Resource.String.itemized_bill_third_title) : item.MandatoryChargesTitle;
                btnGotIt.Text = string.IsNullOrEmpty(item.MandatoryChargesSecButtonText) ? mActicity.GetString(Resource.String.itemized_bill_got_it) : item.MandatoryChargesSecButtonText;
                btnBringMeThere.Text = string.IsNullOrEmpty(item.MandatoryChargesPriButtonText) ? mActicity.GetString(Resource.String.itemized_bill_bring_me_there) : item.MandatoryChargesPriButtonText;
                TextViewUtils.SetMuseoSans500Typeface(txtItemizedTitle, btnGotIt, btnBringMeThere);
                TextViewUtils.SetMuseoSans300Typeface(txtItemizedMessage);
                btnGotIt.Click += delegate
                {
                    mWhyThisAmtCardDialog.Dismiss();
                };
                btnBringMeThere.Click += delegate
                {
                    try
                    {
                        ((SelectAccountsActivity)mActicity).NavigateBillScreen(item);
                    }
                    catch (System.Exception e)
                    {
                        Utility.LoggingNonFatalError(e);
                    }
                    mWhyThisAmtCardDialog.Dismiss();
                    //Intent dashbaord_activity = new Intent(mActicity, typeof(DashboardActivity));
                    //dashbaord_activity.PutExtra(Constants.ITEMZIED_BILLING_VIEW_KEY, true);
                    //dashbaord_activity.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                    //mActicity.StartActivity(dashbaord_activity);
                    //mWhyThisAmtCardDialog.Dismiss();
                };

                mWhyThisAmtCardDialog.Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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