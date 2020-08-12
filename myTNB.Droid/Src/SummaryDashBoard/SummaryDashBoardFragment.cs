using Android.App;
using Android.Content;
using Android.OS;



using Android.Text;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Java.Lang;
using myTNB_Android.Src.AddAccount.Activity;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.FAQ.Activity;
using myTNB_Android.Src.myTNBMenu.Activity;
using myTNB_Android.Src.Notifications.Activity;
using myTNB_Android.Src.SummaryDashBoard.Models;
using myTNB_Android.Src.SummaryDashBoard.MVP;
using myTNB_Android.Src.SummaryDashBoard.SummaryListener;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;
using System;
using System.Collections.Generic;
using System.Linq;
using static myTNB_Android.Src.SummaryDashBoard.MVP.SummaryDashboardContract;

namespace myTNB_Android.Src.SummaryDashBoard
{

    public class SummaryDashBoardFragment : BaseFragment, SummaryDashboardContract.IView, ISummaryListener
    {
        [BindView(Resource.Id.greetingText)]
        TextView greetingTxt;

        [BindView(Resource.Id.ReaccountList)]
        RecyclerView reAccRecyclerView;

        [BindView(Resource.Id.normalAccountList)]
        RecyclerView normalRecyclerView;

        [BindView(Resource.Id.loadMoreText)]
        TextView loadMore;

        [BindView(Resource.Id.summaryRootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.htab_header)]
        ImageView greetingImage;


        [BindView(Resource.Id.userNameText)]
        TextView userNameTxt;

        [BindView(Resource.Id.laod_more_divider)]
        View loadMoreDivider;

        [BindView(Resource.Id.greeting_layout)]
        LinearLayout greetingLayout;

        [BindView(Resource.Id.downtime_layout)]
        LinearLayout downtimeLayout;

        [BindView(Resource.Id.txtDowntimeMessage)]
        TextView txtDowntimeMessage;

        [BindView(Resource.Id.layout_content)]
        LinearLayout layoutContent;

        [BindView(Resource.Id.summaryFooter)]
        TextView addAcount;

        [BindView(Resource.Id.layout_api_refresh)]
        LinearLayout layoutNewRefresh;

        [BindView(Resource.Id.btnRefresh)]
        Button btnNewRefresh;

        [BindView(Resource.Id.refresh_content)]
        TextView txtNewRefreshMessage;

        List<SummaryDashBoardDetails> itemList = null;

        SummaryDashboardPresenter presenter = null;
        SummaryDashboardContract.ISummaryDashBoardListener listener = null;
        private ISummaryFragmentToDashBoardActivtyListener mCallBack = null;
        DashboardActivity activity = null;

        private LoadingOverlay loadingOverlay;

        public override int ResourceId()
        {
            return Resource.Layout.SummaryDashBoardLayout;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here

            SetHasOptionsMenu(true);
        }


        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            try
            {
                mCallBack = context as ISummaryFragmentToDashBoardActivtyListener;
            }
            catch (ClassCastException e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);

