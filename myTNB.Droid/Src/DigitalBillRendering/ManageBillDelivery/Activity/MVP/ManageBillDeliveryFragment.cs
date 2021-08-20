using Android.OS;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.ManageBillDelivery.MVP
{
    public class ManageBillDeliveryFragment : BaseFragmentV4Custom
    {
        private static string TITLE = "slide_title";
        private static string DESCRIPTION = "slide_description";
        private static string IMAGE = "image";
        private static string IS_LAST_ITEM = "islastitem";
        private string appLanguage;
        const string PAGE_ID = "";

        [BindView(Resource.Id.walkthrough_layout)]
        LinearLayout bgLayout;

        [BindView(Resource.Id.img_display)]
        ImageView imageSource;

        [BindView(Resource.Id.txtTitle)]
        TextView titleView;

        [BindView(Resource.Id.txtMessage)]
        TextView descriptionView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public static ManageBillDeliveryFragment Instance(ManageBillDeliveryModel model, bool isLastItem)
        {
            ManageBillDeliveryFragment fragment = new ManageBillDeliveryFragment();
            Bundle args = new Bundle();
            args.PutString(IMAGE, model.Image);
            args.PutString(TITLE, model.Title);
            args.PutString(DESCRIPTION, model.Description);
            args.PutBoolean(IS_LAST_ITEM, isLastItem);
            fragment.Arguments = args;
            return fragment;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            base.OnViewCreated(view, savedInstanceState);
            string imageUrl = Arguments.GetString(IMAGE, "");
            string title = Arguments.GetString(TITLE, "");
            string description = Arguments.GetString(DESCRIPTION, "");
            bool isLastItem = Arguments.GetBoolean(IS_LAST_ITEM, false);

            TextViewUtils.SetMuseoSans500Typeface(titleView);
            TextViewUtils.SetMuseoSans300Typeface(descriptionView);

            TextViewUtils.SetTextSize14(descriptionView);
            TextViewUtils.SetTextSize16(titleView);

            appLanguage = LanguageUtil.GetAppLanguage();

            LinearLayout.LayoutParams imgParam;
            int imgWidth, imgHeight;
            float heightRatio;

            switch (imageUrl)
            {
                case "manage_bill_delivery_0":
                    imageSource.SetImageResource(Resource.Drawable.manage_bill_delivery_0);
                    break;
                case "manage_bill_delivery_1":
                    imageSource.SetImageResource(Resource.Drawable.manage_bill_delivery_1);
                    break;
                case "manage_bill_delivery_2":
                    imageSource.SetImageResource(Resource.Drawable.manage_bill_delivery_2);
                    break;
                case "manage_bill_delivery_3":
                    imageSource.SetImageResource(Resource.Drawable.manage_bill_delivery_3);
                    break;

                default:
                    imageSource.SetImageResource(Resource.Drawable.manage_bill_delivery_0);
                    break;
            }

            titleView.Text = title;
            descriptionView.TextFormatted = GetFormattedText(description);
        }

        public override int ResourceId()
        {
            return Resource.Layout.ManageBillDeliveryFragmentLayout;
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }
    }
}