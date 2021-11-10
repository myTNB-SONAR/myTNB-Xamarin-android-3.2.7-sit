using Android;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using CheeseBind;
using Java.Lang;
using myTNB_Android.Src.AppLaunch.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime;
using System.Text;
using AlertDialog = Android.App.AlertDialog;
using Constants = myTNB_Android.Src.Utils.Constants;

namespace myTNB_Android.Src.Base.Activity
{
    /// <summary>
    /// The class that abstracts the implementation of the resourceId and handling of permissions.
    /// </summary>
    public abstract class BaseAppCompatActivity : AppCompatActivity
    {
        private AlertDialog rationaleDialog;
        public abstract int ResourceId();

        public bool EnforceMissingMemberHandling { get; private set; }
        public bool IgnoreNullValues { get; private set; }

        private bool isClicked = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (savedInstanceState != null
                && savedInstanceState.GetInt("my_pid", -1) != Android.OS.Process.MyPid())
            {
                Intent LaunchViewIntent = new Intent(Application.BaseContext, typeof(LaunchViewActivity));
                LaunchViewActivity.MAKE_INITIAL_CALL = true;
                LaunchViewIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                StartActivity(LaunchViewIntent);
                Runtime.GetRuntime().Exit(0);
            }
            else
            {
                base.OnCreate(savedInstanceState);
                SetContentView(ResourceId());
                Cheeseknife.Bind(this);
                EvaluateRequestPermissions();

                try
                {
                    Configuration configuration = Resources.Configuration;
                    DeviceSizeCache.FontScale = configuration.FontScale;
                    //System.Console.WriteLine("[DEBUG] FONT SCALE 1: " + configuration.FontScale);//2, 1.5, 1.3, 1.1
                    configuration.FontScale = DeviceSizeCache.FontScale;
                    //System.Console.WriteLine("[DEBUG] FONT SCALE 2: " + configuration.FontScale);//2, 1.5, 1.3, 1.1

                    DisplayMetrics metrics = Resources.DisplayMetrics;
                    //System.Console.WriteLine("[DEBUG] SCALED DENSITY 1: " + metrics.ScaledDensity);//7, 5.25, 4.55, 3.85
                    //System.Console.WriteLine("[DEBUG] DENSITY: " + metrics.Density);//3.5, 3.5, 3.5, 3.5
                    //System.Console.WriteLine("[DEBUG] DENSITYDPI: " + metrics.DensityDpi);//D560, D560, D560, D560
                    //280 //Xhigh //D360
                    metrics.ScaledDensity = configuration.FontScale * metrics.Density;
                    //System.Console.WriteLine("[DEBUG] SCALED DENSITY 2: " + metrics.ScaledDensity);

                    if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1)
                    {
                        configuration.SetLocale(LocaleUtils.GetCurrentLocale());
                        CreateConfigurationContext(configuration);
                    }
                    else
                    {
                        configuration.Locale = LocaleUtils.GetCurrentLocale();
                        Resources.UpdateConfiguration(configuration, Resources.DisplayMetrics);
                    }
                }
                catch (Java.Lang.Exception javaEx)
                {
                    Console.WriteLine("[DEBUG] configuration.DensityDpi Java Exception: " + javaEx.Message);
                }
                catch (System.Exception sysEx)
                {
                    Console.WriteLine("[DEBUG] configuration.DensityDpi System Exception: " + sysEx.Message);
                }
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            this.isClicked = false;
            Ready();
        }

        public virtual void SetIsClicked(bool flag)
        {
            this.isClicked = flag;
        }

        public virtual bool GetIsClicked()
        {
            return this.isClicked;
        }

        protected override void OnPause()
        {
            base.OnPause();
            this.isClicked = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutInt("my_pid", Android.OS.Process.MyPid());
        }

