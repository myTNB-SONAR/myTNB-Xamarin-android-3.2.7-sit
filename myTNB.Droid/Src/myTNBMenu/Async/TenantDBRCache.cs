using System;
using System.Collections.Generic;
using myTNB.Mobile.AWS.Models.DBR;

namespace myTNB_Android.Src.myTNBMenu.Async
{
    public sealed class TenantDBRCache
    {
        private static readonly Lazy<TenantDBRCache> lazy =
           new Lazy<TenantDBRCache>(() => new TenantDBRCache());

        public static TenantDBRCache Instance
        {
            get
            {
                return lazy.Value;
            }
        }



        public TenantDBRCache()
        {
        }

        private PostBREligibilityIndicatorsResponse Data { set; get; }

        /// <summary>
        /// Sets the Session Data for eligibility
        /// </summary>
        /// <param name="response">GetEligibilityResponse</param>
        public void SetData(PostBREligibilityIndicatorsResponse response)
        {
            if (response != null)
            {
                Data = response;
            }
        }

        /// <summary>
        /// Checks if feature is enabled
        /// Can check enabled and targetgroup property
        /// Feature and Propery are binded by enum to avoid wrong input
        /// </summary>
        /// <param name="feature">myTNB App Feature</param>
        /// <param name="featureProperty">property</param>
        /// <returns></returns>
        public List<PostBREligibilityIndicatorsModel> IsTenantDBREligible()
        {
            List<PostBREligibilityIndicatorsModel> tenantList = new List<PostBREligibilityIndicatorsModel>();

            if (Data != null
                && Data.StatusDetail != null
                && Data.StatusDetail.IsSuccess
                && Data.Content != null
                && Data.Content.Count > 0)
            {
                for (int i = 0; i < Data.Content.Count; i++)
                {
                    PostBREligibilityIndicatorsModel data = new PostBREligibilityIndicatorsModel();
                    data.caNo = Data.Content[i].caNo;
                    data.IsOwnerAlreadyOptIn = Data.Content[i].IsOwnerAlreadyOptIn;
                    data.IsOwnerOverRule = Data.Content[i].IsOwnerOverRule;
                    data.IsTenantAlreadyOptIn = Data.Content[i].IsTenantAlreadyOptIn;
                    tenantList.Add(data);
                }
                return tenantList;
            }
            else
            {
                return tenantList;
            }
        }

        public void Clear()
        {
            Data = null;
        }
    }
}