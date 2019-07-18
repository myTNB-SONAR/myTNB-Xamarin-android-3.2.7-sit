
using System.Collections.Generic;
using System.Linq;

using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Facebook.Shimmer;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Listener;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Models;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class HomeMenuFragment : BaseFragment
	{
        [BindView(Resource.Id.myServiceShimmerView)]
        ShimmerFrameLayout myServiceShimmerView;

        [BindView(Resource.Id.myServiceList)]
        RecyclerView myServiceListRecycleView;

        [BindView(Resource.Id.myServiceShimmerList)]
        RecyclerView myServiceShimmerList;

        [BindView(Resource.Id.myServiceView)]
        LinearLayout myServiceView;

        //[BindView(Resource.Id.shimmer_view_container)]
        //ShimmerFrameLayout shimmerViewContainer;
        [BindView(Resource.Id.myServiceTitle)]
        TextView myServiceTitle;
        
        [BindView(Resource.Id.accountsHeaderTitle)]
        TextView accountHeaderTitle;
        
        [BindView(Resource.Id.searchAction)]
        ImageView searchActionIcon;

        [BindView(Resource.Id.addAction)]
        ImageView addAccountActionIcon;

        [BindView(Resource.Id.searchEdit)]
        EditText searchEditText;

        [BindView(Resource.Id.accountRecyclerViewContainer)]
        RecyclerView accountsRecyclerView;

        [BindView(Resource.Id.indicatorContainer)]
        LinearLayout indicatorContainer;


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            try
            {
                SetAccountsRecyclerView();
                SetAccountActionHeader();
                SetMyServiceRecycleView();
                TextViewUtils.SetMuseoSans500Typeface(myServiceTitle);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void SetMyServiceRecycleView()
        {
            GridLayoutManager layoutManager = new GridLayoutManager(this.Activity, 3);
            layoutManager.Orientation = RecyclerView.Vertical;
            myServiceListRecycleView.SetLayoutManager(layoutManager);

            GridLayoutManager layoutShimmerManager = new GridLayoutManager(this.Activity, 3);
            layoutShimmerManager.Orientation = RecyclerView.Vertical;
            myServiceShimmerList.SetLayoutManager(layoutShimmerManager);
            // LoadShimmerServiceList(null);
            LoadServiceList(null);
        }

        private void SetAccountActionHeader()
        {
            TextViewUtils.SetMuseoSans500Typeface(accountHeaderTitle);

            searchActionIcon.Click += (s, e) =>
            {
                accountHeaderTitle.Visibility = ViewStates.Gone;
                searchEditText.Visibility = ViewStates.Visible;
                searchActionIcon.Visibility = ViewStates.Gone;
            };

            addAccountActionIcon.Click += (s, e) =>
            {
                accountHeaderTitle.Visibility = ViewStates.Visible;
                searchEditText.Visibility = ViewStates.Gone;
                searchActionIcon.Visibility = ViewStates.Visible;
            };
        }

        private void SetAccountsRecyclerView()
        {
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false);
            accountsRecyclerView.SetLayoutManager(linearLayoutManager);

            AccountsRecyclerViewAdapter accountsAdapter = new AccountsRecyclerViewAdapter(2);
            accountsRecyclerView.SetAdapter(accountsAdapter);

            accountsRecyclerView.AddOnScrollListener(new AccountsRecyclerViewOnScrollListener(linearLayoutManager, indicatorContainer));
        }
        private void SetShimmer()
        {
            //var shimmerBuilder = new Shimmer.AlphaHighlightBuilder();
            //shimmerBuilder = default(Shimmer.AlphaHighlightBuilder);
            //shimmerViewContainer.SetShimmer(shimmerBuilder?.Build());
        }

        public override void OnResume()
        {
            base.OnResume();
            //var shimmerBuilder = new Shimmer.AlphaHighlightBuilder();
            //shimmerBuilder = default(Shimmer.AlphaHighlightBuilder);
            //shimmerViewContainer.SetShimmer(shimmerBuilder?.Build());
            //shimmerViewContainer.StartShimmer();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public override int ResourceId()
        {
            return Resource.Layout.HomeMenuFragmentView;
        }

        public void LoadShimmerServiceList(List<MyService> serviceList)
        {
            myServiceShimmerView.Visibility = ViewStates.Visible;
            myServiceView.Visibility = ViewStates.Gone;
            if (serviceList != null && serviceList.Count() > 0)
            {
                MyServiceShimmerAdapter adapter = new MyServiceShimmerAdapter(serviceList);
                myServiceShimmerList.SetAdapter(adapter);
            }
            else
            {
                List<MyService> dummyList = new List<MyService>();
                for(int i = 0; i < 6; i++)
                {
                    dummyList.Add(new MyService()
                    {
                        MyServiceTitle = ""
                    });
                }
                MyServiceShimmerAdapter adapter = new MyServiceShimmerAdapter(dummyList);
                myServiceShimmerList.SetAdapter(adapter);
            }
            myServiceShimmerView.StartShimmer();
        }

        public void LoadServiceList(List<MyService> serviceList)
        {
            myServiceShimmerView.StopShimmer();
            myServiceShimmerView.Visibility = ViewStates.Gone;
            myServiceView.Visibility = ViewStates.Visible;
            if (serviceList != null && serviceList.Count() > 0)
            {
                MyServiceAdapter adapter = new MyServiceAdapter(serviceList);
                myServiceListRecycleView.SetAdapter(adapter);
            }
            else
            {
                List<MyService> dummyList = new List<MyService>();
                for (int i = 0; i < 5; i++)
                {
                    if(i == 0)
                    {
                        dummyList.Add(new MyService()
                        {
                            Id = "0",
                            MyServiceTitle = "Apply for Self<br/>Meter Reading"
                        });
                    }
                    else if (i == 1)
                    {
                        dummyList.Add(new MyService()
                        {
                            Id = "1",
                            MyServiceTitle = "Check<br/>Status"
                        });
                    }
                    else if (i == 2)
                    {
                        dummyList.Add(new MyService()
                        {
                            Id = "2",
                            MyServiceTitle = "Give Us<br/>Feedback"
                        });
                    }
                    else if (i == 3)
                    {
                        dummyList.Add(new MyService()
                        {
                            Id = "3",
                            MyServiceTitle = "Set<br/>Appointments"
                        });
                    }
                    else if (i == 4)
                    {
                        dummyList.Add(new MyService()
                        {
                            Id = "4",
                            MyServiceTitle = "Apply for<br/>AutoPay"
                        });
                    }
                }
                MyServiceAdapter adapter = new MyServiceAdapter(dummyList);
                myServiceListRecycleView.SetAdapter(adapter);
            }
            //myServiceShimmerView.StopShimmer();
        }

        private Shimmer.AlphaHighlightBuilder ShimmerEffectSetup()
        {
            Shimmer.AlphaHighlightBuilder shimmerBuilder = new Shimmer.AlphaHighlightBuilder();
            shimmerBuilder.SetBaseAlpha(0.1f);
            shimmerBuilder.SetTilt(0f);
            return shimmerBuilder;
        }
    }
}
