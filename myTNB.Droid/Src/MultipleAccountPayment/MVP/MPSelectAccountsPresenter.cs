using Android.Util;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MultipleAccountPayment.Api;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using myTNB_Android.Src.MultipleAccountPayment.Requests;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace myTNB_Android.Src.MultipleAccountPayment.MVP
{
    public class MPSelectAccountsPresenter : MPSelectAccountsContract.IUserActionsListener
    {
        private static readonly string TAG = "MPSelectAccountsPresenter";
        private MPSelectAccountsContract.IView mView;

        public MPSelectAccountsPresenter(MPSelectAccountsContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

        public void Start()
        {

        }

        public void GetMultiAccountDueAmount(string apiKeyID, List<string> accounts)
        {
            ServicePointManager.ServerCertificateValidationCallback += SSLFactoryHelper.CertificateValidationCallBack;
            GetMultiAccountDueAmountAsync(apiKeyID, accounts);
        }

        public async void GetMultiAccountDueAmountAsync(string apiKeyId, List<string> accounts)
        {
            try
            {
                //if (mView.IsActive()) {
                this.mView.ShowProgressDialog();
                //}

#if DEBUG || STUB
                var httpClient = new HttpClient(new HttpLoggingHandler(/*new NativeMessageHandler()*/)) { BaseAddress = new Uri(Constants.SERVER_URL.END_POINT) };
                var api = RestService.For<MPGetAccountsDueAmountApi>(httpClient);
#else
            var api = RestService.For<MPGetAccountsDueAmountApi>(Constants.SERVER_URL.END_POINT);
#endif
                //var api = RestService.For<GetRegisteredCardsApi>(Constants.SERVER_URL.END_POINT);

                List<MPAccount> storeAccounts = new List<MPAccount>();
                bool getDetailsFromApi = false;
                foreach (string account in accounts)
                {
                    if (!SelectBillsEntity.IsSMDataUpdated(account))
                    {
                        SelectBillsEntity storedEntity = SelectBillsEntity.GetItemByAccountNo(account);
                        if (storedEntity != null)
                        {
                            MPAccount storedSMData = JsonConvert.DeserializeObject<MPAccount>(storedEntity.JsonResponse);
                            storeAccounts.Add(storedSMData);
                        }
                        else
                        {
                            getDetailsFromApi = true;
                            break;
                        }
                    }
                    else
                    {
                        getDetailsFromApi = true;
                        break;
                    }
                }

                if (getDetailsFromApi)
                {
                    MPAccountDueResponse result = await api.GetMultiAccountDueAmount(new MPGetAccountDueAmountRequest(apiKeyId, accounts));
                    //if (mView.IsActive())
                    //{
                    this.mView.HideProgressDialog();
                    //}
                    if (result.accountDueAmountResponse != null && !result.accountDueAmountResponse.IsError)
                    {
                        this.mView.GetAccountDueAmountResult(result);
                    }
                    else
                    {
                        this.mView.ShowError(result.accountDueAmountResponse.Message);
                        this.mView.DisablePayButton();
                    }
                }
                else
                {
                    //if (mView.IsActive())
                    //{
                    this.mView.HideProgressDialog();
                    //}
                    this.mView.GetAccountDueAmountResult(storeAccounts);
                }
            }
            catch (Exception e)
            {
                Log.Debug(TAG, e.StackTrace);
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                Utility.LoggingNonFatalError(e);
                this.mView.ShowError("Something went wrong, Please try again.");
            }

        }
    }
}