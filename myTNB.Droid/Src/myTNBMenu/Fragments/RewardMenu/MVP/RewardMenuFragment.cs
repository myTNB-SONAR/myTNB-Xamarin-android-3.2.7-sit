﻿
using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class RewardMenuFragment : BaseFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            try
            {
                FirebaseAnalyticsUtils.SetFragmentScreenName(this, "Rewards");
            }
            catch (Java.Lang.ClassCastException e)
            {
                Utility.LoggingNonFatalError(e);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            try
            {
                
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public bool IsActive()
        {
            return IsAdded && IsVisible && !IsDetached && !IsRemoving;
        }

        public override void OnPause()
        {
            base.OnPause();
            NewAppTutorialUtils.ForceCloseNewAppTutorial();
        }

        public override void OnResume()
        {
            base.OnResume();

            try
            {
                var act = this.Activity as AppCompatActivity;

                var actionBar = act.SupportActionBar;
                actionBar.Show();
                ShowBackButton(false);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return base.OnCreateView(inflater, container, savedInstanceState);
        }


        public override int ResourceId()
        {
            return Resource.Layout.HomeMenuFragmentView;
        }

        public void ShowBackButton(bool flag)
        {
            var act = this.Activity as AppCompatActivity;

            var actionBar = act.SupportActionBar;
            actionBar.SetDisplayHomeAsUpEnabled(flag);
            actionBar.SetDisplayShowHomeEnabled(flag);
        }

        public string GetDeviceId()
        {
            return this.DeviceId();
        }
    }
}