        private void EvaluateRequestPermissions()
        {
#if DEBUG || DEVELOP
            //CrashManager.Register(this);
#endif
            if (CameraPermissionRequired())
            {
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) != (int)Permission.Granted)
                {
                    if (ShouldShowRequestPermissionRationale(Manifest.Permission.Camera))
                    {
                        ShowRationale(Resource.String.runtime_permission_dialog_camera_title, Resource.String.runtime_permission_camera_rationale, Constants.RUNTIME_PERMISSION_CAMERA_REQUEST_CODE);
                    }
                    else
                    {
                        RequestPermissions(new string[] { Manifest.Permission.Camera, Manifest.Permission.Flashlight }, Constants.RUNTIME_PERMISSION_CAMERA_REQUEST_CODE);
                    }
                    return;
                }
            }

            if (TelephonyPermissionRequired())
            {
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadPhoneState) != (int)Permission.Granted)
                {
                    if (ShouldShowRequestPermissionRationale(Manifest.Permission.ReadPhoneState))
                    {
                        ShowRationale(Resource.String.runtime_permission_dialog_phone_title, Resource.String.runtime_permission_phone_rationale, Constants.RUNTIME_PERMISSION_PHONE_REQUEST_CODE);
                    }
                    else
                    {
                        RequestPermissions(new string[] { Manifest.Permission.ReadPhoneState }, Constants.RUNTIME_PERMISSION_PHONE_REQUEST_CODE);
                    }
                    return;
                }
            }

            if (StoragePermissionRequired())
            {
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted &&
                    ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted)
                {
                    if (ShouldShowRequestPermissionRationale(Manifest.Permission.WriteExternalStorage) || ShouldShowRequestPermissionRationale(Manifest.Permission.ReadExternalStorage))
                    {
                        ShowRationale(Resource.String.runtime_permission_dialog_storage_title, Resource.String.runtime_permission_storage_rationale, Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE);
                    }
                    else
                    {
                        RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage }, Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE);
                    }
                    return;
                }
            }

            if (LocationPermissionRequired())
            {
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != (int)Permission.Granted && ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != (int)Permission.Granted)
                {
                    if (ShouldShowRequestPermissionRationale(Manifest.Permission.AccessFineLocation) || ShouldShowRequestPermissionRationale(Manifest.Permission.AccessCoarseLocation))
                    {
                        ShowRationale(LocationTitleRationale(), LocationContentRationale(), Constants.RUNTIME_PERMISSION_LOCATION_REQUEST_CODE);
                    }
                    else
                    {
                        RequestPermissions(new string[] { Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation }, Constants.RUNTIME_PERMISSION_LOCATION_REQUEST_CODE);
                    }
                    return;
                }
            }

            if (CalendarPemissionRequired())
            {
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadCalendar) != (int)Permission.Granted
                    && ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteCalendar) != (int)Permission.Granted)
                {
                    if (ShouldShowRequestPermissionRationale(Manifest.Permission.ReadCalendar)
                        || ShouldShowRequestPermissionRationale(Manifest.Permission.WriteCalendar))
                    {
                        ShowRationale(CalendarTitleRationale(), CalendarContentRationale()
                            , Constants.RUNTIME_PERMISSION_CALENDAR_REQUEST_CODE);
                    }
                    else
                    {
                        RequestPermissions(new string[] { Manifest.Permission.ReadCalendar, Manifest.Permission.WriteCalendar }
                            , Constants.RUNTIME_PERMISSION_CALENDAR_REQUEST_CODE);
                    }
                    return;
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode == Constants.RUNTIME_PERMISSION_CAMERA_REQUEST_CODE)
            {
                if (Utility.IsPermissionHasCount(grantResults))
                {
                    if (grantResults[0] == Permission.Denied)
                    {

                    }
                }
            }
            else if (requestCode == Constants.RUNTIME_PERMISSION_PHONE_REQUEST_CODE)
            {
                if (Utility.IsPermissionHasCount(grantResults))
                {
                    if (grantResults[0] == Permission.Denied)
                    {

                    }
                }
            }
            else if (requestCode == Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE)
            {
                if (Utility.IsPermissionHasCount(grantResults))
                {
                    if (grantResults[0] == Permission.Denied)
                    {

                    }
                }
            }
            else if (requestCode == Constants.RUNTIME_PERMISSION_LOCATION_REQUEST_CODE)
            {
                if (Utility.IsPermissionHasCount(grantResults))
                {
                    if (grantResults[0] == Permission.Denied)
                    {
                        if (!LocationPermissionRequired())
                        {
                            if (ShouldShowRequestPermissionRationale(Manifest.Permission.AccessFineLocation) || ShouldShowRequestPermissionRationale(Manifest.Permission.AccessCoarseLocation))
                            {
                                ShowRationale(LocationTitleRationale(),
                                    LocationContentRationale(),
                                    Constants.RUNTIME_PERMISSION_LOCATION_REQUEST_CODE,
                                    Resource.String.faulty_street_lamps_feedback_runtime_permission_dialog_btn_ok, delegate
                                    {
                                        rationaleDialog.Dismiss();
                                        if (requestCode == Constants.RUNTIME_PERMISSION_CAMERA_REQUEST_CODE)
                                        {
                                            RequestPermissions(new string[] { Manifest.Permission.Camera, Manifest.Permission.Flashlight }, Constants.RUNTIME_PERMISSION_CAMERA_REQUEST_CODE);

                                        }
                                        else if (requestCode == Constants.RUNTIME_PERMISSION_PHONE_REQUEST_CODE)
                                        {
                                            RequestPermissions(new string[] { Manifest.Permission.ReadPhoneState }, Constants.RUNTIME_PERMISSION_PHONE_REQUEST_CODE);

                                        }
                                        else if (requestCode == Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE)
                                        {
                                            RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage }, Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE);

                                        }
                                        else if (requestCode == Constants.RUNTIME_PERMISSION_LOCATION_REQUEST_CODE)
                                        {
                                            RequestPermissions(new string[] { Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation }, Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE);

                                        }
                                    },
                                    Resource.String.faulty_street_lamps_feedback_runtime_permission_dialog_btn_ok, delegate
                                    {
                                        rationaleDialog.Dismiss();
                                    });
                            }
                        }
                    }
                }
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void ShowRationale(int rationaleTitle, int rationaleContent, int requestCode)
        {
            if (rationaleDialog != null && rationaleDialog.IsShowing)
            {
                rationaleDialog.Dismiss();
            }

            rationaleDialog = new AlertDialog.Builder(this)
                .SetTitle(rationaleTitle)
                .SetMessage(rationaleContent)
                .SetCancelable(false)
                .SetPositiveButton(GetString(Resource.String.runtime_permission_dialog_btn_close), delegate
                 {
                     rationaleDialog.Dismiss();
                     if (requestCode == Constants.RUNTIME_PERMISSION_CAMERA_REQUEST_CODE)
                     {
                         RequestPermissions(new string[] { Manifest.Permission.Camera
                             , Manifest.Permission.Flashlight }
                         , Constants.RUNTIME_PERMISSION_CAMERA_REQUEST_CODE);
                     }
                     else if (requestCode == Constants.RUNTIME_PERMISSION_PHONE_REQUEST_CODE)
                     {
                         RequestPermissions(new string[] { Manifest.Permission.ReadPhoneState }
                         , Constants.RUNTIME_PERMISSION_PHONE_REQUEST_CODE);

                     }
                     else if (requestCode == Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE)
                     {
                         RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage
                             , Manifest.Permission.ReadExternalStorage }
                         , Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE);

                     }
                     else if (requestCode == Constants.RUNTIME_PERMISSION_LOCATION_REQUEST_CODE)
                     {
                         RequestPermissions(new string[] { Manifest.Permission.AccessFineLocation
                             , Manifest.Permission.AccessCoarseLocation }
                         , Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE);
                     }
                 })
                .Show();
            int titleId = Resources.GetIdentifier("alertTitle", "id", "android");
            TextView txtTitle = rationaleDialog.FindViewById<TextView>(titleId);
            TextView txtMessage = rationaleDialog.FindViewById<TextView>(Android.Resource.Id.Message);
            Button positiveButton = rationaleDialog.GetButton((int)DialogButtonType.Positive);
            TextViewUtils.SetMuseoSans500Typeface(txtTitle, positiveButton);
            TextViewUtils.SetMuseoSans300Typeface(txtMessage);
        }

        public void ShowRationale(int rationaleTitle
            , int rationaleContent
            , int requestCode
            , int positiveStringButton
            , EventHandler<DialogClickEventArgs> positiveButtonEvent
            , int negativeStringButton
            , EventHandler<DialogClickEventArgs> negativeButtonEvent)
        {
            if (rationaleDialog != null && rationaleDialog.IsShowing)
            {
                rationaleDialog.Dismiss();
            }

            rationaleDialog = new AlertDialog.Builder(this)
                .SetTitle(rationaleTitle)
                .SetMessage(rationaleContent)
                .SetCancelable(false)
                .SetPositiveButton(GetString(positiveStringButton), positiveButtonEvent)
                .SetNegativeButton(GetString(negativeStringButton), negativeButtonEvent)
                .Show();
            int titleId = Resources.GetIdentifier("alertTitle", "id", "android");
            TextView txtTitle = rationaleDialog.FindViewById<TextView>(titleId);
            TextView txtMessage = rationaleDialog.FindViewById<TextView>(Android.Resource.Id.Message);
            Button positiveButton = rationaleDialog.GetButton((int)DialogButtonType.Positive);
            Button negativeButton = rationaleDialog.GetButton((int)DialogButtonType.Negative);
            positiveButton.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.permissionButtonColor)));
            negativeButton.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, Resource.Color.permissionButtonColor)));
            TextViewUtils.SetMuseoSans500Typeface(txtTitle, positiveButton);
            TextViewUtils.SetMuseoSans300Typeface(txtMessage);
        }

        public virtual bool CalendarPemissionRequired()
        {
            return false;
        }

        public virtual bool CameraPermissionRequired()
        {
            return false;
        }

        public virtual bool StoragePermissionRequired()
        {
            return false;
        }

        public virtual bool TelephonyPermissionRequired()
        {
            return false;
        }

        public virtual bool LocationPermissionRequired()
        {
            return false;
        }

        public string GetCameraRationale()
        {
            return GetString(Resource.String.runtime_permission_camera_rationale);
        }

        public string GetStorageRationale()
        {
            return GetString(Resource.String.runtime_permission_storage_rationale);
        }

        public string GetTelephonyRationale()
        {
            return GetString(Resource.String.runtime_permission_phone_rationale);
        }

        public string GetLocationRationale()
        {
            return GetString(Resource.String.runtime_permission_location_rationale);
        }

        public virtual int LocationTitleRationale()
        {
            return Resource.String.runtime_permission_dialog_location_title;
        }

        public virtual int LocationContentRationale()
        {
            return Resource.String.runtime_permission_location_rationale;
        }

        public virtual int CalendarTitleRationale()
        {
            return Resource.String.runtime_permission_dialog_calendar_title;
        }

        public virtual int CalendarContentRationale()
        {
            return Resource.String.runtime_permission_calendar_rationale;
        }

        public bool IsLocationGranted()
        {
            return ContextCompat.CheckSelfPermission(this
                    , Manifest.Permission.AccessFineLocation) == (int)Permission.Granted
                && ContextCompat.CheckSelfPermission(this
                    , Manifest.Permission.AccessCoarseLocation) == (int)Permission.Granted;
        }

        public void InitiateLocationPermission()
        {
            ShowRationale(LocationTitleRationale()
                , LocationContentRationale()
                , Constants.RUNTIME_PERMISSION_LOCATION_REQUEST_CODE
                , Resource.String.faulty_street_lamps_feedback_runtime_permission_dialog_btn_ok
                    , delegate
                    {
                        rationaleDialog.Dismiss();
                        RequestPermissions(new string[] { Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation }, Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE);
                    }
                , Resource.String.faulty_street_lamps_feedback_runtime_permission_dialog_btn_cancel
                    , delegate
                    {
                        rationaleDialog.Dismiss();
                    });
        }

        public virtual void Ready()
        {

        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }

        protected TResult DeSerialze<TResult>(string responseStream)
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(responseStream.ToCharArray());
            using (var stream = new MemoryStream(jsonBytes))
            {
                return Deserialize<TResult>(stream);
            }
        }

        private TResult Deserialize<TResult>(Stream responseStream)
        {
            using (var sr = new StreamReader(responseStream))
            {
                using (var reader = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer
                    {
                        MissingMemberHandling =
                            EnforceMissingMemberHandling ? MissingMemberHandling.Error : MissingMemberHandling.Ignore,
                        NullValueHandling = IgnoreNullValues ? NullValueHandling.Ignore : NullValueHandling.Include
                    };

                    return serializer.Deserialize<TResult>(reader);
                }
            }
        }
    }
}