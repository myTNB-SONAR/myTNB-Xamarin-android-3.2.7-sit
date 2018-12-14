using System.Threading.Tasks;
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
    }
}