using Android.OS;
using Constant = myTNB_Android.Src.Utils.Notification.Notification.Constants;
using Type = myTNB_Android.Src.Utils.Notification.Notification.TypeEnum;
using NotificationTypes = myTNB.Mobile.Constants.NotificationTypes;

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

        private void SetType(string type)
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
                    SetType(extras.GetString(Constant.TYPE));
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
                    PushMapId = extras.GetString(Constant.PUSH_MAP_ID);
                }
                if (extras.ContainsKey(Constant.EMAIL))
                {
                    Email = extras.GetString(Constant.EMAIL);
                }
            }
        }

        private void SetIsDirectPush()
        {
            IsDirectPush = Type == Type.NewBillDesign || PushMapId.IsValid();
        }

        public void ClearData()
        {
            Type = Type.None;
            AccountNumber = string.Empty;
            NotificationType = string.Empty;
            PushMapId = string.Empty;
            Email = string.Empty;
            IsDirectPush = false;
        }
    }
}