
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Fragments;
using AndroidX.Fragment.App;
using Android.Graphics.Drawables;
using Android.Graphics;

using myTNB_Android.Src.myTNBMenu.Activity;
using CheeseBind;
using myTNB_Android.Src.myTNBMenu.Listener;
using Google.Android.Material.BottomSheet;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.DigitalBill.Activity;
using System.Threading.Tasks;
using AndroidX.RecyclerView.Widget;
using myTNB_Android.Src.MyDrawer;
using static myTNB_Android.Src.MyDrawer.MyDrawerModel;
using static myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter.MyServiceAdapter;
using static myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter.MyServiceShimmerAdapter;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter;
using Android.Preferences;
using myTNB_Android.Src.NewAppTutorial.MVP;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using myTNB_Android.Src.MyHome.Activity;
using AndroidX.CoordinatorLayout.Widget;
using myTNB.Mobile;
using Color = Android.Graphics.Color;
using Newtonsoft.Json;
using MyHomeModel = myTNB_Android.Src.MyDrawer.MyHomeModel;

namespace myTNB_Android.Src.MyHome
{
	public class MyHomeDrawerFragment : AndroidX.Fragment.App.DialogFragment
    {
        private Android.App.Activity mContext;
        private DashboardHomeActivity mActivity;

        private static BottomSheetBehavior bottomSheetBehavior;

        LinearLayout bottomSheet, drawer;
        RelativeLayout titleLayout;
        TextView titleLabel;
        ImageView closeIcon;

        RecyclerView myHomeDrawerListRecycleView;
        MyDrawerAdapter myHomeDrawerAdapter;

        List<MyDrawerModel> myDrawerList;

        public MyHomeDrawerFragment(Android.App.Activity ctx, List<MyDrawerModel> modelList)
        {
            this.mContext = ctx;
            this.myDrawerList = modelList;
            if (this.mContext is DashboardHomeActivity)
            {
                this.mActivity = ((DashboardHomeActivity)this.mContext);
            }
        }

        public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
        }

        public override void OnStart()
        {
            base.OnStart();
            try
            {
                if (Dialog != null)
                {
                    Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
                    Dialog.Window.SetDimAmount(0.75f);
                    Dialog.SetCancelable(true);
                    Dialog.SetCanceledOnTouchOutside(true);
                    Dialog.Window.SetLayout(WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.WrapContent);
                    Dialog.SetOnDismissListener(new OnDismissListener(() =>
                    {
                        DynatraceHelper.OnTrack(DynatraceConstants.MyHome.CTAs.Home.Drawer_Dismiss);
                    }));
                }

                Task.Run(async () =>
                {
                    await Task.Delay(200);
                    bottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;

                    DynatraceHelper.OnTrack(DynatraceConstants.MyHome.Screens.Home.Drawer);
                    DynatraceHelper.OnTrack(DynatraceConstants.MyHome.CTAs.Home.Drawer_Open);

                    if (!UserSessions.MyHomeDrawerTutorialHasShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity)))
                    {
                        await Task.Delay(300);
                        NewAppTutorialUtils.OnShowNewAppTutorial(this.Activity, this, PreferenceManager.GetDefaultSharedPreferences(this.Activity), OnGeneraMyHomeDrawerTutorialList(), true);
                    }
                });
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.Window.Attributes.Gravity = GravityFlags.Bottom;

            View rootView = inflater.Inflate(Resource.Layout.MyTNBDrawer, container, false);

            bottomSheet = (LinearLayout)rootView.FindViewById(Resource.Id.myTNBDrawerBottomSheet);
            titleLayout = (RelativeLayout)rootView.FindViewById(Resource.Id.myTNBDrawerTitleLayout);
            drawer = (LinearLayout)rootView.FindViewById(Resource.Id.myTNBDrawer);
            closeIcon = (ImageView)rootView.FindViewById<ImageView>(Resource.Id.myTNBDrawerCloseIcon);
            myHomeDrawerListRecycleView = (RecyclerView)rootView.FindViewById<RecyclerView>(Resource.Id.myTNBDrawerList);
            titleLabel = (TextView)rootView.FindViewById<TextView>(Resource.Id.myTNBDrawerTitle);

            SetUpViews();

            bottomSheetBehavior = BottomSheetBehavior.From(bottomSheet);
            bottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
            bottomSheetBehavior.AddBottomSheetCallback(new MyHomeBottomSheetCallBack(this));

