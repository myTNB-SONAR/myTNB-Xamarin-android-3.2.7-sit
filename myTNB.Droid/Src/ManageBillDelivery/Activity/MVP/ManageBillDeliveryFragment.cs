
using System;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB;
using myTNB_Android.Src.Base;
using myTNB_Android.Src.Base.Fragments;
using myTNB_Android.Src.Database.Model;
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

        [BindView(Resource.Id.btnToggleEN)]
        RadioButton btnToggleEN;

        [BindView(Resource.Id.btnToggleMS)]
        RadioButton btnToggleMS;

        [BindView(Resource.Id.btnToggleContainer)]
        RelativeLayout btnToggleContainer;
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
            TextViewUtils.SetMuseoSans300Typeface(descriptionView, btnToggleEN, btnToggleMS);

            TextViewUtils.SetTextSize12(descriptionView);
            TextViewUtils.SetTextSize14(titleView);

            appLanguage = LanguageUtil.GetAppLanguage();

            LinearLayout.LayoutParams imgParam;
            int imgWidth, imgHeight;
            float heightRatio;
            switch (imageUrl)
            {
                case "manage_bill_delivery_0":
                    btnToggleContainer.Visibility = ViewStates.Visible;
                    imageSource.SetImageResource(Resource.Drawable.manage_bill_delivery_0);
                    bgLayout.SetBackgroundResource(Resource.Drawable.manage_bill_delivery_0);
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(70f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;

                    imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    heightRatio = 216f / 250f;
                    imgHeight = (int)(imgWidth * (heightRatio));
                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
                case "manage_bill_delivery_1":
                    imageSource.SetImageResource(Resource.Drawable.manage_bill_delivery_1);
                    bgLayout.SetBackgroundResource(Resource.Drawable.manage_bill_delivery_1);
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(70f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;

                    imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    heightRatio = 216f / 250f;
                    imgHeight = (int)(imgWidth * (heightRatio));
                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
                case "manage_bill_delivery_2":
                    imageSource.SetImageResource(Resource.Drawable.manage_bill_delivery_2);
                    bgLayout.SetBackgroundResource(Resource.Drawable.manage_bill_delivery_2);
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(79f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;

                    imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    heightRatio = 207f / 250f;
                    imgHeight = (int)(imgWidth * (heightRatio));

                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
                
                default:
                    imageSource.SetImageResource(Resource.Drawable.manage_bill_delivery_0);
                    bgLayout.SetBackgroundResource(Resource.Drawable.InstallWalkthroughFirstBg);
                    bgLayout.SetPadding(bgLayout.PaddingLeft, (int)DPUtils.ConvertDPToPx(70f), bgLayout.PaddingRight, bgLayout.PaddingBottom);

                    imgParam = imageSource.LayoutParameters as LinearLayout.LayoutParams;

                    imgWidth = GetDeviceHorizontalScaleInPixel(0.781f);
                    heightRatio = 216f / 250f;
                    imgHeight = (int)(imgWidth * (heightRatio));
                    imgParam.Width = imgWidth;
                    imgParam.Height = imgHeight;
                    break;
            }

            titleView.Text = title;
            descriptionView.TextFormatted = GetFormattedText(description);

            if (bgLayout.MeasuredHeight <= 0)
            {
                bgLayout.Measure(0, 0);
            }

            float diff = ((float)(DPUtils.GetHeight() - bgLayout.MeasuredHeight) / (float)DPUtils.GetHeight());

            if (isLastItem)
            {
                if (diff < 0.26f)
                {
                    int totalHeight = (int)(DPUtils.GetHeight() * 0.74f);
                    if (titleView.MeasuredHeight <= 0)
                    {
                        titleView.Measure(0, 0);
                    }
                }
            }
            else
            {
                if (diff < 0.20f)
                {
                    int totalHeight = (int)(DPUtils.GetHeight() * 0.8f);
                    if (titleView.MeasuredHeight <= 0)
                    {
                        titleView.Measure(0, 0);
                    }
                }
            }
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