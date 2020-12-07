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
    public class AddAccountTypeFragmentNew : AndroidX.Fragment.App.Fragment 
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

        TextView txtTitlePremise;
        TextView txtTitle;
        TextView txtYes;
        TextView txtNo;
        TextView txtOwnerRights;
        TextView txtNonOwnerRights;
        TextView txtSkipAcc;

        private bool isClicked = false;
        private bool fromRegisterPage = false;

        public AddAccountTypeFragmentNew()
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
                fromRegisterPage = bundle.GetBoolean("fromRegisterPage");
            }

            View rootView = inflater.Inflate(Resource.Layout.AddAccountTypeViewNew, container, false);
            radio_non_owner = rootView.FindViewById<LinearLayout>(Resource.Id.btnNonOwner);
            radio_owner = rootView.FindViewById<LinearLayout>(Resource.Id.btnOwner);
            skip_add_acc = rootView.FindViewById<LinearLayout>(Resource.Id.layoutSkipAcc);

            txtAccountType = rootView.FindViewById<TextView>(Resource.Id.txtAccountType);
            accountType = rootView.FindViewById<TextView>(Resource.Id.selector_account_type);
            txtTitle = rootView.FindViewById<TextView>(Resource.Id.txtTitle);
            txtOwnerRights = rootView.FindViewById<TextView>(Resource.Id.txtOwnerConstrain);
            txtNonOwnerRights = rootView.FindViewById<TextView>(Resource.Id.txtNonOwnerConstrain);
            txtYes = rootView.FindViewById<TextView>(Resource.Id.txtYes);
            txtNo = rootView.FindViewById<TextView>(Resource.Id.txtNo);
            txtTitlePremise = rootView.FindViewById<TextView>(Resource.Id.txtTitlePremise);
            txtSkipAcc = rootView.FindViewById<TextView>(Resource.Id.txtSkipAcc);

            TextViewUtils.SetMuseoSans500Typeface(txtYes, txtNo, txtTitle);
            TextViewUtils.SetMuseoSans300Typeface(txtOwnerRights, txtNonOwnerRights, txtTitlePremise, accountType);

            txtTitlePremise.Text = Utility.GetLocalizedLabel("AddAccount", "AccHeaderText");
            txtTitle.Text = Utility.GetLocalizedLabel("AddAccount", "AccHeaderBlueText");
            txtOwnerRights.Text = Utility.GetLocalizedLabel("AddAccount", "ownerDetails");
            txtNonOwnerRights.Text = Utility.GetLocalizedLabel("AddAccount", "nonOwnerDetails");
            txtYes.Text = Utility.GetLocalizedLabel("AddAccount", "owner");
            txtNo.Text = Utility.GetLocalizedLabel("AddAccount", "nonOwner");
            txtSkipAcc.Text = Utility.GetLocalizedLabel("AddAccount", "skip");
            txtAccountType.Text = Utility.GetLocalizedLabel("AddAccount", "PremisesHint").ToUpper();

            AccountType Individual = new AccountType();
            Individual.Id = "1";
            Individual.Type = Utility.GetLocalizedLabel("AddAccount", "residential");
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

            radio_non_owner.Click += delegate
            {
                Bundle bundle = new Bundle();
                bundle.PutBoolean("isNonOwner", true);
                ((AddAccountActivity)Activity).nextFragment(this, bundle);
            };

            radio_owner.Click += delegate
            {
                Bundle bundle = new Bundle();
                bundle.PutBoolean("isOwner", true);
                ((AddAccountActivity)Activity).nextFragment(this, bundle);
            };


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
                            if (selectedAccountType.Id.Equals("2"))
                            {
                                Bundle bundle = new Bundle();
                                bundle.PutBoolean("isCommercial", true);
                                bundle.PutBoolean("fromRegisterPage", fromRegisterPage);
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