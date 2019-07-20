
using System;
using System.Collections.Generic;
using System.Linq;

using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Facebook.Shimmer;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.myTNBMenu.Fragments.FeedbackMenu;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Adapter;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Listener;
using myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.Models;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class HomeMenuFragment : BaseFragment, HomeMenuContract.IView
	{
        [BindView(Resource.Id.newFAQShimmerView)]
        LinearLayout newFAQShimmerView;

        [BindView(Resource.Id.newFAQList)]
        RecyclerView newFAQListRecycleView;

        [BindView(Resource.Id.newFAQShimmerList)]
        RecyclerView newFAQShimmerList;

        [BindView(Resource.Id.newFAQView)]
        LinearLayout newFAQView;

        [BindView(Resource.Id.newFAQTitle)]
        TextView newFAQTitle;

        [BindView(Resource.Id.myServiceShimmerView)]
        LinearLayout myServiceShimmerView;

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

        System.Timers.Timer timer;
        System.Timers.Timer FAQTimer;

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
                SetNewFAQRecycleView();
                TextViewUtils.SetMuseoSans500Typeface(myServiceTitle, newFAQTitle);
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
            LoadShimmerServiceList(null);
            timer = new System.Timers.Timer();
            timer.Interval = 3000;
            timer.Elapsed += OnMyServiceTimedEvent;
            timer.Enabled = true;
        }

        private void OnMyServiceTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Activity.RunOnUiThread(() =>
            {
                timer.Stop();
                timer.Close();
                LoadServiceList(null);
            });
        }

        private void SetNewFAQRecycleView()
        {
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
            newFAQListRecycleView.SetLayoutManager(linearLayoutManager);

            LinearLayoutManager linearShimmerLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Horizontal, false);
            newFAQShimmerList.SetLayoutManager(linearShimmerLayoutManager);
            LoadShimmerFAQList(null);
            FAQTimer = new System.Timers.Timer();
            FAQTimer.Interval = 3000;
            FAQTimer.Elapsed += OnNewFAQTimedEvent;
            FAQTimer.Enabled = true;
        }

        private void OnNewFAQTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Activity.RunOnUiThread(() =>
            {
                FAQTimer.Stop();
                FAQTimer.Close();
                LoadFAQList(null);
            });
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

            var act = this.Activity as AppCompatActivity;

            var actionBar = act.SupportActionBar;
            actionBar.Hide();
            ShowBackButton(false);
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
                for (int i = 0; i < 6; i++)
                {
                    dummyList.Add(new MyService()
                    {
                        serviceCategoryName = ""
                    });
                }
                MyServiceShimmerAdapter adapter = new MyServiceShimmerAdapter(dummyList);
                myServiceShimmerList.SetAdapter(adapter);
            }
        }

        public void LoadServiceList(List<MyService> serviceList)
        {
            myServiceShimmerView.Visibility = ViewStates.Gone;
            myServiceView.Visibility = ViewStates.Visible;
            if (serviceList != null && serviceList.Count() > 0)
            {
                MyServiceAdapter adapter = new MyServiceAdapter(serviceList);
                myServiceListRecycleView.SetAdapter(adapter);
                adapter.ClickChanged += OnClickChanged;
            }
            else
            {
                List<MyService> dummyList = new List<MyService>();
                for (int i = 0; i < 5; i++)
                {
                    if (i == 0)
                    {
                        dummyList.Add(new MyService()
                        {
                            ServiceCategoryId = "0",
                            serviceCategoryName = "Apply for Self<br/>Meter Reading"
                        });
                    }
                    else if (i == 1)
                    {
                        dummyList.Add(new MyService()
                        {
                            ServiceCategoryId = "1",
                            serviceCategoryName = "Check<br/>Status"
                        });
                    }
                    else if (i == 2)
                    {
                        dummyList.Add(new MyService()
                        {
                            ServiceCategoryId = "2",
                            serviceCategoryName = "Give Us<br/>Feedback"
                        });
                    }
                    else if (i == 3)
                    {
                        dummyList.Add(new MyService()
                        {
                            ServiceCategoryId = "3",
                            serviceCategoryName = "Set<br/>Appointments"
                        });
                    }
                    else if (i == 4)
                    {
                        dummyList.Add(new MyService()
                        {
                            ServiceCategoryId = "4",
                            serviceCategoryName = "Apply for<br/>AutoPay"
                        });
                    }
                }
                MyServiceAdapter adapter = new MyServiceAdapter(dummyList);
                myServiceListRecycleView.SetAdapter(adapter);
                adapter.ClickChanged += OnClickChanged;
            }
        }

        void OnClickChanged(object sender, int position)
        {
            try
            {
                if (position == -1)
                {
                    // Toast.MakeText(this.Activity, "My Service Position Unknown", ToastLength.Long).Show();
                }
                else
                {
                    if (position == 2)
                    {
                        ShowFeedbackMenu();
                    }
                    // Toast.MakeText(this.Activity, "My Service Position: " + position.ToString(), ToastLength.Long).Show();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void LoadShimmerFAQList(List<NewFAQ> faqList)
        {
            newFAQShimmerView.Visibility = ViewStates.Visible;
            newFAQView.Visibility = ViewStates.Gone;
            if (faqList != null && faqList.Count() > 0)
            {
                NewFAQShimmerAdapter adapter = new NewFAQShimmerAdapter(faqList);
                newFAQShimmerList.SetAdapter(adapter);
            }
            else
            {
                List<NewFAQ> dummyList = new List<NewFAQ>();
                for(int i = 0; i < 3; i++)
                {
                    dummyList.Add(new NewFAQ()
                    {
                        Title = ""
                    });
                }
                NewFAQShimmerAdapter adapter = new NewFAQShimmerAdapter(dummyList);
                newFAQShimmerList.SetAdapter(adapter);
            }
        }

        public void LoadFAQList(List<NewFAQ> faqList)
        {
            newFAQShimmerView.Visibility = ViewStates.Gone;
            newFAQView.Visibility = ViewStates.Visible;
            if (faqList != null && faqList.Count() > 0)
            {
                NewFAQAdapter adapter = new NewFAQAdapter(faqList);
                newFAQListRecycleView.SetAdapter(adapter);
                adapter.ClickChanged += OnFAQClickChanged;
            }
            else
            {
                List<NewFAQ> dummyList = new List<NewFAQ>();
                for (int i = 0; i < 6; i++)
                {
                    if(i == 0)
                    {
                        dummyList.Add(new NewFAQ()
                        {
                            ID = "0",
                            Title = "How do I reset my password?"
                        });
                    }
                    else if (i == 1)
                    {
                        dummyList.Add(new NewFAQ()
                        {
                            ID = "1",
                            Title = "Learn how to read your meter."
                        });
                    }
                    else if (i == 2)
                    {
                        dummyList.Add(new NewFAQ()
                        {
                            ID = "2",
                            Title = "Check out how you can apply for AutoPay."
                        });
                    }
                    else if (i == 3)
                    {
                        dummyList.Add(new NewFAQ()
                        {
                            ID = "3",
                            Title = "How can I contact TNB?"
                        });
                    }
                    else if (i == 4)
                    {
                        dummyList.Add(new NewFAQ()
                        {
                            ID = "4",
                            Title = "How do i pay my bills through myTNB app?"
                        });
                    }
                    else if (i == 5)
                    {
                        dummyList.Add(new NewFAQ()
                        {
                            ID = "5",
                            Title = "What’s new on this app?"
                        });
                    }
                }
                NewFAQAdapter adapter = new NewFAQAdapter(dummyList);
                newFAQListRecycleView.SetAdapter(adapter);
                adapter.ClickChanged += OnFAQClickChanged;
            }
        }

        void OnFAQClickChanged(object sender, int position)
        {
            try
            {
                if (position == -1)
                {
                    // Toast.MakeText(this.Activity, "FAQ Position Unknown", ToastLength.Long).Show();
                }
                else
                {
                    // Toast.MakeText(this.Activity, "FAQ Position: " + position.ToString(), ToastLength.Long).Show();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowFeedbackMenu()
        {
            ShowBackButton(true);
            FeedbackMenuFragment fragment = new FeedbackMenuFragment();

            if (((DashboardHomeActivity)Activity) != null)
            {
                ((DashboardHomeActivity)Activity).SetCurrentFragment(fragment);
                ((DashboardHomeActivity)Activity).HideAccountName();
                ((DashboardHomeActivity)Activity).SetToolbarTitle(Resource.String.feedback_menu_activity_title);
            }
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.content_layout, fragment)
                     .CommitAllowingStateLoss();
        }

        public void ShowBackButton(bool flag)
        {
            var act = this.Activity as AppCompatActivity;

            var actionBar = act.SupportActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(flag);
            actionBar.SetDisplayShowHomeEnabled(flag);
        }
    }
}
