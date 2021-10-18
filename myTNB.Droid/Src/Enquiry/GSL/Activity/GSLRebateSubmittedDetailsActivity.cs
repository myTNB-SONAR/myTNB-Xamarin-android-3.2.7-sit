using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using CheeseBind;
using Google.Android.Material.TextField;
using myTNB;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Enquiry.GSL.MVP;
using myTNB_Android.Src.Enquiry.GSL.MVP.GSLSubmittedDetails;
using myTNB_Android.Src.FeedbackDetails.Adapter;
using myTNB_Android.Src.FeedbackFullScreenImage.Activity;
using myTNB_Android.Src.Utils;
using Newtonsoft.Json;

namespace myTNB_Android.Src.Enquiry.GSL.Activity
{
    [Activity(Label = "GSL Rebate",
        ScreenOrientation = ScreenOrientation.Portrait,
        WindowSoftInputMode = SoftInput.AdjustPan,
        Theme = "@style/Theme.Enquiry")]
    public class GSLRebateSubmittedDetailsActivity : BaseToolbarAppCompatActivity, GSLRebateSubmittedDetailsContract.IView
    {
        [BindView(Resource.Id.txtGSLDetailStatusTitle)]
        TextView txtGSLDetailStatusTitle;

        [BindView(Resource.Id.txtGSLDetailStatusValue)]
        TextView txtGSLDetailStatusValue;

        [BindView(Resource.Id.txtGSLDetailRebateType)]
        TextView txtGSLDetailRebateType;


        [BindView(Resource.Id.txtGSLDetailContactTitle)]
        TextView txtGSLDetailContactTitle;

        [BindView(Resource.Id.txtGSLTenantInfoTitle)]
        TextView txtGSLTenantInfoTitle;

        [BindView(Resource.Id.txtGSLIncidentInfoTitle)]
        TextView txtGSLIncidentInfoTitle;


        [BindView(Resource.Id.txtGSLDetailServiceReqNumLayout)]
        TextInputLayout txtGSLDetailServiceReqNumLayout;


        [BindView(Resource.Id.txtGSLDetailContactFullNameLayout)]
        TextInputLayout txtGSLDetailContactFullNameLayout;

        [BindView(Resource.Id.txtGSLDetailEmailLayout)]
        TextInputLayout txtGSLDetailEmailLayout;

        [BindView(Resource.Id.txtGSLDetailMobileLayout)]
        TextInputLayout txtGSLDetailMobileLayout;

        [BindView(Resource.Id.txtGSLDetailAddressLayout)]
        TextInputLayout txtGSLDetailAddressLayout;


        [BindView(Resource.Id.txtGSLTenantInfoFullNameLayout)]
        TextInputLayout txtGSLTenantInfoFullNameLayout;

        [BindView(Resource.Id.txtGSLTenantInfoEmailLayout)]
        TextInputLayout txtGSLTenantInfoEmailLayout;

        [BindView(Resource.Id.txtGSLTenantInfoMobileLayout)]
        TextInputLayout txtGSLTenantInfoMobileLayout;


        [BindView(Resource.Id.gslIcidentDatesContainer)]
        LinearLayout gslIcidentDatesContainer;

        [BindView(Resource.Id.txtGSLIncidentDateLayout)]
        TextInputLayout txtGSLIncidentDateLayout;

        [BindView(Resource.Id.txtGSLIncidentTimeLayout)]
        TextInputLayout txtGSLIncidentTimeLayout;

        [BindView(Resource.Id.txtGSLRestorationDateLayout)]
        TextInputLayout txtGSLRestorationDateLayout;

        [BindView(Resource.Id.txtGSLRestorationTimeLayout)]
        TextInputLayout txtGSLRestorationTimeLayout;


        [BindView(Resource.Id.editTxtGSLDetailServiceReqNum)]
        EditText editTxtGSLDetailServiceReqNum;


        [BindView(Resource.Id.editTxtGSLDetailContactFullName)]
        EditText editTxtGSLDetailContactFullName;

