using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using CheeseBind;
using myTNB;
using myTNB.Mobile;
using myTNB.Mobile.Constants.DS;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.DigitalSignature.IdentityVerification.Fragment;
using myTNB_Android.Src.DigitalSignature.IdentityVerification.MVP;
using myTNB_Android.Src.DigitalSignature.WebView.Activity;
using myTNB_Android.Src.Utils;
using myTNB_Android.Src.Utils.Deeplink;
using Newtonsoft.Json;

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

        [BindView(Resource.Id.dsIdVerifShimmerContainer)]
        readonly LinearLayout dsIdVerifShimmerContainer;

        [BindView(Resource.Id.dsIdVerifButtonShimmerContainer)]
        readonly LinearLayout dsIdVerifButtonShimmerContainer;

        [BindView(Resource.Id.dsIdVerifBottomInfoContainer)]
        readonly LinearLayout dsIdVerifBottomInfoContainer;

        [BindView(Resource.Id.dsIdVerifInfoDropdownContentLayout)]
        readonly LinearLayout dsIdVerifInfoDropdownContentLayout;

        [BindView(Resource.Id.identityVerificationButtonLayout)]
        readonly LinearLayout identityVerificationButtonLayout;

        [BindView(Resource.Id.dsIdVerifInfoTitle)]
        readonly TextView dsIdVerifInfoTitle;

        [BindView(Resource.Id.dsIdVerifInfoMessage)]
        readonly TextView dsIdVerifInfoMessage;

        [BindView(Resource.Id.dsIdVerifInfoDropdownTitle)]
        readonly TextView dsIdVerifInfoDropdownTitle;

        [BindView(Resource.Id.dsIdVerifInfoDropdownMessage)]
        readonly TextView dsIdVerifInfoDropdownMessage;

        private const string PAGE_ID = DSConstants.PageName_DSLanding;
        private const int _totalItem = 3;
        private Action buttonCTA;

        DSDynamicLinkParamsModel _dsDynamicLinkParamsModel;

        private DSIdentityVerificationContract.IUserActionsListener userActionsListener;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                Bundle extras = Intent.Extras;

                if ((extras != null) && extras.ContainsKey(DigitalSignatureConstants.DS_DYNAMIC_LINK_PARAMS_MODEL))
                {
                    _dsDynamicLinkParamsModel = JsonConvert.DeserializeObject<DSDynamicLinkParamsModel>(extras.GetString(DigitalSignatureConstants.DS_DYNAMIC_LINK_PARAMS_MODEL));
                }

                _ = new DSIdentityVerificationPresenter(this, this);
                this.userActionsListener?.OnInitialize();

                DynatraceHelper.OnTrack(DynatraceConstants.DS.Screens.Verification.Verify_How_It_Works);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
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

            SetToolBarTitle(GetLabelByLanguage(DSConstants.I18N_Title));
            SetStatusBarBackground(Resource.Drawable.UsageGradientBackground);
            SetToolbarBackground(Resource.Drawable.CustomGradientToolBar);

            if (identityVerificationTitle != null)
            {
                TextViewUtils.SetMuseoSans500Typeface(identityVerificationTitle);
                TextViewUtils.SetTextSize16(identityVerificationTitle);
                identityVerificationTitle.Text = GetLabelByLanguage(DSConstants.I18N_SubHeader);
            }

            if (identityVerificationBtnContinue != null)
            {
                TextViewUtils.SetMuseoSans500Typeface(identityVerificationBtnContinue);
                TextViewUtils.SetTextSize16(identityVerificationBtnContinue);
                identityVerificationBtnContinue.Text = GetLabelByLanguage(DSConstants.I18N_Continue);
            }

            TextViewUtils.SetTextSize12(dsIdVerifInfoDropdownTitle, dsIdVerifInfoDropdownMessage);
            TextViewUtils.SetTextSize16(dsIdVerifInfoTitle);
            TextViewUtils.SetTextSize14(dsIdVerifInfoMessage);
            TextViewUtils.SetMuseoSans300Typeface(dsIdVerifInfoMessage, dsIdVerifInfoDropdownTitle, dsIdVerifInfoDropdownMessage);
            TextViewUtils.SetMuseoSans500Typeface(dsIdVerifInfoTitle);
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

            DynatraceHelper.OnTrack(DynatraceConstants.DS.CTAs.Verification.Back_From_Verify);
        }

        [OnClick(Resource.Id.identityVerificationBtnContinue)]
        void ContinueOnClick(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                buttonCTA?.Invoke();
                this.SetIsClicked(false);
            }

            DynatraceHelper.OnTrack(DynatraceConstants.DS.CTAs.Verification.Verify_Now);
        }

        private void ProceedOnVerifyNow(DSDynamicLinkParamsModel dsDynamicLinkParamsModel)
        {
            UpdateLoadingShimmer(true);
            this.userActionsListener.GetEKYCStatusOnCall(dsDynamicLinkParamsModel);
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
                        DSIdentityVerificationListItemComponent itemListComponent = new DSIdentityVerificationListItemComponent(this);

                        title = GetLabelByLanguage(string.Format(DSConstants.I18N_HowItWorksTitle, ctr));
                        desc = GetLabelByLanguage(string.Format(DSConstants.I18N_HowItWorksDescription, ctr));

                        var resIcon = ctr switch
                        {
                            1 => Resource.Drawable.Icon_Identity_Verification_1,
                            2 => Resource.Drawable.Icon_Identity_Verification_2,
                            3 => Resource.Drawable.Icon_Identity_Verification_3,
                            _ => Resource.Drawable.Icon_Identity_Verification_1,
                        };
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

        private void BackToHome()
        {
            SetResult(Result.Canceled);
            Finish();
        }

        private void ShowUnMatchUserIdPopUp()
        {
            RunOnUiThread(() =>
            {
                MyTNBAppToolTipBuilder marketingTooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.MYTNB_DIALOG_ICON_ONE_BUTTON)
                   .SetHeaderImage(Resource.Drawable.ic_display_validation_success)
                   .SetTitle(GetLabelByLanguage(DSConstants.I18N_UserIDNotMatchTitle))
                   .SetMessage(GetLabelByLanguage(DSConstants.I18N_UserIDNotMatchMessage))
                   .SetCTALabel(GetLabelCommonByLanguage(LanguageConstants.Common.GOT_IT))
                   .SetCTAaction(() => { })
                   .Build();
                marketingTooltip.Show();
            });
        }

        public void ShowCompletedOnOtherDevicePopUp()
        {
            RunOnUiThread(() =>
            {
                dsIdVerifInfoDropdownContentLayout.Visibility = ViewStates.Gone;
                UpdateButtonState(true);

                dsIdVerifInfoTitle.Text = GetLabelByLanguage(DSConstants.I18N_IDProcessingTitle);
                identityVerificationBtnContinue.Text = GetLabelByLanguage(DSConstants.I18N_BackToHome);

                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    dsIdVerifInfoMessage.TextFormatted = Html.FromHtml(GetLabelByLanguage(DSConstants.I18N_IDProcessingMessage), FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    dsIdVerifInfoMessage.TextFormatted = Html.FromHtml(GetLabelByLanguage(DSConstants.I18N_IDProcessingMessage));
                }

                buttonCTA = BackToHome;
            });
        }

        public void ShowIdNotRegisteredPopUp()
        {
            RunOnUiThread(() =>
            {
                dsIdVerifInfoDropdownContentLayout.Visibility = ViewStates.Gone;
                UpdateButtonState(true);

                dsIdVerifInfoTitle.Text = GetLabelByLanguage(DSConstants.I18N_NoRegisteredIDTitle);
                identityVerificationBtnContinue.Text = GetLabelByLanguage(DSConstants.I18N_VerifyNow);

                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    dsIdVerifInfoMessage.TextFormatted = Html.FromHtml(GetLabelByLanguage(DSConstants.I18N_NoRegisteredIDMessage), FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    dsIdVerifInfoMessage.TextFormatted = Html.FromHtml(GetLabelByLanguage(DSConstants.I18N_NoRegisteredIDMessage));
                }

                buttonCTA = OnVerifyNow;

                DynatraceHelper.OnTrack(DynatraceConstants.DS.Screens.Popup.Document_Ready);
            });
        }

        public void ShowIdentityHasBeenVerified()
        {
            RunOnUiThread(() =>
            {
                dsIdVerifInfoDropdownContentLayout.Visibility = ViewStates.Gone;
                UpdateButtonState(true);

                dsIdVerifInfoTitle.Text = GetLabelByLanguage(DSConstants.I18N_IDVerifiedTitle);
                identityVerificationBtnContinue.Text = GetLabelByLanguage(DSConstants.I18N_BackToHome);

                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    dsIdVerifInfoMessage.TextFormatted = Html.FromHtml(GetLabelByLanguage(DSConstants.I18N_IDVerifiedMessage), FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    dsIdVerifInfoMessage.TextFormatted = Html.FromHtml(GetLabelByLanguage(DSConstants.I18N_IDVerifiedMessage));
                }

                buttonCTA = BackToHome;
            });
        }

        public void ShowPrepareDocumentPopUp(int? idType)
        {
            RunOnUiThread(() =>
            {
                if (idType == null)
                {
                    return;
                }

                try
                {
                    string idKeyString = string.Empty;
                    string idTypeString = string.Empty;
                    var selectorContent = LanguageManager.Instance.GetSelectorsByPage<DSIdTypeSelectorModel>(DigitalSignatureConstants.DS_LANDING_SELECTOR);
                    if (selectorContent.ContainsKey(DigitalSignatureConstants.DS_ID_TYPE))
                    {
                        List<DSIdTypeSelectorModel> idTypeList = new List<DSIdTypeSelectorModel>();
                        idTypeList = selectorContent[DigitalSignatureConstants.DS_ID_TYPE];
                        if (idTypeList.Count > 0)
                        {
                            idKeyString = idTypeList.Find(x => { return x.key == idType.ToString(); }).key;
                            idTypeString = idTypeList.Find(x => { return x.key == idType.ToString(); }).description;

                            var dialogTitle = string.Empty;
                            var dialogMessage = string.Empty;
                            var dropdownTitle = string.Empty;
                            var dropdownMessage = string.Empty;

                            string lastFourDigits = string.Empty;
                            var idNumber = this.userActionsListener.GetDSDynamicLinkParamsModel().IdentificationNo;

                            if (idNumber.IsValid())
                            {
                                lastFourDigits = idNumber[^4..];
                            }
                            
                            switch (idKeyString)
                            {
                                case "1":
                                    dialogTitle = GetLabelByLanguage(DSConstants.I18N_AcceptedIDTitle_IC);
                                    dialogMessage = string.Format(GetLabelByLanguage(DSConstants.I18N_AcceptedIDMessage_IC), idTypeString, lastFourDigits);
                                    dropdownTitle = GetLabelByLanguage(DSConstants.I18N_CantUseOtherIDTitle_IC);
                                    dropdownMessage = string.Format(GetLabelByLanguage(DSConstants.I18N_CantUseOtherIDMessage_IC), idTypeString);
                                    break;
                                case "2":
                                    dialogTitle = GetLabelByLanguage(DSConstants.I18N_AcceptedIDTitle_Passport);
                                    dialogMessage = string.Format(GetLabelByLanguage(DSConstants.I18N_AcceptedIDMessage_Passport), idTypeString, lastFourDigits);
                                    dropdownTitle = GetLabelByLanguage(DSConstants.I18N_CantUseOtherIDTitle_Passport);
                                    dropdownMessage = string.Format(GetLabelByLanguage(DSConstants.I18N_CantUseOtherIDMessage_Passport), idTypeString);
                                    break;
                                case "4":
                                    dialogTitle = GetLabelByLanguage(DSConstants.I18N_AcceptedIDTitle_MyTentera);
                                    dialogMessage = string.Format(GetLabelByLanguage(DSConstants.I18N_AcceptedIDMessage_MyTentera), idTypeString, lastFourDigits);
                                    dropdownTitle = GetLabelByLanguage(DSConstants.I18N_CantUseOtherIDTitle_MyTentera);
                                    dropdownMessage = string.Format(GetLabelByLanguage(DSConstants.I18N_CantUseOtherIDMessage_MyTentera), idTypeString);
                                    break;
                                default:
                                    break;
                            }

                            dsIdVerifInfoDropdownContentLayout.Visibility = ViewStates.Visible;
                            UpdateButtonState(true);

                            dsIdVerifInfoTitle.Text = dialogTitle;
                            identityVerificationBtnContinue.Text = GetLabelByLanguage(DSConstants.I18N_VerifyNow);

                            try
                            {
                                string dropdownTitleBase = Regex.Replace(dropdownTitle, DSConstants.DropDown, string.Empty);

                                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                                {
                                    dsIdVerifInfoMessage.TextFormatted = Html.FromHtml(dialogMessage, FromHtmlOptions.ModeLegacy);
                                    dsIdVerifInfoDropdownTitle.TextFormatted = Html.FromHtml(dropdownTitle, FromHtmlOptions.ModeLegacy);
                                    dsIdVerifInfoDropdownMessage.TextFormatted = Html.FromHtml(dropdownMessage, FromHtmlOptions.ModeLegacy);
                                }
                                else
                                {
                                    dsIdVerifInfoMessage.TextFormatted = Html.FromHtml(dialogMessage);
                                    dsIdVerifInfoDropdownTitle.TextFormatted = Html.FromHtml(dropdownTitle);
                                    dsIdVerifInfoDropdownMessage.TextFormatted = Html.FromHtml(dropdownMessage);
                                }

                                ImageSpan imageSpan = new ImageSpan(this, Resource.Drawable.Icon_DS_Dropdown_Expand, SpanAlign.Bottom);
                                SpannableString imageString = new SpannableString(dsIdVerifInfoDropdownTitle.TextFormatted);

                                imageString.SetSpan(imageSpan, dropdownTitleBase.Length, dropdownTitle.Length, SpanTypes.ExclusiveExclusive);
                                dsIdVerifInfoDropdownTitle.TextFormatted = imageString;

                                dsIdVerifInfoDropdownMessage.Visibility = ViewStates.Gone;

                                var expanded = false;
                                dsIdVerifInfoDropdownTitle.Click += delegate
                                {
                                    expanded = !expanded;
                                    dsIdVerifInfoDropdownMessage.Visibility = expanded ? ViewStates.Visible : ViewStates.Gone;

                                    ImageSpan imageSpan = new ImageSpan(this, expanded ? Resource.Drawable.Icon_DS_Dropdown_Collapse : Resource.Drawable.Icon_DS_Dropdown_Expand, SpanAlign.Bottom);
                                    SpannableString imageString = new SpannableString(dsIdVerifInfoDropdownTitle.TextFormatted);

                                    imageString.SetSpan(imageSpan, dropdownTitleBase.Length, dropdownTitle.Length, SpanTypes.ExclusiveExclusive);
                                    dsIdVerifInfoDropdownTitle.TextFormatted = imageString;
                                };
                            }
                            catch (Exception e)
                            {
                                Utility.LoggingNonFatalError(e);
                            }

                            buttonCTA = OnVerifyNow;

                            DynatraceHelper.OnTrack(DynatraceConstants.DS.Screens.Popup.Document_Ready);
                        }
                    }
                    else
                    {
                        UpdateBottomContainer(false);
                        UpdateButtonState(false);
                    }
                }
                catch (Exception e)
                {
                    Utility.LoggingNonFatalError(e);
                }
            });
        }

        public void ShowErrorMessage(StatusDetail statusDetail)
        {
            MyTNBAppToolTipBuilder errMsg = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(statusDetail.Title)
                .SetMessage(statusDetail.Message)
                .SetCTALabel(statusDetail.PrimaryCTATitle)
                .Build();
            errMsg.Show();
        }

        private void OnVerifyNow()
        {
            Intent intent = new Intent(this, typeof(DSWebViewActivity));
            intent.PutExtra(DigitalSignatureConstants.DS_DYNAMIC_LINK_PARAMS_MODEL, JsonConvert.SerializeObject(this.userActionsListener.GetDSDynamicLinkParamsModel()));
            StartActivity(intent);

            DynatraceHelper.OnTrack(DynatraceConstants.DS.CTAs.Verification.PopUp_Verify_Now);
        }

        public void UpdateLoadingShimmer(bool toShow)
        {
            dsIdVerifShimmerContainer.Visibility = toShow ? ViewStates.Visible : ViewStates.Gone;
            dsIdVerifButtonShimmerContainer.Visibility = toShow ? ViewStates.Visible : ViewStates.Gone;
        }

        public void UpdateBottomContainer(bool toShow)
        {
            dsIdVerifBottomInfoContainer.Visibility = toShow ? ViewStates.Visible : ViewStates.Gone;
        }

        public void UpdateButtonState(bool toShow)
        {
            identityVerificationButtonLayout.Visibility = toShow ? ViewStates.Visible : ViewStates.Gone;
        }

        public void VerifyMatchingID()
        {
            var modelUserID = DeeplinkUtil.Instance.EKYCDynamicLinkModel.UserID ?? string.Empty;
            if (DeeplinkUtil.Instance.TargetScreen == Deeplink.ScreenEnum.IdentityVerification &&
                modelUserID.IsValid())
            {
                string userID = UserEntity.GetActive().UserID.ToLower();
                if (userID.Equals(modelUserID.ToLower()))
                {
                    ProceedOnVerifyNow(DeeplinkUtil.Instance.EKYCDynamicLinkModel);
                }
                else
                {
                    ShowUnMatchUserIdPopUp();
                }
                DeeplinkUtil.Instance.ClearDeeplinkData();
            }
            else
            {
                ProceedOnVerifyNow(_dsDynamicLinkParamsModel);
            }
        }
    }
}
