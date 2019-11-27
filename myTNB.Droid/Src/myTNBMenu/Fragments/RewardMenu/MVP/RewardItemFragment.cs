using Android.Content;
using Android.OS;
using Android.App;
using Android.Views;
using myTNB_Android.Src.Utils;
using Android.Widget;

namespace myTNB_Android.Src.myTNBMenu.Fragments.HomeMenu.MVP
{
    public class RewardItemFragment : Fragment
    {
        TextView txtTest;

        string testTitle = "Test 123";

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (Arguments.ContainsKey("Title"))
            {
                testTitle = Arguments.GetString("Title");
            }
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
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
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.RewardIListtemLayout, container, false);

            txtTest = rootView.FindViewById<TextView>(Resource.Id.txtTest);
            txtTest.Text = testTitle;

            return rootView;
        }
    }
}
