using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.CompoundView;
using myTNB_Android.Src.MultipleAccountPayment.Activity;
using myTNB_Android.Src.NotificationDetails.Models;
using myTNB_Android.Src.NotificationDetails.MVP;
using myTNB_Android.Src.Notifications.Models;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.NotificationDetails.Activity
{
    [Activity(Label = "Notification Detail", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class UserNotificationDetailActivity : BaseToolbarAppCompatActivity, UserNotificationDetailContract.IView
    {
        [BindView(Resource.Id.notificationDetailBannerImg)]
        ImageView notificationDetailBannerImg;

        [BindView(Resource.Id.notificationDetailTitle)]
        TextView notificationDetailTitle;

        [BindView(Resource.Id.notificationDetailMessage)]
        TextView notificationDetailMessage;

        Models.NotificationDetails notificationDetails;
        UserNotificationData userNotificationData;
        int position;
        UserNotificationDetailPresenter mPresenter;
        AlertDialog removeDialog;


        public override int ResourceId()
        {
            return Resource.Layout.UserNotificationDetailLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.NotificationDetailMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_delete_notification:
                    removeDialog = new AlertDialog.Builder(this)

                        .SetTitle(Resource.String.notification_detail_remove_notification_dialog_title)
                        .SetMessage(GetString(Resource.String.notification_detail_remove_notification_dialog_content))
                        .SetNegativeButton(Resource.String.notification_detail_remove_notification_negative_btn,
                        delegate
                        {
                            removeDialog.Dismiss();
                        })
                        .SetPositiveButton(Resource.String.notification_detail_remove_notification_positive_btn,
                        delegate
                        {
                            //this.userActionsListener.OnRemoveNotification(notificationDetails);
                        })
                        .Show();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                mPresenter = new UserNotificationDetailPresenter(this);
                base.OnCreate(savedInstanceState);
                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    if (extras.ContainsKey(Constants.SELECTED_NOTIFICATION_DETAIL_ITEM))
                    {
                        notificationDetails = DeSerialze<NotificationDetails.Models.NotificationDetails>(extras.GetString(Constants.SELECTED_NOTIFICATION_DETAIL_ITEM));
                    }

                    if (extras.ContainsKey(Constants.SELECTED_NOTIFICATION_LIST_ITEM))
                    {
                        userNotificationData = DeSerialze<UserNotificationData>(extras.GetString(Constants.SELECTED_NOTIFICATION_LIST_ITEM));
                    }

                    position = extras.GetInt(Constants.SELECTED_NOTIFICATION_ITEM_POSITION);
                }
                SetStatusBarBackground(Resource.Drawable.dashboard_fluid_background);
                //SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

                mPresenter.EvaluateDetail(notificationDetails);
                RenderUI();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void RenderUI()
        {
            try
            {
                NotificationDetailModel detailModel = mPresenter.GetNotificationDetailModel();
                notificationDetailBannerImg.SetImageResource(detailModel.imageResourceBanner);
                notificationDetailTitle.Text = detailModel.title;
                notificationDetailMessage.TextFormatted = GetFormattedText(detailModel.message);
                NotificationDetailCTAComponent ctaComponent = FindViewById<NotificationDetailCTAComponent>(Resource.Id.notificationCTAComponent);
                ctaComponent.SetCTAButton(detailModel.ctaList);
            }
            catch(Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void ViewBill()
        {
            
        }

        public void PayNow()
        {
            
        }

        public void ContactUs()
        {
            
        }

        public void ViewUsage()
        {
            
        }
    }
}
