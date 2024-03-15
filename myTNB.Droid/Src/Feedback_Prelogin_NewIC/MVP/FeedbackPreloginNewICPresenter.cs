using Android.Content;
using Android.Text;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.MyTNBService.Request;
using myTNB.AndroidApp.Src.MyTNBService.ServiceImpl;
using myTNB.AndroidApp.Src.Utils;
using Newtonsoft.Json;
using Refit;
using System;
using static myTNB.AndroidApp.Src.Feedback_Prelogin_NewIC.Activity.FeedbackPreloginNewICActivity;
using static myTNB.LanguageManager;

namespace myTNB.AndroidApp.Src.Feedback_Prelogin_NewIC.MVP
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

        private void OnGSLRebate(bool isOwner)
        {
            if (LanguageManager.Instance.GetConfigToggleValue(ConfigPropertyEnum.IsGSLRebateEnabled))
            {
                this.mView.ShowGSLRebate(isOwner);
            }
        }

        private void OnOvervoltageClaim()
        {
            this.mView.ShowOvervoltageClaim();
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

                    if (result != null && !string.IsNullOrWhiteSpace(result.GetSearchForAccount[0].FullName) && !string.IsNullOrWhiteSpace(result.GetSearchForAccount[0].IC))
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
                                if (UserEntity.IsCurrentlyActive())
                                {
                                    var ic = data.IC.Trim();
                                    var icAcct = UserEntity.GetActive().IdentificationNo.Trim();
                                    OnGSLRebate(ic.Equals(icAcct));
                                }
                                else
                                {
                                    OnGSLRebate(false);
                                }
                                break;
                            case EnquiryTypeEnum.OvervoltageClaim:
                                OnOvervoltageClaim();
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