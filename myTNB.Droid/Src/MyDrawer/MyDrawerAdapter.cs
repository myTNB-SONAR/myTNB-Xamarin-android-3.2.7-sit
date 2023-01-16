
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using Java.Security.Acl;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.MyDrawer;
using myTNB_Android.Src.MyHome.Model;
using myTNB_Android.Src.Utils;
using static myTNB_Android.Src.MyDrawer.MyDrawerAdapter;

using ServiceEnum = myTNB.Mobile.MobileEnums.ServiceEnum;

namespace myTNB_Android.Src.MyDrawer
{
    public class MyDrawerAdapter : RecyclerView.Adapter
    {
        List<MyDrawerModel> myDrawerList = new List<MyDrawerModel>();

        public event EventHandler<int> ClickChanged;

        private Android.App.Activity mActivity;

        public MyDrawerAdapter(List<MyDrawerModel> data, Android.App.Activity Activity)
        {
            if (data == null)
            {
                this.myDrawerList.Clear();
            }
            else
            {
                this.myDrawerList = data;
            }
            this.mActivity = Activity;
        }

        public override int ItemCount => myDrawerList.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                MyDrawerViewHolder vh = holder as MyDrawerViewHolder;

                MyDrawerModel model = myDrawerList[position];

                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    vh.myDrawerTitle.TextFormatted = Html.FromHtml(model.ServiceName, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    vh.myDrawerTitle.TextFormatted = Html.FromHtml(model.ServiceName);
                }

                ViewGroup.LayoutParams currentCard = vh.myDrawerCardView.LayoutParameters;
                ViewGroup.LayoutParams currentImg = vh.myDrawerImg.LayoutParameters;

