using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Preferences;
using myTNB;
using myTNB.Mobile;
using myTNB.Mobile.AWS;
using myTNB_Android.Src.AddAccount.Models;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MyHome.Model;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.myTNBMenu.MVP
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
            var response = MyTNBAccountManagement.GetInstance().GetPostGetNCDraftResponse();

            List<string> refNosOldList = new List<string>();
            string currentRefNos = UserSessions.GetNCResumePopUpRefNos(prefs);

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
                OnProcessGetNCDraftApplications(presenter, refNosOldList, response);
                MyTNBAccountManagement.GetInstance().SetPostGetNCDraftResponse(null);
            }
            else
            {
                Task.Run(() =>
                {
                    _ = GetNCDraftApplications(presenter, refNosOldList, prefs);
                });
            }
        }

        private static async Task GetNCDraftApplications(this DashboardHomePresenter presenter, List<string> refNosOldList, ISharedPreferences prefs)
        {
            UserEntity user = UserEntity.GetActive();
            var response = await myTNB.Mobile.AWS.ApplicationStatusManager.Instance.PostGetNCDraftApplications(user.UserID, user.Email, refNosOldList);

            MyTNBAccountManagement.GetInstance().SetPostGetNCDraftResponse(response);
            OnProcessGetNCDraftApplications(presenter, refNosOldList, response);
        }

        private static void OnProcessGetNCDraftApplications(this DashboardHomePresenter presenter, List<string> refNosOldList, PostGetNCDraftResponse response)
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
                    if (response.Content.NCApplicationList != null &&
                        response.Content.NCApplicationList.Count > 0)
                    {
                        string newSetOfRefNos = JsonConvert.SerializeObject(response.Content.NCApplicationList);
                        MyTNBAccountManagement.GetInstance().SetNCResumeDraftRefNos(newSetOfRefNos);

                        newRefNosList = response.Content.NCApplicationList.Except(refNosOldList).ToList();
                    }

                    List<PostGetNCDraftResponseItemModel> newNCDraftObjList = new List<PostGetNCDraftResponseItemModel>();

                    if (response.Content.Applications != null &&
                        response.Content.Applications.Count > 0)
                    {
                        newNCDraftObjList = response.Content.Applications;
                        foreach (string refNo in refNosOldList)
                        {
                            int index = response.Content.Applications.FindIndex(x => x.ReferenceNo == refNo);
                            if (index != -1)
                            {
                                newNCDraftObjList.RemoveAt(index);
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

                    presenter.mView.OnShowNCDraftResumePopUp(tooltipModel, newNCDraftObjList, response.Content.IsMultipleDraft);
                }
            }
        }
    }
}
