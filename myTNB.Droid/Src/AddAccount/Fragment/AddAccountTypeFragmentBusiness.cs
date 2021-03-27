using Android.OS;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.AddAccount.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Database.Model;
using System;
using myTNB_Android.Src.AddAccount.Models;
using CheeseBind;
using Android.Content;
using Newtonsoft.Json;
using myTNB_Android.Src.myTNBMenu.Activity;
using Android.App;

namespace myTNB_Android.Src.AddAccount.Fragment
{
    public class AddAccountTypeFragmentBusiness : AndroidX.Fragment.App.Fragment 
    {
        private AccountType selectedAccountType;
        private readonly int SELECT_ACCOUNT_TYPE_REQ_CODE = 2011;

        [BindView(Resource.Id.txtAccountType)]
        TextView txtAccountType;

        [BindView(Resource.Id.selector_account_type)]
        TextView accountType;

        LinearLayout radio_non_owner;
        LinearLayout radio_owner;
        LinearLayout skip_add_acc;
        Button btnNextAccAccount;

        TextView txtTitle;
        TextView txtYes;
        TextView txtNo;
        TextView txtOwnerRights;
        TextView txtNonOwnerRights;

        private bool isClicked = false;
        private bool fromRegisterPage = false;

        public AddAccountTypeFragmentBusiness()
        {

        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (Arguments != null)
            {
                Bundle bundle = this.Arguments;
                fromRegisterPage = bundle.GetBoolean("fromRegisterPage", true);
            }

            View rootView = inflater.Inflate(Resource.Layout.AddAccountTypeViewNewBusiness, container, false);
            radio_non_owner = rootView.FindViewById<LinearLayout>(Resource.Id.btnNonOwner);
            radio_owner = rootView.FindViewById<LinearLayout>(Resource.Id.btnOwner);
            skip_add_acc = rootView.FindViewById<LinearLayout>(Resource.Id.layoutSkipAcc);
            btnNextAccAccount = rootView.FindViewById<Button>(Resource.Id.btnAddAnotherAccount);

            txtAccountType = rootView.FindViewById<TextView>(Resource.Id.txtAccountType);
            accountType = rootView.FindViewById<TextView>(Resource.Id.selector_account_type);
            txtTitle = rootView.FindViewById<TextView>(Resource.Id.txtTitle);
            txtOwnerRights = rootView.FindViewById<TextView>(Resource.Id.txtOwnerConstrain);
            txtNonOwnerRights = rootView.FindViewById<TextView>(Resource.Id.txtNonOwnerConstrain);
            txtYes = rootView.FindViewById<TextView>(Resource.Id.txtYes);
            txtNo = rootView.FindViewById<TextView>(Resource.Id.txtNo);

            txtAccountType.Text = Utility.GetLocalizedLabel("Common", "accountType").ToUpper();

            AccountType Individual = new AccountType();
            Individual.Id = "2";
            Individual.Type = Utility.GetLocalizedLabel("AddAccount", "business");
            Individual.IsSelected = true;
            selectedAccountType = Individual;
            accountType.Text = selectedAccountType.Type;
            accountType.Click += async delegate
            {
                if (!isClicked)
                {
                    isClicked = true;
                    Intent accountType = new Intent(Activity, typeof(SelectAccountActivity));
                    accountType.PutExtra("selectedAccountType", JsonConvert.SerializeObject(selectedAccountType));
                    StartActivityForResult(accountType, SELECT_ACCOUNT_TYPE_REQ_CODE);
                }
            };

            if (fromRegisterPage)
            {
                fromRegisterPage = true;
                skip_add_acc.Visibility = ViewStates.Visible;
            }
            else
            {
                skip_add_acc.Visibility = ViewStates.Gone;
            }

            skip_add_acc.Click += delegate
            {
                Bundle bundle = new Bundle();
                bundle.PutBoolean("fromRegister", true);
                ((AddAccountActivity)Activity).nextFragment(this, bundle);
            };

            /*TextViewUtils.SetMuseoSans500Typeface(txtYes, txtNo, txtTitle);
            TextViewUtils.SetMuseoSans300Typeface(txtOwnerRights, txtNonOwnerRights);

            txtTitle.Text = Utility.GetLocalizedLabel("AddAccount", "addByIDMessage");
            txtOwnerRights.Text = Utility.GetLocalizedLabel("AddAccount", "addAsOwnerMessage");
            txtNonOwnerRights.Text = Utility.GetLocalizedLabel("AddAccount", "addAsTenantMessage");
            txtYes.Text = Utility.GetLocalizedLabel("Common", "yes") + ".";
            txtNo.Text = Utility.GetLocalizedLabel("Common", "no") + ".";*/
            return rootView;
        }


        private string GetHtmlText(String text)
        {
            if (((int)Build.VERSION.SdkInt) >= 24)
            {
                return text;
            }
            else
            {
                return text;
            }

        }

        public override void OnResume()
        {
            base.OnResume();
            isClicked = false;
        }

        public override void OnPause()
        {
            base.OnPause();
            isClicked = true;
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                if (requestCode == Constants.BARCODE_REQUEST_CODE)
                {
                    if (resultCode == (int)Result.Ok)
                    {

                    }
                }
                else if (requestCode == SELECT_ACCOUNT_TYPE_REQ_CODE)
                {

                    if (resultCode == (int)Result.Ok)
                    {
                        selectedAccountType = JsonConvert.DeserializeObject<AccountType>(data.GetStringExtra("selectedAccountType"));
                        if (selectedAccountType != null)
                        {
                            accountType.Text = selectedAccountType.Type;
                            if (selectedAccountType.Id.Equals("1"))
                            {
                                Bundle bundle = new Bundle();
                                bundle.PutBoolean("isResidential", true);
                                ((AddAccountActivity)Activity).nextFragment(this, bundle);
                            }
                        }
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