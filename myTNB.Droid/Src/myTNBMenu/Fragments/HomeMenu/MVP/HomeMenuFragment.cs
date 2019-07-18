
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using CheeseBind;
using Facebook.Shimmer;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.myTNBMenu.Adapter;
using myTNB_Android.Src.myTNBMenu.Models;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class HomeMenuFragment : BaseFragment
	{
        [BindView(Resource.Id.myServiceShimmerView)]
        ShimmerFrameLayout myServiceShimmerView;

        [BindView(Resource.Id.myServiceList)]
        RecyclerView myServiceListRecycleView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return base.OnCreateView(inflater, container, savedInstanceState);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            try
            {
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
