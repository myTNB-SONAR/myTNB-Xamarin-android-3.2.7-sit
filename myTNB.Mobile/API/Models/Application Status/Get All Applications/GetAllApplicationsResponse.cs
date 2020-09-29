using System;
using System.Collections.Generic;
using myTNB.Mobile.Extensions;
using Newtonsoft.Json;

namespace myTNB.Mobile
{
    public class GetAllApplicationsResponse : BaseResponse<GetAllApplicationsModel>
    {

    }

    public class GetAllApplicationsModel
    {
        [JsonProperty("applications")]
        public List<ApplicationModel> Applications { set; get; }

        [JsonProperty("currentPage")]
        public int CurrentPage { set; get; }

        [JsonProperty("total")]
        public int Total { set; get; }

        [JsonProperty("previousPage")]
        public string PreviousPage { set; get; }

        [JsonProperty("nextPage")]
        public string NextPage { set; get; }

        [JsonIgnore]
        public int Limit
        {
            get
            {
                if (!int.TryParse(LanguageManager.Instance.GetPageValueByKey("ApplicationStatusLanding", "displayPerQuery"), out int limit))
                {
                    limit = 5;
                }
                return limit;
            }
        }

        [JsonIgnore]
        public double Pages
        {
            get
            {
                return Math.Ceiling((double)Total / Limit);
            }
        }

        [JsonIgnore]
        public bool IsEmpty
        {
            get
            {
                return Applications == null || Applications.Count == 0;
            }
        }

        [JsonIgnore]
        public bool IsShowMoreDisplayed
        {
            get
            {
                return Total > Limit;
            }
        }

        [JsonIgnore]
        public bool IsViewLess
        {
            get
            {
                return CurrentPage == Pages;
            }
        }
    }

    public class ApplicationModel
    {
        [JsonProperty("savedApplicationId")]
        public string SavedApplicationId { set; get; }

        [JsonProperty("applicationId")]
        public string ApplicationId { set; get; }

        [JsonProperty("referenceNo")]
        public string ReferenceNo { set; get; }

        [JsonProperty("applicationModuleId")]
        public string ApplicationModuleId { set; get; }

        [JsonProperty("srNo")]
        public string SRNo { set; get; }

        [JsonProperty("srType")]
        public string SRType { set; get; }

        [JsonProperty("applicationType")]
        public string ApplicationType { set; get; }

        /// <summary>
        /// Display Application Type
        /// </summary>
        [JsonProperty("applicationModuleDescription")]
        public string ApplicationModuleDescription { set; get; }

        [JsonProperty("statusId")]
        public string StatusID { set; get; }

        [JsonProperty("statusCode")]
        public string StatusCode { set; get; }

        /// <summary>
        /// Display Status
        /// </summary>
        [JsonProperty("statusDescription")]
        public string StatusDescription { set; get; }

        [JsonProperty("isPremiseServiceReady")]
        public bool IsPremiseServiceReady { set; get; }

        [JsonProperty("reTsType")]
        public string RETSType { set; get; }

        [JsonProperty("isSmartMeter")]
        public bool IsSmartMeter { set; get; }

        [JsonProperty("isOpc")]
        public bool IsOpc { set; get; }

        [JsonProperty("isLpc")]
        public bool IsLpc { set; get; }

        [JsonProperty("isExpress")]
        public bool IsExpress { set; get; }

        [JsonProperty("isGe")]
        public bool IsGe { set; get; }

        [JsonProperty("isMeterChanged")]
        public bool IsMeterChanged { set; get; }

        [JsonProperty("isLegacyAndInvalid")]
        public bool IsLegacyAndInvalid { set; get; }

        [JsonProperty("isViewable")]
        public bool IsViewable { set; get; }

        [JsonProperty("ViewMode")]
        public string ViewMode { set; get; }

        [JsonProperty("createdBy")]
        public string CreatedBy { set; get; }

        [JsonProperty("createdByUserId")]
        public string CreatedByUserID { set; get; }

        [JsonProperty("createdByRoleId")]
        public string CreatedByRoleID { set; get; }

        [JsonProperty("createdDate")]
        public DateTime? CreatedDate { set; get; }

        [JsonProperty("lastModifiedDate")]
        public DateTime? LastModifiedDate { set; get; }

        [JsonProperty("system")]
        public string System { set; get; }

        /// <summary>
        /// Display reference number under Application Type
        /// </summary>
        [JsonIgnore]
        public string ReferenceNumberDisplay
        {
            get
            {
                string refno;
                if (SRNo.IsValid())
                {
                    refno = string.Format(LanguageManager.Instance.GetPageValueByKey("ApplicationStatusLanding", SRType.IsValid() ? "sr" : "sn"), SRNo);
                }
                else
                {
                    refno = ReferenceNo ?? string.Empty;
                }
                return refno;
            }
        }

        /// <summary>
        /// This Determines if the Application was Saved in the user listing or it was added by default
        /// </summary>
        [JsonIgnore]
        public bool IsSavedApplication
        {
            get
            {
                return SavedApplicationId.IsValid();
            }
        }

        public RoleType Role
        {
            get
            {
                RoleType rType = RoleType.None;
                if (CreatedByRoleID.IsValid())
                {
                    if (CreatedByRoleID == "2")
                    {
                        rType = RoleType.Contractor;
                    }
                    else if (CreatedByRoleID == "16")
                    {
                        rType = RoleType.Individual;
                    }
                    else if (CreatedByRoleID == "36")
                    {
                        rType = RoleType.Developer;
                    }
                }
                return rType;
            }
        }
    }

    public enum RoleType
    {
        Developer,  //36
        Contractor, //2
        Individual, //16
        None
    }
}