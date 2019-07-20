using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Facebook.Shimmer;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Listener;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class HomeMenuFragment : BaseFragment, HomeMenuContract.IView
	{
        [BindView(Resource.Id.accountsHeaderTitle)]
        TextView accountHeaderTitle;

        [BindView(Resource.Id.accountGreeting)]
        TextView accountGreeting;

        [BindView(Resource.Id.accountGreetingName)]
        TextView accountGreetingName;

        [BindView(Resource.Id.searchAction)]
        ImageView searchActionIcon;

        [BindView(Resource.Id.addAction)]
        ImageView addAccountActionIcon;

        [BindView(Resource.Id.searchEdit)]
        Android.Widget.SearchView searchEditText;

        [BindView(Resource.Id.accountRecyclerViewContainer)]
        RecyclerView accountsRecyclerView;

        [BindView(Resource.Id.indicatorContainer)]
        LinearLayout indicatorContainer;

        AccountsRecyclerViewAdapter accountsAdapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            SetAccountsRecyclerView();
            SetAccountActionHeader();
            OnUpdateAccountListChanged(false);
        }

        public void ShowSearchAction(bool isShow)
        {
            if (isShow)
            {
                accountHeaderTitle.Visibility = ViewStates.Gone;
                searchEditText.Visibility = ViewStates.Visible;
                searchActionIcon.Visibility = ViewStates.Gone;
                searchEditText.ClearFocus();
            }
            else
            {
                accountHeaderTitle.Visibility = ViewStates.Visible;
                searchEditText.Visibility = ViewStates.Gone;
                searchActionIcon.Visibility = ViewStates.Visible;
            }
        }

        private void SetAccountActionHeader()
        {
            TextViewUtils.SetMuseoSans500Typeface(accountHeaderTitle, accountGreeting, accountGreetingName);
            searchEditText.SetOnQueryTextListener(new AccountsSearchOnQueryTextListener(this,accountsAdapter));
            searchActionIcon.Click += (s, e) =>
            {
                ShowSearchAction(true);
            };

            addAccountActionIcon.Click += (s, e) =>
            {
                ShowSearchAction(false);
            };
        }

        private void SetAccountsRecyclerView()
        {
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(Activity, LinearLayoutManager.Horizontal, false);
            accountsRecyclerView.SetLayoutManager(linearLayoutManager);

            accountsAdapter = new AccountsRecyclerViewAdapter(this,16);
            accountsAdapter.SetAccountCards(13);
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

        private void UpdateAccountListIndicator()
        {
            indicatorContainer.RemoveAllViews();
            int accountsCount = accountsAdapter.ItemCount;// accountCardModelList.Count;
            if (accountsCount > 1)
            {
                indicatorContainer.Visibility = ViewStates.Visible;
                for (int i = 0; i < accountsCount; i++)
                {
                    ImageView image = new ImageView(Activity);
                    image.Id = i;
                    LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                    layoutParams.RightMargin = 5;
                    layoutParams.LeftMargin = 5;
                    image.LayoutParameters = layoutParams;
                    if (i == 0)
                    {
                        image.SetImageResource(Resource.Drawable.circle_active);
                    }
                    else
                    {
                        image.SetImageResource(Resource.Drawable.circle);
                    }
                    indicatorContainer.AddView(image, i);
                }
            }
            else
            {
                indicatorContainer.Visibility = ViewStates.Gone;
            }
        }

        public void OnUpdateAccountListChanged(bool isSearchSubmit)
		{
            if (isSearchSubmit)
            {
                ShowSearchAction(false);
            }
            UpdateAccountListIndicator();
		}
    }
}
