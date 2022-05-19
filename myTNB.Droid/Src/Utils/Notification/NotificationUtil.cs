using Android.OS;
using myTNB.Mobile;
using Constant = myTNB_Android.Src.Utils.Notification.Notification.Constants;
using Type = myTNB_Android.Src.Utils.Notification.Notification.TypeEnum;

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
        }

        private void SetType(string type)
        {
            if (type.IsValid())
            {
                switch (type.ToUpper())
                {
                    case MobileConstants.PushNotificationTypes.APP_UPDATE:
                        Type = Type.AppUpdate;
                        break;
                    case MobileConstants.PushNotificationTypes.ACCOUNT_STATEMENT:
                        Type = Type.AccountStatement;
                        break;
                    case MobileConstants.PushNotificationTypes.NEW_BILL_DESIGN:
                        Type = Type.NewBillDesign;
                        break;
                    case MobileConstants.PushNotificationTypes.EKYC_VERIFY_FIRST_NOTIFICATION:
                    case MobileConstants.PushNotificationTypes.EKYC_VERIFY_SECOND_NOTIFICATION:
                    case MobileConstants.PushNotificationTypes.EKYC_SUCCESS:
                    case MobileConstants.PushNotificationTypes.EKYC_FAILED:
                    case MobileConstants.PushNotificationTypes.EKYC_THREE_TIMES_FAILURE:
                    case MobileConstants.PushNotificationTypes.EKYC_ID_NOT_MATCH:
                        Type = Type.EKYC;
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

        public void ClearData()
        {
            Type = Type.None;
            AccountNumber = string.Empty;
            NotificationType = string.Empty;
            PushMapId = string.Empty;
            Email = string.Empty;
        }
    }
}
