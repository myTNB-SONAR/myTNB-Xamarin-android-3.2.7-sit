using Android.OS;
using Constant = myTNB_Android.Src.Utils.Notification.Notification.Constants;
using Type = myTNB_Android.Src.Utils.Notification.Notification.TypeEnum;
using NotificationTypes = myTNB.Mobile.Constants.NotificationTypes;
using myTNB.Mobile;

namespace myTNB_Android.Src.Utils.Notification
{
    public sealed class NotificationUtil
    {
        static NotificationUtil instance;

        public string AccountNumber = string.Empty;
        public string NotificationType = string.Empty;
        public string PushMapId = string.Empty;
        public string Email = string.Empty;
        public Type Type = Type.None;
        public bool IsDirectPush = false;
        public ApplicationStatusNotificationModel ApplicationStatusNotifModel;

        public static NotificationUtil Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NotificationUtil();
                }
                return instance;
            }
        }

        public void SaveData(Bundle extras)
        {
            GetData(extras);
            SetIsDirectPush();
        }

        private void SetType(string type, Bundle extras)
        {
            if (type.IsValid())
            {
                switch (type.ToUpper())
                {
                    case NotificationTypes.APP_UPDATE:
                        Type = Type.AppUpdate;
                        break;
                    case NotificationTypes.DBR.ACCOUNT_STATEMENT:
                        Type = Type.AccountStatement;
                        break;
                    case NotificationTypes.DBR.NEW_BILL_DESIGN:
                        Type = Type.NewBillDesign;
                        break;
                    case NotificationTypes.MyHome.MYHOME_NC_ADDRESS_SEARCH_COMPLETED:
                        Type = Type.NCAddressSearchCompleted;
                        break;
                    case NotificationTypes.MyHome.MYHOME_NC_RESUME_APPLICATION:
                        Type = Type.NCResumeApplication;
                        break;
                    case NotificationTypes.MyHome.MYHOME_NC_APPLICATION_COMPLETED:
                        Type = Type.NCApplicationCompleted;
                        break;
                    case NotificationTypes.MyHome.MYHOME_NC_APPLICATION_CONTRACTOR_COMPLETED:
                        Type = Type.NCApplicationContractorCompleted;
                        break;
                    case NotificationTypes.MyHome.MYHOME_NC_OTP_VERIFY:
                        Type = Type.NCOTPVerify;
                        break;
                    case NotificationTypes.APPLICATIONSTATUS:
                        Type = Type.ApplicationStatus;
                        SetApplicationStatusData(extras);
                        break;
                    default:
                        break;
                }
            }
        }

        private void GetData(Bundle extras)
        {
            if (extras != null)
            {
                if (extras.ContainsKey(Constant.TYPE))
                {
                    SetType(extras.GetString(Constant.TYPE), extras);
                }
                if (extras.ContainsKey(Constant.ACCOUNT_NUMBER))
                {
                    AccountNumber = extras.GetString(Constant.ACCOUNT_NUMBER);
                }
                if (extras.ContainsKey(Constant.NOTIFICATION_TYPE))
                {
                    NotificationType = extras.GetString(Constant.NOTIFICATION_TYPE);
                }
                if (extras.ContainsKey(Constant.PUSH_MAP_ID))
                {
                    string pushMapIdValue = extras.GetString(Constant.PUSH_MAP_ID);
                    if (pushMapIdValue.IsValid() && pushMapIdValue != "null")
                    {
                        PushMapId = pushMapIdValue;
                    }
                }
                if (extras.ContainsKey(Constant.EMAIL))
                {
                    Email = extras.GetString(Constant.EMAIL);
                }
            }
        }

        private void SetApplicationStatusData(Bundle extras)
        {
            ApplicationStatusNotifModel = new ApplicationStatusNotificationModel();
            if (extras.ContainsKey(ApplicationStatusNotificationModel.Param_SAVEAPPLICATIONID))
            {
                ApplicationStatusNotifModel.SaveApplicationID = extras.GetString(ApplicationStatusNotificationModel.Param_SAVEAPPLICATIONID);
            }
            if (extras.ContainsKey(ApplicationStatusNotificationModel.Param_APPLICATIONID))
            {
                ApplicationStatusNotifModel.ApplicationID = extras.GetString(ApplicationStatusNotificationModel.Param_APPLICATIONID);
            }
            if (extras.ContainsKey(ApplicationStatusNotificationModel.Param_APPLICATIONTYPE))
            {
                ApplicationStatusNotifModel.ApplicationType = extras.GetString(ApplicationStatusNotificationModel.Param_APPLICATIONTYPE);
            }
            if (extras.ContainsKey(ApplicationStatusNotificationModel.Param_System))
            {
                ApplicationStatusNotifModel.System = extras.GetString(ApplicationStatusNotificationModel.Param_System);
            }
        }

        private void SetIsDirectPush()
        {
            IsDirectPush = Type == Type.NewBillDesign || Type == Type.ApplicationStatus || PushMapId.IsValid();
        }

        public void ClearData()
        {
            Type = Type.None;
            AccountNumber = string.Empty;
            NotificationType = string.Empty;
            PushMapId = string.Empty;
            Email = string.Empty;
            IsDirectPush = false;
            ApplicationStatusNotifModel = null;
        }
    }
}