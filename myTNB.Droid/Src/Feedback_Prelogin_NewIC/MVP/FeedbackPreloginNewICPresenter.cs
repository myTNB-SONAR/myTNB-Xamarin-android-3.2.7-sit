using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Telephony;
using Android.Text;
using Castle.Core.Internal;
using Java.Text;
using myTNB;
using myTNB_Android.Src.Base.Api;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using static myTNB.LanguageManager;
using static myTNB_Android.Src.Feedback_Prelogin_NewIC.Activity.FeedbackPreloginNewICActivity;

namespace myTNB_Android.Src.Feedback_Prelogin_NewIC.MVP
{
    public class FeedbackPreloginNewICPresenter : FeedbackPreloginNewICContract.IUserActionsListener
    {

        FeedbackPreloginNewICContract.IView mView;

        CustomerBillingAccount selectedCustomerBillingAccount;

        private ISharedPreferences mSharedPref;


        public FeedbackPreloginNewICPresenter(FeedbackPreloginNewICContract.IView mView, ISharedPreferences mSharedPref)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
            this.mSharedPref = mSharedPref;
        }

        public void OnGeneralEnquiry()
        {
            this.mView.ShowGeneralEnquiry();
        }

        public void OnAboutBillEnquiry()
        {
            this.mView.ShowAboutBillEnquiry();
        }

        public void onUpdatePersonalDetail()
        {
            this.mView.showUpdatePersonalDetail();
        }

        private void OnGSLRebate()
        {
            if (LanguageManager.Instance.GetConfigToggleValue(TogglePropertyEnum.IsGSLRebateEnabled))
            {
                this.mView.ShowGSLRebate();
            }
        }

        public void Start()
        {
        }

        public void OnSelectAccount()
        {
            try
            {
                if (CustomerBillingAccount.HasItems())
                {
                    this.mView.ShowSelectAccount();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public void onShowWhereIsMyAcc()
        {
            this.mView.ShowWhereIsMyAcc();
        }

        public void showScan()
        {
            this.mView.onScan();
        }

        public bool CheckRequiredFields(string accno)
        {
            try
            {

                bool allowToProceed = true;

                if (!TextUtils.IsEmpty(accno))
                {

                    if (!Utility.AddAccountNumberValidation(accno.Length))
                    {
                        this.mView.ShowInvalidAccountNumberError();
                        allowToProceed = false;

                    }
                    else
                    {
                        this.mView.RemoveNumberErrorMessage();

                    }

                }
                else
                {
                    //if empty
                    this.mView.ShowEnterOrSelectAccNumber();
                    allowToProceed = false;

                }

                if (allowToProceed)
                {
                    this.mView.toggleEnableClick();
                }
                else
                {
                    this.mView.toggleDisableClick();
                }

                return allowToProceed;

            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
                return false;
            }
        }

        public async void ValidateAccountAsync(string contractAccount, EnquiryTypeEnum type)
        {
            try
            {
                if (mView.IsActive())
                {
                    this.mView.ShowProgressDialog();
                    GetSearchForAccountRequest con = new GetSearchForAccountRequest(contractAccount);

                    var result = await ServiceApiImpl.Instance.ValidateAccIsExist(con);

                    if (result != null && !result.GetSearchForAccount[0].FullName.IsNullOrEmpty() && !result.GetSearchForAccount[0].IC.IsNullOrEmpty())
                    {
                        this.mView.HideProgressDialog();
                        var data = result.GetSearchForAccount[0];
                        UserSessions.SaveGetAccountIsExist(mSharedPref, JsonConvert.SerializeObject(data));

                        switch (type)
                        {
                            case EnquiryTypeEnum.General:
                                OnGeneralEnquiry();
                                break;
                            case EnquiryTypeEnum.UpdatePersonalDetails:
                                onUpdatePersonalDetail();
                                break;
                            case EnquiryTypeEnum.AboutMyBill:
                                OnAboutBillEnquiry();
                                break;
                            case EnquiryTypeEnum.GSLRebate:
                                OnGSLRebate();
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {   // no data
                        this.mView.HideProgressDialog();
                        this.mView.ShowInvalidAccountNumberError();
                        this.mView.makeSetClick(false);
                    }
                }
            }
            catch (System.OperationCanceledException e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                this.mView.makeSetClick(false);
                this.mView.OnSubmitError();
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                this.mView.makeSetClick(false);
                this.mView.OnSubmitError();
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mView.IsActive())
                {
                    this.mView.HideProgressDialog();
                }
                this.mView.makeSetClick(false);
                this.mView.OnSubmitError();
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}