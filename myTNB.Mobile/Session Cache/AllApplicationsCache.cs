using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

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
        //Mark:
        private readonly List<ApplicationModel> ApplicationList = new List<ApplicationModel>();

        //Mark: Accessors
        public int QueryPage { set; get; } = 0;
        public string SearchApplicationType { set; get; } = string.Empty;
        public string StatusDescription { set; get; } = string.Empty;
        public string CreatedDateFrom { set; get; } = string.Empty;
        public string CreatedDateTo { set; get; } = string.Empty;
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
        /// Set Data after service call. Binded directly to common service call
        /// </summary>
        /// <param name="allApplications">response.content</param>
        public void SetData(GetAllApplicationsModel allApplications)
        {
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
        public List<ApplicationModel> GetAllApplications()
        {
            return ApplicationList ?? new List<ApplicationModel>();
        }
        /// <summary>
        /// Returns application based on index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ApplicationModel GetApplication(int index)
        {
            return ApplicationList != null && index < ApplicationList.Count
                ? ApplicationList[index]
                : new ApplicationModel();
        }
        /// <summary>
        /// Returns application when View Less was tapped
        /// </summary>
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
        /// Clears cached
        /// </summary>
        public void Clear()
        {
            ApplicationList.Clear();
            QueryPage = 0;
            SearchApplicationType = string.Empty;
            StatusDescription = string.Empty;
            CreatedDateFrom = string.Empty;
            CreatedDateTo = string.Empty;
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
    }
}