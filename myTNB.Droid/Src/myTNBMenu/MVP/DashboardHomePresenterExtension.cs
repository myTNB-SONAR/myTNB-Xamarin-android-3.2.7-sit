using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Preferences;
using myTNB;
using myTNB.Mobile;
using myTNB.Mobile.AWS;
using myTNB.Android.Src.AddAccount.Models;
using myTNB.Android.Src.Base;
using myTNB.Android.Src.Database.Model;
using myTNB.Android.Src.MyHome.Model;
using myTNB.Android.Src.MyTNBService.Request;
using myTNB.Android.Src.MyTNBService.ServiceImpl;
using myTNB.Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB.Android.Src.myTNBMenu.MVP
{
    internal static class DashboardHomePresenterExtension
    {
        internal static void GetBillEligibilityCheck(this DashboardHomePresenter presenter, string accountNumber)
        {
            if (BillRedesignUtility.Instance.IsCAEligible(accountNumber))
            {
                if (CAIsInTheList(accountNumber, out CustomerBillingAccount account))
                {
                    bool isEligibleForNonOwner = LanguageManager.Instance.GetConfigToggleValue(LanguageManager.ConfigPropertyEnum.ShouldShowAccountStatementToNonOwner);
                    if (isEligibleForNonOwner)
                    {
                        presenter.mView.NavigateToViewAccountStatement(account);
                    }
                    else
                    {
                        ValidateRealOwnerAsync(presenter, account);
                    }
                }
                else
                {
                    presenter.mView.NavigateToAddAccount();
                }
            }
            else if (!CAIsInTheList(accountNumber, out CustomerBillingAccount account))
            {
                presenter.mView.NavigateToAddAccount();
            }
        }

        private static bool CAIsInTheList(string accountNumber, out CustomerBillingAccount account)
        {
            account = null;
            List<CustomerBillingAccount> accountList = CustomerBillingAccount.List();
            if (accountList.Count > 0)
            {
                CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(accountNumber);
                account = customerBillingAccount;
                return customerBillingAccount != null;
            }
            return false;
        }

        private static async void ValidateRealOwnerAsync(this DashboardHomePresenter presenter, CustomerBillingAccount account)
        {
            presenter.mView.ShowProgressDialog();
            GetSearchForAccountRequest request = new GetSearchForAccountRequest(account.AccNum);

            var result = await ServiceApiImpl.Instance.ValidateAccIsExist(request);

            if (result != null &&
                result.GetSearchForAccount != null &&
                result.GetSearchForAccount.Count > 0)
            {
                presenter.mView.HideProgressDialog();
                var data = result.GetSearchForAccount[0];
                var ic = data.IC.Trim();
                var icAcct = UserEntity.GetActive().IdentificationNo.Trim();
                if (ic.Equals(icAcct))
                {
                    presenter.mView.NavigateToViewAccountStatement(account);
                }
                else
                {
                    presenter.mView.TriggerIneligiblePopUp();
                }
            }
            else if (account.isOwned)
            {
                presenter.mView.NavigateToViewAccountStatement(account);
            }
            else
            {
                presenter.mView.TriggerIneligiblePopUp();
            }
        }

        internal static void CheckNCDraftForResume(this DashboardHomePresenter presenter, ISharedPreferences prefs)
        {
            var response = MyTNBAccountManagement.GetInstance().GetPostGetDraftResponse();

            List<string> refNosOldList = new List<string>();
            string currentRefNos = UserSessions.GetResumePopUpRefNos(prefs);

            if (currentRefNos.IsValid())
            {
                refNosOldList = JsonConvert.DeserializeObject<List<string>>(currentRefNos);
                if (refNosOldList == null)
                {
                    refNosOldList = new List<string>();
                }
            }

            if (response != null)
            {
                OnProcessGetDraftApplications(presenter, refNosOldList, response);
                MyTNBAccountManagement.GetInstance().SetPostGetDraftResponse(null);
            }
            else
            {
                Task.Run(() =>
                {
                    _ = GetDraftApplications(presenter, refNosOldList, prefs);
                });
            }
        }

        private static async Task GetDraftApplications(this DashboardHomePresenter presenter, List<string> refNosOldList, ISharedPreferences prefs)
        {
            UserEntity user = UserEntity.GetActive();
            var response = await myTNB.Mobile.AWS.ApplicationStatusManager.Instance.PostGetDraftApplications(user.UserID, refNosOldList);

            MyTNBAccountManagement.GetInstance().SetPostGetDraftResponse(response);
            OnProcessGetDraftApplications(presenter, refNosOldList, response);
        }

        private static void OnProcessGetDraftApplications(this DashboardHomePresenter presenter, List<string> refNosOldList, PostGetDraftResponse response)
        {
            if (response != null &&
                response.StatusDetail != null &&
                response.StatusDetail.IsSuccess)
            {
                if (response.Content != null &&
                    response.Content.ReminderTitle.IsValid() &&
                    response.Content.ReminderMessage.IsValid())
                {
                    List<string> newRefNosList = new List<string>();
                    if (response.Content.ApplicationList != null &&
                        response.Content.ApplicationList.Count > 0)
                    {
                        string newSetOfRefNos = JsonConvert.SerializeObject(response.Content.ApplicationList);
                        MyTNBAccountManagement.GetInstance().SetResumeDraftRefNos(newSetOfRefNos);

                        newRefNosList = response.Content.ApplicationList.Except(refNosOldList).ToList();
                    }

                    List<PostGetDraftResponseItemModel> newDraftObjList = new List<PostGetDraftResponseItemModel>();

                    if (response.Content.Applications != null &&
                        response.Content.Applications.Count > 0)
                    {
                        newDraftObjList = response.Content.Applications;
                        foreach (string refNo in refNosOldList)
                        {
                            int index = response.Content.Applications.FindIndex(x => x.ReferenceNo == refNo);
                            if (index != -1)
                            {
                                newDraftObjList.RemoveAt(index);
                            }
                        }
                    }

                    MyHomeToolTipModel tooltipModel = new MyHomeToolTipModel()
                    {
                        Title = response.Content.ReminderTitle,
                        Message = response.Content.ReminderMessage,
                        PrimaryCTA = response.Content.PrimaryCTA,
                        SecondaryCTA = response.Content.SecondaryCTA
                    };

                    presenter.mView.OnShowDraftResumePopUp(tooltipModel, newDraftObjList, response.Content.IsMultipleDraft);
                }
            }
        }
    }
}
