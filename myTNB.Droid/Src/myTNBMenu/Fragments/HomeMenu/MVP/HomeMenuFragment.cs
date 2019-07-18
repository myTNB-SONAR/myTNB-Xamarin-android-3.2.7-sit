
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
        //[BindView(Resource.Id.shimmer_view_container)]
        //ShimmerFrameLayout shimmerViewContainer;
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
                GridLayoutManager layoutManager = new GridLayoutManager(this.Activity, 2);
                layoutManager.Orientation = RecyclerView.Horizontal;
                myServiceListRecycleView.SetLayoutManager(layoutManager);
                LoadShimmerServiceList(null);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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
            if (serviceList != null && serviceList.Count() > 0)
            {
                MyServiceShimmerAdapter adapter = new MyServiceShimmerAdapter(serviceList);
                myServiceListRecycleView.SetAdapter(adapter);
            }
            else
            {
                List<MyService> dummyList = new List<MyService>();
                for(int i = 0; i < 4; i++)
                {
                    dummyList.Add(new MyService()
                    {
                        MyServiceTitle = ""
                    });
                }
                MyServiceShimmerAdapter adapter = new MyServiceShimmerAdapter(dummyList);
                myServiceListRecycleView.SetAdapter(adapter);
            }
            myServiceShimmerView.StartShimmer();
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
