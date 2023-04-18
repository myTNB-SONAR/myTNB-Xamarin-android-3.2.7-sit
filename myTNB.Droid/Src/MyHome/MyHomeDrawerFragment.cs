
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
using MyHomeModel = myTNB_Android.Src.MyHome.Model.MyHomeModel;
using myTNB_Android.Src.DeviceCache;
using myTNB.Mobile.Constants;

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
        List<NewAppModel> _tutorialList;
        string _drawerTitle;

        public MyHomeDrawerFragment(Android.App.Activity ctx, List<MyDrawerModel> modelList, string drawerTitle)
        {
            this.mContext = ctx;
            this.myDrawerList = modelList;
            if (this.mContext is DashboardHomeActivity)
            {
                this.mActivity = ((DashboardHomeActivity)this.mContext);
            }
            _drawerTitle = drawerTitle;
            _tutorialList = new List<NewAppModel>();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
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

                    await Task.Delay(300);
                    OnCheckTutorial();
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
            titleLabel.Text = _drawerTitle;

            closeIcon.Click += CloseOnClick;

            GridLayoutManager layoutManager = new GridLayoutManager(this.Activity, 3);
            layoutManager.Orientation = RecyclerView.Vertical;
            myHomeDrawerListRecycleView.SetLayoutManager(layoutManager);

            myHomeDrawerAdapter = new MyDrawerAdapter(myDrawerList, this.Activity);
            myHomeDrawerAdapter.ClickChanged += OnClickChanged;
            myHomeDrawerListRecycleView.SetAdapter(myHomeDrawerAdapter);
        }

        private void OnCheckTutorial()
        {
            foreach (MyDrawerModel myDrawer in myDrawerList)
            {
                if (!UserSessions.MyHomeDrawerTutorialHasShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity), myDrawer.ServiceId))
                {
                    UpdateTutorialList(myDrawer);
                }
            }

            if (_tutorialList.Count > 0)
            {
                NewAppTutorialUtils.OnShowNewAppTutorial(this.Activity, this, PreferenceManager.GetDefaultSharedPreferences(this.Activity), _tutorialList, true);
                _tutorialList = new List<NewAppModel>();
            }
        }

        private void UpdateTutorialList(MyDrawerModel myDrawer)
        {
            if (myDrawer.ServiceId == "1009")
            {
                _tutorialList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.TopLeft,
                    ContentTitle = Utility.GetLocalizedLabel("Tutorial", myHome.TutorialOverlay.I18N_ConnectMyPremiseTitle),
                    ContentMessage = Utility.GetLocalizedLabel("Tutorial", myHome.TutorialOverlay.I18N_ConnectMyPremiseMessage),
                    DynatraceVisitTag = DynatraceConstants.MyHome.Screens.Tutorial.Drawer_Start_Your_Application,
                    DynatraceActionTag = DynatraceConstants.MyHome.CTAs.Tutorial.Drawer_Start_Your_Application_Skip,
                    Tag = myDrawer.ServiceId
                });
            }
            else if (myDrawer.ServiceId == "1010")
            {
                _tutorialList.Add(new NewAppModel()
                {
                    ContentShowPosition = ContentType.TopLeft,
                    ContentTitle = Utility.GetLocalizedLabel("Tutorial", myHome.TutorialOverlay.I18N_myHomeChecklistTitle),
                    ContentMessage = Utility.GetLocalizedLabel("Tutorial", myHome.TutorialOverlay.I18N_myHomeChecklistMessage),
                    DynatraceVisitTag = DynatraceConstants.MyHome.Screens.Tutorial.Drawer_MyHome_Checklist,
                    DynatraceActionTag = DynatraceConstants.MyHome.CTAs.Tutorial.Drawer_MyHome_Checklist_Skip,
                    Tag = myDrawer.ServiceId
                });
            }
        }

        private void OnClickChanged(object sender, int position)
        {
            if (myDrawerList != null && myDrawerList.Count > 0)
            {
                MyDrawerModel drawerItem = myDrawerList[position];
                if (drawerItem != null)
                {
                    switch (drawerItem.ServiceType)
                    {
                        case MobileEnums.ServiceEnum.CONNECTMYPREMISE:
                            DynatraceHelper.OnTrack(DynatraceConstants.MyHome.CTAs.Home.Drawer_Connect_My_Premise);

                            if (!UserSessions.ConnectMyPremiseHasShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity)))
                            {
                                UserSessions.SetShownConnectMyPremise(PreferenceManager.GetDefaultSharedPreferences(this.Activity));
                            }
                            break;
                        case MobileEnums.ServiceEnum.HMO:
                            DynatraceHelper.OnTrack(DynatraceConstants.MyHome.CTAs.Home.Drawer_Checklist);

                            if (!UserSessions.HMOHasShown(PreferenceManager.GetDefaultSharedPreferences(this.Activity)))
                            {
                                UserSessions.SetShownHMO(PreferenceManager.GetDefaultSharedPreferences(this.Activity));
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            ShowProgressDialog();
            Task.Run(() =>
            {
                _ = GetAccessToken(position);
            });
        }

        private async Task GetAccessToken(int pos)
        {
            UserEntity user = UserEntity.GetActive();
            string accessToken = await AccessTokenManager.Instance.GetUserServiceAccessToken(user.UserID);
            AccessTokenCache.Instance.SaveUserServiceAccessToken(this.Activity, accessToken);
            if (accessToken.IsValid())
            {
                this.Activity.RunOnUiThread(() =>
                {
                    HideProgressDialog();

                    MyDrawerModel model = myDrawerList[pos];
                    MyHomeModel myHomeModel = new MyHomeModel()
                    {
                        ServiceId = model.ServiceId,
                        ServiceName = model.ServiceName,
                        SSODomain = model.SSODomain,
                        OriginURL = model.OriginURL,
                        RedirectURL = model.RedirectURL,
                        CancelURL = string.Empty
                    };

                    bottomSheetBehavior.State = BottomSheetBehavior.StateHidden;

                    Intent micrositeActivity = new Intent(this.Activity, typeof(MyHomeMicrositeActivity));
                    micrositeActivity.PutExtra(MyHomeConstants.ACCESS_TOKEN, accessToken);
                    micrositeActivity.PutExtra(MyHomeConstants.MYHOME_MODEL, JsonConvert.SerializeObject(myHomeModel));
                    this.mActivity.StartActivityForResult(micrositeActivity, Constants.MYHOME_MICROSITE_REQUEST_CODE);
                });
            }
            else
            {
                this.Activity.RunOnUiThread(() =>
                {
                    HideProgressDialog();
                    ShowGenericErrorPopUp();
                });
            }
            HideProgressDialog();
        }

        private void ShowGenericErrorPopUp()
        {
            MyTNBAppToolTipBuilder errorPopUp = MyTNBAppToolTipBuilder.Create(this.Activity, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(Utility.GetLocalizedErrorLabel(LanguageConstants.Error.DEFAULT_ERROR_TITLE))
                .SetMessage(Utility.GetLocalizedErrorLabel(LanguageConstants.Error.DEFAULT_ERROR_MSG))
                .SetCTALabel(Utility.GetLocalizedCommonLabel(LanguageConstants.Common.OK))
                .Build();
            errorPopUp.Show();
        }

        private void CloseOnClick(object sender, EventArgs e)
        {
            bottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
            DynatraceHelper.OnTrack(DynatraceConstants.MyHome.CTAs.Home.Drawer_Cancel);
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

        public int GetMyDrawerItemHeight()
        {
            var servicesList = myDrawerList;

            int a = (int)System.Math.Ceiling((double)servicesList.Count / 3);
            var itemHeight = myHomeDrawerListRecycleView.Height / a;

            return itemHeight;
        }

        public int GetMyDrawerItemWidth()
        {
            var itemWidth = myHomeDrawerListRecycleView.Width / 3;
            return itemWidth;
        }

        public int GetMyDrawerItemTopPosition(string id)
        {
            var topPosition = 0;
            var servicesList = myDrawerList;
            int itemPosition = servicesList.FindIndex(x => x.ServiceId == id) + 1;

            int row = (int)System.Math.Ceiling((double)itemPosition / 3);
            int rowIndex = row - 1;
            topPosition = rowIndex * GetMyDrawerItemHeight();

            return topPosition;
        }

        public int GetMyDrawerItemLeftPosition(string id)
        {
            var leftPosition = 0;

            int column = 0;

            var servicesList = myDrawerList;
            int itemIndex = servicesList.FindIndex(x => x.ServiceId == id);

            column = (itemIndex % 3);
            leftPosition = column * GetMyDrawerItemWidth();

            return leftPosition;
        }

        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this.Activity);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this.Activity);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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

