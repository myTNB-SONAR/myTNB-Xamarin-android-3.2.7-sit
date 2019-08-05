using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using myTNB.SitecoreCMS.Model;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.SSMR.SSMRMeterReadingTooltip.Adapter;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB_Android.Src.SSMR.SSMRMeterReadingTooltip.MVP
{
	public class SSMRMeterReadingDialogFragment : DialogFragment
	{

		private Context mContext;
		private ViewPager pager;
		private SSMRMeterReadingPagerAdapter adapter;
		private LinearLayout indicator;
        private CheckBox dontShowAgainCheckbox;
        private LinearLayout txtBtnFirst;
        private bool isSinglePhase = false;
        private List<SSMRMeterReadingModel> SSMRMeterReadingModelList = new List<SSMRMeterReadingModel>();

        public SSMRMeterReadingDialogFragment(Context ctx, bool setIsSinglePhase, List<SSMRMeterReadingModel> list)
		{
			this.mContext = ctx;
            this.isSinglePhase = setIsSinglePhase;
            if (list != null && list.Count > 0)
            {
                this.SSMRMeterReadingModelList = list;
            }
        }

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
			Dialog.SetCancelable(false);
			Dialog.SetCanceledOnTouchOutside(false);
            WindowManagerLayoutParams wlp = Dialog.Window.Attributes;
            wlp.Gravity = GravityFlags.Center;
            wlp.Width = ViewGroup.LayoutParams.MatchParent;
            wlp.Height = ViewGroup.LayoutParams.WrapContent;
            Dialog.Window.Attributes = wlp;

			View rootView = inflater.Inflate(Resource.Layout.CustomDialogWithImgViewPageOneButtonLayout, container, false);

			try
			{
                pager = rootView.FindViewById<ViewPager>(Resource.Id.onBoardingSMRViewPager);
                indicator = rootView.FindViewById<LinearLayout>(Resource.Id.indicatorContainer);
                dontShowAgainCheckbox = rootView.FindViewById<CheckBox>(Resource.Id.dontShowAgainCheckbox);
                txtBtnFirst = rootView.FindViewById<LinearLayout>(Resource.Id.txtBtnFirst);

                if (SSMRMeterReadingModelList.Count == 0)
                {
                    if (isSinglePhase)
                    {
                        SSMRMeterReadingModelList.AddRange(OnGetOnePhaseData());
                    }
                    else
                    {
                        SSMRMeterReadingModelList.AddRange(OnGetThreePhaseData());
                    }
                }

                if (SSMRMeterReadingModelList.Count > 0)
                {
                    indicator.Visibility = ViewStates.Visible;
                    for (int i = 0; i < SSMRMeterReadingModelList.Count; i++)
                    {
                        ImageView image = new ImageView(mContext);
                        image.Id = i;
                        LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                        layoutParams.RightMargin = 8;
                        layoutParams.LeftMargin = 8;
                        image.LayoutParameters = layoutParams;
                        if (i == 0)
                        {
                            image.SetImageResource(Resource.Drawable.onboarding_circle_active);
                        }
                        else
                        {
                            image.SetImageResource(Resource.Drawable.onboarding_circle_inactive);
                        }
                        indicator.AddView(image, i);
                    }
                }

                adapter = new SSMRMeterReadingPagerAdapter(mContext, SSMRMeterReadingModelList);
                pager.Adapter = adapter;

                pager.PageSelected += (object sender, ViewPager.PageSelectedEventArgs e) => {
                    for (int i = 0; i < SSMRMeterReadingModelList.Count; i++)
                    {
                        ImageView selectedDot = (ImageView)indicator.GetChildAt(i);
                        if (e.Position == i)
                        {
                            selectedDot.SetImageResource(Resource.Drawable.onboarding_circle_active);
                        }
                        else
                        {
                            selectedDot.SetImageResource(Resource.Drawable.onboarding_circle_inactive);
                        }
                    }
                };


                txtBtnFirst.Click += GotIt_Click;


            }
			catch (Exception ex)
			{
				Utility.LoggingNonFatalError(ex);
			}
			return rootView;
		}

        private void GotIt_Click(object sender, EventArgs e)
        {
            if (dontShowAgainCheckbox.Checked)
            {
                MyTNBAccountManagement.GetInstance().UpdateIsSMRMeterReadingOnboardingShown();
            }
            this.Dismiss();
        }

        public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

        public List<SSMRMeterReadingModel> OnGetThreePhaseData()
        {
            List<SSMRMeterReadingModel> items = new List<SSMRMeterReadingModel>();
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
                items.Add(dataModel);
            }
            return items;
        }

        private List<SSMRMeterReadingModel> OnGetOnePhaseData()
        {
            List<SSMRMeterReadingModel> items = new List<SSMRMeterReadingModel>();
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
                items.Add(dataModel);
            }
            return items;
        }

    }
}