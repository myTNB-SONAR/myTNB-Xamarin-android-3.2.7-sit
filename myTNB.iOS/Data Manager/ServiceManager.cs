using RestSharp;
using Newtonsoft.Json;
using myTNB.Model;
using System.Collections.Generic;
using myTNB.Enum;
using myTNB.Model.AddMultipleSupplyAccountsToUserReg;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace myTNB
{
    public class ServiceManager
    {
        /// <summary>
        /// Bases the service call.
        /// </summary>
        /// <returns>The service call.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public BaseResponseModel BaseServiceCall(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new BaseResponseModel()
                    : JsonConvert.DeserializeObject<BaseResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Gets the user authentication.
        /// </summary>
        /// <returns>The user authentication.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public UserAuthenticationResponseModel GetUserAuthentication(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                   || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new UserAuthenticationResponseModel()
                    : JsonConvert.DeserializeObject<UserAuthenticationResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Gets the customer billing account list.
        /// </summary>
        /// <returns>The customer billing account list.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public CustomerAccountResponseModel GetCustomerBillingAccountList(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new CustomerAccountResponseModel()
                    : JsonConvert.DeserializeObject<CustomerAccountResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Adds the accounts to user registration.
        /// </summary>
        /// <returns>The accounts to user registration.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public AddAccountListModel AddAccountsToUserRegistration(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new AddAccountListModel()
                    : JsonConvert.DeserializeObject<AddAccountListModel>(rawResponse.Content);
        }
        /// <summary>
        /// Registers the new customer.
        /// </summary>
        /// <returns>The new customer.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public NewUserResponseModel RegisterNewCustomer(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new NewUserResponseModel()
                    : JsonConvert.DeserializeObject<NewUserResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Gets the account usage history for graph.
        /// </summary>
        /// <returns>The account usage history for graph.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public ChartModel GetAccountUsageHistoryForGraph(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);

            try
            {
                if (!string.IsNullOrEmpty(rawResponse.Content))
                {
                    var jsonData = JObject.Parse(rawResponse.Content);
                    var contentData = jsonData["d"];

                    var responseBase = new BaseModel();
                    if (IsValidResponse(contentData, ref responseBase))
                    {
                        var model = contentData.ToObject<ChartModel>();

                        if (model != null)
                        {
                            return model;
                        }
                    }
                    else
                    {
                        return new ChartModel(responseBase);
                    }
                }
            }
            catch (JsonSerializationException)
            { }
            catch (JsonReaderException ex)
            {
                Debug.WriteLine(ex);
            }
            return new ChartModel();
        }

        /// <summary>
        /// Gets the smart meter account data.
        /// </summary>
        /// <returns>The smart meter account data.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public SmartChartModel GetSmartMeterAccountData(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);

            try
            {
                if (!string.IsNullOrEmpty(rawResponse.Content))
                {
                    var jsonData = JObject.Parse(rawResponse.Content);
                    var contentData = jsonData["d"];

                    var responseBase = new BaseModel();
                    if (IsValidResponse(contentData, ref responseBase))
                    {
                        var model = contentData.ToObject<SmartChartModel>();

                        if (model != null)
                        {
                            return model;
                        }
                    }
                    else
                    {
                        return new SmartChartModel(responseBase);
                    }
                }
            }
            catch (JsonSerializationException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (JsonReaderException ex)
            {
                Debug.WriteLine(ex);
            }
            return new SmartChartModel();
        }

        /// <summary>
        /// Checks if the valid response.
        /// </summary>
        /// <returns><c>true</c>, if valid response was ised, <c>false</c> otherwise.</returns>
        /// <param name="jToken">J token.</param>
        public bool IsValidResponse(JToken jToken, ref BaseModel resp)
        {
            try
            {
                if (jToken != null)
                {
                    var model = jToken.ToObject<BaseModel>();
                    if (model != null)
                    {
                        resp = model;
                        return model.didSucceed;
                    }
                }
            }
            catch (JsonSerializationException ex)
            {
                Debug.WriteLine(ex);
            }
            catch
            {
                Debug.WriteLine("General JSON parsing error");
            }
            return false;
        }

        /// <summary>
        /// Gets the rate us questions.
        /// </summary>
        /// <returns>The rate us questions.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public FeedbackQuestionRequestModel GetRateUsQuestions(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);

            try
            {
                if (!string.IsNullOrEmpty(rawResponse.Content))
                {
                    var jsonData = JObject.Parse(rawResponse.Content);
                    var contentData = jsonData["d"];

                    var responseBase = new BaseModel();
                    if (IsValidResponse(contentData, ref responseBase))
                    {
                        var model = contentData.ToObject<FeedbackQuestionRequestModel>();

                        if (model != null)
                        {
                            return model;
                        }
                    }
                    else
                    {
                        return new FeedbackQuestionRequestModel(responseBase);
                    }
                }
            }
            catch (JsonSerializationException ex)
            {
                Debug.WriteLine(ex);
            }
            return new FeedbackQuestionRequestModel();
        }

        /// <summary>
        /// Gets the linked accounts summary info.
        /// </summary>
        /// <returns>The linked accounts summary info.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public AmountDueStatusResponseModel GetLinkedAccountsSummaryInfo(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);

            try
            {
                if (!string.IsNullOrEmpty(rawResponse.Content))
                {
                    var jsonData = JObject.Parse(rawResponse.Content);
                    var contentData = jsonData["d"];

                    var responseBase = new BaseModel();
                    if (IsValidResponse(contentData, ref responseBase))
                    {
                        var model = contentData.ToObject<AmountDueStatusResponseModel>();

                        if (model != null)
                        {
                            return model;
                        }
                    }
                    else
                    {
                        return new AmountDueStatusResponseModel(responseBase);
                    }
                }
            }
            catch (JsonSerializationException ex)
            {
                Debug.WriteLine(ex);
            }
            return new AmountDueStatusResponseModel();
        }

        /// <summary>
        /// Gets the app launch master data.
        /// </summary>
        /// <returns>The app launch master data.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public MasterDataResponseModel GetAppLaunchMasterData(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);

            try
            {
                if (!string.IsNullOrEmpty(rawResponse.Content))
                {
                    var jsonData = JObject.Parse(rawResponse.Content);
                    var contentData = jsonData["d"];

                    var responseBase = new BaseModel();
                    if (IsValidResponse(contentData, ref responseBase))
                    {
                        var model = contentData.ToObject<MasterDataResponseModel>();

                        if (model != null)
                        {
                            return model;
                        }
                    }
                    else
                    {
                        return new MasterDataResponseModel(responseBase);
                    }
                }
            }
            catch (JsonSerializationException ex)
            {
                Debug.WriteLine(ex);
            }
            return new MasterDataResponseModel();
        }

        /// <summary>
        /// Gets the phone verification status.
        /// </summary>
        /// <returns>The phone verification status.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public PhoneVerificationStatusResponseModel GetPhoneVerificationStatus(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);

            try
            {
                if (!string.IsNullOrEmpty(rawResponse.Content))
                {
                    var jsonData = JObject.Parse(rawResponse.Content);
                    var contentData = jsonData["d"];

                    var responseBase = new BaseModel();
                    if (IsValidResponse(contentData, ref responseBase))
                    {
                        var model = contentData.ToObject<PhoneVerificationStatusResponseModel>();

                        if (model != null)
                        {
                            return model;
                        }
                    }
                    else
                    {
                        return new PhoneVerificationStatusResponseModel(responseBase);
                    }
                }
            }
            catch (JsonSerializationException ex)
            {
                Debug.WriteLine(ex);
            }
            return new PhoneVerificationStatusResponseModel();
        }

        /// <summary>
        /// Gets the billing account details.
        /// </summary>
        /// <returns>The billing account details.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public BillingAccountDetailsResponseModel GetBillingAccountDetails(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new BillingAccountDetailsResponseModel()
                    : JsonConvert.DeserializeObject<BillingAccountDetailsResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Gets the PDF URL.
        /// </summary>
        /// <returns>The PDFS ervice URL.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public string GetPDFServiceURL(string suffix, Dictionary<string, string> requestParams)
        {
            BaseService baseService = new BaseService();
            return baseService.GetFormattedURL(suffix, requestParams, APIVersion.V5);
        }
        /// <summary>
        /// Gets the bill history.
        /// </summary>
        /// <returns>The bill history.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public BillHistoryResponseModel GetBillHistory(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new BillHistoryResponseModel()
                    : JsonConvert.DeserializeObject<BillHistoryResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Gets the payment history.
        /// </summary>
        /// <returns>The payment history.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public PaymentHistoryResponseModel GetPaymentHistory(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new PaymentHistoryResponseModel()
                    : JsonConvert.DeserializeObject<PaymentHistoryResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Sends the registration token sms.
        /// </summary>
        /// <returns>The registration token sms.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public RegistrationTokenSMSResponseModel SendRegistrationTokenSMS(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new RegistrationTokenSMSResponseModel()
                    : JsonConvert.DeserializeObject<RegistrationTokenSMSResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Gets the registered cards.
        /// </summary>
        /// <returns>The registered cards.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public RegisteredCardsResponseModel GetRegisteredCards(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new RegisteredCardsResponseModel()
                    : JsonConvert.DeserializeObject<RegisteredCardsResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Requests the pay bill.
        /// </summary>
        /// <returns>The pay bill.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public RequestPayBillResponseModel RequestPayBill(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new RequestPayBillResponseModel()
                    : JsonConvert.DeserializeObject<RequestPayBillResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Gets the payment URL.
        /// </summary>
        /// <returns>The payment URL.</returns>
        /// <param name="requestParams">Request parameters.</param>
        /// <param name="paymentURL">Payment URL.</param>
        public string GetPaymentURL(Dictionary<string, string> requestParams, string paymentURL)
        {
            BaseService baseService = new BaseService();
            return baseService.GetFormattedURL(requestParams, true, paymentURL);
        }
        /// <summary>
        /// Gets the notification types.
        /// </summary>
        /// <returns>The notification types.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public NotificationTypeResponseModel GetNotificationTypes(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new NotificationTypeResponseModel()
                    : JsonConvert.DeserializeObject<NotificationTypeResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Gets the notification channels.
        /// </summary>
        /// <returns>The notification channels.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public NotificationChannelResponseModel GetNotificationChannels(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new NotificationChannelResponseModel()
                    : JsonConvert.DeserializeObject<NotificationChannelResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Saves the user notification preference.
        /// </summary>
        /// <returns>The user notification preference.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public NotificationPreferenceUpdateResponseModel SaveUserNotificationPreference(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new NotificationPreferenceUpdateResponseModel()
                    : JsonConvert.DeserializeObject<NotificationPreferenceUpdateResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Gets the user notifications.
        /// </summary>
        /// <returns>The user notifications.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public UserNotificationResponseModel GetUserNotifications(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new UserNotificationResponseModel()
                    : JsonConvert.DeserializeObject<UserNotificationResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Gets the notification detailed info.
        /// </summary>
        /// <returns>The notification detailed info.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public NotificationDetailedInfoResponseModel GetNotificationDetailedInfo(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new NotificationDetailedInfoResponseModel()
                    : JsonConvert.DeserializeObject<NotificationDetailedInfoResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Deletes the user notification.
        /// </summary>
        /// <returns>The user notification.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public DeleteNotificationResponseModel DeleteUserNotification(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new DeleteNotificationResponseModel()
                    : JsonConvert.DeserializeObject<DeleteNotificationResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Reads the user notification.
        /// </summary>
        /// <returns>The user notification.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public ReadNotificationResponseModel ReadUserNotification(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new ReadNotificationResponseModel()
                    : JsonConvert.DeserializeObject<ReadNotificationResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Gets the receipt.
        /// </summary>
        /// <returns>The receipt.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public ReceiptResponseModel GetReceipt(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new ReceiptResponseModel()
                    : JsonConvert.DeserializeObject<ReceiptResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Adds the multiple supply accounts.
        /// </summary>
        /// <returns>The multiple supply accounts.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public AddMultipleSupplyAccountsResponseModel AddMultipleSupplyAccounts(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new AddMultipleSupplyAccountsResponseModel()
                    : JsonConvert.DeserializeObject<AddMultipleSupplyAccountsResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Validates the manual account linking.
        /// </summary>
        /// <returns>The manual account linking.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public ValidateManualAccountLinkingResponseModel ValidateManualAccountLinking(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new ValidateManualAccountLinkingResponseModel()
                    : JsonConvert.DeserializeObject<ValidateManualAccountLinkingResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Gets the account due amount.
        /// </summary>
        /// <returns>The account due amount.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public DueAmountResponseModel GetAccountDueAmount(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new DueAmountResponseModel()
                    : JsonConvert.DeserializeObject<DueAmountResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Gets the web links.
        /// </summary>
        /// <returns>The web links.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public WebLinksResponseModel GetWebLinks(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new WebLinksResponseModel()
                    : JsonConvert.DeserializeObject<WebLinksResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Gets the location types.
        /// </summary>
        /// <returns>The location types.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public LocationTypesResponseModel GetLocationTypes(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new LocationTypesResponseModel()
                    : JsonConvert.DeserializeObject<LocationTypesResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Gets the locations.
        /// </summary>
        /// <returns>The locations.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public GetLocationsResponseModel GetLocations(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new GetLocationsResponseModel()
                    : JsonConvert.DeserializeObject<GetLocationsResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Gets the feedback category.
        /// </summary>
        /// <returns>The feedback category.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public FeedbackCategoryResponseModel GetFeedbackCategory(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new FeedbackCategoryResponseModel()
                    : JsonConvert.DeserializeObject<FeedbackCategoryResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Submits the feedback.
        /// </summary>
        /// <returns>The feedback.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public SubmitFeedbackResponseModel SubmitFeedback(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new SubmitFeedbackResponseModel()
                    : JsonConvert.DeserializeObject<SubmitFeedbackResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Gets the states for feedback.
        /// </summary>
        /// <returns>The states for feedback.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public StatesForFeedbackResponseModel GetStatesForFeedback(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new StatesForFeedbackResponseModel()
                    : JsonConvert.DeserializeObject<StatesForFeedbackResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Gets the type of the other feedback.
        /// </summary>
        /// <returns>The other feedback type.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public OtherFeedbackTypeResponseModel GetOtherFeedbackType(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new OtherFeedbackTypeResponseModel()
                    : JsonConvert.DeserializeObject<OtherFeedbackTypeResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Gets the submitted feedback list.
        /// </summary>
        /// <returns>The submitted feedback list.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public SubmittedFeedbackResponseModel GetSubmittedFeedbackList(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new SubmittedFeedbackResponseModel()
                    : JsonConvert.DeserializeObject<SubmittedFeedbackResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Gets the submitted feedback details.
        /// </summary>
        /// <returns>The submitted feedback details.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public SubmittedFeedbackDetailsResponseModel GetSubmittedFeedbackDetails(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new SubmittedFeedbackDetailsResponseModel()
                    : JsonConvert.DeserializeObject<SubmittedFeedbackDetailsResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Gets the multi account due amount.
        /// </summary>
        /// <returns>The multi account due amount.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public MultiAccountDueAmountResponseModel GetMultiAccountDueAmount(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new MultiAccountDueAmountResponseModel()
                    : JsonConvert.DeserializeObject<MultiAccountDueAmountResponseModel>(rawResponse.Content);
        }
        /// <summary>
        /// Requests the multi pay bill.
        /// </summary>
        /// <returns>The multi pay bill.</returns>
        /// <param name="suffix">Suffix.</param>
        /// <param name="requestParams">Request parameters.</param>
        public RequestPayBillResponseModel RequestMultiPayBill(string suffix, object requestParams)
        {
            BaseService baseService = new BaseService();
            RestResponse rawResponse = baseService.ExecuteWebservice(suffix, requestParams, APIVersion.V5);
            return (string.IsNullOrEmpty(rawResponse.Content)
                    || string.IsNullOrWhiteSpace(rawResponse.Content)) ? new RequestPayBillResponseModel()
                    : JsonConvert.DeserializeObject<RequestPayBillResponseModel>(rawResponse.Content);
        }
    }
}