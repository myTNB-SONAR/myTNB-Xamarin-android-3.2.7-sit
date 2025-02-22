﻿using Android.OS;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.myTNBMenu.Fragments.BillsMenu
{
    public class BillingPaymentNoInternetConnection : BaseFragment
    {

        [BindView(Resource.Id.txtNoInternetContent)]
        TextView txtNoInternetContent;

        public override int ResourceId()
        {
            return Resource.Layout.BillsPaymentTabNoInternetConnectionView;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            TextViewUtils.SetMuseoSans300Typeface(txtNoInternetContent);
        }
    }
}