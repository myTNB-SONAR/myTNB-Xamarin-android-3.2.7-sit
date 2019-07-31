using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils.Custom.ProgressDialog;

namespace myTNB_Android.Src.SSMRTerminate.MVP
{
    [Activity(Label = "Discontinue Self Reading", Theme = "@style/Theme.Dashboard")]
    public class SSMRTerminateActivity : BaseToolbarAppCompatActivity
    {
        LoadingOverlay loadingOverlay;

        public override int ResourceId()
        {
            return Resource.Layout.SSMRDiscontinueApplicationLayout;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(string name, Context context, IAttributeSet attrs)
        {
            return base.OnCreateView(name, context, attrs);
        }
        
    }
}
