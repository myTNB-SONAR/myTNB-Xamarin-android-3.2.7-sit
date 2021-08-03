using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using myTNB.Mobile;
using myTNB.Mobile.AWS.Models;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DBR.DBRApplication.Adapter;
using myTNB_Android.Src.DeviceCache;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.DBR.DBRApplication.MVP
{
    [Activity(Label = "Select Electricity Account"
    , ScreenOrientation = ScreenOrientation.Portrait
    , Theme = "@style/Theme.Dashboard")]
    public class SelectDBRAccountActivity : BaseActivityCustom
    {
        List<DBRAccount> accountList = new List<DBRAccount>();
        private SelectAccountAdapter selectAccountAdapter;
        const string PAGE_ID = "SelectElectricityAccounts";
        string selectedAccountNumber;

        [BindView(Resource.Id.account_list_view)]
        ListView accountDBRList;

        [BindView(Resource.Id.noEligibleAccountContainer)]
        LinearLayout noEligibleAccountContainer;

        [BindView(Resource.Id.eligibleAccountListContainer)]
        LinearLayout eligibleAccountListContainer;

        [BindView(Resource.Id.noEligibleAccountMessage)]
        TextView noEligibleAccountMessage;

        public override int ResourceId()
        {
            return Resource.Layout.SelectDBRAccountLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void UpdateSelectedAccount(int position)
        {
            for (int i = 0; i < accountList.Count; i++)
            {
                if (i == position)
                {
                    accountList[i].accountSelected = true;
                }
                else
                {
                    accountList[i].accountSelected = false;
                }
            }

            UserSessions.SetRealDBREligibilityAccountList(accountList);
            Intent returnIntent = new Intent();
            returnIntent.PutExtra("SELECTED_ACCOUNT_NUMBER", accountList.Find(x => { return x.accountSelected; }).accountNumber);
            SetResult(Result.Ok, returnIntent);
            Finish();
        }
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetTheme(TextViewUtils.IsLargeFonts ? Resource.Style.Theme_DashboardLarge : Resource.Style.Theme_Dashboard);
            TextViewUtils.SetTextSize14(noEligibleAccountMessage);
            Bundle extras = Intent.Extras;
            if (extras != null && extras.ContainsKey("DBR_ELIGIBLE_ACCOUNT_LIST"))
            {
                try
                {
                    accountList = JsonConvert.DeserializeObject<List<DBRAccount>>(extras.GetString("DBR_ELIGIBLE_ACCOUNT_LIST"));
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
            if (extras != null && extras.ContainsKey("SELECTED_ACCOUNT_NUMBER"))
            {
                try
                {
                    selectedAccountNumber = JsonConvert.DeserializeObject<string>(extras.GetString("SELECTED_ACCOUNT_NUMBER"));
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            }
            

            if (accountList != null && accountList.Count > 0)
            {
                if (CustomerBillingAccount.HasSelected())
                {
                    int index = accountList.FindIndex(x => x.accountNumber == selectedAccountNumber);
                    if (index != -1)
                    {
                        accountList[index].accountSelected = true;
                    }
                    else
                    {
                        accountList[0].accountSelected = true;
                    }
                }
                else
                {
                    accountList[0].accountSelected = true;
                }
               
            }
            noEligibleAccountContainer.Visibility = ViewStates.Gone;
            eligibleAccountListContainer.Visibility = ViewStates.Visible;
            List<DBRAccount> newItemList = accountList.GetRange(0, accountList.Count);
            newItemList.Add(new DBRAccount()); //To show info item
            selectAccountAdapter = new SelectAccountAdapter(this, newItemList);
            accountDBRList.Adapter = selectAccountAdapter;

            accountDBRList.ItemClick += OnItemClick;
        }

        public class OnSelectAccountListener : Java.Lang.Object, RecyclerView.IOnClickListener
        {
            public void OnClick(View v)
            {
                throw new NotImplementedException();
            }
        }

        internal void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (e.Position == accountList.Count)//Handling Account list Info tooltip from list
            {
                MyTNBAppToolTipBuilder dbrTooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                 .SetTitle(Utility.GetLocalizedLabel("SelectElectricityAccounts", "accountsMissing"))
                   .SetMessage(Utility.GetLocalizedLabel("SelectElectricityAccounts", "dbrMissingAccountsMessage"))
                  .SetCTALabel(Utility.GetLocalizedCommonLabel("gotIt"))
                  .Build();

                dbrTooltip.Show();
            }
            else
            {
                for (int i = 0; i < accountList.Count; i++)
                {
                    if (i == e.Position)
                    {
                        accountList[i].accountSelected = true;
                    }
                    else
                    {
                        accountList[i].accountSelected = false;
                    }
                }
                GetBillRenderingAsync(accountList.Find(x => { return x.accountSelected; }).accountNumber);
                
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            try
            {
                FirebaseAnalyticsUtils.SetScreenName(this, "Select DBR Electricity Account");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
        private async void GetBillRenderingAsync(string accountNumber)
        {
            try
            {
                ShowProgressDialog();
                if (!AccessTokenCache.Instance.HasTokenSaved(this))
                {
                    string accessToken = await AccessTokenManager.Instance.GenerateAccessToken(UserEntity.GetActive().UserID ?? string.Empty);
                    AccessTokenCache.Instance.SaveAccessToken(this, accessToken);
                }
                GetBillRenderingResponse response = await DBRManager.Instance.GetBillRendering(accountNumber, AccessTokenCache.Instance.GetAccessToken(this));

                HideProgressDialog();
                //Nullity Check
                if (response != null
                   && response.StatusDetail != null
                   && response.StatusDetail.IsSuccess)
                {
                    Intent returnIntent = new Intent();
                    returnIntent.PutExtra("billrenderingresponse", JsonConvert.SerializeObject(response));
                    returnIntent.PutExtra("SELECTED_ACCOUNT_NUMBER", accountList.Find(x => { return x.accountSelected; }).accountNumber);
                    SetResult(Result.Ok, returnIntent);
                    Finish();
                }
                else
                {
                    MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                                        .SetTitle(response.StatusDetail.Title)
                                        .SetMessage(response.StatusDetail.Message)
                                        .SetCTALabel(response.StatusDetail.PrimaryCTATitle)
                                        .Build();
                    errorPopup.Show();
                }

            }
            catch (System.Exception e)
            {
                HideProgressDialog();
                Utility.LoggingNonFatalError(e);
            }
        }
        protected override void OnPause()
        {
            base.OnPause();
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }
        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}