                int cardWidth = (this.mActivity.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(14f);

                if (DPUtils.ConvertDPToPixel(cardWidth) > 91f && DPUtils.ConvertPxToDP(cardWidth) <= 120f)
                {
                    cardWidth = (this.mActivity.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(12f);
                }
                else if (DPUtils.ConvertPxToDP(cardWidth) <= 91f)
                {
                    cardWidth = (this.mActivity.Resources.DisplayMetrics.WidthPixels / 3) - (int)DPUtils.ConvertDPToPx(10f);
                }

                currentCard.Width = cardWidth;

                float imgHeightRatio = 48f / 107f;
                int imgHeight = (int)(cardWidth * (imgHeightRatio));
                if (DPUtils.ConvertPxToDP(imgHeight) < 48f)
                {
                    imgHeight = (int)DPUtils.ConvertDPToPx(48f);
                }
                currentImg.Height = imgHeight;
                currentImg.Width = imgHeight;

                switch(model.ServiceType)
                {
                    case ServiceEnum.CONNECTMYPREMISE:
                        DynamicIconHandling(vh, model, Resource.Drawable.Icon_Connect_My_Premise);
                        if (UserSessions.ConnectMyPremiseHasShown(PreferenceManager.GetDefaultSharedPreferences(this.mActivity)))
                        {
                            vh.newLabel.Visibility = ViewStates.Gone;
                        }
                        break;
                }

                RelativeLayout.LayoutParams currentNewLabel = vh.newLabel.LayoutParameters as RelativeLayout.LayoutParams;
                currentNewLabel.LeftMargin = imgHeight;
                currentNewLabel.Width = ViewGroup.LayoutParams.WrapContent;
                vh.newLabel.SetPadding((int)DPUtils.ConvertDPToPx(4f), 0, (int)DPUtils.ConvertDPToPx(4f), 0);

                TextViewUtils.SetMuseoSans500Typeface(vh.myDrawerTitle, vh.txtNewLabel);
                vh.txtNewLabel.Text = Utility.GetLocalizedCommonLabel("new");
                TextViewUtils.SetTextSize8(vh.txtNewLabel);
                TextViewUtils.SetTextSize10(vh.myDrawerTitle);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void DynamicIconHandling(MyDrawerViewHolder vh, MyServiceModel model, int fallbackImgRes, bool isDisabled = false)
        {
            try
            {
                vh.myDrawerImg.SetImageResource(fallbackImgRes);

                if (model != null)
                {
                    string timestamp = UserSessions.GetServicesTimeStamp(PreferenceManager.GetDefaultSharedPreferences(this.mActivity));
                    MyServiceIconEntity iconEntity = new MyServiceIconEntity()
                    {
                        ServiceId = model.ServiceId,
                        ServiceIconUrl = model.ServiceIconUrl,
                        DisabledServiceIconUrl = model.DisabledServiceIconUrl,
                        ServiceBannerUrl = model.ServiceBannerUrl,
                        TimeStamp = timestamp
                    };

                    MyServiceIconEntity iconManager = new MyServiceIconEntity();
                    MyServiceIconEntity myServiceIconEntity = iconManager.GetMyServiceItem(model.ServiceId);

                    if (myServiceIconEntity != null)
                    {
                        string serviceIconURL = myServiceIconEntity.ServiceIconUrl;
                        string serviceIconB64 = myServiceIconEntity.ServiceIconB64;
                        string disableServiceIconURL = myServiceIconEntity.DisabledServiceIconUrl;
                        string disableServiceIconB64 = myServiceIconEntity.DisabledServiceIconB64;

                        if (timestamp != myServiceIconEntity.TimeStamp)
                        {
                            Task.Run(() =>
                            {
                                var bitmapImage = ImageUtils.GetImageBitmapFromUrlWithTimeOut(isDisabled ? disableServiceIconURL : serviceIconURL);
                                if (bitmapImage != null)
                                {
                                    vh.myDrawerImg.SetImageBitmap(bitmapImage);
                                }
                                else
                                {
                                    Bitmap convertedImage = ImageUtils.Base64ToBitmap(isDisabled ? disableServiceIconB64 : serviceIconB64);
                                    if (convertedImage != null)
                                    {
                                        vh.myDrawerImg.SetImageBitmap(convertedImage);
                                    }
                                    else
                                    {
                                        vh.myDrawerImg.SetImageResource(fallbackImgRes);
                                    }
                                }
                            });
                        }
                        else
                        {
                            Bitmap convertedImage = ImageUtils.Base64ToBitmap(isDisabled ? disableServiceIconB64 : serviceIconB64);
                            if (convertedImage != null)
                            {
                                vh.myDrawerImg.SetImageBitmap(convertedImage);
                            }
                            else
                            {
                                vh.myDrawerImg.SetImageResource(fallbackImgRes);
                            }
                        }
                    }
                    else
                    {
                        Task.Run(() =>
                        {
                            var bitmapImage = ImageUtils.GetImageBitmapFromUrlWithTimeOut(isDisabled ? model.DisabledServiceIconUrl : model.ServiceIconUrl);
                            if (bitmapImage != null)
                            {
                                vh.myDrawerImg.SetImageBitmap(bitmapImage);
                                string base64String = ImageUtils.GetBase64FromBitmapPNG(bitmapImage, 100);
                                iconEntity.ServiceIconB64 = base64String;

                                iconManager.InsertItem(iconEntity);
                            }
                            else
                            {
                                vh.myDrawerImg.SetImageResource(fallbackImgRes);
                            }
                        });
                    }
                }
                else
                {
                    vh.myDrawerImg.SetImageResource(fallbackImgRes);
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
                vh.myDrawerImg.SetImageResource(fallbackImgRes);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.MyDrawerItemView;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new MyDrawerViewHolder(itemView, OnClick);
        }

        void OnClick(MyDrawerViewHolder sender, int position)
        {
            if (ClickChanged != null)
            {
                if (sender != null)
                {
                    sender.newLabel.Visibility = ViewStates.Gone;
                }
                ClickChanged(this, position);
            }
        }

        public class MyDrawerViewHolder : RecyclerView.ViewHolder
        {
            public ImageView myDrawerImg { get; private set; }

            public TextView myDrawerTitle { get; private set; }

            public LinearLayout myDrawerCardView { get; private set; }

            public LinearLayout newLabel { get; private set; }

            public TextView txtNewLabel { get; private set; }

            public MyDrawerViewHolder(View itemView, Action<MyDrawerViewHolder, int> listener) : base(itemView)
            {
                myDrawerImg = itemView.FindViewById<ImageView>(Resource.Id.myDrawerImg);
                myDrawerTitle = itemView.FindViewById<TextView>(Resource.Id.myDrawerTitle);

                myDrawerCardView = itemView.FindViewById<LinearLayout>(Resource.Id.rootView);
                newLabel = itemView.FindViewById<LinearLayout>(Resource.Id.newLabel);
                txtNewLabel = itemView.FindViewById<TextView>(Resource.Id.txtNewLabel);
                TextViewUtils.SetTextSize12(myDrawerTitle);
                myDrawerCardView.Click += (s, e) => listener((this), base.LayoutPosition);
            }
        }
    }
}

