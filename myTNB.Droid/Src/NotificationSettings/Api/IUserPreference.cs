﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Refit;
using System.Threading.Tasks;
using myTNB_Android.Src.NotificationSettings.Models;
using myTNB_Android.Src.NotificationSettings.Requests;
using System.Threading;

namespace myTNB_Android.Src.NotificationSettings.Api
{
    public interface IUserPreference
    {
        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/SaveUserNotificationChannelPreference")]
        Task<SaveUserPreferenceResponse> SaveUserNotificationChannelPreference([Body] SaveUserNotificationChannelPreferenceRequest request, CancellationToken cancellationToken);

        [Headers("Content-Type:application/json; charset=utf-8")]
        [Post("/v5/my_billingssp.asmx/SaveUserNotificationTypePreference")]
        Task<SaveUserPreferenceResponse> SaveUserNotificationTypePreference([Body] SaveUserNotificationTypePreferenceRequest request, CancellationToken cancellationToken);

    }
}