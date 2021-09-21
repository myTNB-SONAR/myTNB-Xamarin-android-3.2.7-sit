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

        private const string PAGE_ID = "NBRDiscoverMore";
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
            this.userActionsListener.OnStart();
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
            }

            SetToolBarTitle("TNB New Bill Design");
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

        public void RenderContent(NBRDiscoverMoreModel model)
        {
            RunOnUiThread(() =>
            {
                nbrDiscoverMoreTitle.Text = model.Title;
                nbrDiscoverMoreDesc.Text = model.Description;
                nbrDiscoverMoreFooterMsg.Text = model.FooterMessage;

                nbrDiscoverMoreFooterMsg.TextFormatted = GetFormattedText(model.FooterMessage);
                nbrDiscoverMoreFooterMsg = LinkRedirectionUtils
                        .Create(this, Title)
                        .SetTextView(nbrDiscoverMoreFooterMsg)
                        .SetMessage(model.FooterMessage)
                        .Build()
                        .GetProcessedTextView();

                if (model.DiscoverMoreItemList != null)
                {
                    for (int j = 0; j < model.DiscoverMoreItemList.Count; j++)
                    {
                        NBRDiscoverMoreModel.DiscoverMoreItem discoverMoreItem = model.DiscoverMoreItemList[j];

                        NBRDiscoverMoreListItemComponent itemListComponent = new NBRDiscoverMoreListItemComponent(this);
                        itemListComponent.SetItemNumber("" + (j + 1));
                        itemListComponent.SetItemTitle(discoverMoreItem.Title);
                        itemListComponent.SetItemContent(discoverMoreItem.Content);
                        itemListComponent.SetBannerImage(discoverMoreItem.Banner);

                        nbrDiscoverMoreViewList?.AddView(itemListComponent);
                    }
                }
                UpdateView(false);
            });
        }
    }
}