using System;
using System.Runtime;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;

using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Enquiry.Component;
using myTNB_Android.Src.Enquiry.GSL.MVP;
using myTNB_Android.Src.Utils;
using Android.Provider;
using Android;
using FileUtils = myTNB_Android.Src.Utils.FileUtils;
using Java.Text;
using Java.Util;
using Android.Preferences;
using myTNB_Android.Src.Base.Models;
using System.Threading.Tasks;
using myTNB_Android.Src.Enquiry.Adapter;
using Android.Support.Design.Widget;
using Newtonsoft.Json;
using myTNB_Android.Src.Database.Model;
using CheeseBind;
using Castle.Core.Internal;

namespace myTNB_Android.Src.Enquiry.GSL.Activity
{
    [Activity(Label = "GSL Rebate Step Three"
      , ScreenOrientation = ScreenOrientation.Portrait
              , WindowSoftInputMode = SoftInput.AdjustPan
      , Theme = "@style/Theme.Enquiry")]
    public class GSLRebateStepThreeActivity : BaseToolbarAppCompatActivity, GSLRebateStepThreeContract.IView
    {
        [BindView(Resource.Id.rootView)]
        CoordinatorLayout rootView;

        [BindView(Resource.Id.gslStepThreePageTitle)]
        TextView gslStepThreePageTitle;

        [BindView(Resource.Id.txtStepThreeUploadTitle)]
        TextView txtStepThreeUploadTitle;

        [BindView(Resource.Id.gslStepThreeUploadViewList)]
        readonly LinearLayout gslStepThreeUploadViewList;

        [BindView(Resource.Id.gslStepThreebtnNext)]
        Button gslStepThreebtnNext;

        private AlertDialog chooseDialog;
        private Snackbar errorMessageSnackBar;

        private GSLRebateStepThreeContract.IUserActionsListener presenter;

        UploadDocumentItemListComponent tenancyAgreementItem;
        UploadDocumentItemListComponent ownerICItem;

        GSLDocumentType activeDocumentType;

        LinearLayoutManager layoutManagerTenancy, layoutManagerOwnerIC;
        UploadDocumentItemAdapter tenancyAdapter, ownerICAdapter;

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override int ResourceId()
        {
            return Resource.Layout.GSLRebateStepThreeView;
        }

        public override Boolean ShowCustomToolbarTitle()
        {
            return true;
        }

        public bool IsActive()
        {
            return this.Window.DecorView.RootView.IsShown;
        }

        public override bool CameraPermissionRequired()
        {
            return true;
        }

        public override bool StoragePermissionRequired()
        {
            return true;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                _ = new GSLRebateStepThreePresenter(this, PreferenceManager.GetDefaultSharedPreferences(this));
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
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }

        public void SetPresenter(GSLRebateStepThreeContract.IUserActionsListener userActionListener)
        {
            this.presenter = userActionListener;
        }

        public void SetUpViews()
        {
            SetTheme(TextViewUtils.IsLargeFonts
                ? Resource.Style.Theme_DashboardLarge
                : Resource.Style.Theme_Dashboard);

            SetToolBarTitle(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_HEADER_TITLE));

            TextViewUtils.SetMuseoSans500Typeface(gslStepThreePageTitle, gslStepThreebtnNext, txtStepThreeUploadTitle);
            TextViewUtils.SetTextSize12(gslStepThreePageTitle);
            TextViewUtils.SetTextSize16(gslStepThreebtnNext, txtStepThreeUploadTitle); ;

            var stepTitleString = string.Format(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_STEP_TITLE), 3, 4);
            gslStepThreePageTitle.Text = stepTitleString;