        [BindView(Resource.Id.editTxtGSLDetailEmail)]
        EditText editTxtGSLDetailEmail;

        [BindView(Resource.Id.editTxtGSLDetailMobile)]
        EditText editTxtGSLDetailMobile;

        [BindView(Resource.Id.editTxtGSLDetailAddress)]
        EditText editTxtGSLDetailAddress;


        [BindView(Resource.Id.editTxtGSLTenantInfoFullName)]
        EditText editTxtGSLTenantInfoFullName;

        [BindView(Resource.Id.editTxtGSLTenantInfoEmail)]
        EditText editTxtGSLTenantInfoEmail;

        [BindView(Resource.Id.editTxtGSLTenantInfoMobile)]
        EditText editTxtGSLTenantInfoMobile;


        [BindView(Resource.Id.editTxtGSLIncidentDate)]
        EditText editTxtGSLIncidentDate;

        [BindView(Resource.Id.editTxtGSLIncidentTime)]
        EditText editTxtGSLIncidentTime;

        [BindView(Resource.Id.editTxtGSLRestorationDate)]
        EditText editTxtGSLRestorationDate;

        [BindView(Resource.Id.editTxtGSLRestorationTime)]
        EditText editTxtGSLRestorationTime;

        [BindView(Resource.Id.txtRequiredDocumentsTitle)]
        TextView txtRequiredDocumentsTitle;


        [BindView(Resource.Id.recyclerViewDocument)]
        RecyclerView recyclerViewDocument;


        [BindView(Resource.Id.gslDetailContactLinearLayout)]
        LinearLayout gslDetailContactLinearLayout;

        [BindView(Resource.Id.gslTenantInfoLinearLayout)]
        LinearLayout gslTenantInfoLinearLayout;

        [BindView(Resource.Id.gslIncidentInfoLinearLayout)]
        LinearLayout gslIncidentInfoLinearLayout;

        [BindView(Resource.Id.gslAttachmentLinearLayout)]
        LinearLayout gslAttachmentLinearLayout;


        private GSLRebateSubmittedDetailsContract.IUserActionsListener presenter;

        LinearLayoutManager layoutManager;
        FeedbackImageRecyclerAdapterNew adapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                _ = new GSLRebateSubmittedDetailsPresenter(this, this);

                string selectedFeedback = UserSessions.GetSelectedFeedback(PreferenceManager.GetDefaultSharedPreferences(this));
                SubmittedFeedbackDetails submittedFeedback = JsonConvert.DeserializeObject<SubmittedFeedbackDetails>(selectedFeedback);

                this.presenter?.OnInitialize(submittedFeedback);
            }
            catch (System.Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetUpViews()
        {
            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);

            SetToolBarTitle(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_HEADER_TITLE));

            SetFontStyle();
            SetFontSize();
            SetTextLayout();
            SetTextValues();
            SetAttachments();

