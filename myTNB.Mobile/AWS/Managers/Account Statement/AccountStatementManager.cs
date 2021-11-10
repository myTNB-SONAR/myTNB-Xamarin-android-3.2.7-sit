using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using myTNB.Mobile.AWS.Models.AccountStatement;
using myTNB.Mobile.AWS.Services.AccountStatement;
using myTNB.Mobile.Extensions;
using Newtonsoft.Json;
using Refit;
using myTNB.Mobile.AWS;

namespace myTNB.Mobile
{
    public sealed class AccountStatementManager
    {
        private static readonly Lazy<AccountStatementManager> lazy =
           new Lazy<AccountStatementManager>(() => new AccountStatementManager());

        public static AccountStatementManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }
        public AccountStatementManager() { }

        public async Task<PostAccountStatementResponse> PostAccountStatement(string accountNumber
            , string statementPeriod
            , bool isOwner
            , string accessToken)
        {
            PostAccountStatementResponse response = new PostAccountStatementResponse();
            int timeout = LanguageManager.Instance.GetConfigTimeout(LanguageManager.TogglePropertyEnum.AccountStatementTimeout);
            IAccountStatementService service = RestService.For<IAccountStatementService>(AWSConstants.Domains.Domain);
            Guid referenceNumber = Guid.NewGuid();
            try
            {
                PostAccountStatementRequest request = new PostAccountStatementRequest
                {
                    ReferenceNo = referenceNumber.ToString(),
                    CaNo = accountNumber,
                    StatementPeriod = statementPeriod,
                    IsOwnedAccount = isOwner
                };
                Debug.WriteLine("[DEBUG] PostAccountStatement Request: " + JsonConvert.SerializeObject(request));

                HttpResponseMessage rawResponse = await service.PostAccountStatement(request
                   , NetworkService.GetCancellationToken(timeout == 0 ? AWSConstants.AccountStatementTimeOut : timeout)
                   , accessToken
                   , AppInfoManager.Instance.ViewInfo);
                //Mark: Check for 404 First
                if ((int)rawResponse.StatusCode != 200)
                {
                    response.StatusDetail = new StatusDetail();
                    response.StatusDetail = AWSConstants.Services.PostAccountStatement.GetStatusDetails(MobileConstants.DEFAULT);
                    response.StatusDetail.IsSuccess = false;
                    return response;
                }

                string responseString = await rawResponse.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<PostAccountStatementResponse>(responseString);
                if (response != null
                    && response.Content != null
                    && response.StatusDetail != null
                    && response.StatusDetail.Code.IsValid())
                {
                    response.StatusDetail = AWSConstants.Services.PostAccountStatement.GetStatusDetails(response.StatusDetail.Code);
                }
                else
                {
                    if (response != null && response.StatusDetail != null && response.StatusDetail.Code.IsValid())
                    {
                        response.StatusDetail = AWSConstants.Services.PostAccountStatement.GetStatusDetails(response.StatusDetail.Code);
                    }
                    else
                    {
                        response = new PostAccountStatementResponse
                        {
                            StatusDetail = new StatusDetail()
                        };
                        response.StatusDetail = AWSConstants.Services.PostAccountStatement.GetStatusDetails(MobileConstants.DEFAULT);
                    }
                }
                Debug.WriteLine("[DEBUG] [PostAccountStatement] Response: " + JsonConvert.SerializeObject(response));
                return response;
            }
            catch (ApiException apiEx)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG] [PostAccountStatement] Refit Exception: " + apiEx.Message);
#endif
            }
            catch (OperationCanceledException operationCancelledError)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG] [PostAccountStatement] OperationCanceledException: " + operationCancelledError.Message);
#endif
                OnPostAccountStatementNotification(referenceNumber.ToString(), accessToken);
                response = new PostAccountStatementResponse
                {
                    StatusDetail = new StatusDetail()
                };
                response.StatusDetail = AWSConstants.Services.PostAccountStatement.GetStatusDetails(MobileConstants.TIMEOUT);
                Debug.WriteLine("[DEBUG] [PostAccountStatement] Response: " + JsonConvert.SerializeObject(response));
                return response;
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG] [PostAccountStatement] General Exception: " + ex.Message);
#endif
            }

            response = new PostAccountStatementResponse
            {
                StatusDetail = new StatusDetail()
            };
            response.StatusDetail = AWSConstants.Services.PostAccountStatement.GetStatusDetails(MobileConstants.DEFAULT);
            Debug.WriteLine("[DEBUG] [PostAccountStatement] Response: " + JsonConvert.SerializeObject(response));
            return response;
        }

        private void OnPostAccountStatementNotification(string referenceNumber
            , string accessToken)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    IAccountStatementService service = RestService.For<IAccountStatementService>(AWSConstants.Domains.Domain);
                    PostAccountStatementNotificationRequest request = new PostAccountStatementNotificationRequest
                    {
                        ReferenceNo = referenceNumber
                    };
                    Debug.WriteLine("[DEBUG] [OnPostAccountStatementNotification] Request: " + JsonConvert.SerializeObject(request));
                    _ = service.PostAccountStatementNotification(request
                        , NetworkService.GetCancellationToken()
                        , accessToken
                        , AppInfoManager.Instance.ViewInfo);
                }).ConfigureAwait(false);
            }
            catch (ApiException apiEx)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG] [OnPostAccountStatementNotification] Refit Exception: " + apiEx.Message);
#endif
            }
            catch (OperationCanceledException operationCancelledError)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG] [OnPostAccountStatementNotification] OperationCanceledException: " + operationCancelledError.Message);
#endif
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine("[DEBUG] [OnPostAccountStatementNotification] General Exception: " + ex.Message);
#endif
            }
        }
    }
}