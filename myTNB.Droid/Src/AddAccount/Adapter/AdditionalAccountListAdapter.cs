using Android.Content;
using Android.Graphics;
using Android.Preferences;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.TextField;
using myTNB_Android.Src.AddAccount.Activity;
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
                OwnerNoContactLinearLayout.RequestFocus();
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
                OwnerNoContactLinearLayout.RequestFocus();
                MobileLinearLayout.Visibility = ViewStates.Visible;
                textInputLayoutEmailEditText.Visibility = ViewStates.Gone;
                EmailEditText.Visibility = ViewStates.Gone;
                NoMobileFieldDetail.SetBackgroundResource(Resource.Drawable.rectangle_rounded_corner_dark_blue_bg);
                NoMobileFieldDetail.SetTextColor(Color.ParseColor("#ffffff"));
                EmailFieldDetail.SetBackgroundResource(Resource.Drawable.rectangle_rounded_corner_blue_outline);
                EmailFieldDetail.SetTextColor(Color.ParseColor("#1c79ca"));
            };



            NoMobileFieldDetail.Text = Utility.GetLocalizedCommonLabel("mobileNumber");
            EmailFieldDetail.Text = Utility.GetLocalizedCommonLabel("emailAddress");
            textInputLayoutAccountLabel.Hint = Utility.GetLocalizedCommonLabel("acctNickname");
            textInputLayoutAccountLabel.SetHintTextAppearance(TextViewUtils.IsLargeFonts
                    ? Resource.Style.TextInputLayout_TextAppearance_Large
                    : Resource.Style.TextInputLayout_TextAppearance_Small);
            textInputLayoutAccountLabel.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
                ? Resource.Style.TextInputLayoutFeedbackCountLarge
                : Resource.Style.TextInputLayoutFeedbackCount);
            context = itemView.Context;
            AccountLabel.AddTextChangedListener(new InputFilterFormField(AccountLabel, textInputLayoutAccountLabel));
           

            TextViewUtils.SetTextSize16(AccountLabel, EmailFieldDetail, NoMobileFieldDetail);
            TextViewUtils.SetTextSize14(AccountNumber, OwnerDetailTitle);
            TextViewUtils.SetTextSize12(AccountAddress);

            OwnerDetailTitle.Text = Utility.GetLocalizedLabel("AddAccount", "titleOwnerDetailRegion");
            textInputLayoutEmailEditText.SetErrorTextAppearance(TextViewUtils.IsLargeFonts ? Resource.Style.TextInputLayoutFeedbackCountLarge : Resource.Style.TextInputLayoutFeedbackCount);

            // TextViewUtils.SetTextSize12(AccountAddress);
            // TextViewUtils.SetTextSize14(AccountNumber);
            // TextViewUtils.SetTextSize16(AccountLabel);

            //TextViewUtils.SetMuseoSans300Typeface(AccountNumber, AccountAddress, AccountLabel, OwnerDetailTitle);
            //TextViewUtils.SetMuseoSans500Typeface(EmailFieldDetail, NoMobileFieldDetail);
            //TextViewUtils.SetMuseoSans300Typeface(textInputLayoutAccountLabel, textInputLayoutEmailEditText);

            //context = itemView.Context;
            //AccountLabel.AddTextChangedListener(new InputFilterFormField(AccountLabel, textInputLayoutAccountLabel));
            EmailEditText.FocusChange += txtEmailReg_FocusChange;
            EmailEditText.AddTextChangedListener(new InputFilterFormField(EmailEditText, textInputLayoutEmailEditText));
            EmailEditText.TextChanged += TxtEmailReg_TextChanged;
            EmailEditText.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.placeholder_email_new, 0, 0, 0);

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
        }

        public void OnTapCountryCode()
        {
            item.CountryCheck = true;
            item.CountryCheckNoPhone = true;
            PreferenceManager.GetDefaultSharedPreferences(context).Edit().Remove("selectedcountry").Apply();
            context.StartActivity(new Intent(context, typeof(SelectCountryActivity)));
        }

        private void txtEmailReg_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (!e.HasFocus)
            {
                string email = EmailEditText.Text.ToString().Trim();

                try
                {
                    bool isCorrect = true;

                    ShowInvalidEmailError();

                    if (!string.IsNullOrEmpty(email))
                    {

                        if (!Patterns.EmailAddress.Matcher(email).Matches())
                        {
                            ShowInvalidEmailError();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utility.LoggingNonFatalError(ex);
                }
            }
        }

        private void TxtEmailReg_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ShowInvalidEmailError();

            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void ShowInvalidEmailError()
        {
            textInputLayoutEmailEditText.SetErrorTextAppearance(TextViewUtils.IsLargeFonts
               ? Resource.Style.TextInputLayoutBottomErrorHintLarge
               : Resource.Style.TextInputLayoutBottomErrorHint);

            if (!string.IsNullOrEmpty(EmailEditText.Text))
            {
                string email = EmailEditText.Text.ToString().Trim();

                if (!Patterns.EmailAddress.Matcher(email).Matches())
                {
                   
                    textInputLayoutEmailEditText.SetErrorTextAppearance(Resource.Style.TextInputLayoutBottomErrorHint);

                    if (textInputLayoutEmailEditText.Error != Utility.GetLocalizedLabel("RegisterNew", "invalidEmailTryAgain"))
                    {
                        textInputLayoutEmailEditText.Error = Utility.GetLocalizedLabel("RegisterNew", "invalidEmailTryAgain");  // fix bouncing issue
                    }

                    textInputLayoutEmailEditText.RequestLayout();
                    //((LinkAccountActivity)context).DisableConfirmButton();
                }
                else
                {
                    ClearEmailError();
                    //((LinkAccountActivity)context).EnableConfirmButton();
                }
            }
        }

        public void ClearEmailError()
        {
            if (!string.IsNullOrEmpty(textInputLayoutEmailEditText.Error))
            {
                textInputLayoutEmailEditText.Error = null;
                //textInputLayoutEmailEditText.ErrorEnabled = false;
            }
            else
            {
                textInputLayoutEmailEditText.Error = null;
                //textInputLayoutEmailEditText.ErrorEnabled = false;
            }

        }


        public void checkingEmailnPhone()
        {
            ((LinkAccountActivity)context).CheckingEmailAndPhoneNo();
        }

        private void OnValidateMobileNumber(bool isValidated)
        {
            string ISD = mobileNumberInputComponent.GetISDOnly();
            string validatedMobileNumber = "";
            if (mobileNumberInputComponent.GetMobileNumberReset() && !isValidated)
            {
                if (this.item.mobileNoOwner.Length == 2)
                {
                    validatedMobileNumber = this.item.mobileNoOwner.Substring(0, 2);
                }
                else if (this.item.mobileNoOwner.Length == 3)
                {
                    validatedMobileNumber = this.item.mobileNoOwner.Substring(0, 3);
                }
                else if (this.item.mobileNoOwner.Length > 3)
                {
                    validatedMobileNumber = this.item.mobileNoOwner.Substring(0, 3);
                }

                if (validatedMobileNumber != ISD && item.CountryCheckNoPhone)
                {
                    item.CountryCheckNoPhone = false;
                    this.item.mobileNoOwner = "";
                }
                else if (!validatedMobileNumber.Equals("") && validatedMobileNumber.Substring(0, 2) == ISD.Substring(0, 2))
                {
                    string MobileNumber = this.item.mobileNoOwner.Substring(ISD.Length, this.item.mobileNoOwner.Length - ISD.Length);
                    if (!MobileNumber.Equals("") && MobileNumber.Length == 1)
                    {
                        this.item.mobileNoOwner = "";
                    }
                }
            }
            else if (!mobileNumberInputComponent.GetMobileNumberReset() && !isValidated && item.CountryCheckNoPhone)
            {
                item.CountryCheckNoPhone = false;
                this.item.mobileNoOwner = "";
            }
            else if (isValidated && this.item.mobileNoOwner != this.item.ISDmobileNo)
            {
                this.item.ISDmobileNo = mobileNumberInputComponent.GetISDOnly();
                this.item.mobileNoOwner = mobileNumberInputComponent.GetMobileNumberValueWithISDCode();
            }
            checkingEmailnPhone();
        }

        public void PopulateData(NewAccount item)
        {
            this.item = item;
            int noISDONly;
            try
            {
                AccountNumber.Text = this.item.accountNumber;
                AccountAddress.Text = this.item.accountAddress;

                //if not owner mask the address IRUL
                //if (!this.item.isOwner == true) {
                //    AccountAddress.Text = Utility.StringSpaceMasking(Utility.Masking.Address, this.item.accountAddress);
                //    //AccountAddress.Text = Utility.StringMasking(Utility.Masking.Address, this.item.accountAddress);
                //}
                //else
                //{
                //    AccountAddress.Text = this.item.accountAddress;
                //}


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
                    //item.accountLabel = AccountLabel.Text.Trim();
                    if (!string.IsNullOrEmpty(item.accountLabel))
                    {
                        textInputLayoutAccountLabel.Error = Utility.GetLocalizedHintLabel("nickname");
                    }
                    else
                    {
                        textInputLayoutAccountLabel.Error = Utility.GetLocalizedHintLabel("nickname");
                    }
                };

                EmailEditText.Text = this.item.emailOwner;
                EmailEditText.AfterTextChanged += (sender, args) =>
                {
                    item.emailOwner = EmailEditText.Text.Trim();
                    checkingEmailnPhone();
                };

                //checking and display non-onwer layout
                if (item.isNoDetailOwner && item.type.Equals("1"))
                {
                    OwnerNoContactLinearLayout.Visibility = ViewStates.Visible;
                }
                else
                {
                    OwnerNoContactLinearLayout.Visibility = ViewStates.Gone;
                }

                //checking and display country per data

                string selectedcountry = UserSessions.GetSelectedCountry(PreferenceManager.GetDefaultSharedPreferences(context));
                if (item.CountryCheck && selectedcountry != null)
                {
                    item.countryDetail = selectedcountry;
                    item.CountryCheck = false;
                    Country selectedCountry = JsonConvert.DeserializeObject<Country>(selectedcountry);
                    this.item.ISDmobileNo = selectedCountry.isd;
                    mobileNumberInputComponent.SetSelectedCountry(selectedCountry);

                }
                else if (!item.countryDetail.Equals(""))
                {
                    Country selectedCountry = JsonConvert.DeserializeObject<Country>(item.countryDetail);
                    this.item.ISDmobileNo = selectedCountry.isd;
                    mobileNumberInputComponent.SetSelectedCountry(selectedCountry);
                }

                noISDONly = this.item.ISDmobileNo.Length;
                if (!item.mobileNoOwner.Equals("") && item.isNoDetailOwner && item.type.Equals("1"))
                {
                    string mobile = this.item.mobileNoOwner;
                    string mobileNoOnly = mobile.Substring(noISDONly, mobile.Length - noISDONly);
                    if (mobileNoOnly.Length > 0)
                    {
                        int value = Java.Lang.Integer.ParseInt(mobileNoOnly);
                        mobileNumberInputComponent.SetMobileNumber(value);
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}