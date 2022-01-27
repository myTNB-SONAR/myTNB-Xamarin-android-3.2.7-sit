using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Bills.NewBillRedesign.MVP;
using myTNB_Android.Src.Bills.NewBillRedesign.Fragment;
using myTNB_Android.Src.Bills.NewBillRedesign.Model;
using Android.Graphics;
using System;
using System.Threading.Tasks;
using System.Threading;
using Android.Content;
using myTNB_Android.Src.Bills.NewBillRedesign.Activity;
using myTNB.Mobile;

namespace myTNB_Android.Src.Bills.NewBillRedesign
{
    [Activity(Label = "TNB New Bill Design", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class NBRDiscoverMoreActivity : BaseActivityCustom, NBRDiscoverMoreContract.IView
    {
        [BindView(Resource.Id.nbrDiscoverMoreScrollView)]
        readonly ScrollView nbrDiscoverMoreScrollView;

        [BindView(Resource.Id.nbrDiscoverMoreShimmerContainer)]
        readonly LinearLayout nbrDiscoverMoreShimmerContainer;

        [BindView(Resource.Id.nbrDiscoverMoreTitle)]
        readonly TextView nbrDiscoverMoreTitle;

        [BindView(Resource.Id.nbrDiscoverMoreDesc)]
        readonly TextView nbrDiscoverMoreDesc;

        [BindView(Resource.Id.nbrDiscoverMoreViewList)]
        readonly LinearLayout nbrDiscoverMoreViewList;

        [BindView(Resource.Id.nbrDiscoverMoreFooterMsg)]
        TextView nbrDiscoverMoreFooterMsg;

        [BindView(Resource.Id.btnGoToBills)]
        readonly Button nbrDiscoverMoreBtn;

        [BindView(Resource.Id.headerBannerShimmerContainer)]
        LinearLayout headerBannerShimmerContainer;

        [BindView(Resource.Id.headerBannerContainer)]
        LinearLayout headerBannerContainer;

        [BindView(Resource.Id.nbrDiscoverMoreBannerLeft)]
        ImageView nbrDiscoverMoreBannerLeft;

        [BindView(Resource.Id.nbrDiscoverMoreBannerRight)]
        ImageView nbrDiscoverMoreBannerRight;

        private const string PAGE_ID = "NBRDiscoverMore";
        private const string PLACEHOLDER_IMG = "Banner_NBR_Placeholder_{0}";
        private const string ITEM_NO = "{0}";
        private string leftImagePath, rightImagePath;
        private NBRDiscoverMoreContract.IUserActionsListener userActionsListener;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _ = new NBRDiscoverMorePresenter(this);
            this.userActionsListener?.OnInitialize();
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override string GetPageId()
        {
            return PAGE_ID;
        }

        public override int ResourceId()
        {
            return Resource.Layout.NBRDiscoverMoreView;
        }

        public override bool ShowCustomToolbarTitle()
        {
            return true;
        }

        public void SetUpViews()
        {
            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);

            if (nbrDiscoverMoreTitle != null)
            {
                TextViewUtils.SetMuseoSans500Typeface(nbrDiscoverMoreTitle);
                TextViewUtils.SetTextSize16(nbrDiscoverMoreTitle);
            }

            if (nbrDiscoverMoreDesc != null)
            {
                TextViewUtils.SetMuseoSans300Typeface(nbrDiscoverMoreDesc);
                TextViewUtils.SetTextSize12(nbrDiscoverMoreDesc);
            }

            if (nbrDiscoverMoreFooterMsg != null)
            {
                TextViewUtils.SetMuseoSans300Typeface(nbrDiscoverMoreFooterMsg);
                TextViewUtils.SetTextSize12(nbrDiscoverMoreFooterMsg);
            }

            if (nbrDiscoverMoreBtn != null)
            {
                TextViewUtils.SetMuseoSans500Typeface(nbrDiscoverMoreBtn);
                TextViewUtils.SetTextSize16(nbrDiscoverMoreBtn);
                nbrDiscoverMoreBtn.Text = Utility.GetLocalizedLabel(LanguageConstants.NBR_COMMS, LanguageConstants.NBRComms.NBR_BTN_TITLE);
            }

            SetToolBarTitle(Utility.GetLocalizedLabel(LanguageConstants.NBR_COMMS, LanguageConstants.NBRComms.NBR_TITLE));
            SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);
        }

        public void SetPresenter(NBRDiscoverMoreContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        public void UpdateView(bool isLoading)
        {
            nbrDiscoverMoreShimmerContainer.Visibility = isLoading ? ViewStates.Visible : ViewStates.Gone;
            nbrDiscoverMoreScrollView.Visibility = isLoading ? ViewStates.Gone : ViewStates.Visible;
        }

        [OnClick(Resource.Id.nbrDiscoverMoreBannerLeft)]
        void LeftBannerOnClick(object sender, EventArgs eventArgs)
        {
            if (leftImagePath.IsValid())
            {
                Intent nbrDiscoverMoreIntent = new Intent(this, typeof(NBRDiscoverMoreBannerFullViewActivity));
                nbrDiscoverMoreIntent.PutExtra("IMAGE_PATH", leftImagePath);
                StartActivity(nbrDiscoverMoreIntent);
            }
        }

        [OnClick(Resource.Id.nbrDiscoverMoreBannerRight)]
        void RightBannerOnClick(object sender, EventArgs eventArgs)
        {
            if (rightImagePath.IsValid())
            {
                Intent nbrDiscoverMoreIntent = new Intent(this, typeof(NBRDiscoverMoreBannerFullViewActivity));
                nbrDiscoverMoreIntent.PutExtra("IMAGE_PATH", rightImagePath);
                StartActivity(nbrDiscoverMoreIntent);
            }
        }

        [OnClick(Resource.Id.btnGoToBills)]
        void GoToBillsOnClick(object sender, EventArgs eventArgs)
        {
            DynatraceHelper.OnTrack(DynatraceConstants.BR.CTAs.BillRedesignComms.View_Bill);
            SetResult(Result.Ok);
            Finish();
        }

        public override void OnBackPressed()
        {
            SetResult(Result.Canceled);
            Finish();
        }

        public void RenderContent(NBRDiscoverMoreModel model)
        {
            RunOnUiThread(() =>
            {
                try
                {
                    _ = GetImageAsync(model);
                    nbrDiscoverMoreTitle.Text = model.Title;
                    nbrDiscoverMoreDesc.Text = model.Description;
                    nbrDiscoverMoreFooterMsg.Text = model.FooterMessage;

                    nbrDiscoverMoreFooterMsg.TextFormatted = GetFormattedText(model.FooterMessage);
                    nbrDiscoverMoreFooterMsg = LinkRedirectionUtils
                            .Create(this, Utility.GetLocalizedLabel(LanguageConstants.NBR_COMMS, LanguageConstants.NBRComms.NBR_TITLE))
                            .SetTextView(nbrDiscoverMoreFooterMsg)
                            .SetMessage(model.FooterMessage)
                            .Build(model.ShouldTrackFooter ? model.DynatraceTagFooter : string.Empty)
                            .GetProcessedTextView();

                    if (model.DiscoverMoreItemList != null && model.DiscoverMoreItemList.Count > 0)
                    {
                        nbrDiscoverMoreViewList.Visibility = ViewStates.Visible;
                        for (int j = 0; j < model.DiscoverMoreItemList.Count; j++)
                        {
                            NBRDiscoverMoreModel.DiscoverMoreItem discoverMoreItem = model.DiscoverMoreItemList[j];

                            NBRDiscoverMoreListItemComponent itemListComponent = new NBRDiscoverMoreListItemComponent(this);
                            itemListComponent.SetBannerPlaceholder(string.Format(PLACEHOLDER_IMG, j));
                            itemListComponent.SetItemNumber(string.Format(ITEM_NO, j + 1));
                            itemListComponent.SetItemTitle(discoverMoreItem.Title);
                            itemListComponent.SetItemContent(discoverMoreItem.Content);
                            itemListComponent.itemContentText = LinkRedirectionUtils
                                .Create(this, Utility.GetLocalizedLabel(LanguageConstants.NBR_COMMS, LanguageConstants.NBRComms.NBR_TITLE))
                                .SetTextView(itemListComponent.itemContentText)
                                .SetMessage(discoverMoreItem.Content)
                                .Build(discoverMoreItem.ShouldTrack ? discoverMoreItem.DynatraceTag : string.Empty)
                                .GetProcessedTextView();
                            itemListComponent.SetBannerImage(discoverMoreItem.Banner);

                            nbrDiscoverMoreViewList?.AddView(itemListComponent);
                        }
                    }
                    UpdateView(false);
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            });
        }

        public async Task GetImageAsync(NBRDiscoverMoreModel model)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Bitmap leftImageBitmap = null;
            Bitmap rightImageBitmap = null;
            try
            {
                float deviceWidth = this.Resources.DisplayMetrics.WidthPixels - DPUtils.ConvertDPToPx(36f);

                await Task.Run(() =>
                {
                    leftImageBitmap = ImageUtils.GetImageBitmapFromUrl(model.Banner1);
                }, cts.Token);

                await Task.Run(() =>
                {
                    rightImageBitmap = ImageUtils.GetImageBitmapFromUrl(model.Banner2);
                }, cts.Token);

                headerBannerShimmerContainer.Visibility = ViewStates.Gone;
                headerBannerContainer.Visibility = ViewStates.Visible;

                leftImagePath = model.IsZoomable ? model.Banner1 : string.Empty;
                rightImagePath = model.IsZoomable ? model.Banner2 : string.Empty;

                if (leftImageBitmap != null)
                {
                    nbrDiscoverMoreBannerLeft.SetImageBitmap(Bitmap.CreateScaledBitmap(leftImageBitmap, (int)(deviceWidth / 2), (int)((deviceWidth / 2) * 1.4), false));
                }
                else
                {
                    nbrDiscoverMoreBannerLeft.SetImageResource(Resource.Drawable.Banner_New_Bill_1);
                }
                if (rightImageBitmap != null)
                {
                    nbrDiscoverMoreBannerRight.SetImageBitmap(Bitmap.CreateScaledBitmap(rightImageBitmap, (int)(deviceWidth / 2), (int)((deviceWidth / 2) * 1.4), false));
                }
                else
                {
                    nbrDiscoverMoreBannerRight.SetImageResource(Resource.Drawable.Banner_New_Bill_2);
                }
            }
            catch (Exception e)
            {
                nbrDiscoverMoreBannerLeft.SetImageResource(Resource.Drawable.Banner_New_Bill_1);
                nbrDiscoverMoreBannerRight.SetImageResource(Resource.Drawable.Banner_New_Bill_2);
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}