using Android.Content;
using myTNB;
using myTNB.Mobile;
using myTNB.Mobile.SessionCache;
using myTNB_Android.Src.AddAccount.Activity;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusDetail.MVP;
using myTNB_Android.Src.ApplicationStatus.ApplicationStatusListing.MVP;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Bills.NewBillRedesign;
using myTNB_Android.Src.Bills.AccountStatement.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Enquiry.GSL.Activity;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Deeplink;
using Newtonsoft.Json;
using myTNB_Android.Src.OverVoltageFeedback.Activity;
using myTNB_Android.Src.Utils.Notification;
using Android.Preferences;
using Refit;
using System;
using myTNB_Android.Src.MyTNBService.Request;
using myTNB_Android.Src.MyTNBService.ServiceImpl;
using myTNB_Android.Src.MyTNBService.Response;
using myTNB_Android.Src.NotificationDetails.Activity;
using System.Threading.Tasks;
using System.Collections.Generic;
using myTNB_Android.Src.DeviceCache;
using myTNB_Android.Src.ManageBillDelivery.MVP;
using System.Linq;
using myTNB.Mobile.AWS.Models;

namespace myTNB_Android.Src.myTNBMenu.Activity
{
    internal static class DashboardHomeActivityExtension
    {
        internal static void DeeplinkValidation(this DashboardHomeActivity mainActivity)
        {
            switch (DeeplinkUtil.Instance.TargetScreen)
            {
                case Deeplink.ScreenEnum.Rewards:
                    DeeplinkRewardValidation(mainActivity);
                    break;
                case Deeplink.ScreenEnum.WhatsNew:
                    DeeplinkWhatsNewValidation(mainActivity);
                    break;
                case Deeplink.ScreenEnum.ApplicationListing:
                    DeeplinkAppListingValidation(mainActivity);
                    break;
                case Deeplink.ScreenEnum.ApplicationDetails:
                    DeeplinkAppDetailsValidation(mainActivity);
                    break;
                case Deeplink.ScreenEnum.OvervoltageClaimDetails:
                    DeeplinkOvervoltageFeedbackValidation(mainActivity);
                    break;
                case Deeplink.ScreenEnum.GetBill:
                    DeeplinkGetBillValidation(mainActivity);
                    break;
                case Deeplink.ScreenEnum.ManageBillDelivery:
                    DeeplinkManageBillDeliveryValidation(mainActivity);
                    break;
                default:
                    break;
            }
        }

