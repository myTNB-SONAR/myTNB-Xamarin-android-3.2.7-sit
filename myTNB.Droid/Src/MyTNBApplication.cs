using Android.App;
using Android.Runtime;
using Firebase;
using Firebase.Analytics;
using myTNB_Android.Src.Database.Model;
using System;
using Xamarin.Facebook;

namespace myTNB_Android.Src
{
    //The Android Manifest contains the android:debuggable attribute, which controls whether or not the application may be debugged.
    //It is considered a good practice to set the android:debuggable attribute to false. 
    //The simplest way to do this is by adding a conditional compile statement in AssemblyInfo.cs:
#if DEBUG
    [Application(Debuggable = true, LargeHeap = true)]
#else
    [Application(Debuggable=false , LargeHeap = true)]
#endif
    public class MyTNBApplication : Android.App.Application
    {
        public static bool siteCoreUpdated = false;

        public MyTNBApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {

        }

        public override void OnCreate()
        {
            base.OnCreate();
            FirebaseApp.InitializeApp(Context);
            FirebaseAnalytics.GetInstance(Context);
            Fabric.Fabric.With(Context, new Crashlytics.Crashlytics());
            Crashlytics.Crashlytics.HandleManagedExceptions();
            FacebookSdk.SdkInitialize(ApplicationContext);
            //# if DEBUG 
            //Stetho.InitializeWithDefaults(this);
            //#endif 
            AccountTypeEntity.CreateTable();
            UserEntity.CreateTable();
            UserRegister.CreateTable();
            CustomerBillingAccount.CreateTable();
            UserNotificationEntity.CreateTable();
            NotificationTypesEntity.CreateTable();
            NotificationChannelEntity.CreateTable();
            UserNotificationChannelEntity.CreateTable();
            UserNotificationTypesEntity.CreateTable();
            NotificationFilterEntity.CreateTable();
            FirebaseTokenEntity.CreateTable();
            WeblinkEntity.CreateTable();
            LocationTypesEntity.CreateTable();
            FeedbackCategoryEntity.CreateTable();
            FeedbackStateEntity.CreateTable();
            SubmittedFeedbackEntity.CreateTable();
            FeedbackTypeEntity.CreateTable();
            SMUsageHistoryEntity.CreateTable();
            DownTimeEntity.CreateTable();
            UsageHistoryEntity.CreateTable();
            BillHistoryEntity.CreateTable();
            PaymentHistoryEntity.CreateTable();
            REPaymentHistoryEntity.CreateTable();
            AccountDataEntity.CreateTable();
            SummaryDashBoardAccountEntity.CreateTable();
            SelectBillsEntity.CreateTable();
            MyServiceEntity.CreateTable();
        }
    }
}