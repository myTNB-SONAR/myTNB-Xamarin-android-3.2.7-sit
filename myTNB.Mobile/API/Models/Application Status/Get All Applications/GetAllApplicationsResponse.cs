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

        /// <summary>
        /// Determines if the response already hit the last results of all applications
        /// </summary>
        [JsonIgnore]
        public bool IsLastResults
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

        [JsonProperty("system")]
        public string System { set; get; }

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

        [JsonProperty("statusDescriptionColor")]
        public string StatusDescriptionColor { set; get; }

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

        [JsonProperty("sequence")]
        public int Sequence { set; get; }

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

        /// <summary>
        /// RGB of the Status
        /// </summary>
        [JsonIgnore]
        public int[] StatusColor
        {
            get
            {
                switch (StatusColorDisplay)
                {
                    case Color.Green:
                        {
                            return new int[] { 32, 189, 76 };
                        }
                    case Color.Orange:
                        {
                            return new int[] { 255, 158, 67 };
                        }
                    case Color.Grey:
                    default:
                        {
                            return new int[] { 73, 73, 74 };
                        }
                }
            }
        }

        [JsonIgnore]
        private Color StatusColorDisplay
        {
            get
            {
                Color color = Color.Grey;
                if (StatusDescriptionColor.IsValid())
                {

                    switch (StatusDescriptionColor.ToUpper())
                    {
                        case "COMPLETED":
                            {
                                color = Color.Green;
                                break;
                            }
                        case "ACTION":
                            {
                                color = Color.Orange;
                                break;
                            }
                        case "CANCELLED":
                        default:
                            {
                                color = Color.Grey;
                                break;
                            }
                    }
                }
                return color;
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

        [JsonIgnore]
        public bool IsUpdated { set; get; }
    }

    public enum RoleType
    {
        Developer,  //Mark: 36
        Contractor, //Mark: 2
        Individual, //Mark: 16
        None        //Mark: 0
    }
}