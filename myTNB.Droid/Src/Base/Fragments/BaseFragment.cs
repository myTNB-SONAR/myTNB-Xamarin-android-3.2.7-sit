using Android;
using AndroidX.Fragment.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using myTNB.AndroidApp.Src.Utils;
using System;
using AlertDialog = Android.App.AlertDialog;
using Constants = myTNB.AndroidApp.Src.Utils.Constants;
using Android.Util;

namespace myTNB.AndroidApp.Src.Base.Fragments
{
    /// <summary>
    /// The class that abstracts the implementation of the resourceId and permissions
    /// </summary>
    public abstract class BaseFragment : Fragment
    {
        private AlertDialog rationaleDialog;

        private bool isClicked = false;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View inflateView = null;
            try
            {
                inflateView = inflater.Inflate(ResourceId(), container, false);
                Cheeseknife.Bind(this, inflateView);
                EvaluateRequestPermissions();

                try
                {
                    Android.Content.Res.Configuration configuration = Resources.Configuration;
                    DeviceSizeCache.FontScale = configuration.FontScale;
                    configuration.FontScale = DeviceSizeCache.FontScale;
                    DisplayMetrics metrics = Resources.DisplayMetrics;
                    metrics.ScaledDensity = configuration.FontScale * metrics.Density;
                    Resources.UpdateConfiguration(configuration, metrics);
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
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return inflateView;
        }

        /// <summary>
        /// The layout resource id
        /// </summary>
        /// <returns></returns>
        public abstract int ResourceId();

        public override void OnResume()
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

        public override void OnPause()
        {
            base.OnPause();
            this.isClicked = true;
        }


        private void EvaluateRequestPermissions()
        {
            if (CameraPermissionRequired())
            {
                if (ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.Camera) != (int)Permission.Granted)
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
                if (ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.ReadPhoneState) != (int)Permission.Granted)
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
                if (ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted &&
                    ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted)
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
                if (ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.AccessFineLocation) != (int)Permission.Granted && ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.AccessCoarseLocation) != (int)Permission.Granted)
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

            rationaleDialog = new AlertDialog.Builder(this.Activity)
                .SetTitle(rationaleTitle)
                .SetMessage(rationaleContent)
                .SetCancelable(false)
                .SetPositiveButton(GetString(Resource.String.runtime_permission_dialog_btn_close), delegate
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
                })
                .Show();
            int titleId = Resources.GetIdentifier("alertTitle", "id", "android");
            TextView txtTitle = rationaleDialog.FindViewById<TextView>(titleId);
            TextView txtMessage = rationaleDialog.FindViewById<TextView>(Android.Resource.Id.Message);
            Button positiveButton = rationaleDialog.GetButton((int)DialogButtonType.Positive);
            TextViewUtils.SetMuseoSans500Typeface(txtTitle, positiveButton);
            TextViewUtils.SetMuseoSans300Typeface(txtMessage);

        }

        public void ShowRationale(int rationaleTitle, int rationaleContent, int requestCode, int positiveStringButton, EventHandler<DialogClickEventArgs> positiveButtonEvent, int negativeStringButton, EventHandler<DialogClickEventArgs> negativeButtonEvent)
        {
            if (rationaleDialog != null && rationaleDialog.IsShowing)
            {
                rationaleDialog.Dismiss();
            }

            rationaleDialog = new AlertDialog.Builder(this.Activity)
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
            positiveButton.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.permissionButtonColor)));
            negativeButton.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.permissionButtonColor)));
            TextViewUtils.SetMuseoSans500Typeface(txtTitle, positiveButton);
            TextViewUtils.SetMuseoSans300Typeface(txtMessage);

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

        public bool IsLocationGranted()
        {
            return ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.AccessFineLocation) == (int)Permission.Granted && ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.AccessCoarseLocation) == (int)Permission.Granted;
        }

        public void InitiateLocationPermission()
        {
            ShowRationale(LocationTitleRationale(),
                                LocationContentRationale(),
                                Constants.RUNTIME_PERMISSION_LOCATION_REQUEST_CODE,
                                Resource.String.faulty_street_lamps_feedback_runtime_permission_dialog_btn_ok, delegate
                                {
                                    rationaleDialog.Dismiss();
                                    RequestPermissions(new string[] { Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation }, Constants.RUNTIME_PERMISSION_STORAGE_REQUEST_CODE);

                                },
                                Resource.String.faulty_street_lamps_feedback_runtime_permission_dialog_btn_cancel, delegate
                                {
                                    rationaleDialog.Dismiss();
                                });
        }

        public virtual void Ready()
        {

        }


        protected Intent GetIntentObject(Type type)
        {
            if (GetActivityObject() != null)
            {
                return new Intent(GetActivityObject(), type);
            }
            else if (this.Activity != null)
            {
                return new Intent(this.Activity, type);
            }
            return null;
        }


        protected virtual Android.App.Activity GetActivityObject()
        {
            return null;
        }

        public int GetDeviceHorizontalScaleInPixel(float percentageValue)
        {
            var deviceWidth = Resources.DisplayMetrics.WidthPixels;
            return GetScaleInPixel(deviceWidth, percentageValue);
        }

        public int GetDeviceVerticalScaleInPixel(float percentageValue)
        {
            var deviceHeight = Resources.DisplayMetrics.HeightPixels;
            return GetScaleInPixel(deviceHeight, percentageValue);
        }

        public int GetScaleInPixel(int basePixel, float percentageValue)
        {
            int scaledInPixel = (int)((float)basePixel * percentageValue);
            return scaledInPixel;
        }
    }
}
