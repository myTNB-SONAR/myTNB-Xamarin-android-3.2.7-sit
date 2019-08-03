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

        public SSMRMeterReadingDialogFragment(Context ctx, bool setIsSinglePhase)
		{
			this.mContext = ctx;
            this.isSinglePhase = setIsSinglePhase;
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
                SSMRMeterReadingModelList.Clear();
                if (isSinglePhase)
                {
                    SSMRMeterReadingModelList.AddRange(OnGetOnePhaseData());
                }
                else
                {
                    SSMRMeterReadingModelList.AddRange(OnGetThreePhaseData());
                }

                pager = rootView.FindViewById<ViewPager>(Resource.Id.onBoardingSMRViewPager);
                indicator = rootView.FindViewById<LinearLayout>(Resource.Id.indicatorContainer);
                dontShowAgainCheckbox = rootView.FindViewById<CheckBox>(Resource.Id.dontShowAgainCheckbox);
                txtBtnFirst = rootView.FindViewById<LinearLayout>(Resource.Id.txtBtnFirst);

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

        private List<SSMRMeterReadingModel> OnGetThreePhaseData()
        {
            SSMRMeterReadingThreePhaseScreensEntity entity = new SSMRMeterReadingThreePhaseScreensEntity();
            List<SSMRMeterReadingModel> items = new List<SSMRMeterReadingModel>();
            if (entity.GetAllItems().Count > 0)
            {
                foreach (SSMRMeterReadingThreePhaseScreensEntity model in entity.GetAllItems())
                {
                    SSMRMeterReadingModel dataModel = new SSMRMeterReadingModel();
                    dataModel.Image = model.Image;
                    dataModel.Title = model.Title;
                    dataModel.Description = model.Description;
                    items.Add(dataModel);
                }
            }
            return items;
        }

        private List<SSMRMeterReadingModel> OnGetOnePhaseData()
        {
            SSMRMeterReadingScreensEntity entity = new SSMRMeterReadingScreensEntity();
            List<SSMRMeterReadingModel> items = new List<SSMRMeterReadingModel>();
            if (entity.GetAllItems().Count > 0)
            {
                foreach (SSMRMeterReadingScreensEntity model in entity.GetAllItems())
                {
                    SSMRMeterReadingModel dataModel = new SSMRMeterReadingModel();
                    dataModel.Image = model.Image;
                    dataModel.Title = model.Title;
                    dataModel.Description = model.Description;
                    items.Add(dataModel);
                }
            }
            return items;
        }
    }
}