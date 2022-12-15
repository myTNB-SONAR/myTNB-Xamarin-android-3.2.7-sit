
using Android.OS;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using System;
using System.Threading.Tasks;

namespace myTNB_Android.Src.NewWalkthrough.MVP
{
    public class NewWalkthroughFragment : BaseFragmentV4Custom
    {
        private static string TITLE = "slide_title";
        private static string DESCRIPTION = "slide_description";
        private static string IMAGE = "image";
        private static string IS_LAST_ITEM = "islastitem";
        private static string BG_IMAGE = "bg_image";
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

        [BindView(Resource.Id.walkthroughBottomView)]
        LinearLayout walkthroughBottomView;


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public static NewWalkthroughFragment Instance(NewWalkthroughModel model, bool isLastItem)
        {
            NewWalkthroughFragment fragment = new NewWalkthroughFragment();
            Bundle args = new Bundle();
            args.PutString(IMAGE, model.Image);
            args.PutString(TITLE, model.Title);
            args.PutString(DESCRIPTION, model.Description);
            args.PutString(BG_IMAGE, model.Background);
            args.PutBoolean(IS_LAST_ITEM, isLastItem);
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
            bool isLastItem = Arguments.GetBoolean(IS_LAST_ITEM, false);
            string bgImage = Arguments.GetString(BG_IMAGE, "");

            TextViewUtils.SetMuseoSans500Typeface(titleView);
            TextViewUtils.SetMuseoSans300Typeface(descriptionView, btnToggleEN, btnToggleMS);
            TextViewUtils.SetTextSize12(descriptionView);
            TextViewUtils.SetTextSize13(btnToggleEN, btnToggleMS);
            TextViewUtils.SetTextSize16(titleView);

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
            bool IsRewardsDisabled = MyTNBAccountManagement.GetInstance().IsRewardsDisabled();
            bool IsLargeFontDisabled = MyTNBAccountManagement.GetInstance().IsLargeFontDisabled();
            int bgResource = Resource.Drawable.OnboardingBG1;

            try
            {
                bgResource = Resources.GetIdentifier(bgImage.ToLower(), "drawable", ((NewWalkthroughActivity)Activity).PackageName);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            switch (imageUrl)
            {
                case "walkthrough_img_install_0":
                    btnToggleContainer.Visibility = ViewStates.Visible;
                    imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_0);
                    bgLayout.SetBackgroundResource(bgResource);
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
                    bgLayout.SetBackgroundResource(bgResource);
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(70f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;

                    imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    heightRatio = 216f / 250f;
                    imgHeight = (int)(imgWidth * (heightRatio));
                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
                case "walkthrough_img_install_2":
                    imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_2);
                    bgLayout.SetBackgroundResource(bgResource);
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
                    bgLayout.SetBackgroundResource(bgResource);
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
                    bgLayout.SetBackgroundResource(bgResource);
                    imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(111f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    heightRatio = 175f / 250f;
                    imgHeight = (int)(imgWidth * (heightRatio));
                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
                case "walkthrough_img_install_5":
                    imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_5);
                    bgLayout.SetBackgroundResource(bgResource);
                    imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(111f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    heightRatio = 175f / 250f;
                    imgHeight = (int)(imgWidth * (heightRatio));
                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
                case "walkthrough_img_install_6":
                    imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_6);
                    bgLayout.SetBackgroundResource(bgResource);
                    imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(111f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    heightRatio = 175f / 250f;
                    imgHeight = (int)(imgWidth * (heightRatio));
                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
                case "walkthrough_img_install_7":
                    imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_7);
                    bgLayout.SetBackgroundResource(bgResource);

                    imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(111f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    heightRatio = 175f / 250f;
                    imgHeight = (int)(imgWidth * (heightRatio));
                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
                case "walkthrough_img_install_8":
                    imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_8);
                    bgLayout.SetBackgroundResource(bgResource);

                    imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(111f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    heightRatio = 175f / 250f;
                    imgHeight = (int)(imgWidth * (heightRatio));
                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
                case "walkthrough_img_install_9":
                    //imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_9);
                    bool IsDigitalBillApplied = false; // MyTNBAccountManagement.GetInstance().IsDigitalBilApplied();
                    bool IsBillPostConversion = false;
                    if (IsDigitalBillApplied)
                    {
                        imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_9_1);
                    }
                    else if (IsBillPostConversion)
                    {
                        imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_9);
                    }
                    else
                    {
                        imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_9);
                    }

