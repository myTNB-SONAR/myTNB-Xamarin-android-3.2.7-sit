using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;

using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB.Droid.Models;
using myTNB.SitecoreCMS.Model;
using myTNB.SQLite.SQLiteDataManager;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.PreLogin.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.WalkThrough.Adapter;
using myTNB_Android.Src.WalkThrough.MVP;
using System;
using System.Collections.Generic;
using System.Runtime;

namespace myTNB_Android.Src.WalkThrough
{

    [Activity(Label = "@string/app_name"
       , NoHistory = true
              , Icon = "@drawable/ic_launcher"
       , LaunchMode = LaunchMode.SingleInstance
       , ScreenOrientation = ScreenOrientation.Portrait
              , Theme = "@style/Theme.Dashboard")]
    public class WalkThroughActivity : BaseAppCompatActivity, WalkThroughContract.IView, ViewPager.IOnPageChangeListener
    {

        System.Timers.Timer t;
        [BindView(Resource.Id.viewpager)]
        ViewPager viewPager;

        WalkThroughDeck walkThroughDeck;

        [BindView(Resource.Id.next)]
        ImageButton btnNext;

        [BindView(Resource.Id.done)]
        Button btnDone;

        [BindView(Resource.Id.skip)]
        Button btnSkip;

        [BindView(Resource.Id.circles)]
        LinearLayout circles;

        [BindView(Resource.Id.progressBar)]
        ProgressBar mProgressBar;

        [BindView(Resource.Id.txtErrorMessage)]
        TextView txtErrorMessage;

        private WalkThroughAdapter pagerAdapter;
        private WalkThroughPresenter mPresenter;
        private WalkThroughContract.IUserActionsListener userActionsListener;
        public static readonly string TAG = "WalkThroughActivity";

        private string savedTimeStamp = "0000000";

        public int GetCurrentItem()
        {
            return viewPager.CurrentItem;
        }

        public bool IsActive()
        {
            return Window.DecorView.RootView.IsShown;
        }

        public override int ResourceId()
        {
            return Resource.Layout.WalkThroughView;
        }

