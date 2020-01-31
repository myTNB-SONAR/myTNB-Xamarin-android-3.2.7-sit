using System.Collections.Generic;
using myTNB.Enums;
using Newtonsoft.Json;

namespace myTNB.Model
{
    public class UserNotificationResponseModel
    {
        public UserNotificationModel d { set; get; } = new UserNotificationModel();
    }

    public class UserNotificationModel : BaseModelV2
    {
        public UserNotificationsDataModel data { set; get; } = new UserNotificationsDataModel();
    }

    public class UserNotificationsDataModel
    {
        public List<UserNotificationDataModel> UserNotificationList { set; get; } = new List<UserNotificationDataModel>();
    }

    public class UserNotificationDataModel
    {
        public string Id { set; get; } = string.Empty;
        public string Email { set; get; } = string.Empty;
        public string DeviceId { set; get; } = string.Empty;
        public string AccountNum { set; get; } = string.Empty;
        public string Title { set; get; } = string.Empty;
        public string Message { set; get; } = string.Empty;
        public string IsRead { set; get; } = string.Empty;
        public string IsDeleted { set; get; } = string.Empty;
        public string NotificationTypeId { set; get; } = string.Empty;
        public string BCRMNotificationTypeId { set; get; } = string.Empty;
        public string CreatedDate { set; get; } = string.Empty;
        public AccountDetailsModel AccountDetails { set; get; }
        public string NotificationTitle { set; get; } = string.Empty;
        public string NotificationType { set; get; } = string.Empty;
        public string Target { set; get; } = string.Empty;
        public bool IsSMRPeriodOpen { set; get; }
        public string ODNBatchSubcategory { set; get; } = string.Empty;

        public class AccountDetailsModel
        {
            public string BillDate { set; get; } = string.Empty;
            public double AmountPayable { set; get; } = 0;
            public string PaymentDueDate { set; get; } = string.Empty;
        }

        [JsonIgnore]
        public bool IsValidNotification
        {
            get
            {
                if (!NotificationType.Equals("ODN"))
                {
                    if (ODNBatchSubcategory.IsValid() && ODNBatchSubcategory.ToUpper().Equals("ODNASBATCH"))
                    {
                        return true;
                    }
                    SQLite.SQLiteDataManager.UserEntity userInfo = DataManager.DataManager.SharedInstance.UserEntity?.Count > 0
                        ? DataManager.DataManager.SharedInstance.UserEntity[0]
                        : new SQLite.SQLiteDataManager.UserEntity();
                    int accIndex = -1;
                    if (DataManager.DataManager.SharedInstance != null
                    && DataManager.DataManager.SharedInstance.AccountRecordsList != null
                    && DataManager.DataManager.SharedInstance.AccountRecordsList.d != null
                    && DataManager.DataManager.SharedInstance.AccountRecordsList.d.Count > 0)
                    {
                        accIndex = DataManager.DataManager.SharedInstance.AccountRecordsList.d.FindIndex(x => x.accNum == AccountNum);
                    }
                    return Email.Equals(userInfo.email) && accIndex > -1;
                }
                return true;
            }
        }

        [JsonIgnore]
        public bool IsReadNotification
        {
            get
            {
                if (!string.IsNullOrEmpty(IsRead) && !string.IsNullOrWhiteSpace(IsRead))
                {
                    return IsRead.ToUpper() == "TRUE";
                }
                return false;
            }
        }

        [JsonIgnore]
        public BCRMNotificationEnum BCRMNotificationType
        {
            get
            {
                BCRMNotificationEnum notificationType = BCRMNotificationEnum.None;
                if (!string.IsNullOrEmpty(BCRMNotificationTypeId))
                {
                    switch (BCRMNotificationTypeId)
                    {
                        case "01":
                            notificationType = BCRMNotificationEnum.NewBill;
                            break;
                        case "02":
                            notificationType = BCRMNotificationEnum.BillDue;
                            break;
                        case "03":
                            notificationType = BCRMNotificationEnum.Dunning;
                            break;
                        case "04":
                            notificationType = BCRMNotificationEnum.Disconnection;
                            break;
                        case "05":
                            notificationType = BCRMNotificationEnum.Reconnection;
                            break;
                        /*case "97":
                            notificationType = BCRMNotificationEnum.Promotion;
                            break;
                        case "98":
                            notificationType = BCRMNotificationEnum.News;
                            break;*/
                        case "99":
                            notificationType = BCRMNotificationEnum.Maintenance;
                            break;
                        case "50":
                        case "51":
                        case "52":
                        case "53":
                        case "0009":
                        case "0010":
                        case "0011":
                            notificationType = BCRMNotificationEnum.SSMR;
                            break;
                    }
                }

                return notificationType;
            }
        }

        [JsonIgnore]
        public SSMRNotificationEnum SSMRNotificationType
        {
            get
            {
                SSMRNotificationEnum ssmrType = SSMRNotificationEnum.None;
                if (BCRMNotificationType == BCRMNotificationEnum.SSMR)
                {
                    switch (BCRMNotificationTypeId)
                    {
                        case "50":
                            ssmrType = SSMRNotificationEnum.RegistrationCompleted;
                            break;
                        case "51":
                            ssmrType = SSMRNotificationEnum.RegistrationCancelled;
                            break;
                        case "52":
                            ssmrType = SSMRNotificationEnum.TerminationCompleted;
                            break;
                        case "53":
                            ssmrType = SSMRNotificationEnum.TerminationCancelled;
                            break;
                        case "0009":
                            ssmrType = SSMRNotificationEnum.OpenMeterReadingPeriod;
                            break;
                        case "0010":
                            ssmrType = SSMRNotificationEnum.NoSubmissionReminder;
                            break;
                        case "0011":
                            ssmrType = SSMRNotificationEnum.MissedSubmission;
                            break;
                    }
                }
                return ssmrType;
            }
        }

        [JsonIgnore]
        public bool IsSelected { set; get; }
    }
}