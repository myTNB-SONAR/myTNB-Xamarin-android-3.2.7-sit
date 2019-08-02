using AFollestad.MaterialDialogs;
using Android.Support.V4.View;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.SSMR.Adapter;
using System.Collections.Generic;
using System.Net;

namespace myTNB_Android.Src.Utils
{
    public class SMRSubmitMeterPopUpUtil
    {
        public static MaterialDialog ShowSubmitMeterPopUp(Android.App.Activity mActivity, bool isSinglePhase)
        {
            MaterialDialog popUp = new MaterialDialog.Builder(mActivity)
                    .CustomView(Resource.Layout.CustomDialogWithImgViewPageOneButtonLayout, false)
                    .Cancelable(false)
                    .CanceledOnTouchOutside(false)
                    .Build();

            View dialogView = popUp.Window.DecorView;
            dialogView.SetBackgroundResource(Android.Resource.Color.Transparent);
            WindowManagerLayoutParams wlp = popUp.Window.Attributes;
            wlp.Gravity = GravityFlags.Center;
            wlp.Width = ViewGroup.LayoutParams.MatchParent;
            wlp.Height = ViewGroup.LayoutParams.WrapContent;
            popUp.Window.Attributes = wlp;

            List<SSMRMeterReadingModel> SSMRMeterReadingModelList = new List<SSMRMeterReadingModel>();
            SSMRMeterReadingScreensEntity entity = new SSMRMeterReadingScreensEntity();
            List<SSMRMeterReadingScreensEntity> items = entity.GetAllItems();
            if (items.Count > 0)
            {
                foreach (SSMRMeterReadingModel model in items)
                {
                    SSMRMeterReadingModel dataModel = new SSMRMeterReadingModel();
                    dataModel.Image = model.Image;
                    dataModel.Title = model.Title;
                    dataModel.Description = model.Description;
                    SSMRMeterReadingModelList.Add(dataModel);
                }
            }
            else
            {
                if (!isSinglePhase)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        SSMRMeterReadingModel dataModel = new SSMRMeterReadingModel();
                        if (i == 0)
                        {
                            dataModel.Image = "tooltip_bg_1";
                            dataModel.Title = "Alright, what do I need to read?";
                            dataModel.Description = "You'll need to read 3 reading values (kWh, kVARh, kW). Your meter will automatically flash one after the other.";
                        }
                        else if (i == 1)
                        {
                            dataModel.Image = "tooltip_bg_2";
                            dataModel.Title = "But wait, how do I read my meter?";
                            dataModel.Description = "You can enter each reading manually or just snap/upload a photo, and we’ll do the reading for you.";
                        }
                        else
                        {
                            dataModel.Image = "tooltip_bg_3";
                            dataModel.Title = "How do I enter these values?";
                            dataModel.Description = "Enter the numbers according to its unit in the input. You’ll see your previous month's reading as a reference.";
                        }
                        SSMRMeterReadingModelList.Add(dataModel);
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        SSMRMeterReadingModel dataModel = new SSMRMeterReadingModel();
                        if (i == 0)
                        {
                            dataModel.Image = "tooltip_bg_2";
                            dataModel.Title = "Alright, what do I need to read?";
                            dataModel.Description = "Your meter will display the kWh reading by default. Enter the reading manually or just snap/upload a photo, and we’ll do the reading for you.";
                        }
                        else if (i == 1)
                        {
                            dataModel.Image = "tooltip_bg_3";
                            dataModel.Title = "How do I enter the value?";
                            dataModel.Description = "For manual reading, enter the kWh numbers in the input. You’ll see your previous month's reading as a reference.";
                        }
                        SSMRMeterReadingModelList.Add(dataModel);
                    }
                }
            }

            ViewPager onBoardViewPager = (ViewPager) mActivity.FindViewById(Resource.Id.onBoardingSMRViewPager);
            LinearLayout indicatorContainer = (LinearLayout) mActivity.FindViewById(Resource.Id.indicatorContainer);

            // SSMRMeterReadingAdapter onBoardingSMRAdapter = new SSMRMeterReadingAdapter(SupportFragmentManager);
            // onBoardingSMRAdapter.SetData(this.presenter.GetOnBoardingSMRData());

            // onBoardViewPager.Adapter = onBoardingSMRAdapter;

            onBoardViewPager.PageSelected += (object sender, ViewPager.PageSelectedEventArgs e) => {
                if (!isSinglePhase)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        ImageView selectedDot = (ImageView)indicatorContainer.GetChildAt(i);
                        if (e.Position == i)
                        {
                            selectedDot.SetImageResource(Resource.Drawable.onboarding_circle_active);
                        }
                        else
                        {
                            selectedDot.SetImageResource(Resource.Drawable.onboarding_circle_inactive);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        ImageView selectedDot = (ImageView)indicatorContainer.GetChildAt(i);
                        if (e.Position == i)
                        {
                            selectedDot.SetImageResource(Resource.Drawable.onboarding_circle_active);
                        }
                        else
                        {
                            selectedDot.SetImageResource(Resource.Drawable.onboarding_circle_inactive);
                        }
                    }
                }
            };

            return popUp;
        }
    }
}