            return rootView;
        }

        private void SetUpViews()
        {
            TextViewUtils.SetMuseoSans500Typeface(titleLabel);
            TextViewUtils.SetTextSize16(titleLabel);
            titleLabel.Text = Utility.GetLocalizedLabel("DashboardHome", "myHome");

            closeIcon.Click += CloseOnClick;

            GridLayoutManager layoutManager = new GridLayoutManager(this.Activity, 3);
            layoutManager.Orientation = RecyclerView.Vertical;
            myHomeDrawerListRecycleView.SetLayoutManager(layoutManager);

            myHomeDrawerAdapter = new MyDrawerAdapter(myDrawerList, this.Activity);
            myHomeDrawerAdapter.ClickChanged += OnClickChanged;
            myHomeDrawerListRecycleView.SetAdapter(myHomeDrawerAdapter);
        }

        private void OnClickChanged(object sender, int position)
        {
            DynatraceHelper.OnTrack(DynatraceConstants.MyHome.CTAs.Home.Drawer_Connect_My_Premise);

            if (!UserSessions.ConnectMyPremiseHasShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity)))
            {
                UserSessions.SetShownConnectMyPremise(PreferenceManager.GetDefaultSharedPreferences(this.Activity));
            }

            MyDrawerModel model = myDrawerList[position];
            MyHomeModel myHomeModel = new MyHomeModel()
            {
                ServiceId = model.ServiceId,
                ServiceName = model.ServiceName,
                SSODomain = model.SSODomain,
                OriginURL = model.OriginURL,
                RedirectURL = model.RedirectURL
            };

            Intent micrositeActivity = new Intent(this.Activity, typeof(MyHomeMicrositeActivity));
            micrositeActivity.PutExtra(MyHomeConstants.DRAWER_MODEL, JsonConvert.SerializeObject(myHomeModel));
            StartActivity(micrositeActivity);
        }

        private void CloseOnClick(object sender, EventArgs e)
        {
            bottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
            DynatraceHelper.OnTrack(DynatraceConstants.MyHome.CTAs.Home.Drawer_Cancel);
        }

        private List<NewAppModel> OnGeneraMyHomeDrawerTutorialList()
        {
            List<NewAppModel> tutorialList = new List<NewAppModel>();

            tutorialList.Add(new NewAppModel()
            {
                ContentShowPosition = ContentType.TopLeft,
                ContentTitle = Utility.GetLocalizedLabel("Tutorial", "connectMyPremiseTitle"),
                ContentMessage = Utility.GetLocalizedLabel("Tutorial", "connectMyPremiseMessage"),
                Feature = FeatureType.MyHome,
                DynatraceVisitTag = DynatraceConstants.MyHome.Screens.Tutorial.Drawer_Start_Your_Application,
                DynatraceActionTag = DynatraceConstants.MyHome.CTAs.Tutorial.Drawer_Start_Your_Application_Skip
            });

            return tutorialList;
        }

        public int GetRecyclerViewWidth()
        {
            return myHomeDrawerListRecycleView.Width;
        }

        public int GetRecyclerViewHeight()
        {
            return myHomeDrawerListRecycleView.Height;
        }

        public int GetTopHeight()
        {
            int i = 0;

            try
            {
                int[] location = new int[2];
                myHomeDrawerListRecycleView.GetLocationOnScreen(location);
                i = location[1];
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

            return i;
        }

        private class MyHomeBottomSheetCallBack : BottomSheetBehavior.BottomSheetCallback
        {
            private readonly MyHomeDrawerFragment fragment;

            public MyHomeBottomSheetCallBack(MyHomeDrawerFragment mFragment)
            {
                this.fragment = mFragment;
            }

            public override void OnSlide(View bottomSheet, float slideOffset)
            {
                if (slideOffset < -1 || slideOffset == -1)
                {
                    this.fragment.Dismiss();
                    DynatraceHelper.OnTrack(DynatraceConstants.MyHome.CTAs.Home.Drawer_Dismiss);
                }
            }

            public override void OnStateChanged(View bottomSheet, int newState) { }
        }

        private sealed class OnDismissListener : Java.Lang.Object, IDialogInterfaceOnDismissListener
        {
            private readonly Action action;

            public OnDismissListener(Action action)
            {
                this.action = action;
            }

            public void OnDismiss(IDialogInterface dialog)
            {
                this.action();
            }
        }
    }
}

