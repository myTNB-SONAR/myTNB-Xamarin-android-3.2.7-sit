
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;

namespace myTNB_Android.Src.NewWalkthrough.MVP
{
    public class NewWalkthroughFragment : BaseFragmentV4Custom
    {
        private static string TITLE = "slide_title";
        private static string DESCRIPTION = "slide_description";
        private static string IMAGE = "image";
        private string appLanguage;
        const string PAGE_ID = "";

        [BindView(Resource.Id.walkthrough_layout)]
        LinearLayout bgLayout;

        [BindView(Resource.Id.img_display)]
        ImageView imageSource;

        [BindView(Resource.Id.txtTitle)]
        TextView titleView;

        [BindView(Resource.Id.txtMessage)]
        TextView descriptionView;

        [BindView(Resource.Id.btnToggleEN)]
        RadioButton btnToggleEN;

        [BindView(Resource.Id.btnToggleMS)]
        RadioButton btnToggleMS;

        [BindView(Resource.Id.btnToggleContainer)]
        RelativeLayout btnToggleContainer;


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public static NewWalkthroughFragment Instance(NewWalkthroughModel model)
        {
            NewWalkthroughFragment fragment = new NewWalkthroughFragment();
            Bundle args = new Bundle();
            args.PutString(IMAGE, model.Image);
            args.PutString(TITLE, model.Title);
            args.PutString(DESCRIPTION, model.Description);
            fragment.Arguments = args;
            return fragment;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            base.OnViewCreated(view, savedInstanceState);
            string imageUrl = Arguments.GetString(IMAGE, "");
            string title = Arguments.GetString(TITLE, "");
            string description = Arguments.GetString(DESCRIPTION, "");

            TextViewUtils.SetMuseoSans500Typeface(titleView);
            TextViewUtils.SetMuseoSans300Typeface(descriptionView, btnToggleEN, btnToggleMS);

            appLanguage = LanguageUtil.GetAppLanguage();

            if (appLanguage == Constants.DEFAULT_LANG)
            {
                btnToggleEN.Checked = true;
                btnToggleEN.Selected = true;
                btnToggleMS.Selected = false;
            }
            else
            {
                btnToggleMS.Checked = true;
                btnToggleMS.Selected = true;
                btnToggleEN.Selected = false;
            }

            LinearLayout.LayoutParams imgParam;
            int imgWidth, imgHeight;
            float heightRatio;
            btnToggleContainer.Visibility = ViewStates.Gone;
            switch (imageUrl)
            {
                case "walkthrough_img_install_0":
                    btnToggleContainer.Visibility = ViewStates.Visible;
                    imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_0);
                    bgLayout.SetBackgroundResource(Resource.Drawable.InstallWalkthroughZeroBg);
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(70f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;

                    imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    heightRatio = 216f / 250f;
                    imgHeight = (int)(imgWidth * (heightRatio));
                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
                case "walkthrough_img_install_1":
                    imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_1);
                    bgLayout.SetBackgroundResource(Resource.Drawable.InstallWalkthroughFirstBg);
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int) DPUtils.ConvertDPToPx(70f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;

                    imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    heightRatio = 216f / 250f;
                    imgHeight = (int)(imgWidth * (heightRatio));
                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
                case "walkthrough_img_install_2":
                    imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_2);
                    bgLayout.SetBackgroundResource(Resource.Drawable.InstallWalkthroughSecondBg);
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(79f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;

                    imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    heightRatio = 207f / 250f;
                    imgHeight = (int)(imgWidth * (heightRatio));

                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
                case "walkthrough_img_install_3":
                    imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_3);
                    bgLayout.SetBackgroundResource(Resource.Drawable.InstallWalkthroughThirdBg);
                    imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(111f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    heightRatio = 175f / 250f;
                    imgHeight = (int)(imgWidth * (heightRatio));
                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
                case "walkthrough_img_install_4":
                    imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_4);
                    bgLayout.SetBackgroundResource(Resource.Drawable.InstallWalkthroughForthBg);
                    imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(111f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    heightRatio = 175f / 250f;
                    imgHeight = (int)(imgWidth * (heightRatio));
                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
                case "walkthrough_img_update_1":
                    imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_4);
                    bgLayout.SetBackgroundResource(Resource.Drawable.InstallWalkthroughFirstBg);
                    imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(111f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    heightRatio = 175f / 250f;
                    imgHeight = (int)(imgWidth * (heightRatio));
                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
                default:
                    imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_1);
                    bgLayout.SetBackgroundResource(Resource.Drawable.InstallWalkthroughFirstBg);
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(70f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;

                    imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    heightRatio = 216f / 250f;
                    imgHeight = (int)(imgWidth * (heightRatio));
                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
            }

