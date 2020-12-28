using Android.Content;
using Android.Graphics;
using Android.Preferences;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.TextField;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Common.Activity;
using myTNB_Android.Src.Common.Model;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
        public TextView OwnerDetailTitle { get; private set; }                          //module owner tenant add
        public TextView EmailFieldDetail { get; private set; }
        public TextView NoMobileFieldDetail { get; private set; }
        public LinearLayout MobileLinearLayout { get; private set; }
        public LinearLayout OwnerNoContactLinearLayout { get; private set; }
        public EditText EmailEditText { get; private set; }
        public TextInputLayout textInputLayoutEmailEditText { get; private set; }

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

            //adding owner no contact part
            EmailEditText = itemView.FindViewById<EditText>(Resource.Id.txtEmailReg);
            OwnerDetailTitle = itemView.FindViewById<TextView>(Resource.Id.textOwnerDetail);
            EmailFieldDetail = itemView.FindViewById<TextView>(Resource.Id.btnEmailAdress);
            NoMobileFieldDetail = itemView.FindViewById<TextView>(Resource.Id.btnMobileNumber);
            MobileLinearLayout = itemView.FindViewById<LinearLayout>(Resource.Id.mobileNumberFieldContainer);
            OwnerNoContactLinearLayout = itemView.FindViewById<LinearLayout>(Resource.Id.layout_owner_no_contact);
            textInputLayoutEmailEditText = itemView.FindViewById<TextInputLayout>(Resource.Id.textInputLayoutEmailReg);
            
            EmailFieldDetail.Click += (sender, e) => 
            {
                MobileLinearLayout.Visibility = ViewStates.Gone;
                textInputLayoutEmailEditText.Visibility = ViewStates.Visible;
                EmailEditText.Visibility = ViewStates.Visible;
                NoMobileFieldDetail.SetBackgroundResource(Resource.Drawable.rectangle_rounded_corner_blue_outline);
                NoMobileFieldDetail.SetTextColor(Color.ParseColor("#1c79ca"));
                EmailFieldDetail.SetBackgroundResource(Resource.Drawable.rectangle_rounded_corner_dark_blue_bg);
                EmailFieldDetail.SetTextColor(Color.ParseColor("#ffffff"));
            };

            NoMobileFieldDetail.Click += (sender, e) =>
            {
                MobileLinearLayout.Visibility = ViewStates.Visible;
                textInputLayoutEmailEditText.Visibility = ViewStates.Gone;
                EmailEditText.Visibility = ViewStates.Gone;
                NoMobileFieldDetail.SetBackgroundResource(Resource.Drawable.rectangle_rounded_corner_dark_blue_bg);
                NoMobileFieldDetail.SetTextColor(Color.ParseColor("#ffffff"));
                EmailFieldDetail.SetBackgroundResource(Resource.Drawable.rectangle_rounded_corner_blue_outline);
                EmailFieldDetail.SetTextColor(Color.ParseColor("#1c79ca"));
            };


            TextViewUtils.SetMuseoSans300Typeface(AccountNumber, AccountAddress, AccountLabel, OwnerDetailTitle);
            TextViewUtils.SetMuseoSans500Typeface(EmailFieldDetail, NoMobileFieldDetail);
            TextViewUtils.SetMuseoSans300Typeface(textInputLayoutAccountLabel);

            textInputLayoutAccountLabel.Hint = Utility.GetLocalizedCommonLabel("acctNickname");
            context = itemView.Context;
            AccountLabel.AddTextChangedListener(new InputFilterFormField(AccountLabel, textInputLayoutAccountLabel));

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

            MobileLinearLayout.RemoveAllViews();
            mobileNumberInputComponent = new MobileNumberInputComponent(context);
            mobileNumberInputComponent.SetOnTapCountryCodeAction(OnTapCountryCode);
            mobileNumberInputComponent.SetMobileNumberLabel(Utility.GetLocalizedCommonLabel("mobileNo"));
            mobileNumberInputComponent.SetSelectedCountry(CountryUtil.Instance.GetDefaultCountry());
            mobileNumberInputComponent.SetValidationAction(OnValidateMobileNumber);
            MobileLinearLayout.AddView(mobileNumberInputComponent);
           
            string selectedcountry = UserSessions.GetSelectedCountry(PreferenceManager.GetDefaultSharedPreferences(context));
            if (selectedcountry != null)
            {
                Country selectedCountry = JsonConvert.DeserializeObject<Country>(selectedcountry);
                mobileNumberInputComponent.SetSelectedCountry(selectedCountry);
            }

        }

        public void OnTapCountryCode()
        {
            context.StartActivity(new Intent(context, typeof(SelectCountryActivity)));
        }

        private void OnValidateMobileNumber(bool isValidated)
        {
            //string mobile = this.item.mobileNoOwner;
            //int value = Java.Lang.Integer.ParseInt(mobile);
            //mobileNumberInputComponent.SetMobileNumber(value);
            string noISDMobileNo = mobileNumberInputComponent.GetMobileNumberValue();
            phone_no = mobileNumberInputComponent.GetMobileNumberValueWithISDCode();
            if(!noISDMobileNo.Equals(""))
            {
                this.item.mobileNoOwner = phone_no;
            }
        }

        public void PopulateData(NewAccount item)
        {
            this.item = item;
            try
            {
                AccountNumber.Text = this.item.accountNumber;
                AccountAddress.Text = Utility.StringMasking(Utility.Masking.Address, this.item.accountAddress);
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

                EmailEditText.AfterTextChanged += (sender, args) =>
                {
                    item.emailOwner = EmailEditText.Text.Trim();
                    if (!Patterns.EmailAddress.Matcher(item.emailOwner).Matches())
                    {
                        textInputLayoutEmailEditText.Error = Utility.GetLocalizedErrorLabel("invalid_email");
                    }
                    else
                    {
                        textInputLayoutEmailEditText.Error = null;
                    }
                };

                if (item.isNoDetailOwner)
                {
                    OwnerNoContactLinearLayout.Visibility = ViewStates.Visible;
                }
                else
                {
                    OwnerNoContactLinearLayout.Visibility = ViewStates.Gone;
                }

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}