            txtStepThreeUploadTitle.Text = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_UPLOAD_TITLE);
            gslStepThreebtnNext.Text = Utility.GetLocalizedLabel(LanguageConstants.COMMON, LanguageConstants.Common.NEXT);

            RenderUploadDocumentLayoutList();
        }

        private void RenderUploadDocumentLayoutList()
        {
            tenancyAgreementItem = new UploadDocumentItemListComponent(this);
            tenancyAgreementItem.SetUploadDocumentLabel(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_UPLOAD_TA_TITLE));
            tenancyAgreementItem.SetUploadDocumentHint(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.GSL_UPLOAD_TA_DEC));
            tenancyAgreementItem.SetToolTipToVisible(false);

            layoutManagerTenancy = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            tenancyAdapter = new UploadDocumentItemAdapter(true);
            tenancyAdapter.Insert(new Base.Models.AttachedImage()
            {
                ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
            });

            tenancyAgreementItem.SetLayoutManager(layoutManagerTenancy);
            tenancyAgreementItem.SetAdapter(tenancyAdapter);

            tenancyAdapter.AddClickEvent += delegate { AdapterAddClickEvent(GSLDocumentType.TENANCY_AGREEMENT); };
            tenancyAdapter.RemoveClickEvent += AdapterRemoveClickEventForTenancy;

            gslStepThreeUploadViewList.AddView(tenancyAgreementItem);

            ownerICItem = new UploadDocumentItemListComponent(this);
            ownerICItem.SetUploadDocumentLabel(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.UPLOAD_OWNER_IC));
            ownerICItem.SetUploadDocumentHint(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.UPLOAD_ATTACH_DESC));
            ownerICItem.SetToolTipToVisible(true);
            ownerICItem.SetUploadDocumentToolTipLabel(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.UPLOAD_IC_TOOLTIP));
            ownerICItem.SetToolTipAction(ShowOwnerICToolTip);

            layoutManagerOwnerIC = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            ownerICAdapter = new UploadDocumentItemAdapter(true);
            ownerICAdapter.Insert(new Base.Models.AttachedImage()
            {
                ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
            });

            ownerICItem.SetLayoutManager(layoutManagerOwnerIC);
            ownerICItem.SetAdapter(ownerICAdapter);

            ownerICAdapter.AddClickEvent += delegate { AdapterAddClickEvent(GSLDocumentType.OWNER_IC); };
            ownerICAdapter.RemoveClickEvent += AdapterRemoveClickEventForOwnerIC;

            gslStepThreeUploadViewList.AddView(ownerICItem);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            this.presenter.OnActivityResult(requestCode, resultCode, data);
        }

        [Preserve]
        private void AdapterAddClickEvent(GSLDocumentType type)
        {
            try
            {
                activeDocumentType = type;

                var takePhotoString = Utility.GetLocalizedLabel(LanguageConstants.FEEDBACK_FORM, LanguageConstants.FeedbackForm.TAKE_PHOTO);
                var chooseFromLibraryString = Utility.GetLocalizedLabel(LanguageConstants.FEEDBACK_FORM, LanguageConstants.FeedbackForm.CHOOSE_FROM_LIBRARY);
                var documentPDFString = Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.DOCUMENT_PDF);
                var cancelString = Utility.GetLocalizedCommonLabel(LanguageConstants.Common.CANCEL);

                string[] items = { takePhotoString, chooseFromLibraryString, documentPDFString, cancelString };

                AlertDialog.Builder builder = new AlertDialog.Builder(this)
                    .SetTitle(Utility.GetLocalizedLabel(LanguageConstants.FEEDBACK_FORM, LanguageConstants.FeedbackForm.SELECT_OPTIONS));
                builder.SetItems(items, (lsender, args) =>
                {
                    if (items[args.Which].Equals(takePhotoString))
                    {
                        ShowCamera();
                    }
                    else if (items[args.Which].Equals(chooseFromLibraryString))
                    {
                        ShowGallery();
                    }
                    else if (items[args.Which].Equals(documentPDFString))
                    {
                        ShowPDF();
                    }
                    else if (items[args.Which].Equals(cancelString))
                    {
                        chooseDialog.Dismiss();
                    }
                }
                );
                chooseDialog = builder.Show();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [Preserve]
        private void AdapterRemoveClickEventForOwnerIC(object sender, int e)
        {
            try
            {
                ownerICAdapter.Remove(e);
                if (ownerICAdapter.GetAllImages().Count == 1 && ownerICAdapter.ItemCount == 1)
                {
                    ownerICAdapter.Add(new AttachedImage()
                    {
                        ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                    });
                }
                else
                {
                    if (ownerICAdapter.ItemCount == 0)
                    {
                        ownerICAdapter.Add(new AttachedImage()
                        {
                            ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                        });
                        ownerICItem.SetHintToHidden(false);
                    }
                }
                this.presenter.SetOwnerIC(string.Empty);
                this.UpdateButtonState(false);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        [Preserve]
        private void AdapterRemoveClickEventForTenancy(object sender, int e)
        {
            try
            {
                tenancyAdapter.Remove(e);
                if (tenancyAdapter.GetAllImages().Count == 1 && tenancyAdapter.ItemCount == 1)
                {
                    tenancyAdapter.Add(new AttachedImage()
                    {
                        ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                    });
                }
                else
                {
                    if (tenancyAdapter.ItemCount == 0)
                    {
                        tenancyAdapter.Add(new AttachedImage()
                        {
                            ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                        });
                        tenancyAgreementItem.SetHintToHidden(false);
                    }
                }
                this.presenter.SetTenancyDocument(string.Empty);
                this.UpdateButtonState(false);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public string GetImageName(int itemCount)
        {
            SimpleDateFormat dateFormatter = new SimpleDateFormat(EnquiryConstants.UPLOAD_DOCUMENT_DATE_FORMAT);
            Calendar calendar = Calendar.GetInstance(Locale.Default);
            return GetString(Resource.String.feedback_image_name_convention, dateFormatter.Format(calendar.TimeInMillis), UserSessions.GetCurrentImageCount(PreferenceManager.GetDefaultSharedPreferences(this)) + itemCount);
        }

        public string GetTemporaryImageFilePath(string pFolder, string pFileName)
        {
            return FileUtils.GetTemporaryImageFilePath(this, pFolder, pFileName);
        }

        public Task<string> SaveGalleryImage(Android.Net.Uri selectedImage, string pTempImagePath, string pFileName)
        {
            return Task.Run<string>(() =>
            {
                return FileUtils.ProcessGalleryImage(this, selectedImage, pTempImagePath, pFileName);
            });
        }

        private void ShowCamera()
        {
            if (!this.GetIsClicked())
            {
                Permission cameraPermission = ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera);
                if (cameraPermission == (int)Permission.Granted)
                {
                    this.SetIsClicked(true);
                    var takePictureIntent = new Intent(MediaStore.ActionImageCapture);
                    Java.IO.File file = new Java.IO.File(FileUtils.GetTemporaryImageFilePath(this, FileUtils.TEMP_IMAGE_FOLDER, string.Format("{0}.jpeg", "temporaryImage")));
                    Android.Net.Uri fileUri = FileProvider.GetUriForFile(this,
                                                    ApplicationContext.PackageName + ".fileprovider", file);
                    takePictureIntent.PutExtra(Android.Provider.MediaStore.ExtraOutput, fileUri);
                    StartActivityForResult(takePictureIntent, Constants.REQUEST_ATTACHED_CAMERA_IMAGE);
                }
            }
        }

        public void ShowGallery()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);
                Intent galleryIntent = new Intent(Intent.ActionPick, MediaStore.Images.Media.ExternalContentUri);
                galleryIntent.SetType("image/*");
                StartActivityForResult(Intent.CreateChooser(galleryIntent, GetString(Resource.String.bill_related_feedback_select_images)), Constants.RUNTIME_PERMISSION_GALLERY_REQUEST_CODE);
            }
        }

        public void ShowPDF()
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);

                Intent galleryIntent = new Intent(Intent.ActionGetContent);
                galleryIntent.SetType("application/pdf");
                galleryIntent.AddFlags(ActivityFlags.GrantReadUriPermission);
                galleryIntent.AddFlags(ActivityFlags.GrantWriteUriPermission);
                galleryIntent.AddFlags(ActivityFlags.GrantPersistableUriPermission);
                galleryIntent.PutExtra(Intent.ExtraLocalOnly, true);
                StartActivityForResult(Intent.CreateChooser(galleryIntent, GetString(Resource.String.bill_related_feedback_select_images)), Constants.RUNTIME_PERMISSION_GALLERY_PDF_REQUEST_CODE);
            }
        }

        public void UpdateAdapter(string pFilePath, string pFileName, string tFullname = "")
        {
            try
            {
                if (activeDocumentType == GSLDocumentType.TENANCY_AGREEMENT)
                {
                    tenancyAdapter.Update(tenancyAdapter.ItemCount - 1, new AttachedImage()
                    {
                        ViewType = Constants.VIEW_TYPE_REAL_RECORD,
                        Name = pFileName,
                        Path = pFilePath,
                        FileName = tFullname
                    });
                    if (tenancyAdapter.ItemCount < 1)
                    {
                        tenancyAdapter.Add(new AttachedImage()
                        {
                            ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                        });
                    }
                    this.presenter.SetTenancyDocument(JsonConvert.SerializeObject(tenancyAdapter?.GetAllImages()));
                }
                else if (activeDocumentType == GSLDocumentType.OWNER_IC)
                {
                    ownerICAdapter.Update(ownerICAdapter.ItemCount - 1, new AttachedImage()
                    {
                        ViewType = Constants.VIEW_TYPE_REAL_RECORD,
                        Name = pFileName,
                        Path = pFilePath,
                        FileName = tFullname
                    });
                    if (ownerICAdapter.ItemCount < 1)
                    {
                        ownerICAdapter.Add(new AttachedImage()
                        {
                            ViewType = Constants.VIEW_TYPE_DUMMY_RECORD
                        });
                    }
                    this.presenter.SetOwnerIC(JsonConvert.SerializeObject(ownerICAdapter?.GetAllImages()));
                }
                CheckRequiredFields();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public Task<string> SaveCameraImage(string tempImagePath, string fileName)
        {
            return Task.Run<string>(() =>
            {
                return FileUtils.ProcessCameraImage(this, tempImagePath, fileName);
            });
        }

        public string GetFilename(Android.Net.Uri uri)
        {
            try
            {
                string filename = FileUtils.getFilenameUsingContentResolver(this, uri);
                return filename;
            }
            catch
            {
                return null;
            }
        }

        public string GetActualPath(Android.Net.Uri uri)
        {
            string path = FileUtils.GetActualPathForFile(uri, this);
            return path;
        }

        public string CopyPDFGetFilePath(Android.Net.Uri realFilePath, string filename)
        {
            string filePath = FileUtils.CopyPDF(this, realFilePath, FileUtils.PDF_FOLDER, filename);
            return filePath;
        }

        public void ShowLoadingImage()
        {
            try
            {
                if (activeDocumentType == GSLDocumentType.TENANCY_AGREEMENT)
                {
                    int position = tenancyAdapter.ItemCount - 1;
                    AttachedImage attachImage = tenancyAdapter.GetItemObject(position);
                    if (attachImage != null && attachImage.ViewType == Constants.VIEW_TYPE_DUMMY_RECORD)
                    {
                        attachImage.IsLoading = true;
                        tenancyAdapter.Update(position, attachImage);
                    }
                }
                else if (activeDocumentType == GSLDocumentType.OWNER_IC)
                {
                    int position = ownerICAdapter.ItemCount - 1;
                    AttachedImage attachImage = ownerICAdapter.GetItemObject(position);
                    if (attachImage != null && attachImage.ViewType == Constants.VIEW_TYPE_DUMMY_RECORD)
                    {
                        attachImage.IsLoading = true;
                        ownerICAdapter.Update(position, attachImage);
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void HideLoadingImage()
        {
            try
            {
                if (activeDocumentType == GSLDocumentType.TENANCY_AGREEMENT)
                {
                    int position = tenancyAdapter.ItemCount - 1;
                    AttachedImage attachImage = tenancyAdapter.GetItemObject(position);
                    if (attachImage != null && attachImage.ViewType == Constants.VIEW_TYPE_DUMMY_RECORD)
                    {
                        attachImage.IsLoading = false;
                        tenancyAdapter.Update(position, attachImage);
                    }
                    tenancyAgreementItem.SetHintToHidden(true);
                }
                else if (activeDocumentType == GSLDocumentType.OWNER_IC)
                {
                    int position = ownerICAdapter.ItemCount - 1;
                    AttachedImage attachImage = ownerICAdapter.GetItemObject(position);
                    if (attachImage != null && attachImage.ViewType == Constants.VIEW_TYPE_DUMMY_RECORD)
                    {
                        attachImage.IsLoading = false;
                        ownerICAdapter.Update(position, attachImage);
                    }
                    ownerICItem.SetHintToHidden(true);
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        public void ShowError(string message = null)
        {
            try
            {
                if (this.errorMessageSnackBar != null && this.errorMessageSnackBar.IsShown)
                {
                    this.errorMessageSnackBar.Dismiss();
                }

                if (string.IsNullOrEmpty(message))
                {
                    message = Utility.GetLocalizedErrorLabel("defaultErrorMessage");
                }

                this.errorMessageSnackBar = Snackbar.Make(rootView, message, Snackbar.LengthIndefinite)
                .SetAction(Utility.GetLocalizedCommonLabel("close"), delegate { this.errorMessageSnackBar.Dismiss(); }
                );
                View v = this.errorMessageSnackBar.View;
                TextView tv = (TextView)v.FindViewById<TextView>(Resource.Id.snackbar_text);
                tv.SetMaxLines(5);

                this.errorMessageSnackBar.Show();
                this.SetIsClicked(false);
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private void ShowOwnerICToolTip()
        {
            string base64Image = TooltipImageDirectEntity.GetImageBase64(TooltipImageDirectEntity.IMAGE_CATEGORY.IC_SAMPLE);

            if (!base64Image.IsNullOrEmpty())
            {
                var imageCache = ImageUtils.Base64ToBitmap(base64Image);
                MyTNBAppToolTipBuilder infoLabelWhoIsRegistered = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                .SetHeaderImageBitmap(imageCache)
                .SetTitle(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.UPLOAD_IC_TOOLTIP_TITLE))
                .SetMessage(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.UPLOAD_IC_TOOLTIP_MSG))
                .SetCTALabel(Utility.GetLocalizedCommonLabel(LanguageConstants.Common.GOT_IT))
                .SetCTAaction(() => { this.SetIsClicked(false); })
                .Build();
                infoLabelWhoIsRegistered.Show();
            }
            else
            {
                MyTNBAppToolTipBuilder infoLabelWhoIsRegistered = MyTNBAppToolTipBuilder.Create(this, MyTNBAppToolTipBuilder.ToolTipType.IMAGE_HEADER)
                .SetHeaderImage(Resource.Drawable.icSample)
                .SetTitle(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.UPLOAD_IC_TOOLTIP_TITLE))
                .SetMessage(Utility.GetLocalizedLabel(LanguageConstants.SUBMIT_ENQUIRY, LanguageConstants.SubmitEnquiry.UPLOAD_IC_TOOLTIP_MSG))
                .SetCTALabel(Utility.GetLocalizedCommonLabel(LanguageConstants.Common.GOT_IT))
                .SetCTAaction(() => { this.SetIsClicked(false); })
                .Build();
                infoLabelWhoIsRegistered.Show();
            }
        }

        private void CheckRequiredFields()
        {
            UpdateButtonState(this.presenter.CheckRequiredFields());
        }

        public void UpdateButtonState(bool isEnabled)
        {
            gslStepThreebtnNext.Enabled = isEnabled;
            gslStepThreebtnNext.Background = ContextCompat.GetDrawable(this, isEnabled ? Resource.Drawable.green_button_background :
                Resource.Drawable.silver_chalice_button_background);
        }

        [OnClick(Resource.Id.gslStepThreebtnNext)]
        public void ButtonNextOnClick(object sender, EventArgs eventArgs)
        {
            if (!this.GetIsClicked())
            {
                this.SetIsClicked(true);

                if (this.presenter.CheckRequiredFields())
                {
                    OnShowGSLRebateStepFourActivity();
                }
                else
                {
                    this.SetIsClicked(false);
                }
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

        private void OnShowGSLRebateStepFourActivity()
        {
            this.SetIsClicked(true);
            Intent stepFourActivity = new Intent(this, typeof(GSLRebateStepFourActivity));
            stepFourActivity.PutExtra(GSLRebateConstants.REBATE_MODEL, JsonConvert.SerializeObject(this.presenter.GetGSLRebateModel()));
            StartActivity(stepFourActivity);
        }
    }
}