            titleView.Text = title;
            descriptionView.TextFormatted = GetFormattedText(description);
        }

        [OnClick(Resource.Id.btnToggleEN)]
        void OnToggleEN(object sender, EventArgs eventArgs)
        {
            if (btnToggleEN.Selected != true)
            {
                Utility.ShowChangeLanguageDialog(Context, appLanguage, () =>
                {
                    ((NewWalkthroughActivity)Activity).ShowProgressDialog();
                    _ = RunUpdateLanguage("EN");
                }, () =>
                {
                    btnToggleMS.Checked = true;
                    btnToggleMS.Selected = true;
                    btnToggleEN.Selected = false;
                });
                btnToggleMS.Selected = false;
                btnToggleEN.Selected = true;
            }
        }

        private Task RunUpdateLanguage(string language)
        {
            return Task.Run(() =>
            {
                LanguageUtil.SaveAppLanguage(language);
                MyTNBAccountManagement.GetInstance().UpdateAppMasterData();
                _ = CheckAppMasterDataDone();
            });
        }

        private Task CheckAppMasterDataDone()
        {
            return Task.Delay(Constants.LANGUAGE_MASTER_DATA_CHECK_TIMEOUT).ContinueWith(_ => {
                if (MyTNBAccountManagement.GetInstance().GetIsAppMasterComplete())
                {
                    if (MyTNBAccountManagement.GetInstance().GetIsAppMasterFailed())
                    {
                        MyTNBAccountManagement.GetInstance().UpdateAppMasterData();
                        _ = CheckAppMasterDataDone();
                    }
                    else if (MyTNBAccountManagement.GetInstance().GetIsAppMasterMaintenance())
                    {
                        try
                        {
                            this.Activity.RunOnUiThread(() =>
                            {
                                MyTNBAccountManagement.GetInstance().ClearSitecoreItem();
                                MyTNBAccountManagement.GetInstance().ClearAppCacheItem();
                                SMRPopUpUtils.OnResetSSMRMeterReadingTimestamp();
                                ((NewWalkthroughActivity)Activity).OnMaintenanceProceed();
                            });
                        }
                        catch (Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                    }
                    else
                    {
                        try
                        {
                            this.Activity.RunOnUiThread(() =>
                            {
                                MyTNBAccountManagement.GetInstance().ClearSitecoreItem();
                                MyTNBAccountManagement.GetInstance().ClearAppCacheItem();
                                SMRPopUpUtils.OnResetSSMRMeterReadingTimestamp();
                                ((NewWalkthroughActivity)Activity).UpdateContent();
                            });
                        }
                        catch (Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                    }
                }
                else
                {
                    _ = CheckAppMasterDataDone();
                }
            });
        }

        [OnClick(Resource.Id.btnToggleMS)]
        void OnToggleMS(object sender, EventArgs eventArgs)
        {
            if (btnToggleMS.Selected != true)
            {
                Utility.ShowChangeLanguageDialog(Context, appLanguage, () =>
                {
                    ((NewWalkthroughActivity)Activity).ShowProgressDialog();
                    _ = RunUpdateLanguage("MS");
                }, () =>
                {
                    btnToggleEN.Checked = true;
                    btnToggleMS.Selected = false;
                    btnToggleEN.Selected = true;
                });
                btnToggleMS.Selected = true;
                btnToggleEN.Selected = false;
            }
        }

        public override int ResourceId()
        {
            return Resource.Layout.NewWalkthroughFragmentLayout;
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }
}
