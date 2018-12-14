using System.Collections.Generic;
using System.Threading.Tasks;
using myTNB.Enums;
using myTNB.Model;

namespace myTNB.DataManager
{
    public static class ServiceCall
    {
        /// <summary>
        /// Gets the registered cards.
        /// </summary>
        /// <returns>The registered cards.</returns>
        public static Task GetRegisteredCards()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    email = DataManager.SharedInstance.UserEntity[0].email
                };
                DataManager.SharedInstance.RegisteredCards = serviceManager
                    .GetRegisteredCards("GetRegisteredCards", requestParameter);
            });
        }

        /// <summary>
        /// Gets the customer billing account list.
        /// </summary>
        /// <returns>The customer billing account list.</returns>
        public static Task GetCustomerBillingAccountList()
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    apiKeyID = TNBGlobal.API_KEY_ID,
                    userID = DataManager.SharedInstance.UserEntity[0].userID
                };
                DataManager.SharedInstance.CustomerAccounts = serviceManager
                    .GetCustomerBillingAccountList("GetCustomerBillingAccountList", requestParameter);
            });
        }

        /// <summary>
        /// Validates the base response.
        /// </summary>
        /// <returns><c>true</c>, if base response was validated, <c>false</c> otherwise.</returns>
        /// <param name="response">Response.</param>
        public static bool ValidateBaseResponse(BaseResponseModel response)
        {
            return response != null
                && response.d != null
                && response.d.isError.ToLower().Equals("false");
        }

        /// <summary>
        /// Validates the response item.
        /// </summary>
        /// <returns>The response item.</returns>
        /// <param name="item">Item.</param>
        public static string ValidateResponseItem(string item)
        {
            if (string.IsNullOrEmpty(item) || string.IsNullOrWhiteSpace(item))
            {
                return string.Empty;
            }
            return item;
        }

        /// <summary>
        /// Check if there are items in account list
        /// </summary>
        /// <returns><c>true</c>, if account list have items, <c>false</c> otherwise.</returns>
        public static bool HasAccountList()
        {
            bool hasAccount = DataManager.SharedInstance.AccountRecordsList != null
                              && DataManager.SharedInstance.AccountRecordsList.d != null
                              && DataManager.SharedInstance.AccountRecordsList.d.Count > 0;
            return hasAccount;
        }

        /// <summary>
        /// Gets the account list count.
        /// </summary>
        /// <returns>The account list count.</returns>
        public static int GetAccountListCount()
        {
            return HasAccountList() ? DataManager.SharedInstance.AccountRecordsList.d.Count : 0;
        }

        /// <summary>
        /// Check for selected account
        /// </summary>
        /// <returns><c>true</c>, there is selected , <c>false</c> otherwise.</returns>
        public static bool HasSelectedAccount()
        {
            return DataManager.SharedInstance.SelectedAccount != null;
        }

        /// <summary>
        /// Sends the update phone token sms.
        /// </summary>
        /// <returns>The update phone token sms.</returns>
        /// <param name="mobileNumber">Mobile number.</param>
        public static async Task<BaseResponseModel> SendUpdatePhoneTokenSMS(string mobileNumber)
        {
            ServiceManager serviceManager = new ServiceManager();
            BaseResponseModel response;
            object requestParameter = new
            {
                apiKeyID = TNBGlobal.API_KEY_ID,
                ipAddress = TNBGlobal.API_KEY_ID,
                clientType = TNBGlobal.API_KEY_ID,
                activeUserName = TNBGlobal.API_KEY_ID,
                devicePlatform = TNBGlobal.API_KEY_ID,
                deviceVersion = TNBGlobal.API_KEY_ID,
                deviceCordova = TNBGlobal.API_KEY_ID,
                username = DataManager.SharedInstance.UserEntity[0].email,
                userEmail = DataManager.SharedInstance.UserEntity[0].email,
                mobileNo = mobileNumber,
                sspUserId = DataManager.SharedInstance.UserEntity[0].userID
            };
            response = await Task.Run(() =>
            {
                return serviceManager.BaseServiceCall("SendUpdatePhoneTokenSMS", requestParameter);
            });

            return response;
        }

        /// <summary>
        /// Updates the phone number.
        /// </summary>
        /// <returns>The phone number.</returns>
        /// <param name="mobileNumber">Mobile number.</param>
        /// <param name="tokenStr">Token string.</param>
        /// <param name="isFromLogin">If set to <c>true</c> is from login.</param>
        public static async Task<BaseResponseModel> UpdatePhoneNumber(string mobileNumber, string tokenStr, bool isFromLogin)
        {
            BaseResponseModel response;
            ServiceManager serviceManager = new ServiceManager();
            object requestParameter = new
            {
                apiKeyID = TNBGlobal.API_KEY_ID,
                sspUserId = DataManager.SharedInstance.UserEntity[0].userID,
                email = DataManager.SharedInstance.UserEntity[0].email,
                oldPhoneNumber = isFromLogin ? string.Empty : DataManager.SharedInstance.UserEntity[0].mobileNo,
                newPhoneNumber = mobileNumber,
                token = tokenStr
            };

            response = await Task.Run(() =>
            {
                return serviceManager.BaseServiceCall("UpdatePhoneNumber_v2", requestParameter);
            });

            return response;
        }

        /// <summary>
        /// Gets the billing account details.
        /// </summary>
        /// <returns>The billing account details.</returns>
        public static async Task<BillingAccountDetailsResponseModel> GetBillingAccountDetails()
        {
            BillingAccountDetailsResponseModel response;

            ServiceManager serviceManager = new ServiceManager();
            object requestParameter = new
            {
                apiKeyID = TNBGlobal.API_KEY_ID,
                CANum = DataManager.SharedInstance.SelectedAccount.accNum
            };
            response = await Task.Run(() =>
            {
                return serviceManager.GetBillingAccountDetails("GetBillingAccountDetails", requestParameter);
            });

            return response;
        }

        /// <summary>
        /// Gets the rate us questions.
        /// </summary>
        /// <returns>The rate us questions.</returns>
        /// <param name="questionCategory">Question category.</param>
        public static async Task<FeedbackQuestionRequestModel> GetRateUsQuestions(QuestionCategoryEnum questionCategory)
        {
            FeedbackQuestionRequestModel questionResponse = null;
            ServiceManager serviceManager = new ServiceManager();
            object requestParameter = new
            {
                ApiKeyID = TNBGlobal.API_KEY_ID,
                QuestionCategoryId = (int)questionCategory
            };
            questionResponse = await Task.Run(() =>
            {
                return serviceManager.GetRateUsQuestions("GetRateUsQuestions", requestParameter);
            });

            return questionResponse;

        }

        /// <summary>
        /// Submits the rate us answers.
        /// </summary>
        /// <returns>The rate us.</returns>
        /// <param name="answer">Answer.</param>
        public static Task SubmitRateUs(InputAnswerModel answer)
        {
            return Task.Factory.StartNew(() =>
            {
                ServiceManager serviceManager = new ServiceManager();
                object requestParameter = new
                {
                    ApiKeyID = TNBGlobal.API_KEY_ID,
                    InputAnswer = answer
                };
                BaseResponseModel response = serviceManager.BaseServiceCall("SubmitRateUs", requestParameter);
            });
        }

        /// <summary>
        /// Gets the linked accounts summary info.
        /// </summary>
        /// <returns>The linked accounts summary info.</returns>
        /// <param name="accountsList">Accounts list.</param>
        public static async Task<AmountDueStatusResponseModel> GetLinkedAccountsSummaryInfo(List<string> accountsList)
        {
            AmountDueStatusResponseModel response = null;
            ServiceManager serviceManager = new ServiceManager();

            var userId = string.Empty;

            if (DataManager.SharedInstance.UserEntity?.Count > 0)
            {
                userId = DataManager.SharedInstance.UserEntity[0]?.userID;
            }

            object requestParameter = new
            {
                apiKeyID = TNBGlobal.API_KEY_ID,
                SSPUserId = userId,
                accounts = accountsList ?? new List<string>()
            };
            response = await Task.Run(() =>
            {
                return serviceManager.GetLinkedAccountsSummaryInfo("GetLinkedAccountsSummaryInfo", requestParameter);
            });

            return response;

        }

        /// <summary>
        /// Gets the app launch master data.
        /// </summary>
        /// <returns>The app launch master data.</returns>
        public static async Task<MasterDataResponseModel> GetAppLaunchMasterData()
        {
            MasterDataResponseModel response = null;
            ServiceManager serviceManager = new ServiceManager();

            string userEmail = string.Empty;
            if (DataManager.SharedInstance.UserEntity != null && DataManager.SharedInstance.UserEntity.Count > 0)
            {
                userEmail = DataManager.SharedInstance.UserEntity[0].email;
            }

            object requestParameter = new
            {
                ApiKeyID = TNBGlobal.API_KEY_ID,
                SSPUserId = string.Empty,
                Email = userEmail,
                DeviceId = DataManager.SharedInstance.UDID,
                AppVersion = AppVersionHelper.GetBuildVersion(),
                OsType = TNBGlobal.DEVICE_PLATFORM_IOS,
                OsVersion = DeviceHelper.GetOSVersion()
            };
            response = await Task.Run(() =>
            {
                return serviceManager.GetAppLaunchMasterData("GetAppLaunchMasterData", requestParameter);
            });

            return response;
        }

        /// <summary>
        /// Gets the phone verification status.
        /// </summary>
        /// <returns>The phone verification status.</returns>
        public static async Task<PhoneVerificationStatusResponseModel> GetPhoneVerificationStatus()
        {
            PhoneVerificationStatusResponseModel response = null;
            ServiceManager serviceManager = new ServiceManager();

            string userEmail = string.Empty;
            string sspId = string.Empty;
            if (DataManager.SharedInstance.UserEntity != null && DataManager.SharedInstance.UserEntity.Count > 0)
            {
                userEmail = DataManager.SharedInstance.UserEntity[0].email;
                sspId = DataManager.SharedInstance.UserEntity[0].userID;
            }

            object requestParameter = new
            {
                ApiKeyID = TNBGlobal.API_KEY_ID,
                SSPUserID = sspId,
                Email = userEmail,
                DeviceID = DataManager.SharedInstance.UDID
            };
            response = await Task.Run(() =>
            {
                return serviceManager.GetPhoneVerificationStatus("GetPhoneVerifyStatus", requestParameter);
            });

            return response;
        }

    }
}
