
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

namespace myTNB_Android.Src.MyHome
{
	public class MyHomeDrawerFragment : AndroidX.Fragment.App.DialogFragment
    {
        private Android.App.Activity mContext;
        private DashboardHomeActivity mActivity;

        private static BottomSheetBehavior bottomSheetBehavior;

        LinearLayout bottomSheet;
        TextView titleLabel;
        ImageView closeIcon;

        RecyclerView myHomeDrawerListRecycleView;
        MyDrawerAdapter myHomeDrawerAdapter;

        public MyHomeDrawerFragment(Android.App.Activity ctx)
        {
            this.mContext = ctx;
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
                    Dialog.SetCancelable(false);
                    Dialog.SetCanceledOnTouchOutside(false);
                    Dialog.Window.SetLayout(WindowManagerLayoutParams.MatchParent, WindowManagerLayoutParams.WrapContent);
                }

                Task.Run(async () =>
                {
                    await Task.Delay(200);
                    bottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
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
            titleLabel.Text = "myHome"; //STUB

            closeIcon.Click += CloseOnClick;

            GridLayoutManager layoutManager = new GridLayoutManager(this.Activity, 3);
            layoutManager.Orientation = RecyclerView.Vertical;
            myHomeDrawerListRecycleView.SetLayoutManager(layoutManager);
            //myHomeDrawerListRecycleView.AddItemDecoration(new MyServiceItemDecoration(3, 3, false, this.Activity));

            //STUB
            List<MyDrawerModel> myDrawerList = new List<MyDrawerModel>();

            MyDrawerModel model = new MyDrawerModel();
            model.ServiceCategoryId = "001";
            model.serviceCategoryName = "Connect My Home";
            myDrawerList.Add(model);

            MyDrawerModel model2 = new MyDrawerModel();
            model2.ServiceCategoryId = "002";
            model2.serviceCategoryName = "Home Move Organizer";
            myDrawerList.Add(model2);

            MyDrawerModel model3 = new MyDrawerModel();
            model3.ServiceCategoryId = "003";
            model3.serviceCategoryName = "Application Status";
            myDrawerList.Add(model3);

            myHomeDrawerAdapter = new MyDrawerAdapter(myDrawerList, this.Activity);
            myHomeDrawerListRecycleView.SetAdapter(myHomeDrawerAdapter);
        }

        private void CloseOnClick(object sender, EventArgs e)
        {
            bottomSheetBehavior.State = BottomSheetBehavior.StateHidden;
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
                }
            }

            public override void OnStateChanged(View bottomSheet, int newState) { }
        }
    }
}