        private static void DeeplinkRewardValidation(this DashboardHomeActivity mainActivity)
        {
            bool IsRewardsDisabled = MyTNBAccountManagement.GetInstance().IsRewardsDisabled();
            if (!IsRewardsDisabled && mainActivity.bottomNavigationView.Menu.FindItem(Resource.Id.menu_reward) != null)
            {
                string rewardID = DeeplinkUtil.Instance.ScreenKey;
                if (!string.IsNullOrEmpty(rewardID))
                {
                    rewardID = "{" + rewardID + "}";
                    RewardsEntity wtManager = new RewardsEntity();
                    RewardsEntity item = wtManager.GetItem(rewardID);
                    mainActivity.mPresenter.OnStartRewardThread();
                }
            }
            else
            {
                mainActivity.IsRootTutorialShown = true;
                MyTNBAppToolTipBuilder.Create(mainActivity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                    .SetTitle(Utility.GetLocalizedLabel(LanguageConstants.ERROR, LanguageConstants.Error.RWDS_UNAVAILABLE_TITLE))
                    .SetMessage(Utility.GetLocalizedLabel(LanguageConstants.ERROR, LanguageConstants.Error.RWDS_UNAVAILABLE_MSG))
                    .SetCTALabel(Utility.GetLocalizedLabel(LanguageConstants.COMMON, LanguageConstants.Common.GOT_IT))
                    .SetCTAaction(() =>
                    {
                        mainActivity.IsRootTutorialShown = false;
                        if (DashboardHomeActivity.currentFragment.GetType() == typeof(HomeMenuFragment))
                        {
                            HomeMenuFragment fragment = (HomeMenuFragment)mainActivity.SupportFragmentManager.FindFragmentById(Resource.Id.content_layout);
                            fragment.CallOnCheckShowHomeTutorial();
                        }
                    })
                    .Build().Show();
            }
        }

        private static void DeeplinkWhatsNewValidation(DashboardHomeActivity mainActivity)
        {
            string whatsNewID = DeeplinkUtil.Instance.ScreenKey;
            if (!string.IsNullOrEmpty(whatsNewID))
            {
                whatsNewID = "{" + whatsNewID + "}";
                WhatsNewEntity wtManager = new WhatsNewEntity();
                WhatsNewEntity item = wtManager.GetItem(whatsNewID);
                mainActivity.mPresenter.OnStartWhatsNewThread();
            }
        }

        private static async void DeeplinkAppListingValidation(DashboardHomeActivity mainActivity)
        {
            DeeplinkUtil.Instance.ClearDeeplinkData();
            SearchApplicationTypeResponse searchApplicationTypeResponse = SearchApplicationTypeCache.Instance.GetData();
            if (searchApplicationTypeResponse == null)
            {
                mainActivity.ShowProgressDialog();
                searchApplicationTypeResponse = await ApplicationStatusManager.Instance.SearchApplicationType("16", UserEntity.GetActive() != null);
                if (searchApplicationTypeResponse != null
                    && searchApplicationTypeResponse.StatusDetail != null
                    && searchApplicationTypeResponse.StatusDetail.IsSuccess)
                {
                    SearchApplicationTypeCache.Instance.SetData(searchApplicationTypeResponse);
                }
                mainActivity.HideProgressDialog();
            }
            if (searchApplicationTypeResponse != null
                && searchApplicationTypeResponse.StatusDetail != null
                && searchApplicationTypeResponse.StatusDetail.IsSuccess)
            {
                AllApplicationsCache.Instance.Clear();
                AllApplicationsCache.Instance.Reset();
                Intent applicationLandingIntent = new Intent(mainActivity, typeof(ApplicationStatusLandingActivity));
                mainActivity.StartActivity(applicationLandingIntent);
            }
            else
            {
                MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(mainActivity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                     .SetTitle(searchApplicationTypeResponse.StatusDetail.Title)
                     .SetMessage(searchApplicationTypeResponse.StatusDetail.Message)
                     .SetCTALabel(searchApplicationTypeResponse.StatusDetail.PrimaryCTATitle)
                     .Build();
                errorPopup.Show();
            }
        }

        private static async void DeeplinkAppDetailsValidation(DashboardHomeActivity mainActivity)
        {
            DeeplinkUtil.Instance.ClearDeeplinkData();
            mainActivity.ShowProgressDialog();
            SearchApplicationTypeResponse searchApplicationTypeResponse = SearchApplicationTypeCache.Instance.GetData();
            if (searchApplicationTypeResponse == null)
            {
                searchApplicationTypeResponse = await ApplicationStatusManager.Instance.SearchApplicationType("16", UserEntity.GetActive() != null);
                if (searchApplicationTypeResponse != null
                    && searchApplicationTypeResponse.StatusDetail != null
                    && searchApplicationTypeResponse.StatusDetail.IsSuccess)
                {
                    SearchApplicationTypeCache.Instance.SetData(searchApplicationTypeResponse);
                }
            }
            if (searchApplicationTypeResponse != null
                && searchApplicationTypeResponse.StatusDetail != null
                && searchApplicationTypeResponse.StatusDetail.IsSuccess)
            {
                ApplicationDetailDisplay detailsResponse = await ApplicationStatusManager.Instance.GetApplicationDetail(ApplicationDetailsDeeplinkCache.Instance.SaveID
                    , ApplicationDetailsDeeplinkCache.Instance.ID
                    , ApplicationDetailsDeeplinkCache.Instance.Type
                    , ApplicationDetailsDeeplinkCache.Instance.System);

                if (detailsResponse.StatusDetail.IsSuccess)
                {
                    Intent applicationStatusDetailIntent = new Intent(mainActivity, typeof(ApplicationStatusDetailActivity));
                    applicationStatusDetailIntent.PutExtra("applicationStatusResponse", JsonConvert.SerializeObject(detailsResponse.Content));
                    mainActivity.StartActivity(applicationStatusDetailIntent);
                }
                else
                {
                    MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(mainActivity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                     .SetTitle(detailsResponse.StatusDetail.Title)
                     .SetMessage(detailsResponse.StatusDetail.Message)
                     .SetCTALabel(detailsResponse.StatusDetail.PrimaryCTATitle)
                     .Build();
                    errorPopup.Show();
                }
            }
            else
            {
                MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(mainActivity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                     .SetTitle(searchApplicationTypeResponse.StatusDetail.Title)
                     .SetMessage(searchApplicationTypeResponse.StatusDetail.Message)
                     .SetCTALabel(searchApplicationTypeResponse.StatusDetail.PrimaryCTATitle)
                     .Build();
                errorPopup.Show();
            }
            mainActivity.HideProgressDialog();
        }

        private static void DeeplinkOvervoltageFeedbackValidation(DashboardHomeActivity mainActivity)
        {
            DeeplinkUtil.Instance.ClearDeeplinkData();
            UserEntity user = UserEntity.GetActive();
            if (user.UserID == EnquiryDetailsDeeplinkCache.Instance.UserID)
            {
                //Overvoltage detail page
                ShowOverVoltageFeedback(mainActivity);
            }
        }

        private static void DeeplinkGetBillValidation(DashboardHomeActivity mainActivity)
        {
            string accountNum = DeeplinkUtil.Instance.ScreenKey;
            mainActivity.mPresenter.OnGetBillEligibilityCheck(accountNum);
            DeeplinkUtil.Instance.ClearDeeplinkData();
        }

        private static void DeeplinkManageBillDeliveryValidation(DashboardHomeActivity mainActivity)
        {
            if (DBRUtility.Instance.IsAccountEligible)
            {
                GetBillRendering(mainActivity);
            }
            DeeplinkUtil.Instance.ClearDeeplinkData();
        }

        private static void DeeplinkNewBillDesignValidation(DashboardHomeActivity mainActivity)
        {
            if (BillRedesignUtility.Instance.ShouldShowHomeCard && BillRedesignUtility.Instance.IsAccountEligible)
            {
                mainActivity.NavigateToNBR();
            }
        }

        internal static void ShowAddAccount(this DashboardHomeActivity mainActivity)
        {
            Intent linkAccount = new Intent(mainActivity, typeof(LinkAccountActivity));
            linkAccount.PutExtra("fromDashboard", true);
            mainActivity.StartActivity(linkAccount);
        }

        internal static void ShowViewAccountStatement(this DashboardHomeActivity mainActivity, CustomerBillingAccount account)
        {
            AccountData accountData = AccountData.Copy(account, true);
            Intent newIntent = new Intent(mainActivity, typeof(AccountStatementSelectionActivity));
            newIntent.PutExtra(Constants.SELECTED_ACCOUNT, JsonConvert.SerializeObject(accountData));
            mainActivity.StartActivity(newIntent);
        }

        internal static void ShowIneligiblePopUp(this DashboardHomeActivity mainActivity)
        {
            MyTNBAppToolTipBuilder whatIsThisTooltip = MyTNBAppToolTipBuilder.Create(mainActivity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(Utility.GetLocalizedLabel(LanguageConstants.BILLS, LanguageConstants.Bills.TOOLTIP_ACT_STMT_TITLE))
                .SetMessage(Utility.GetLocalizedLabel(LanguageConstants.BILLS, LanguageConstants.Bills.TOOLTIP_ACT_STMT_MSG))
                .SetCTALabel(Utility.GetLocalizedLabel(LanguageConstants.COMMON, LanguageConstants.Common.GOT_IT))
                .Build();
            whatIsThisTooltip.Show();
        }

        internal static void ShowNewBillRedesign(this DashboardHomeActivity mainActivity)
        {
            Intent nbrDiscoverMoreIntent = new Intent(mainActivity, typeof(NBRDiscoverMoreActivity));
            mainActivity.StartActivityForResult(nbrDiscoverMoreIntent, Constants.NEW_BILL_REDESIGN_REQUEST_CODE);
        }

        internal static void ShowGSLInfoScreen(this DashboardHomeActivity mainActivity)
        {
            Intent gslInfoIntent = new Intent(mainActivity, typeof(GSLRebateInfoActivity));
            mainActivity.StartActivity(gslInfoIntent);
        }

        internal static void ShowOverVoltageFeedback(this DashboardHomeActivity mainActivity)
        {
            Intent viewReceipt = new Intent(mainActivity, typeof(OverVoltageFeedbackDetailActivity));
            viewReceipt.PutExtra("ClaimId", EnquiryDetailsDeeplinkCache.Instance.ClaimID);
            mainActivity.StartActivity(viewReceipt);
        }

        internal static void NotificationValidation(this DashboardHomeActivity mainActivity)
        {
            switch (NotificationUtil.Instance.Type)
            {
                case Notification.TypeEnum.AppUpdate:
                case Notification.TypeEnum.AccountStatement:
                    UserSessions.RemoveNotificationSession(PreferenceManager.GetDefaultSharedPreferences(mainActivity));
                    OnGetNotificationDetails(mainActivity);
                    break;
                case Notification.TypeEnum.NewBillDesign:
                    NavigateToBillRedesign(mainActivity);
                    break;
                default:
                    break;
            }
        }

        internal static void NavigateToBillRedesign(DashboardHomeActivity mainActivity)
        {
            if (BillRedesignUtility.Instance.ShouldShowHomeCard && BillRedesignUtility.Instance.IsAccountEligible)
            {
                mainActivity.NavigateToNBR();
            }
        }

        private async static void OnGetNotificationDetails(DashboardHomeActivity mainActivity)
        {
            mainActivity.ShowProgressDialog();
            try
            {
                string notifType = NotificationUtil.Instance.NotificationType;
                string pushMapId = NotificationUtil.Instance.PushMapId;
                UserNotificationDetailsRequest request = new UserNotificationDetailsRequest(string.Empty, notifType)
                {
                    PushMapId = pushMapId
                };
                UserNotificationDetailsResponse response = await ServiceApiImpl.Instance.GetNotificationDetails(request);
                if (response.IsSuccessResponse())
                {
                    Utility.SetIsPayDisableNotFromAppLaunch(!response.Response.IsPayEnabled);
                    ShowNotificationDetails(mainActivity, response.GetData().UserNotificationDetail);
                    mainActivity.HideProgressDialog();
                }
                else
                {
                    mainActivity.HideProgressDialog();
                }
                NotificationUtil.Instance.ClearData();
            }
            catch (System.OperationCanceledException e)
            {
                if (mainActivity.IsActive())
                {
                    mainActivity.HideProgressDialog();
                }
                Utility.LoggingNonFatalError(e);
            }
            catch (ApiException apiException)
            {
                if (mainActivity.IsActive())
                {
                    mainActivity.HideProgressDialog();
                }
                Utility.LoggingNonFatalError(apiException);
            }
            catch (Exception e)
            {
                if (mainActivity.IsActive())
                {
                    mainActivity.HideProgressDialog();
                }
                Utility.LoggingNonFatalError(e);
            }
        }

        internal static void ShowNotificationDetails(this DashboardHomeActivity mainActivity, NotificationDetails.Models.NotificationDetails details)
        {
            Intent notificationDetails = new Intent(mainActivity, typeof(UserNotificationDetailActivity));
            notificationDetails.PutExtra(Constants.SELECTED_NOTIFICATION_DETAIL_ITEM, JsonConvert.SerializeObject(details));
            mainActivity.StartActivityForResult(notificationDetails, Constants.NOTIFICATION_DETAILS_REQUEST_CODE);
        }

        internal static CustomerBillingAccount GetEligibleDBRAccount()
        {
            CustomerBillingAccount customerAccount = CustomerBillingAccount.GetSelected();
            List<string> dBRCAs = DBRUtility.Instance.GetCAList();
            List<CustomerBillingAccount> allAccountList = CustomerBillingAccount.List();
            CustomerBillingAccount account = new CustomerBillingAccount();
            if (dBRCAs.Count > 0)
            {
                var dbrSelected = dBRCAs.Where(x => x == customerAccount.AccNum).FirstOrDefault();
                if (dbrSelected != string.Empty)
                {
                    account = allAccountList.Where(x => x.AccNum == dbrSelected).FirstOrDefault();
                }
                if (account == null)
                {
                    foreach (var dbrca in dBRCAs)
                    {
                        account = allAccountList.Where(x => x.AccNum == dbrca).FirstOrDefault();
                        if (account != null)
                        {
                            break;
                        }
                    }
                }
            }
            return account;
        }

        internal static void GetBillRendering(DashboardHomeActivity mainActivity)
        {
            mainActivity.ShowProgressDialog();
            Task.Run(() =>
            {
                _ = GetBillRenderingAsync(mainActivity);
            });
        }

        internal static async Task GetBillRenderingAsync(DashboardHomeActivity mainActivity)
        {
            try
            {
                string caNumber = string.Empty;
                if (DBRUtility.Instance.IsAccountEligible)
                {
                    List<string> caList = DBRUtility.Instance.GetCAList();
                    caNumber = caList != null && caList.Count > 0
                        ? caList[0]
                        : string.Empty;
                }
                else
                {
                    CustomerBillingAccount dbrAccount = GetEligibleDBRAccount();
                    if (dbrAccount == null)
                    {
                        mainActivity.RunOnUiThread(() =>
                        {
                            try
                            {
                                mainActivity.HideProgressDialog();
                                MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(mainActivity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                                    .SetTitle(Utility.GetLocalizedLabel(LanguageConstants.ERROR, LanguageConstants.Error.DEFAULT_ERROR_TITLE))
                                    .SetMessage(Utility.GetLocalizedLabel(LanguageConstants.ERROR, LanguageConstants.Error.DEFAULT_ERROR_MSG))
                                    .SetCTALabel(Utility.GetLocalizedLabel(LanguageConstants.COMMON, LanguageConstants.Common.GOT_IT))
                                    .Build();
                                errorPopup.Show();
                            }
                            catch (System.Exception ex)
                            {
                                Utility.LoggingNonFatalError(ex);
                            }
                        });

                        return;
                    }
                    caNumber = dbrAccount.AccNum;
                }

                if (!AccessTokenCache.Instance.HasTokenSaved(mainActivity))
                {
                    string accessToken = await AccessTokenManager.Instance.GenerateAccessToken(UserEntity.GetActive().UserID ?? string.Empty);
                    AccessTokenCache.Instance.SaveAccessToken(mainActivity, accessToken);
                }
                GetBillRenderingResponse billRenderingResponse = await DBRManager.Instance.GetBillRendering(caNumber
                    , AccessTokenCache.Instance.GetAccessToken(mainActivity));

                mainActivity.HideProgressDialog();

                //Nullity Check
                if (billRenderingResponse != null
                   && billRenderingResponse.StatusDetail != null
                   && billRenderingResponse.StatusDetail.IsSuccess
                   && billRenderingResponse.Content != null
                   && billRenderingResponse.Content.DBRType != MobileEnums.DBRTypeEnum.None)
                {
                    Intent intent = new Intent(mainActivity, typeof(ManageBillDeliveryActivity));
                    intent.PutExtra("billRenderingResponse", JsonConvert.SerializeObject(billRenderingResponse));
                    intent.PutExtra("accountNumber", caNumber);
                    mainActivity.StartActivity(intent);
                }
                else
                {
                    string title = billRenderingResponse != null && billRenderingResponse.StatusDetail != null && billRenderingResponse.StatusDetail.Title.IsValid()
                        ? billRenderingResponse?.StatusDetail?.Title
                        : Utility.GetLocalizedLabel(LanguageConstants.ERROR, LanguageConstants.Error.DEFAULT_ERROR_TITLE);

                    string message = billRenderingResponse != null && billRenderingResponse.StatusDetail != null && billRenderingResponse.StatusDetail.Message.IsValid()
                       ? billRenderingResponse?.StatusDetail?.Message
                       : Utility.GetLocalizedLabel(LanguageConstants.ERROR, LanguageConstants.Error.DEFAULT_ERROR_MSG);

                    string cta = billRenderingResponse != null && billRenderingResponse.StatusDetail != null && billRenderingResponse.StatusDetail.PrimaryCTATitle.IsValid()
                       ? billRenderingResponse?.StatusDetail?.PrimaryCTATitle
                       : Utility.GetLocalizedLabel(LanguageConstants.COMMON, LanguageConstants.Common.OK);

                    mainActivity.RunOnUiThread(() =>
                    {
                        MyTNBAppToolTipBuilder errorPopup = MyTNBAppToolTipBuilder.Create(mainActivity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                            .SetTitle(title ?? Utility.GetLocalizedLabel(LanguageConstants.ERROR, LanguageConstants.Error.DEFAULT_ERROR_TITLE))
                            .SetMessage(message ?? Utility.GetLocalizedLabel(LanguageConstants.ERROR, LanguageConstants.Error.DEFAULT_ERROR_MSG))
                            .SetCTALabel(cta ?? Utility.GetLocalizedLabel(LanguageConstants.COMMON, LanguageConstants.Common.OK))
                            .Build();
                        errorPopup.Show();
                    });
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}
