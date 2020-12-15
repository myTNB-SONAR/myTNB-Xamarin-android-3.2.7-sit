using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using myTNB.Mobile.Extensions;

namespace myTNB.Mobile.SessionCache
{
    public sealed class AllApplicationsCache
    {
        private static readonly Lazy<AllApplicationsCache> lazy
            = new Lazy<AllApplicationsCache>(() => new AllApplicationsCache());

        public static AllApplicationsCache Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private AllApplicationsCache() { }
        private List<ApplicationModel> ApplicationList = new List<ApplicationModel>();

        //Mark: Accessors
        //Mark: Query always starts at 1.
        public int QueryPage { set; get; } = 1;
        /// <summary>
        /// Should be the application type ID/Key
        /// </summary>
        public string ApplicationTypeID { set; get; }
        /// <summary>
        /// Should send empty string for All applications
        /// </summary>
        public string ApplicationType
        {
            get
            {
                if (ApplicationTypeID.IsValid())
                {
                    return ApplicationTypeID.ToUpper() == "ALL" ? string.Empty : ApplicationTypeID;
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// Should be the application status ID/Key
        /// </summary>
        public string StatusDescription { set; get; } = string.Empty;
        /// <summary>
        /// Should be in yyyy/mm/dd format, call FilterHelper.FormatCreatedDate
        /// </summary>
        public string CreatedDateFrom { set; get; } = string.Empty;
        /// <summary>
        /// Should be in yyyy/mm/dd format, call FilterHelper.FormatCreatedDate
        /// </summary>
        public string CreatedDateTo { set; get; } = string.Empty;
        /// <summary>
        /// Determines if Filter has results
        /// </summary>
        public bool HasFilterResult { set; get; }
        //Mark: These 3 items are for display. Use this as display storage
        public string DisplayType { set; get; }
        public string DisplayStatus { set; get; }
        public string DisplayDate { set; get; }
        //Mark: Set this to know if the page state is filtered or clear.
        public bool IsFiltertriggered { set; get; }
        public bool IsCleartriggered { set; get; }
        public int Limit
        {
            get
            {
                string limitString = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusLanding", "displayPerQuery");
                if (!int.TryParse(limitString, out int limit))
                {
                    limit = 5;
                }
                return limit;
            }
        }
        public int Total { private set; get; }
        public double Pages { private set; get; }

        /// <summary>
        /// Response Cache if needed
        /// </summary>
        public GetAllApplicationsResponse AllApplicationResponse { set; get; }

        /// <summary>
        /// Set Data after service call. Binded directly to common service call
        /// </summary>
        /// <param name="allApplications">response.content</param>
        internal void SetData(GetAllApplicationsModel allApplications, bool isFilter)
        {
            if (isFilter)
            {
                ClearData();
            }
            if (allApplications != null && !allApplications.IsEmpty)
            {
                AppendResultList(allApplications.Applications);
            }
            Total = allApplications.Total;
            Pages = allApplications.Pages;
        }

        /// <summary>
        /// Returns All applications
        /// </summary>
        /// <returns></returns>
        /// iOS Call:
        /// 1. On display of applications
        public List<ApplicationModel> GetAllApplications()
        {
            return ApplicationList ?? new List<ApplicationModel>();
        }

        /// <summary>
        /// Returns application based on index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// Get Application for GetApplicationDetails
        public ApplicationModel GetApplication(int index)
        {
            return ApplicationList != null && index < ApplicationList.Count
                ? ApplicationList[index]
                : new ApplicationModel();
        }

        /// <summary>
        /// Returns application when View Less was tapped
        /// </summary>
        /// Call when view Less is Tapped in Landing
        public List<ApplicationModel> LessApplications
        {
            get
            {
                return ApplicationList != null && ApplicationList.Count >= Limit
                    ? ApplicationList.Take(Limit).ToList()
                    : new List<ApplicationModel>();
            }
        }

        /// <summary>
        /// Clears the cache
        /// </summary>
        /// <param name="isClearApplications">Flag that tells if application result will be cleared.</param>
        /// iOS Call:
        /// 1. Filter Page Back action and set to false.
        /// 2. Filter Page Clear CTA
        /// 3. Landing On Page Create
        public void Clear(bool isClearApplications = true)
        {
            if (isClearApplications)
            {
                ApplicationList.Clear();
            }
            QueryPage = 1;
            ApplicationTypeID = string.Empty;
            //ApplicationType = string.Empty;
            StatusDescription = string.Empty;
            CreatedDateFrom = string.Empty;
            CreatedDateTo = string.Empty;
            DisplayType = string.Empty;
            DisplayStatus = string.Empty;
            DisplayDate = string.Empty;
            AllApplicationResponse = null;
        }
        /// <summary>
        /// Reset bool properties
        /// </summary>
        public void Reset()
        {
            IsFiltertriggered = false;
            IsCleartriggered = false;
            HasFilterResult = false;
        }

        private void AppendResultList(List<ApplicationModel> list)
        {
            try
            {
                ApplicationList.AddRange(list);
            }
            catch (Exception e)
            {
                Debug.WriteLine("[Debug] AppendResultList: " + e.Message);
            }
        }

        /// <summary>
        /// Clears data
        /// </summary>
        private void ClearData()
        {
            ApplicationList = new List<ApplicationModel>();
            Total = 0;
            Pages = 0;
        }
    }
}