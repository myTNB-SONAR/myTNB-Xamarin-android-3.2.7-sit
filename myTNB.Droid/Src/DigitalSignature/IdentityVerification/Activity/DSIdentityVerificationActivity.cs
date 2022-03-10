using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.DigitalSignature.IdentityVerification.Fragment;
using myTNB_Android.Src.DigitalSignature.IdentityVerification.MVP;
using myTNB_Android.Src.DigitalSignature.WebView.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.DigitalSignature.IdentityVerification.Activity
{
    [Activity(Label = "DS Identity Verification", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme.Dashboard")]
    public class DSIdentityVerificationActivity : BaseActivityCustom, DSIdentityVerificationContract.IView
    {
        [BindView(Resource.Id.identityVerificationTitle)]
        readonly TextView identityVerificationTitle;

        [BindView(Resource.Id.identityVerificationBtnContinue)]
        readonly Button identityVerificationBtnContinue;

        [BindView(Resource.Id.identityVerificationListContainer)]
        readonly LinearLayout identityVerificationListContainer;

        private const string PAGE_ID = "DSIdentityVerification";
        private const int _totalItem = 3;

        private DSIdentityVerificationContract.IUserActionsListener userActionsListener;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _ = new DSIdentityVerificationPresenter(this);
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
            return Resource.Layout.DSIdentityVerificationView;
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

            SetToolBarTitle(Utility.GetLocalizedLabel(LanguageConstants.DS_LANDING, LanguageConstants.DSLanding.TITLE));
            SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            SetToolbarBackground(Resource.Drawable.CustomDashboardGradientToolbar);

            if (identityVerificationTitle != null)
            {
                TextViewUtils.SetMuseoSans500Typeface(identityVerificationTitle);
                TextViewUtils.SetTextSize16(identityVerificationTitle);
                identityVerificationTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.DS_LANDING, LanguageConstants.DSLanding.SUB_HEADER);
            }

            if (identityVerificationBtnContinue != null)
            {
                TextViewUtils.SetMuseoSans500Typeface(identityVerificationBtnContinue);
                TextViewUtils.SetTextSize16(identityVerificationBtnContinue);
                identityVerificationBtnContinue.Text = Utility.GetLocalizedLabel(LanguageConstants.DS_LANDING, LanguageConstants.DSLanding.CONTINUE);
            }
        }

        public void SetPresenter(DSIdentityVerificationContract.IUserActionsListener userActionListener)
        {
            this.userActionsListener = userActionListener;
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        public override void OnBackPressed()
        {
            SetResult(Result.Canceled);
            Finish();
        }

        [OnClick(Resource.Id.identityVerificationBtnContinue)]
        void ContinueOnClick(object sender, EventArgs eventArgs)
        {
            Intent nbrDiscoverMoreIntent = new Intent(this, typeof(DSWebViewActivity));
            StartActivity(nbrDiscoverMoreIntent);
        }

        public void RenderContent()
        {
            RunOnUiThread(() =>
            {
                try
                {
                    int ctr = 0;
                    for (int j = 0; j < _totalItem; j++)
                    {
                        ctr++;
                        string title, desc;
                        int resIcon;
                        DSIdentityVerificationListItemComponent itemListComponent = new DSIdentityVerificationListItemComponent(this);
                        switch (ctr)
                        {
                            case 1:
                                title = Utility.GetLocalizedLabel(LanguageConstants.DS_LANDING, LanguageConstants.DSLanding.ITEM_LIST_TITLE_1);
                                desc = Utility.GetLocalizedLabel(LanguageConstants.DS_LANDING, LanguageConstants.DSLanding.ITEM_LIST_DESC_1);
                                resIcon = Resource.Drawable.Icon_Identity_Verification_1;
                                break;
                            case 2:
                                title = Utility.GetLocalizedLabel(LanguageConstants.DS_LANDING, LanguageConstants.DSLanding.ITEM_LIST_TITLE_2);
                                desc = Utility.GetLocalizedLabel(LanguageConstants.DS_LANDING, LanguageConstants.DSLanding.ITEM_LIST_DESC_2);
                                resIcon = Resource.Drawable.Icon_Identity_Verification_2;
                                break;
                            case 3:
                                title = Utility.GetLocalizedLabel(LanguageConstants.DS_LANDING, LanguageConstants.DSLanding.ITEM_LIST_TITLE_3);
                                desc = Utility.GetLocalizedLabel(LanguageConstants.DS_LANDING, LanguageConstants.DSLanding.ITEM_LIST_DESC_3);
                                resIcon = Resource.Drawable.Icon_Identity_Verification_3;
                                break;
                            default:
                                title = string.Empty;
                                desc = string.Empty;
                                resIcon = Resource.Drawable.Icon_Identity_Verification_1;
                                break;
                        }

                        itemListComponent.SetItemTitleText(title);
                        itemListComponent.SetItemDescText(desc);
                        itemListComponent.SetItemIcon(resIcon);
                        identityVerificationListContainer.AddView(itemListComponent);
                    }
                }
                catch (Exception e)
                {
                    identityVerificationListContainer.Visibility = ViewStates.Gone;
                    Utility.LoggingNonFatalError(e);
                }
            });
        }
    }
}
