﻿using System;
using System.Runtime;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.myTNBMenu.Models;
using Newtonsoft.Json;
using myTNB_Android.Src.Utils;
using Android.Content.PM;
using Android.Support.V7.Widget;
using myTNB_Android.Src.SSMRTerminate.Api;
using System.Collections.Generic;
using myTNB_Android.Src.SSMRTerminate.Adapter;
using CheeseBind;
using Android.Graphics.Drawables;

namespace myTNB_Android.Src.SSMRTerminate.MVP
{
    [Activity(Label = "Select Reason"
        , ScreenOrientation = ScreenOrientation.Portrait
        , Theme = "@style/Theme.Dashboard")]
    public class SSMRTerminationReasonSelectionActivity : BaseToolbarAppCompatActivity
    {
        [BindView(Resource.Id.ssmrTerminationList)]
        RecyclerView mTerminationRecyclerView;

        SSMRTerminationReasonAdapter adapter;

        List<TerminationReasonModel> list = new List<TerminationReasonModel>();

        public override int ResourceId()
        {
            return Resource.Layout.SSMRTerminationSelectionLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            mTerminationRecyclerView.SetLayoutManager(new LinearLayoutManager(this));

            Bundle extras = Intent.Extras;

            if (extras.ContainsKey(Constants.SMR_TERMINATION_REASON_KEY))
            {
                list = JsonConvert.DeserializeObject<List<TerminationReasonModel>>(extras.GetString(Constants.SMR_TERMINATION_REASON_KEY));
                adapter = new SSMRTerminationReasonAdapter(list);
                mTerminationRecyclerView.SetAdapter(adapter);
                adapter.ClickChanged += OnClickChanged;
            }

            /* Drawable drawable = Resources.GetDrawable(Resource.Drawable.GradientStatusBar);
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Lollipop)
            {
                this.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                this.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                this.Window.SetBackgroundDrawable(drawable);
            }
            this.toolbar.SetBackgroundDrawable(drawable);*/

        }

        public override View OnCreateView(string name, Context context, IAttributeSet attrs)
        {
            return base.OnCreateView(name, context, attrs);
        }

        void OnClickChanged(object sender, int position)
        {
            try
            {
                if (position != -1)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i == position)
                        {
                            list[i].IsSelected = true;
                        }
                        else
                        {
                            list[i].IsSelected = false;
                        }
                    }
                }

                Intent returnIntent = new Intent();
                returnIntent.PutExtra(Constants.SMR_TERMINATION_REASON_KEY, JsonConvert.SerializeObject(list));
                SetResult(Result.Ok, returnIntent);
                Finish();
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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

        public string GetDeviceId()
        {
            return this.DeviceId();
        }

    }
}
