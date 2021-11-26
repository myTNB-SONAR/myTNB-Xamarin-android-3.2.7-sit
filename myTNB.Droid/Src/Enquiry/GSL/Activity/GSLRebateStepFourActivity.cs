using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Content;
using CheeseBind;
using Google.Android.Material.Snackbar;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Base.Request;
using myTNB_Android.Src.Common.Activity;
using myTNB_Android.Src.Common.Model;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Enquiry.Common;
using myTNB_Android.Src.Enquiry.Component;
using myTNB_Android.Src.Enquiry.GSL.MVP;
using myTNB_Android.Src.SubmitEnquirySuccess.Activity;
using myTNB_Android.Src.Utils;
using FileUtils = myTNB_Android.Src.Utils.FileUtils;

namespace myTNB_Android.Src.Enquiry.GSL.Activity
{
    [Activity(Label = "GSL Rebate Step Four"
      , ScreenOrientation = ScreenOrientation.Portrait
              , WindowSoftInputMode = SoftInput.AdjustPan
      , Theme = "@style/Theme.Enquiry")]
    public class GSLRebateStepFourActivity : BaseToolbarAppCompatActivity, GSLRebateStepFourContract.IView
    {
        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.gslStepFourPageTitle)]
        TextView gslStepFourPageTitle;

        [BindView(Resource.Id.txtStepFourYourContactTitle)]
        TextView txtStepFourYourContactTitle;

        [BindView(Resource.Id.gslStepFourTnCCheckBox)]
        CheckBox gslStepFourTnCCheckBox;

        [BindView(Resource.Id.gslStepFourbtnNext)]
        Button gslStepFourbtnNext;

        [BindView(Resource.Id.txtGSLTnC)]
        TextView txtGSLTnC;

        [BindView(Resource.Id.gslStepFourContactDetailsView)]
        readonly LinearLayout gslStepFourContactDetailsView;

        private EnquiryAccountDetailsComponent accountDetailsComponent;

        private GSLRebateStepFourContract.IUserActionsListener presenter;

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override int ResourceId()
        {
            return Resource.Layout.GSLRebateStepFourView;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        public void SetPresenter(GSLRebateStepFourContract.IUserActionsListener userActionListener)
        {
            this.presenter = userActionListener;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                _ = new GSLRebateStepFourPresenter(this);
                this.presenter?.OnInitialize();

                Bundle extras = Intent.Extras;
                if (extras != null)
                {
                    if (extras.ContainsKey(GSLRebateConstants.REBATE_MODEL))
                    {
                        var rebateModel = DeSerialze<GSLRebateModel>(extras.GetString(GSLRebateConstants.REBATE_MODEL));
                        this.presenter.SetRebateModel(rebateModel);
                    }
                }
                SetUpViews();
            }
            catch (Exception e)
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

            TextViewUtils.SetMuseoSans500Typeface(gslStepFourPageTitle, gslStepFourbtnNext, txtStepFourYourContactTitle);
            TextViewUtils.SetMuseoSans300Typeface(txtGSLTnC);
            TextViewUtils.SetTextSize12(gslStepFourPageTitle, txtGSLTnC);
            TextViewUtils.SetTextSize16(gslStepFourbtnNext, txtStepFourYourContactTitle); ;

            int stepNo = this.presenter.GetGSLRebateModel().NeedsIncident ? 4 : 3;
            int stepTotalNo = this.presenter.GetGSLRebateModel().NeedsIncident ? 4 : 3;

            var stepTitleString = string.Format(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_STEP_TITLE), stepNo, stepTotalNo);
            gslStepFourPageTitle.Text = stepTitleString;

            txtStepFourYourContactTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.CONTACT_DETAILS_TITLE);
            gslStepFourbtnNext.Text = Utility.GetLocalizedLabel(LanguageConstants.COMMON, LanguageConstants.Common.SUBMIT);

            gslStepFourTnCCheckBox.Checked = false;
            txtGSLTnC.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.ENQUIRY_TNC));

            RenderAccountDetailsComponent();
        }

        private void RenderAccountDetailsComponent()
        {
            accountDetailsComponent = new EnquiryAccountDetailsComponent(this, this);
            accountDetailsComponent.SetOnTapCountryCodeAction(OnTapCountryCode);
            accountDetailsComponent.SetCheckRequiredFieldsAction(CheckRequiredFields);
            gslStepFourContactDetailsView.AddView(accountDetailsComponent);

            PopulateFieldsForLoggedInUser();
        }

        public void PopulateFieldsForLoggedInUser()
        {
            if (UserEntity.IsCurrentlyActive())
            {
                accountDetailsComponent.SetFullNameField(UserEntity.GetActive().DisplayName);
                accountDetailsComponent.SetEmailAddressField(UserEntity.GetActive().Email);
                accountDetailsComponent.SetMobileNumberField(UserEntity.GetActive().MobileNo);
                accountDetailsComponent.ValidateAllFields();
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            this.presenter.OnActivityResult(requestCode, resultCode, data);
        }

        public void UpdateButtonState(bool isEnabled)
        {
            gslStepFourbtnNext.Enabled = isEnabled;
            gslStepFourbtnNext.Background = ContextCompat.GetDrawable(this, isEnabled ? Resource.Drawable.green_button_background :
                Resource.Drawable.silver_chalice_button_background);
        }

        private void OnTapCountryCode()
        {
            Intent selectCountryIntent = new Intent(this, typeof(SelectCountryActivity));
            StartActivityForResult(selectCountryIntent, EnquiryConstants.COUNTRY_CODE_SELECT_REQUEST);
        }

        public void SetSelectedCountry(Country country)
        {
            accountDetailsComponent.SetSelectedCountry(country);
        }

        private void CheckRequiredFields(bool fieldsAreValid)
        {
            this.UpdateButtonState(fieldsAreValid && this.presenter.GetTncAcceptedFlag());
        }

        private void UpdateTnCText(bool isChecked)
        {
            txtGSLTnC.TextFormatted = GetFormattedText(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, isChecked ? LanguageConstants.SubmitEnquiry.ENQUIRY_TNC_READ :
                LanguageConstants.SubmitEnquiry.ENQUIRY_TNC));
        }

        [OnClick(Resource.Id.gslStepFourTnCCheckBox)]
        void OnTncCheckBox(object sender, EventArgs eventArgs)
        {
            var accepted = !this.presenter.GetTncAcceptedFlag();
            UpdateTnCText(accepted);
            gslStepFourTnCCheckBox.Checked = accepted;
            this.presenter.SetTncAcceptedFlag(accepted);
            this.UpdateButtonState(this.presenter.GetTncAcceptedFlag() && accountDetailsComponent.FieldsAreValid());
        }

        [OnClick(Resource.Id.txtGSLTnC)]
        void OnClickTermsConditions(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                if (accountDetailsComponent.FieldsAreValid())
                {
                    this.SetIsClicked(true);
                    SaveFields();
                    ShowTermsAndConditions();
                }
                else
                {
                    accountDetailsComponent.ValidateAllFields();
                }
            }
        }

        [OnClick(Resource.Id.gslStepFourbtnNext)]
        public void ButtonNextOnClick(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                SaveFields();

                if (this.presenter.CheckModelIfValid())
                {
                    OnSubmitGSLEnquiry();
                }
                else
                {
                    this.SetIsClicked(false);
                }
            }
        }

        private void SaveFields()
        {
            this.presenter.SetAccountFullName(accountDetailsComponent.GetFullNameValue());
            this.presenter.SetAccountEmailAddress(accountDetailsComponent.GetEmailValue());
            this.presenter.SetAccountMobileNumber(accountDetailsComponent.GetMobileNumValue());
        }

        private void ShowTermsAndConditions()
        {
            var enquiryTnC = new Intent(this, typeof(EnquiryTnCActivity));
            enquiryTnC.PutExtra(Constants.REQ_EMAIL, this.presenter.GetAccountEmailAddress().Trim());
            enquiryTnC.PutExtra(Constants.SELECT_REGISTERED_OWNER, this.presenter.GetIsOwner().ToString());
            enquiryTnC.PutExtra(Constants.ENTERED_NAME, this.presenter.GetAccountFullName().Trim());
            StartActivity(enquiryTnC);
        }

        private void OnSubmitGSLEnquiry()
        {
            this.SetIsClicked(true);
            this.presenter.OnSubmitActionAsync();
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

        public Task<AttachedImageRequest> SaveImage(AttachedImage attachedImage)
        {
            return Task.Run<AttachedImageRequest>(() =>
            {
                BitmapFactory.Options bmOptions = new BitmapFactory.Options();

                if (attachedImage.Name.ToLower().Contains("pdf"))
                {
                    try
                    {
                        byte[] pdfByteData = FileUtils.GetPDFByte(this, attachedImage.Path);

                        int size = pdfByteData.Length;
                        string hexString = HttpUtility.UrlEncode(FileUtils.ByteArrayToString(pdfByteData), Encoding.UTF8);
                        System.Console.WriteLine(string.Format("Hex string {0}", hexString));
                        return new AttachedImageRequest()
                        {
                            ImageHex = hexString,
                            FileSize = size,
                            FileName = attachedImage.Name
                        };
                    }
                    catch (Exception e)
                    {
                        Utility.LoggingNonFatalError(e);

                        Bitmap bitmap = BitmapFactory.DecodeFile(attachedImage.Path, bmOptions);
                        byte[] imageBytes = FileUtils.GetCompress(this, bitmap);

                        int size = imageBytes.Length;
                        string hexString = HttpUtility.UrlEncode(FileUtils.ByteArrayToString(imageBytes), Encoding.UTF8);
                        if (bitmap != null && !bitmap.IsRecycled)
                        {
                            bitmap.Recycle();
                        }
                        System.Console.WriteLine(string.Format("Hex string {0}", hexString));
                        return new AttachedImageRequest()
                        {
                            ImageHex = hexString,
                            FileSize = size,
                            FileName = attachedImage.Name
                        };
                    }
                }
                else
                {
                    Bitmap bitmap = BitmapFactory.DecodeFile(attachedImage.Path, bmOptions);
                    byte[] imageBytes = FileUtils.GetCompress(this, bitmap);

                    int size = imageBytes.Length;
                    string hexString = HttpUtility.UrlEncode(FileUtils.ByteArrayToString(imageBytes), Encoding.UTF8);
                    if (bitmap != null && !bitmap.IsRecycled)
                    {
                        bitmap.Recycle();
                    }
                    System.Console.WriteLine(string.Format("Hex string {0}", hexString));
                    return new AttachedImageRequest()
                    {
                        ImageHex = hexString,
                        FileSize = size,
                        FileName = attachedImage.Name
                    };
                }
            });
        }

        public List<AttachedImage> GetDeSerializeImage(string image)
        {
            try
            {
                if (image.IsValid())
                {
                    return DeSerialze<List<AttachedImage>>(image);
                }
                return null;
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
                return null;
            }
        }

        public void OnSubmitError(string message = null)
        {
            if (this.mErrorMessageSnackBar != null && this.mErrorMessageSnackBar.IsShown)
            {
                this.mErrorMessageSnackBar.Dismiss();
            }

            if (string.IsNullOrEmpty(message))
            {
                message = Utility.GetLocalizedErrorLabel("defaultErrorMessage");
            }

            this.mErrorMessageSnackBar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
            .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { this.mErrorMessageSnackBar.Dismiss(); }
            );
            View v = this.mErrorMessageSnackBar.View;
            TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
            tv.SetMaxLines(5);

            this.mErrorMessageSnackBar.Show();
            this.SetIsClicked(false);
        }

        public void ShowSuccess(string date, string feedbackId, int imageCount)
        {
            ISharedPreferences sharedPref = PreferenceManager.GetDefaultSharedPreferences(this);
            int currentCount = UserSessions.GetCurrentImageCount(sharedPref);
            UserSessions.SetCurrentImageCount(sharedPref, currentCount + imageCount);

            var successIntent = new Intent(this, typeof(SubmitEnquirySuccessActivity));
            successIntent.PutExtra(EnquiryConstants.FEEDBACK_CATEGORY_ID, EnquiryConstants.GSL_FEEDBACK_CATEGORY_ID);
            successIntent.PutExtra(Constants.RESPONSE_FEEDBACK_DATE, date);
            successIntent.PutExtra(Constants.RESPONSE_FEEDBACK_ID, feedbackId);
            StartActivity(successIntent);
        }
    }
}
