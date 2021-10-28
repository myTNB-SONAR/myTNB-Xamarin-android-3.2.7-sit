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
            try
            {
                Guid referenceNumber = Guid.NewGuid();
                PostAccountStatementRequest request = new PostAccountStatementRequest
                {
                    ReferenceNo = referenceNumber.ToString(),
                    CaNo = accountNumber,
                    StatementPeriod = statementPeriod,
                    IsOwnedAccount = isOwner
                };

                IAccountStatementService service = RestService.For<IAccountStatementService>(AWSConstants.Domains.Domain);
                HttpResponseMessage rawResponse = await service.PostAccountStatement(request
                   , NetworkService.GetCancellationToken(timeout == 0 ? AWSConstants.TimeOut : timeout)
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
                Debug.WriteLine("[DEBUG] [PostAccountStatement]: " + JsonConvert.SerializeObject(response));
                return response;
            }
            catch (ApiException apiEx)
            {
                Debug.WriteLine("[DEBUG] [PostAccountStatement] Refit Exception: " + apiEx.Message);
            }
            catch (OperationCanceledException operationCancelledError)
            {
                Debug.WriteLine("[DEBUG] [PostAccountStatement] OperationCanceledException: " + operationCancelledError.Message);
                response = new PostAccountStatementResponse
                {
                    StatusDetail = new StatusDetail()
                };
                response.StatusDetail = AWSConstants.Services.PostAccountStatement.GetStatusDetails(MobileConstants.TIMEOUT);
                return response;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[DEBUG] [PostAccountStatement] General Exception: " + ex.Message);
            }

            response = new PostAccountStatementResponse
            {
                StatusDetail = new StatusDetail()
            };
            response.StatusDetail = AWSConstants.Services.PostAccountStatement.GetStatusDetails(MobileConstants.DEFAULT);
            return response;
        }
    }
}