            try
            {
                mCallBack = activity as ISummaryFragmentToDashBoardActivtyListener;
            }
            catch (ClassCastException e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnResume()
        {
            base.OnResume();
            this.Activity.InvalidateOptionsMenu();
            if (activity != null)
            {
                activity.SetCurrentFragment(this);
            }
        }

        public override void OnStart()
        {
            try
            {
                base.OnStart();

                listener.Start();
                loadData();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        private IMenu menu;
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.DashboardToolbarMenu, menu);
            this.menu = menu;
            if (UserNotificationEntity.HasNotifications())
            {
                menu.FindItem(Resource.Id.action_notification).SetIcon(ContextCompat.GetDrawable(this.Activity, Resource.Drawable.ic_header_notification_unread));
            }
            else
            {
                menu.FindItem(Resource.Id.action_notification).SetIcon(ContextCompat.GetDrawable(this.Activity, Resource.Drawable.ic_header_notification));
            }
            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_notification:
                    this.listener.OnNotification();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }



        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            try
            {
                activity = ((DashboardActivity)Activity);

                TextViewUtils.SetMuseoSans500Typeface(greetingTxt, loadMore, userNameTxt);
                TextViewUtils.SetMuseoSans500Typeface(addAcount, btnNewRefresh);
                TextViewUtils.SetMuseoSans300Typeface(txtDowntimeMessage, txtNewRefreshMessage);

                reAccRecyclerView.SetLayoutManager(new LinearLayoutManager(this.Activity));
                normalRecyclerView.SetLayoutManager(new LinearLayoutManager(this.Activity));

                addAcount.Visibility = ViewStates.Gone;
                IsLoadMoreButtonVisible(false);

                presenter = new SummaryDashboardPresenter(this);

                loadMore.Click += delegate
                {
                    listener.DoLoadMoreAccount();
                };

                btnNewRefresh.Click += delegate
                {
                    btnNewRefresh.Enabled = false;
                    btnNewRefresh.Clickable = false;
                    Handler h = new Handler();
                    Action myAction = () =>
                    {
                        btnNewRefresh.Enabled = true;
                        btnNewRefresh.Clickable = true;
                    };
                    h.PostDelayed(myAction, 2000);

                    ShowProgressDialog();
                    this.listener.RefreshAccountSummary();
                };

                addAcount.Click += delegate
                {
                    Intent linkAccount = new Intent(this.Activity, typeof(LinkAccountActivity));
                    linkAccount.PutExtra("fromDashboard", true);
                    StartActivity(linkAccount);
                };

                DownTimeEntity bcrmEntity = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
                txtDowntimeMessage.Click += delegate
                {
                    if (bcrmEntity.IsDown)
                    {
                        string textMessage = bcrmEntity.DowntimeMessage;
                        if (textMessage != null && textMessage.Contains("http"))
                        {
                            //Launch webview
                            int startIndex = textMessage.LastIndexOf("=") + 2;
                            int lastIndex = textMessage.LastIndexOf("\"");
                            int lengthOfId = (lastIndex - startIndex);
                            if (lengthOfId < textMessage.Length)
                            {
                                string url = textMessage.Substring(startIndex, lengthOfId);
                                if (!string.IsNullOrEmpty(url))
                                {
                                    Intent intent = new Intent(Intent.ActionView);
                                    intent.SetData(Android.Net.Uri.Parse(url));
                                    StartActivity(intent);
                                }
                            }
                        }
                        else if (textMessage != null && textMessage.Contains("faq"))
                        {
                            //Lauch FAQ
                            int startIndex = textMessage.LastIndexOf("=") + 1;
                            int lastIndex = textMessage.LastIndexOf("}");
                            int lengthOfId = (lastIndex - startIndex) + 1;
                            if (lengthOfId < textMessage.Length)
                            {
                                string faqid = textMessage.Substring(startIndex, lengthOfId);
                                if (!string.IsNullOrEmpty(faqid))
                                {
                                    Intent faqIntent = new Intent(this.Activity, typeof(FAQListActivity));
                                    faqIntent.PutExtra(Constants.FAQ_ID_PARAM, faqid);
                                    Activity.StartActivity(faqIntent);
                                }
                            }
                        }
                    }

                };
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }


        private void loadData()
        {
            try
            {
                DownTimeEntity bcrmDownTime = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
                if (bcrmDownTime != null && bcrmDownTime.IsDown)
                {
                    downtimeLayout.Visibility = ViewStates.Visible;
                    greetingLayout.Visibility = ViewStates.Gone;
                    layoutNewRefresh.Visibility = ViewStates.Gone;
                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                    {
                        txtDowntimeMessage.TextFormatted = Html.FromHtml(bcrmDownTime.DowntimeMessage, FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
                        txtDowntimeMessage.TextFormatted = Html.FromHtml(bcrmDownTime.DowntimeMessage);
                    }
                }
                else
                {
                    downtimeLayout.Visibility = ViewStates.Gone;
                    greetingLayout.Visibility = ViewStates.Visible;
                    layoutNewRefresh.Visibility = ViewStates.Gone;
                }
                listener.FetchAccountSummary();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }



        public void SetPresenter(SummaryDashboardContract.ISummaryDashBoardListener userActionListener)
        {
            this.listener = userActionListener;
        }

        public bool IsActive()
        {
            return IsVisible;
        }

        public void ShowProgressDialog()
        {
            try
            {
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }

                loadingOverlay = new LoadingOverlay(this.Activity, Resource.Style.LoadingOverlyDialogStyle);
                loadingOverlay.Show();
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
                if (loadingOverlay != null && loadingOverlay.IsShowing)
                {
                    loadingOverlay.Dismiss();
                }
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void LoadREAccountData(List<SummaryDashBoardDetails> summaryDetails)
        {
            layoutContent.Visibility = ViewStates.Visible;
            if (summaryDetails != null && summaryDetails.Count() > 0)
            {
                SummaryDashBoardAdapter adapter = new SummaryDashBoardAdapter(summaryDetails, this);
                reAccRecyclerView.SetAdapter(adapter);
            }

        }

        public void SetUserName(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                userNameTxt.Text = userName;
            }

        }


        public void LoadNormalAccountData(List<SummaryDashBoardDetails> summaryDetails)
        {
            layoutContent.Visibility = ViewStates.Visible;
            if (summaryDetails != null && summaryDetails.Count() > 0)
            {
                SummaryNormalAdapter adapter = new SummaryNormalAdapter(summaryDetails, this);
                normalRecyclerView.SetAdapter(adapter);
            }

            if (addAcount.Visibility == ViewStates.Gone)
            {
                listener.EnableLoadMore();
                addAcount.Visibility = ViewStates.Visible;
            }

        }

        public void IsLoadMoreButtonVisible(bool isVisible)
        {
            loadMore.Visibility = isVisible ? ViewStates.Visible : ViewStates.Gone;
            loadMoreDivider.Visibility = isVisible ? ViewStates.Visible : ViewStates.Gone;
        }

        public void OnClick(SummaryDashBoardDetails summaryDashBoardDetails)
        {
            if (summaryDashBoardDetails != null && !string.IsNullOrEmpty(summaryDashBoardDetails.AccNumber))
            {
                CustomerBillingAccount.RemoveSelected();
                CustomerBillingAccount.SetSelected(summaryDashBoardDetails.AccNumber);

                if (mCallBack != null)
                {
                    mCallBack.NavigateToDashBoardFragment();
                    //ShowBackArrowIndicator()
                }
            }


        }

        public bool HasNetworkConnection()
        {
            return ConnectionUtils.HasInternetConnection(this.Activity);
        }

        public void ShowNotification()
        {
            StartActivity(new Intent(this.Activity, typeof(NotificationActivity)));
        }

        public void ShowRefreshSummaryDashboard(bool yesno, string contentMsg, string buttonMsg)
        {
            try
            {
                layoutContent.Visibility = ViewStates.Visible;
                DownTimeEntity bcrmDownTime = DownTimeEntity.GetByCode(Constants.BCRM_SYSTEM);
                if (bcrmDownTime != null && bcrmDownTime.IsDown)
                {
                    downtimeLayout.Visibility = ViewStates.Visible;
                    greetingLayout.Visibility = ViewStates.Gone;
                    layoutNewRefresh.Visibility = ViewStates.Gone;
                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                    {
                        txtDowntimeMessage.TextFormatted = Html.FromHtml(bcrmDownTime.DowntimeMessage, FromHtmlOptions.ModeLegacy);
                    }
                    else
                    {
                        txtDowntimeMessage.TextFormatted = Html.FromHtml(bcrmDownTime.DowntimeMessage);
                    }
                }
                else
                {
                    downtimeLayout.Visibility = ViewStates.Gone;
                    greetingLayout.Visibility = yesno ? ViewStates.Gone : ViewStates.Visible;
                    layoutNewRefresh.Visibility = yesno ? ViewStates.Visible : ViewStates.Gone;
                    if (yesno)
                    {
                        btnNewRefresh.Text = string.IsNullOrEmpty(buttonMsg) ? GetString(Resource.String.text_new_refresh) : buttonMsg;

                        if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                        {
                            txtNewRefreshMessage.TextFormatted = string.IsNullOrEmpty(contentMsg) ? Html.FromHtml(GetString(Resource.String.text_new_refresh_content), FromHtmlOptions.ModeLegacy) : Html.FromHtml(contentMsg, FromHtmlOptions.ModeLegacy);
                        }
                        else
                        {
                            txtNewRefreshMessage.TextFormatted = string.IsNullOrEmpty(contentMsg) ? Html.FromHtml(GetString(Resource.String.text_new_refresh_content)) : Html.FromHtml(contentMsg);
                        }
                    }
                }


            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetGreetingImageAndText(eGreeting greeting, string text)
        {
            greetingTxt.Text = text;
            switch (greeting)
            {
                case eGreeting.MORNING:
                    greetingImage.SetImageResource(Resource.Drawable.illustration_Morning);
                    break;
                case eGreeting.AFTERNOON:
                    greetingImage.SetImageResource(Resource.Drawable.illustration_Afternoon);
                    break;
                default:
                    greetingImage.SetImageResource(Resource.Drawable.illustration_Evening);
                    break;
            }
        }




    }
}