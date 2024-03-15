using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Firebase.Messaging;
using myTNB.AndroidApp.Src.Base.Activity;
using myTNB.AndroidApp.Src.Base.Fragments;
using System;
using System.Security.Cryptography;
using System.Text;

namespace myTNB.AndroidApp.Src.Utils
{
    public static class DeviceIdUtils
    {
        //internal static string DeviceId(this BaseToolbarAppCompatActivity context)
        //{
        //    var telephonyDeviceID = string.Empty;
        //    var telephonySIMSerialNumber = string.Empty;
        //    TelephonyManager telephonyManager = (TelephonyManager)context.GetSystemService(Context.TelephonyService);
        //    if (telephonyManager != null)
        //    {
        //        if (!string.IsNullOrEmpty(telephonyManager.DeviceId))
        //            telephonyDeviceID = telephonyManager.DeviceId;
        //        if (!string.IsNullOrEmpty(telephonyManager.SimSerialNumber))
        //            telephonySIMSerialNumber = telephonyManager.SimSerialNumber;
        //    }
        //    var androidID = Android.Provider.Settings.Secure.GetString(context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
        //    var deviceUuid = new UUID(androidID.GetHashCode(), ((long)telephonyDeviceID.GetHashCode() << 32) | telephonySIMSerialNumber.GetHashCode());
        //    return deviceUuid.ToString();
        //}

        internal static string DeviceId(this BaseAppCompatActivity context)
        {

            //var telephonyDeviceID = string.Empty;
            //var telephonySIMSerialNumber = string.Empty;
            //TelephonyManager telephonyManager = (TelephonyManager)context.GetSystemService(Context.TelephonyService);
            //if (telephonyManager != null)
            //{
            //    if (!string.IsNullOrEmpty(telephonyManager.DeviceId))
            //        telephonyDeviceID = telephonyManager.DeviceId;
            //    if (!string.IsNullOrEmpty(telephonyManager.SimSerialNumber))
            //        telephonySIMSerialNumber = telephonyManager.SimSerialNumber;
            //}
            var androidID = Android.Provider.Settings.Secure.GetString(context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
            var deviceUuid = GenerateDeviceIdentifier(context, androidID);//new UUID(androidID.GetHashCode(), ((long)telephonyDeviceID.GetHashCode() << 32) | telephonySIMSerialNumber.GetHashCode());
            return deviceUuid.ToString();

        }

        internal static string DeviceId(this BaseFragment baseFragment)
        {

            //var telephonyDeviceID = string.Empty;
            //var telephonySIMSerialNumber = string.Empty;
            //TelephonyManager telephonyManager = (TelephonyManager)baseFragment.Activity.GetSystemService(Context.TelephonyService);
            //if (telephonyManager != null)
            //{
            //    if (!string.IsNullOrEmpty(telephonyManager.DeviceId))
            //        telephonyDeviceID = telephonyManager.DeviceId;
            //    if (!string.IsNullOrEmpty(telephonyManager.SimSerialNumber))
            //        telephonySIMSerialNumber = telephonyManager.SimSerialNumber;
            //}
            var androidID = Android.Provider.Settings.Secure.GetString(baseFragment.Activity.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
            var deviceUuid = GenerateDeviceIdentifier(baseFragment.Activity, androidID);//new UUID(androidID.GetHashCode(), ((long)telephonyDeviceID.GetHashCode() << 32) | telephonySIMSerialNumber.GetHashCode());
            return deviceUuid.ToString();
        }

        internal static string DeviceId(this BaseFragmentCustom baseFragmentCustom)
        {
            var androidID = Android.Provider.Settings.Secure.GetString(baseFragmentCustom.Activity.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
            var deviceUuid = GenerateDeviceIdentifier(baseFragmentCustom.Activity, androidID);//new UUID(androidID.GetHashCode(), ((long)telephonyDeviceID.GetHashCode() << 32) | telephonySIMSerialNumber.GetHashCode());
            return deviceUuid.ToString();
        }

        internal static string DeviceId(this FirebaseMessagingService service)
        {

            //var telephonyDeviceID = string.Empty;
            //var telephonySIMSerialNumber = string.Empty;
            //TelephonyManager telephonyManager = (TelephonyManager)service.GetSystemService(Context.TelephonyService);
            //if (telephonyManager != null)
            //{
            //    if (!string.IsNullOrEmpty(telephonyManager.DeviceId))
            //        telephonyDeviceID = telephonyManager.DeviceId;
            //    if (!string.IsNullOrEmpty(telephonyManager.SimSerialNumber))
            //        telephonySIMSerialNumber = telephonyManager.SimSerialNumber;
            //}
            var androidID = Android.Provider.Settings.Secure.GetString(service.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
            var deviceUuid = GenerateDeviceIdentifier(service.ApplicationContext, androidID);//new UUID(androidID.GetHashCode(), ((long)telephonyDeviceID.GetHashCode() << 32) | telephonySIMSerialNumber.GetHashCode());
            return deviceUuid.ToString();
        }

        public static String GenerateDeviceIdentifier(Context context, String androidId)
        {

            String pseudoId = "35" +
                    Build.Board.Length % 10 +
                    Build.Brand.Length % 10 +
                    Build.CpuAbi.Length % 10 +
                    Build.Device.Length % 10 +
                    Build.Display.Length % 10 +
                    Build.Host.Length % 10 +
                    Build.Id.Length % 10 +
                    Build.Manufacturer.Length % 10 +
                    Build.Model.Length % 10 +
                    Build.Product.Length % 10 +
                    Build.Tags.Length % 10 +
                    Build.Type.Length % 10 +
                    Build.User.Length % 10;

            String longId = "";
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.S)      //Starting android 12 disable bluetooth.address checking deviceId
            {                
                longId = pseudoId + androidId;
            }
            else
            {
                BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
                String btId = "";

                if (bluetoothAdapter != null)
                {
                    btId = bluetoothAdapter.Address;
                }
                longId = pseudoId + androidId + btId;
            }

            try
            {
                // creating a hex string
                String identifier = "";

                byte[] ByteData = Encoding.ASCII.GetBytes(longId);
                //MD5 creating MD5 object.
                MD5 oMd5 = MD5.Create();
                //Hash değerini hesaplayalım.
                byte[] HashData = oMd5.ComputeHash(ByteData);

                //convert byte array to hex format
                StringBuilder oSb = new StringBuilder();

                for (int x = 0; x < HashData.Length; x++)
                {
                    //hexadecimal string value
                    oSb.Append(HashData[x].ToString("x2"));
                }

                // hex string to uppercase
                identifier = oSb.ToString();
                identifier = identifier.ToUpper();
                return identifier;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return "";
        }

        public static string GetAndroidVersion()
        {
            string androidVersion = null;
            try
            {
                androidVersion = Build.VERSION.Release;
            }
            catch (Exception e)
            {

            }
            return androidVersion;
        }

        public static string GetAppVersionName()
        {
            string appVersion = null;
            try
            {
                Context context = MyTNBApplication.Context;
                appVersion = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
            }
            catch (Exception e)
            {

            }
            return appVersion;
        }

        public static int GetAppVersionCode()
        {
            int appCode = 0;
            try
            {
                Context context = MyTNBApplication.Context;
                appCode = context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionCode;
            }
            catch (Exception e)
            {

            }
            return appCode;
        }
    }
}