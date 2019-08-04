using AFollestad.MaterialDialogs;
using Android.Graphics;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.SSMR.SMRApplication.MVP;
using myTNB_Android.Src.SSMR.SSMRMeterReadingTooltip.MVP;
using System.Collections.Generic;
using System.Net;

namespace myTNB_Android.Src.Utils
{
    public class SMRPopUpUtils
    {
        private static SMRActivityInfoResponse smrResponse;

        public static SSMRMeterReadingDialogFragment OnShowSMRMeterReadingTooltipOnActivity(bool isSinglePhase, Android.App.Activity mActivity, Android.Support.V4.App.FragmentManager mManager)
        {
            SSMRMeterReadingDialogFragment dialogFragmnet = new SSMRMeterReadingDialogFragment(mActivity, isSinglePhase);
            dialogFragmnet.Cancelable = false;
            dialogFragmnet.Show(mManager, "SMRMeterReading Dialog");
            return dialogFragmnet;
        }

        public static void OnSetSMRActivityInfoResponse(SMRActivityInfoResponse response)
        {
            smrResponse = response;
        }

        public static MaterialDialog OnBuildSMRPhotoTooltip(bool isTakePhoto, bool isSinglePhase, Android.App.Activity mActivity)
        {
            MaterialDialog popup;

            List<SMRPhotoPopUpDetailsModel> list = new List<SMRPhotoPopUpDetailsModel>();
            SMRPhotoPopUpDetailsModel item = new SMRPhotoPopUpDetailsModel();

            if (smrResponse != null)
            {
                if (isSinglePhase)
                {
                    list.AddRange(smrResponse.Response.Data.SMRPhotoPopUpDetails.FindAll(x => x.Type.Contains("Single_")));
                }
                else
                {
                    list.AddRange(smrResponse.Response.Data.SMRPhotoPopUpDetails.FindAll(x => x.Type.Contains("Multi_")));
                }

                if (isTakePhoto)
                {
                    item = list.Find(x => x.Type.Contains("TakePhoto"));
                }
                else
                {
                    item = list.Find(x => x.Type.Contains("UploadPhoto"));
                }
            }


            popup = new MaterialDialog.Builder(mActivity)
                    .CustomView(Resource.Layout.CustomDialogWithImgOneButtonLayout, false)
                    .Cancelable(false)
                    .CanceledOnTouchOutside(false)
                    .Build();

            View dialogView = popup.Window.DecorView;
            dialogView.SetBackgroundResource(Android.Resource.Color.Transparent);
            WindowManagerLayoutParams wlp = popup.Window.Attributes;
            wlp.Gravity = GravityFlags.Center;
            wlp.Width = ViewGroup.LayoutParams.MatchParent;
            wlp.Height = ViewGroup.LayoutParams.WrapContent;
            popup.Window.Attributes = wlp;


            ImageView imgPhasePhoto = popup.FindViewById<ImageView>(Resource.Id.imgPhasePhoto);
            RelativeLayout imgKvah = popup.FindViewById<RelativeLayout>(Resource.Id.imgKvah);
            RelativeLayout imgKw = popup.FindViewById<RelativeLayout>(Resource.Id.imgKw);
            RelativeLayout imgKwh = popup.FindViewById<RelativeLayout>(Resource.Id.imgKwh);

            if (isSinglePhase)
            {
                imgPhasePhoto.SetImageResource(Resource.Drawable.single_phase);
                imgKvah.Visibility = ViewStates.Gone;
                imgKw.Visibility = ViewStates.Gone;
                imgKwh.Visibility = ViewStates.Gone;
            }
            else
            {
                imgPhasePhoto.SetImageResource(Resource.Drawable.three_phase);
                imgKvah.Visibility = ViewStates.Visible;
                imgKw.Visibility = ViewStates.Visible;
                imgKwh.Visibility = ViewStates.Visible;
            }

            TextView txtTitle = popup.FindViewById<TextView>(Resource.Id.txtTitle);
            TextView txtMessage = popup.FindViewById<TextView>(Resource.Id.txtMessage);
            TextView txtBtnFirst = popup.FindViewById<TextView>(Resource.Id.txtBtnFirst);
            txtMessage.MovementMethod = new ScrollingMovementMethod();
            if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
            {
                txtMessage.TextFormatted = Html.FromHtml(item.Description, FromHtmlOptions.ModeLegacy);
            }
            else
            {
                txtMessage.TextFormatted = Html.FromHtml(item.Description);
            }

            txtTitle.Text = item.Title;
            txtBtnFirst.Text = item.CTA;
            TextViewUtils.SetMuseoSans500Typeface(txtTitle, txtBtnFirst);
            TextViewUtils.SetMuseoSans300Typeface(txtMessage);

            return popup;
        }
    }
}