                    bgLayout.SetBackgroundResource(bgResource);

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
                    bgLayout.SetBackgroundResource(bgResource);
                    imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(111f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    heightRatio = 175f / 250f;
                    imgHeight = (int)(imgWidth * (heightRatio));
                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
                case "walkthrough_img_update_2":
                    imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_5);
                    bgLayout.SetBackgroundResource(bgResource);
                    imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(111f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    heightRatio = 175f / 250f;
                    imgHeight = (int)(imgWidth * (heightRatio));
                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
                case "walkthrough_img_update_3":
                    imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_6);
                    bgLayout.SetBackgroundResource(bgResource);
                    imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(111f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    heightRatio = 175f / 250f;
                    imgHeight = (int)(imgWidth * (heightRatio));
                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
                case "walkthrough_img_update_4":
                    imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_7);
                    bgLayout.SetBackgroundResource(bgResource);
                    imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(111f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    heightRatio = 175f / 250f;
                    imgHeight = (int)(imgWidth * (heightRatio));
                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
                case "walkthrough_img_update_5":
                    imageSource.SetImageResource(Resource.Drawable.walkthrough_img_install_8);
                    bgLayout.SetBackgroundResource(bgResource);
                    imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(111f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    heightRatio = 175f / 250f;
                    imgHeight = (int)(imgWidth * (heightRatio));
                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
                case "walkthrough_img_update_6":
                    bgLayout.SetBackgroundResource(bgResource);
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
                    bgLayout.SetBackgroundResource(bgResource);
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

            if (bgLayout.MeasuredHeight <= 0)
            {
                bgLayout.Measure(0, 0);
            }

            float diff = ((float)(DPUtils.GetHeight() - bgLayout.MeasuredHeight) / (float)DPUtils.GetHeight());

            if (isLastItem)
            {
                if (diff < 0.26f)
                {
                    int totalHeight = (int)(DPUtils.GetHeight() * 0.74f);
                    if (titleView.MeasuredHeight <= 0)
                    {
                        titleView.Measure(0, 0);
                    }
                    int leftHeight = totalHeight - titleView.MeasuredHeight - imgHeight - (int)DPUtils.ConvertDPToPx(48f);

                    LinearLayout.LayoutParams bottomParam = walkthroughBottomView.LayoutParameters as LinearLayout.LayoutParams;
                    bottomParam.Height = leftHeight;
                }
            }
            else
            {
                if (diff < 0.20f)
                {
                    int totalHeight = (int)(DPUtils.GetHeight() * 0.8f);
                    if (titleView.MeasuredHeight <= 0)
                    {
                        titleView.Measure(0, 0);
                    }
                    int leftHeight = totalHeight - titleView.MeasuredHeight - imgHeight - (int)DPUtils.ConvertDPToPx(48f);

                    LinearLayout.LayoutParams bottomParam = walkthroughBottomView.LayoutParameters as LinearLayout.LayoutParams;
                    bottomParam.Height = leftHeight;
                }
            }
        }

        [OnClick(Resource.Id.btnToggleEN)]
        void OnToggleEN(object sender, EventArgs eventArgs)
        {
            if (btnToggleEN.Selected != true)
            {
                Utility.ShowChangeLanguageDialog(this.Activity, appLanguage, () =>
                {
                    ((NewWalkthroughActivity)Activity).ShowProgressDialog();
                    AppInfoManager.Instance.SetLanguage(LanguageManager.Language.EN);
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
                if (UserEntity.IsCurrentlyActive())
                {
                    _ = LanguageUtil.SaveUpdatedLanguagePreference();
                }
                MyTNBAccountManagement.GetInstance().UpdateAppMasterData();
                _ = CheckAppMasterDataDone();
            });
        }

        private Task CheckAppMasterDataDone()
        {
            return Task.Delay(Constants.LANGUAGE_MASTER_DATA_CHECK_TIMEOUT).ContinueWith(_ =>
            {
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
                Utility.ShowChangeLanguageDialog(this.Activity, appLanguage, () =>
                {
                    ((NewWalkthroughActivity)Activity).ShowProgressDialog();
                    AppInfoManager.Instance.SetLanguage(LanguageManager.Language.MS);
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