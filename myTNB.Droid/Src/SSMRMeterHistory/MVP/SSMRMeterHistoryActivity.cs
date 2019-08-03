using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.AppLaunch.Models;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.SSMRMeterHistory.Adapter;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB_Android.Src.SSMRMeterHistory.MVP
{
	[Activity(Label = "@string/ssmr_meter_history_activity_title"
		  , ScreenOrientation = ScreenOrientation.Portrait
		  , Theme = "@style/Theme.SSMRMeterHistoryStyle")]
	public class SSMRMeterHistoryActivity : BaseToolbarAppCompatActivity
	{
        [BindView(Resource.Id.smr_submitted_img)]
        ImageView SMRMainImg;

        [BindView(Resource.Id.smr_submitted_title)]
        TextView SMRMainTitle;

        [BindView(Resource.Id.smr_submitted_content)]
        TextView SMRMainContent;

        [BindView(Resource.Id.smr_content_history_header)]
        TextView SMRListHeader;

        [BindView(Resource.Id.btnSubmitMeter)]
        Button btnSubmitMeter;

        [BindView(Resource.Id.smr_history_recyclerview)]
        RecyclerView mSMRRecyclerView;

        private IMenu ssmrMenu;

        private SMRActivityInfoResponse smrResponse;

        private AccountData selectedAccount;

        private MaterialDialog SSMRMenuDialog;

        public override int ResourceId()
		{
			return Resource.Layout.SSMRMeterHistoryLayout;
		}

		public override bool ShowCustomToolbarTitle()
		{
			return true;
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			try
			{
                this.toolbar.SetBackgroundColor(Resources.GetColor(Resource.Color.action_color));
                if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Lollipop)
                {
                    this.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                    this.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                    this.Window.SetStatusBarColor(Resources.GetColor(Resource.Color.action_color));
                }

                TextViewUtils.SetMuseoSans500Typeface(SMRMainTitle, SMRListHeader, btnSubmitMeter);
                TextViewUtils.SetMuseoSans300Typeface(SMRMainContent);

                mSMRRecyclerView.SetLayoutManager(new LinearLayoutManager(this));

                Bundle extras = Intent.Extras;

                if (extras.ContainsKey(Constants.SELECTED_ACCOUNT))
                {
                    selectedAccount = JsonConvert.DeserializeObject<AccountData>(extras.GetString(Constants.SELECTED_ACCOUNT));
                }

                if (extras.ContainsKey(Constants.SMR_RESPONSE_KEY))
                {
                    smrResponse = JsonConvert.DeserializeObject<SMRActivityInfoResponse>(extras.GetString(Constants.SMR_RESPONSE_KEY));
                    SSMRMeterHistoryAdapter adapter = new SSMRMeterHistoryAdapter(smrResponse.Response.Data.MeterReadingHistory);
                    mSMRRecyclerView.SetAdapter(adapter);

                    // TODO: Enable when confirm
                    /*if (smrResponse.Response.Data.DashboardCTAType == Constants.SMR_SUBMIT_METER_KEY)
                    {
                        SMRMainImg.SetImageResource(Resource.Drawable.smr_open);
                        // TODO: Enable when confirm
                        // btnSubmitMeter.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        // TODO: Enable when confirm
                        // btnSubmitMeter.Visibility = ViewStates.Gone;
                        if (selectedAccount != null)
                        {
                            CustomerBillingAccount selectedCustomer = CustomerBillingAccount.FindByAccNum(selectedAccount.AccountNum);
                            if (selectedCustomer.IsPeriodOpen)
                            {
                                SMRMainImg.SetImageResource(Resource.Drawable.smr_submitted);
                            }
                            else
                            {
                                SMRMainImg.SetImageResource(Resource.Drawable.smr_closed);
                            }
                        }
                        else
                        {
                            SMRMainImg.SetImageResource(Resource.Drawable.smr_closed);
                        }
                    }*/

                    if(!string.IsNullOrEmpty(smrResponse.Response.Data.HistoryViewTitle))
                    {
                        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.N)
                        {
                            SMRMainTitle.TextFormatted = Html.FromHtml(smrResponse.Response.Data.HistoryViewTitle, FromHtmlOptions.ModeLegacy);
                        }
                        else
                        {
                            SMRMainTitle.TextFormatted = Html.FromHtml(smrResponse.Response.Data.HistoryViewTitle);
                        }
                    }

                    if (!string.IsNullOrEmpty(smrResponse.Response.Data.HistoryViewMessage))
                    {
                        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.N)
                        {
                            SMRMainContent.TextFormatted = Html.FromHtml(smrResponse.Response.Data.HistoryViewMessage, FromHtmlOptions.ModeLegacy);
                        }
                        else
                        {
                            SMRMainContent.TextFormatted = Html.FromHtml(smrResponse.Response.Data.HistoryViewMessage);
                        }
                    }
                }

                /* SMRPhotoTooltip */
                // How to use SMRPhotoTooltip
                // 1) need to build your own function for this
                // 2) then call that function
                // How to build your own function, refer to down there
                // This tooltip must be only can be use after first call of GetSMRAccountActivityInfo

                // Note:
                // Button Integration is yet done on this

                // Example
                // OnShowSMRPhotoTooltip(true, true, this);
                // OnShowSMRPhotoTooltip(true, false, this);
                // OnShowSMRPhotoTooltip(false, true, this);
                // OnShowSMRPhotoTooltip(false, false, this);
                /* SMRPhotoTooltip */



                /* SMRMeterReadingTooltip */
                // How to use SMRMeterReadingTooltip
                // SMRPopUpUtils.OnShowSMRMeterReadingTooltipOnActivity(isSinglePhase, your Activity, SupportFragmentManager);

                // Only Limitation
                // Only Can be use on Activity

                // Note:
                // Button Integration is done on this (if I did not missed)
                // When checked on the checkbox, and click I'm Ready, it will write to here:
                // MyTNBAccountManagement.GetInstance().UpdateIsSMRMeterReadingOnboardingShown();
                // The checking for that flag:
                // MyTNBAccountManagement.GetInstance().IsSMRMeterReadingOnboardingShown();

                // Example
                // SMRPopUpUtils.OnShowSMRMeterReadingTooltipOnActivity(true, this, SupportFragmentManager);
                // SMRPopUpUtils.OnShowSMRMeterReadingTooltipOnActivity(false, this, SupportFragmentManager);
                /* SMRMeterReadingTooltip */
            }
            catch (Exception e)
			{
				Utility.LoggingNonFatalError(e);
			}
		}

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SSMRMeterReadingMenu, menu);
            ssmrMenu = menu;
            return base.OnCreateOptionsMenu(menu);
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_ssmr_more:
                    ssmrMenu.FindItem(Resource.Id.action_ssmr_more).SetVisible(false);
                    OnClickSMRMenuMore();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void OnClickSMRMenuMore()
        {
            try
            {
                SSMRMenuDialog = new MaterialDialog.Builder(this)
                    .CustomView(Resource.Layout.SSMRMenuListLayout, false)
                    .Cancelable(false)
                    .CanceledOnTouchOutside(false)
                    .Build();

                View dialogView = SSMRMenuDialog.Window.DecorView;
                dialogView.SetBackgroundResource(Android.Resource.Color.Transparent);
                WindowManagerLayoutParams wlp = SSMRMenuDialog.Window.Attributes;
                wlp.Gravity = GravityFlags.Top;
                wlp.Width = ViewGroup.LayoutParams.MatchParent;
                wlp.Height = ViewGroup.LayoutParams.WrapContent;
                SSMRMenuDialog.Window.Attributes = wlp;

                ImageView btnSMRMenuClose = SSMRMenuDialog.FindViewById<ImageView>(Resource.Id.btnSMRMenuClose);
                TextView btnReadMeter = SSMRMenuDialog.FindViewById<TextView>(Resource.Id.btnReadMeter);
                TextView btnWhenSubmitMeter = SSMRMenuDialog.FindViewById<TextView>(Resource.Id.btnWhenSubmitMeter);
                TextView btnWhyReadingRejected = SSMRMenuDialog.FindViewById<TextView>(Resource.Id.btnWhyReadingRejected);
                TextView btnDiscontinueSMR = SSMRMenuDialog.FindViewById<TextView>(Resource.Id.btnDiscontinueSMR);
                TextViewUtils.SetMuseoSans500Typeface(btnDiscontinueSMR);
                TextViewUtils.SetMuseoSans300Typeface(btnWhyReadingRejected, btnWhenSubmitMeter, btnReadMeter);
                btnSMRMenuClose.Click += delegate
                {
                    ssmrMenu.FindItem(Resource.Id.action_ssmr_more).SetVisible(true);
                    SSMRMenuDialog.Dismiss();
                };
                btnReadMeter.Click += delegate
                {
                    ssmrMenu.FindItem(Resource.Id.action_ssmr_more).SetVisible(true);
                    SSMRMenuDialog.Dismiss();
                };
                btnWhenSubmitMeter.Click += delegate
                {
                    ssmrMenu.FindItem(Resource.Id.action_ssmr_more).SetVisible(true);
                    SSMRMenuDialog.Dismiss();
                };
                btnWhyReadingRejected.Click += delegate
                {
                    ssmrMenu.FindItem(Resource.Id.action_ssmr_more).SetVisible(true);
                    SSMRMenuDialog.Dismiss();
                };
                btnDiscontinueSMR.Click += delegate
                {
                    ssmrMenu.FindItem(Resource.Id.action_ssmr_more).SetVisible(true);
                    SSMRMenuDialog.Dismiss();
                };

                SSMRMenuDialog.Show();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.btnSubmitMeter)]
        internal void OnSubmitMeter(object sender, EventArgs eventArgs)
        {
            // REMARK TODO for Chris from LinSiong: Submit Meter Goes Here;
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

        /* SMRPhotoTooltip */
        // How to use this Tooltip
        private void OnShowSMRPhotoTooltip(bool isTakePhoto, bool isSinglePhase, Activity mActivity)
        {
            MaterialDialog myDiaLog = SMRPopUpUtils.OnBuildSMRPhotoTooltip(isTakePhoto, isSinglePhase, mActivity);
            LinearLayout btnFirst = myDiaLog.FindViewById<LinearLayout>(Resource.Id.btnFirst);

            btnFirst.Click += delegate
            {
                // Do your handling here
                myDiaLog.Dismiss();
            };

            myDiaLog.Show();
        }
        /* SMRPhotoTooltip */
    }
}