            this.presenter.PrepareDataAsync();
        }

        private void SetFontStyle()
        {
            TextViewUtils.SetMuseoSans500Typeface(txtGSLDetailStatusTitle, txtGSLDetailStatusValue, txtGSLDetailContactTitle, txtGSLTenantInfoTitle, txtGSLIncidentInfoTitle);
            TextViewUtils.SetMuseoSans300Typeface(txtGSLDetailRebateType, editTxtGSLDetailServiceReqNum, txtRequiredDocumentsTitle);
            TextViewUtils.SetMuseoSans300Typeface(txtGSLDetailServiceReqNumLayout);

            TextViewUtils.SetMuseoSans300Typeface(txtGSLDetailContactFullNameLayout, txtGSLDetailEmailLayout, txtGSLDetailMobileLayout, txtGSLDetailAddressLayout);
            TextViewUtils.SetMuseoSans300Typeface(txtGSLTenantInfoFullNameLayout, txtGSLTenantInfoEmailLayout, txtGSLTenantInfoMobileLayout);
            TextViewUtils.SetMuseoSans300Typeface(txtGSLIncidentDateLayout, txtGSLIncidentTimeLayout, txtGSLRestorationDateLayout, txtGSLRestorationTimeLayout);

            TextViewUtils.SetMuseoSans300Typeface(editTxtGSLDetailContactFullName, editTxtGSLDetailEmail, editTxtGSLDetailMobile, editTxtGSLDetailAddress);
            TextViewUtils.SetMuseoSans300Typeface(editTxtGSLTenantInfoFullName, editTxtGSLTenantInfoEmail, editTxtGSLTenantInfoMobile);
            TextViewUtils.SetMuseoSans300Typeface(editTxtGSLIncidentDate, editTxtGSLIncidentTime, editTxtGSLRestorationDate, editTxtGSLRestorationTime);
        }

        private void SetFontSize()
        {
            TextViewUtils.SetTextSize16(txtGSLDetailStatusTitle, txtGSLDetailStatusValue, editTxtGSLDetailServiceReqNum);
            TextViewUtils.SetTextSize14(txtGSLDetailRebateType, txtGSLDetailContactTitle, txtGSLTenantInfoTitle, txtGSLIncidentInfoTitle);
            TextViewUtils.SetTextSize10(txtRequiredDocumentsTitle);

            TextViewUtils.SetTextSize16(editTxtGSLDetailContactFullName, editTxtGSLDetailEmail, editTxtGSLDetailMobile, editTxtGSLDetailAddress);
            TextViewUtils.SetTextSize16(editTxtGSLTenantInfoFullName, editTxtGSLTenantInfoEmail, editTxtGSLTenantInfoMobile);
            TextViewUtils.SetTextSize16(editTxtGSLIncidentDate, editTxtGSLIncidentTime, editTxtGSLRestorationDate, editTxtGSLRestorationTime);
        }

        private void SetTextLayout()
        {
            txtGSLDetailServiceReqNumLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts
                    ? Resource.Style.TextInputLayout_TextAppearance_Large
                    : Resource.Style.TextInputLayout_TextAppearance_Small);

            txtGSLDetailContactFullNameLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts
                    ? Resource.Style.TextInputLayout_TextAppearance_Large
                    : Resource.Style.TextInputLayout_TextAppearance_Small);
            txtGSLDetailEmailLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts
                    ? Resource.Style.TextInputLayout_TextAppearance_Large
                    : Resource.Style.TextInputLayout_TextAppearance_Small);
            txtGSLDetailMobileLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts
                    ? Resource.Style.TextInputLayout_TextAppearance_Large
                    : Resource.Style.TextInputLayout_TextAppearance_Small);
            txtGSLDetailAddressLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts
                    ? Resource.Style.TextInputLayout_TextAppearance_Large
                    : Resource.Style.TextInputLayout_TextAppearance_Small);

            txtGSLTenantInfoFullNameLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts
                    ? Resource.Style.TextInputLayout_TextAppearance_Large
                    : Resource.Style.TextInputLayout_TextAppearance_Small);
            txtGSLTenantInfoEmailLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts
                    ? Resource.Style.TextInputLayout_TextAppearance_Large
                    : Resource.Style.TextInputLayout_TextAppearance_Small);
            txtGSLTenantInfoMobileLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts
                    ? Resource.Style.TextInputLayout_TextAppearance_Large
                    : Resource.Style.TextInputLayout_TextAppearance_Small);

            txtGSLIncidentDateLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts
                    ? Resource.Style.TextInputLayout_TextAppearance_Large
                    : Resource.Style.TextInputLayout_TextAppearance_Small);
            txtGSLIncidentTimeLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts
                    ? Resource.Style.TextInputLayout_TextAppearance_Large
                    : Resource.Style.TextInputLayout_TextAppearance_Small);
            txtGSLRestorationDateLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts
                    ? Resource.Style.TextInputLayout_TextAppearance_Large
                    : Resource.Style.TextInputLayout_TextAppearance_Small);
            txtGSLRestorationTimeLayout.SetHintTextAppearance(TextViewUtils.IsLargeFonts
                    ? Resource.Style.TextInputLayout_TextAppearance_Large
                    : Resource.Style.TextInputLayout_TextAppearance_Small);

            txtGSLDetailServiceReqNumLayout.Hint = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.SERVICE_NO_TITLE).ToUpper();

            txtGSLDetailContactFullNameLayout.Hint = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.FULL_NAME_HINT).ToUpper();
            txtGSLDetailEmailLayout.Hint = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.EMAIL_HINT).ToUpper();
            txtGSLDetailMobileLayout.Hint = Utility.GetLocalizedCommonLabel(LanguageConstants.Common.MOBILE_NO).ToUpper();
            txtGSLDetailAddressLayout.Hint = "PREMISES ADDRESS";//stub

            txtGSLTenantInfoFullNameLayout.Hint = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.FULL_NAME_HINT).ToUpper();
            txtGSLTenantInfoEmailLayout.Hint = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.EMAIL_HINT).ToUpper();
            txtGSLTenantInfoMobileLayout.Hint = Utility.GetLocalizedCommonLabel(LanguageConstants.Common.MOBILE_NO).ToUpper();

            txtGSLIncidentDateLayout.Hint = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.INCIDENT_DATE_HINT).ToUpper();
            txtGSLIncidentTimeLayout.Hint = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.INCIDENT_TIME_HINT).ToUpper();
            txtGSLRestorationDateLayout.Hint = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.RESTORATION_DATE_HINT).ToUpper();
            txtGSLRestorationTimeLayout.Hint = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.RESTORATION_TIME_HINT).ToUpper();
        }

        private void SetTextValues()
        {
            txtGSLDetailStatusTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.APPLICATION_STATUS_DETAILS, LanguageConstants.ApplicationStatusDetails.STATUS_LABEL);
            txtGSLDetailContactTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.CONTACT_DETAILS_TITLE);
            txtGSLTenantInfoTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_TENANT_INFO_TITLE);
            txtGSLIncidentInfoTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_INCIDENT_INFO_TITLE);
            txtRequiredDocumentsTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.SUPPORTING_DOCS_TITLE);
        }

        private void SetAttachments()
        {
            adapter = new FeedbackImageRecyclerAdapterNew(true);
            layoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);

            recyclerViewDocument.SetLayoutManager(layoutManager);
            recyclerViewDocument.SetAdapter(adapter);

            adapter.SelectClickEvent += Adapter_SelectClickEvent; ;
        }

        private void Adapter_SelectClickEvent(object sender, int e)
        {
            System.Console.WriteLine("AttachmentOnTap()");
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                AttachedImage selectedImage = adapter.GetItemObject(e);
                if (selectedImage.Name.ToLower().Contains(".pdf"))
                {
                    try
                    {
                        System.Console.WriteLine("OpenPDF()");
                        OpenPDF(selectedImage.Path);
                    }
                    catch (Exception g)
                    {
                        Utility.LoggingNonFatalError(g);
                    }
                }
                else
                {
                    System.Console.WriteLine("OpenImage()");
                    var fullImageIntent = new Intent(this, typeof(FeedbackDetailsFullScreenImageActivity));
                    fullImageIntent.PutExtra(Constants.SELECTED_FEEDBACK_DETAIL_IMAGE, JsonConvert.SerializeObject(selectedImage));
                    StartActivity(fullImageIntent);
                }
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override int ResourceId()
        {
            return Resource.Layout.GSLRebateSubmittedDetailsView;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public void SetPresenter(GSLRebateSubmittedDetailsContract.IUserActionsListener userActionListener)
        {
            this.presenter = userActionListener;
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
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

        private string GetRebateTypeFromSelectorWithKey(string key)
        {
            var selector = LanguageManager.Instance.GetSelectorsByPage<SelectorModel>(LanguageConstants.SUBMIT_ENQUIRY);
            if (selector.ContainsKey(LanguageConstants.SubmitEnquiry.REBATE_TYPE))
            {
                var itemList = selector[LanguageConstants.SubmitEnquiry.REBATE_TYPE];
                if (itemList.Count > 0)
                {
                    var item = itemList.Find(x => x.Key.Equals(key));
                    if (item != null)
                    {
                        return item.Description;
                    }
                }
            }
            return string.Empty;
        }

        public void RenderAttachments(List<AttachedImage> attachedImages)
        {
            if (attachedImages != null && attachedImages.Count > 0)
            {
                adapter.AddAll(attachedImages);
            }
            else
            {
                gslAttachmentLinearLayout.Visibility = ViewStates.Gone;
            }
        }

        public void RenderUIFromModel(GSLRebateModel model)
        {
            try
            {
                if (model == null)
                {
                    return;
                }

                txtGSLDetailStatusValue.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this, model.StatusColor)));
                txtGSLDetailStatusValue.Text = model.StatusDesc;

                txtGSLDetailRebateType.Text = string.Format("{0}", Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.FOR)
                    + GetRebateTypeFromSelectorWithKey(model.RebateTypeKey));

                editTxtGSLDetailServiceReqNum.Text = model.ServiceReqNo;

                if (model.ContactInfo != null)
                {
                    editTxtGSLDetailContactFullName.Text = model.ContactInfo.FullName;
                    editTxtGSLDetailEmail.Text = model.ContactInfo.Email;
                    editTxtGSLDetailMobile.Text = model.ContactInfo.MobileNumber;
                    if (model.ContactInfo.Address.IsValid())
                    {
                        editTxtGSLDetailAddress.Text = model.ContactInfo.Address;
                    }
                    else
                    {
                        txtGSLDetailAddressLayout.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    gslDetailContactLinearLayout.Visibility = ViewStates.Gone;
                }

                if (model.TenantInfo != null)
                {
                    editTxtGSLTenantInfoFullName.Text = model.TenantInfo.FullName;
                    editTxtGSLTenantInfoEmail.Text = model.TenantInfo.Email;
                    editTxtGSLTenantInfoMobile.Text = model.TenantInfo.MobileNumber;
                }
                else
                {
                    gslTenantInfoLinearLayout.Visibility = ViewStates.Gone;
                }

                if (model.IncidentList != null && model.IncidentList.Count > 0)
                {
                    var incident = model.IncidentList[0];
                    CultureInfo dateCultureInfo = CultureInfo.CreateSpecificCulture(LanguageUtil.GetAppLanguage());

                    var incidentDate = DateTime.ParseExact(incident.IncidentDateTime, GSLRebateConstants.DATETIME_PARSE_FORMAT,
                            CultureInfo.InvariantCulture, DateTimeStyles.None);
                    var restorationDate = DateTime.ParseExact(incident.RestorationDateTime, GSLRebateConstants.DATETIME_PARSE_FORMAT,
                            CultureInfo.InvariantCulture, DateTimeStyles.None);

                    editTxtGSLIncidentDate.Text = incidentDate.ToString(GSLRebateConstants.DATE_FORMAT, dateCultureInfo);
                    editTxtGSLRestorationDate.Text = restorationDate.ToString(GSLRebateConstants.DATE_FORMAT, dateCultureInfo);

                    editTxtGSLIncidentTime.Text = incidentDate.ToString(GSLRebateConstants.TIME_FORMAT, dateCultureInfo);
                    editTxtGSLRestorationTime.Text = restorationDate.ToString(GSLRebateConstants.TIME_FORMAT, dateCultureInfo);
                }
                else
                {
                    gslIcidentDatesContainer.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void OpenPDF(string path)
        {
            try
            {
                Java.IO.File file = new Java.IO.File(path);
                Android.Net.Uri fileUri = FileProvider.GetUriForFile(this,
                                            ApplicationContext.PackageName + ".fileprovider", file);

                Intent intent = new Intent(Intent.ActionView);
                intent.SetDataAndType(fileUri, "application/pdf");
                intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                StartActivity(intent);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            base.OnTrimMemory(level);

            switch (level)
            {
                case TrimMemory.RunningLow:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
                default:
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect();
                    break;
            }
        }
    }
}
