using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
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

        private const string PAGE_ID = DSConstants.PageName_DSLanding;
        private const int _totalItem = 3;

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
                this.SetIsClicked(false);
            }

            DynatraceHelper.OnTrack(DynatraceConstants.DS.CTAs.Verification.Verify_Now);
        }

        private void ProceedOnVerifyNow(DSDynamicLinkParamsModel dsDynamicLinkParamsModel)
        {
            ShowProgressDialog();
            this.userActionsListener.GetEKYCIdentificationOnCall(dsDynamicLinkParamsModel);
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

        public void ShowProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnRunLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void HideProgressDialog()
        {
            try
            {
                LoadingOverlayUtils.OnStopLoadingAnimation(this);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        private void ShowUnMatchUserIdPopUp()
        {
            RunOnUiThread(() =>
            {
                HideProgressDialog();

                MyTNBAppToolTipBuilder marketingTooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.MYTNB_DIALOG_ICON_ONE_BUTTON)
                   .SetHeaderImage(Resource.Drawable.ic_display_validation_success)
                   .SetTitle(GetLabelByLanguage(DSConstants.I18N_UserIDNotMatchTitle))
                   .SetMessage(GetLabelByLanguage(DSConstants.I18N_UserIDNotMatchMessage))
                   .SetCTALabel(GetLabelCommonByLanguage(LanguageConstants.Common.OK))
                   .SetCTAaction(() => { })
                   .Build();
                marketingTooltip.Show();
            });
        }

        public void ShowCompletedOnOtherDevicePopUp()
        {
            RunOnUiThread(() =>
            {
                HideProgressDialog();

                MyTNBAppToolTipBuilder marketingTooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.MYTNB_DIALOG_ICON_ONE_BUTTON)
                   .SetHeaderImage(Resource.Drawable.Icon_DS_Verify_Processing_Pop_Up)
                   .SetTitle(GetLabelByLanguage(DSConstants.I18N_IDProcessingTitle))
                   .SetMessage(GetLabelByLanguage(DSConstants.I18N_IDProcessingMessage))
                   .SetCTALabel(GetLabelByLanguage(DSConstants.I18N_BackToHome))
                   .SetCTAaction(() =>
                   {
                       SetResult(Result.Canceled);
                       Finish();
                   })
                   .Build();
                marketingTooltip.Show();
            });
        }

        public void ShowIdNotRegisteredPopUp()
        {
            RunOnUiThread(() =>
            {
                HideProgressDialog();

                MyTNBAppToolTipBuilder marketingTooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.MYTNB_DIALOG_ICON_TWO_BUTTON)
                   .SetHeaderImage(Resource.Drawable.Icon_DS_Verify_Pop_Up)
                   .SetTitle(GetLabelByLanguage(DSConstants.I18N_NoRegisteredIDTitle))
                   .SetMessage(GetLabelByLanguage(DSConstants.I18N_NoRegisteredIDMessage))
                   .SetCTALabel(GetLabelByLanguage(DSConstants.I18N_Cancel))
                   .SetSecondaryCTALabel(GetLabelByLanguage(DSConstants.I18N_VerifyNow))
                   .SetCTAaction(() => { })
                   .SetSecondaryCTAaction(() => OnVerifyNow())
                   .Build();
                marketingTooltip.Show();

                DynatraceHelper.OnTrack(DynatraceConstants.DS.Screens.Popup.Document_Ready);
            });
        }

        public void ShowIdentityHasBeenVerified()
        {
            RunOnUiThread(() =>
            {
                HideProgressDialog();

                MyTNBAppToolTipBuilder marketingTooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.MYTNB_DIALOG_ICON_ONE_BUTTON)
                   .SetHeaderImage(Resource.Drawable.Icon_Profile_Verified)
                   .SetTitle(GetLabelByLanguage(DSConstants.I18N_IDVerifiedTitle))
                   .SetMessage(GetLabelByLanguage(DSConstants.I18N_IDVerifiedMessage))
                   .SetCTALabel(GetLabelByLanguage(DSConstants.I18N_BackToHome))
                   .SetCTAaction(() =>
                   {
                       SetResult(Result.Canceled);
                       Finish();
                   })
                   .Build();
                marketingTooltip.Show();
            });
        }

        public void ShowPrepareDocumentPopUp(int? idType)
        {
            RunOnUiThread(() =>
            {
                HideProgressDialog();
                
                if (idType == null)
                {
                    return;
                }

                try
                {
                    string idTypeString = string.Empty;
                    var selectorContent = LanguageManager.Instance.GetSelectorsByPage<DSIdTypeSelectorModel>(DigitalSignatureConstants.DS_LANDING_SELECTOR);
                    if (selectorContent.ContainsKey(DigitalSignatureConstants.DS_ID_TYPE))
                    {
                        List<DSIdTypeSelectorModel> idTypeList = new List<DSIdTypeSelectorModel>();
                        idTypeList = selectorContent[DigitalSignatureConstants.DS_ID_TYPE];
                        if (idTypeList.Count > 0)
                        {
                            idTypeString = idTypeList.Find(x => { return x.key == idType.ToString(); }).description;

                            var dialogTitle = string.Empty;
                            var dialogMessage = string.Empty;
                            var dropdownTitle = string.Empty;
                            var dropdownMessage = string.Empty;

                            if (idTypeString.ToLower() == DSConstants.Passport.ToLower())
                            {
                                string lastFourDigits = string.Empty;
                                var passportNumber = this.userActionsListener.GetIdentificationModel().IdentificationNo;

                                if (passportNumber.IsValid())
                                {
                                    lastFourDigits = passportNumber[^4..];
                                }

                                dialogTitle = GetLabelByLanguage(DSConstants.I18N_AcceptedIDTitle_Passport);
                                dialogMessage = string.Format(GetLabelByLanguage(DSConstants.I18N_AcceptedIDMessage_Passport), idTypeString, lastFourDigits);
                                dropdownTitle = GetLabelByLanguage(DSConstants.I18N_CantUseOtherIDTitle_Passport);
                                dropdownMessage = string.Format(GetLabelByLanguage(DSConstants.I18N_CantUseOtherIDMessage_Passport), idTypeString);
                            }
                            else
                            {
                                dialogTitle = GetLabelByLanguage(DSConstants.I18N_AcceptedIDTitle_IC);
                                dialogMessage = string.Format(GetLabelByLanguage(DSConstants.I18N_AcceptedIDMessage_IC), idTypeString);
                                dropdownTitle = GetLabelByLanguage(DSConstants.I18N_CantUseOtherIDTitle_IC);
                                dropdownMessage = string.Format(GetLabelByLanguage(DSConstants.I18N_CantUseOtherIDMessage_IC), idTypeString);
                            }

                            MyTNBAppToolTipBuilder marketingTooltip = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.MYTNB_DIALOG_ICON_DROPDOWN_TWO_BUTTON)
                               .SetHeaderImage(Resource.Drawable.Icon_DS_Verify_Pop_Up)
                               .SetTitle(dialogTitle)
                               .SetMessage(dialogMessage)
                               .SetDropdownTitle(dropdownTitle)
                               .SetDropdownMessage(dropdownMessage)
                               .SetCTALabel(GetLabelByLanguage(DSConstants.I18N_Cancel))
                               .SetSecondaryCTALabel(GetLabelByLanguage(DSConstants.I18N_VerifyNow))
                               .SetCTAaction(() => { })
                               .SetSecondaryCTAaction(() => OnVerifyNow())
                               .Build();
                            marketingTooltip.Show();

                            DynatraceHelper.OnTrack(DynatraceConstants.DS.Screens.Popup.Document_Ready);
                        }
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
            MyTNBAppToolTipBuilder whereisMyacc = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.NORMAL_WITH_HEADER)
                .SetTitle(statusDetail.Title)
                .SetMessage(statusDetail.Message)
                .SetCTALabel(statusDetail.PrimaryCTATitle)
                .Build();
            whereisMyacc.Show();
        }

        private void OnVerifyNow()
        {
            Intent intent = new Intent(this, typeof(DSWebViewActivity));
            intent.PutExtra(DigitalSignatureConstants.DS_IDENTIFICATION_MODEL, JsonConvert.SerializeObject(this.userActionsListener.GetIdentificationModel()));
            intent.PutExtra(DigitalSignatureConstants.DS_DYNAMIC_LINK_PARAMS_MODEL, JsonConvert.SerializeObject(this.userActionsListener.GetDSDynamicLinkParamsModel()));
            StartActivity(intent);

            DynatraceHelper.OnTrack(DynatraceConstants.DS.CTAs.Verification.PopUp_Verify_Now);
        }
    }
}