        public void SetPresenter(WalkThroughContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public void ShowNext(int index)
        {
            try
            {
                if (index == viewPager.ChildCount)
                {
                    userActionsListener.NavigatePrelogin();
                }
                else
                {
                    viewPager.SetCurrentItem(index % viewPager.ChildCount, true);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public int GetTotalItems()
        {
            return viewPager.ChildCount;
        }

        public void ShowPreLogin()
        {
            StartActivity(typeof(PreLoginActivity));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                mPresenter = new WalkThroughPresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));

                //Deck deckOne = new Deck();
                //deckOne.imageId = Resource.Drawable.dashboard;
                //deckOne.heading = Resources.GetString(Resource.String.walkthrough_1_heading);
                //deckOne.content = Resources.GetString(Resource.String.walkthrough_1_content);

                //Deck deckTwo = new Deck();
                //deckTwo.imageId = Resource.Drawable.billing;
                //deckTwo.heading = Resources.GetString(Resource.String.walkthrough_2_heading);
                //deckTwo.content = Resources.GetString(Resource.String.walkthrough_2_content);

                //Deck deckThree = new Deck();
                //deckThree.imageId = Resource.Drawable.payment;
                //deckThree.heading = Resources.GetString(Resource.String.walkthrough_3_heading);
                //deckThree.content = Resources.GetString(Resource.String.walkthrough_3_content);

                //Deck[] deckParam = new Deck[3];
                //deckParam[0] = deckOne;
                //deckParam[1] = deckTwo;
                //deckParam[2] = deckThree;
                //walkThroughDeck = new WalkThroughDeck(deckParam);
                //viewPager.OffscreenPageLimit = walkThroughDeck.NumCards;
                //pagerAdapter = new WalkThroughAdapter(SupportFragmentManager, walkThroughDeck);
                viewPager.Adapter = pagerAdapter;
                //[Preserve]
                //viewPager.AddOnPageChangeListener(this);
                //buildCircles();

                TextViewUtils.SetMuseoSans300Typeface(btnDone, btnSkip);
                TextViewUtils.SetMuseoSans500Typeface(txtErrorMessage);

                //this.userActionsListener.Start();

                if (MyTNBApplication.siteCoreUpdated)
                {
                    mProgressBar.Visibility = ViewStates.Visible;
                    btnNext.Visibility = ViewStates.Gone;
                    this.userActionsListener.OnGetWalkThroughData();
                }
                else
                {
                    ShowWalkThroughData(true);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void buildCircles()
        {
            try
            {
                float scale = this.Resources.DisplayMetrics.Density;
                int padding = (int)(3 * scale + 0.5f);
                for (int i = 0; i < pagerAdapter.Count; i++)
                {
                    ImageView circle = new ImageView(this);
                    circle.SetImageResource(Resource.Drawable.inactive_indicator);
                    circle.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                    circle.SetAdjustViewBounds(true);
                    circle.SetPadding(padding, 0, padding, 0);
                    circles.AddView(circle);
                }

                setIndicator(0);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void setIndicator(int index)
        {
            try
            {
                if (index < pagerAdapter.Count)
                {
                    for (int i = 0; i < pagerAdapter.Count; i++)
                    {
                        ImageView circle = (ImageView)circles.GetChildAt(i);
                        if (i == index)
                        {
                            //TODO: R.color.text_selected
                            circle.SetImageResource(Resource.Drawable.active_indicator);
                        }
                        else
                        {
                            circle.SetImageResource(Resource.Drawable.inactive_indicator);
                            //circle.setColorFilter(ResourcesCompat.getCoFFlor(getResources(), android.R.color.transparent, null));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        [OnClick(Resource.Id.next)]
        void OnNext(object sender, EventArgs e)
        {
            this.userActionsListener.NavigateNextScreen();
        }

        [OnClick(Resource.Id.skip)]
        void OnSkip(object sender, EventArgs e)
        {
            this.userActionsListener.OnSkip();
            this.userActionsListener.NavigatePrelogin();
        }

        [OnClick(Resource.Id.done)]
        void OnDone(object sender, EventArgs e)
        {
            this.userActionsListener.NavigatePrelogin();
        }

        public void ShowDone()
        {
            // TODO : IMPLEMENT SHOW 'DONE' TEXT REPLACING ARROW
            Log.Debug(TAG, "Show Done is Called");
            btnDone.Visibility = ViewStates.Visible;
            btnNext.Visibility = ViewStates.Gone;
            btnSkip.Visibility = ViewStates.Gone;
        }

        public void OnPageScrollStateChanged(int state)
        {
            //throw new NotImplementedException();
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
            //throw new NotImplementedException();
        }

        public void OnPageSelected(int position)
        {
            this.userActionsListener.OnPageSelected(position);
            setIndicator(position);
            Log.Debug(TAG, "Position " + position);
        }

        public void OnTimeStampRecieved(string timestamp)
        {
            try
            {
                if (timestamp != null)
                {
                    if (timestamp.Equals(savedTimeStamp))
                    {
                        MyTNBApplication.siteCoreUpdated = false;
                        ShowWalkThroughData(true);
                    }
                    else
                    {
                        MyTNBApplication.siteCoreUpdated = true;
                        this.userActionsListener.OnGetWalkThroughData();
                    }
                }
                else
                {
                    MyTNBApplication.siteCoreUpdated = true;
                    this.userActionsListener.OnGetWalkThroughData();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ShowWalkThroughData(bool success)
        {
            try
            {
                if (success)
                {
                    WalkthroughScreensEntity wtManager = new WalkthroughScreensEntity();
                    List<WalkthroughScreensEntity> items = wtManager.GetAllItems();
                    if (items != null)
                    {
                        Deck[] deckParam = new Deck[items.Count];
                        int i = 0;
                        if (items.Count != 0)
                        {
                            foreach (WalkthroughScreensModel obj in items)
                            {
                                Deck deck = new Deck();
                                deck.imageId = Resource.Drawable.payment;
                                deck.imageUrl = obj.Image;
                                deck.heading = obj.Text;
                                deck.content = obj.SubText;

                                deckParam[i] = deck;
                                i++;
                            }
                        }
                        else
                        {
                            deckParam = GetDefaultData();
                        }
                        try
                        {
                            RunOnUiThread(() =>
                            {
                                walkThroughDeck = new WalkThroughDeck(deckParam);
                                viewPager.OffscreenPageLimit = walkThroughDeck.NumCards;
                                pagerAdapter = new WalkThroughAdapter(SupportFragmentManager, walkThroughDeck);
                                viewPager.Adapter = pagerAdapter;
                                pagerAdapter.NotifyDataSetChanged();
                                viewPager.AddOnPageChangeListener(this);
                                mProgressBar.Visibility = ViewStates.Gone;
                                btnNext.Visibility = ViewStates.Visible;
                                txtErrorMessage.Visibility = ViewStates.Gone;
                                buildCircles();
                            });
                        }
                        catch (Exception e)
                        {
                            Log.Error(TAG, e.Message);
                            txtErrorMessage.Visibility = ViewStates.Visible;
                        }
                    }
                    else
                    {
                        mProgressBar.Visibility = ViewStates.Gone;
                        txtErrorMessage.Visibility = ViewStates.Visible;
                    }
                }
                else
                {
                    mProgressBar.Visibility = ViewStates.Gone;
                    txtErrorMessage.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnSavedTimeStampRecievd(string timestamp)
        {
            try
            {
                if (timestamp != null)
                {
                    savedTimeStamp = timestamp;
                }
                this.userActionsListener.OnGetTimeStamp();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OnSiteCoreServiceFailed(string message)
        {
            txtErrorMessage.Visibility = ViewStates.Visible;
        }

        public Deck[] GetDefaultData()
        {
            Deck[] deckParam = new Deck[3];
            try
            {
                Deck deckOne = new Deck();
                deckOne.imageId = Resource.Drawable.dashboard;
                deckOne.heading = Resources.GetString(Resource.String.walkthrough_1_heading);
                deckOne.content = Resources.GetString(Resource.String.walkthrough_1_content);

                Deck deckTwo = new Deck();
                deckTwo.imageId = Resource.Drawable.billing;
                deckTwo.heading = Resources.GetString(Resource.String.walkthrough_2_heading);
                deckTwo.content = Resources.GetString(Resource.String.walkthrough_2_content);

                Deck deckThree = new Deck();
                deckThree.imageId = Resource.Drawable.payment;
                deckThree.heading = Resources.GetString(Resource.String.walkthrough_3_heading);
                deckThree.content = Resources.GetString(Resource.String.walkthrough_3_content);


                deckParam[0] = deckOne;
                deckParam[1] = deckTwo;
                deckParam[2] = deckThree;
                walkThroughDeck = new WalkThroughDeck(deckParam);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
            return deckParam;
        }



        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                FirebaseAnalyticsUtils.SetScreenName(this, "Pre-Login Walkthrough");
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}