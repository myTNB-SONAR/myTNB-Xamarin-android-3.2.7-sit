using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.Enquiry.GSL.MVP
{
    public class GSLRebateStepThreePresenter : GSLRebateStepThreeContract.IUserActionsListener
    {
        private ISharedPreferences mSharedPref;
        private readonly GSLRebateStepThreeContract.IView view;

        private GSLRebateModel rebateModel;

        public GSLRebateStepThreePresenter(GSLRebateStepThreeContract.IView view, ISharedPreferences sharedPreferences)
        {
            this.view = view;
            this.view?.SetPresenter(this);
            this.mSharedPref = sharedPreferences;
        }

        public void OnInitialize()
        {
            OnInit();
            this.view?.UpdateButtonState(false);
            this.view?.SetUpViews();
        }

        private void OnInit()
        {
            this.rebateModel = new GSLRebateModel
            {
                Documents = new GSLRebateDocumentModel(),
            };
        }

        public void Start() { }

        public void SetRebateModel(GSLRebateModel model)
        {
            this.rebateModel = model;
        }

        public GSLRebateModel GetGSLRebateModel()
        {
            return rebateModel;
        }

        public bool CheckRequiredFields()
        {
            try
            {
                return this.rebateModel.Documents.TenancyAgreement.IsValid() && this.rebateModel.Documents.OwnerIC.IsValid();
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
                return false;
            }
        }

        public void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            try
            {
                if (requestCode == Constants.REQUEST_ATTACHED_CAMERA_IMAGE)
                {
                    if (resultCode == Result.Ok)
                    {
                        var counter = UserSessions.GetUploadFileNameCounter(mSharedPref);
                        string fileName = string.Format("{0}.jpeg", this.view.GetImageName(counter));
                        UserSessions.SetUploadFileNameCounter(mSharedPref, counter + 1);
                        string tempImagePath = this.view.GetTemporaryImageFilePath(FileUtils.TEMP_IMAGE_FOLDER, string.Format("{0}.jpeg", "temporaryImage"));

                        OnSaveCameraImage(tempImagePath, fileName);

                        GC.Collect();
                    }
                }
                else if (requestCode == Constants.RUNTIME_PERMISSION_GALLERY_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {
                        Android.Net.Uri selectedImage = data.Data;
                        var counter = UserSessions.GetUploadFileNameCounter(mSharedPref);
                        string fileName = string.Format("{0}.jpeg", this.view.GetImageName(counter));
                        UserSessions.SetUploadFileNameCounter(mSharedPref, counter + 1);
                        OnSaveGalleryImage(selectedImage, fileName);
                        GC.Collect();
                    }
                }
                else if (requestCode == Constants.RUNTIME_PERMISSION_GALLERY_PDF_REQUEST_CODE)
                {
                    if (resultCode == Result.Ok)
                    {
                        Android.Net.Uri selectedImage = data.Data;
                        var counter = UserSessions.GetUploadFileNameCounter(mSharedPref);
                        string fileName = string.Format("{0}.pdf", this.view.GetImageName(counter));
                        UserSessions.SetUploadFileNameCounter(mSharedPref, counter + 1);

                        string fileNameIfonlyLocal = fileName;  // fallback filename
                        string contentResolverName = this.view.GetFilename(selectedImage); //actual filename

                        if (contentResolverName != null)
                            if (contentResolverName.Contains("pdf"))
                            {
                                fileNameIfonlyLocal = contentResolverName;
                            }

                        // reacreate from uri path
                        string copiedpath = this.view.CopyPDFGetFilePath(selectedImage, fileNameIfonlyLocal);

                        if (!string.IsNullOrEmpty(copiedpath) && copiedpath.ToLower().Contains(".pdf") && fileNameIfonlyLocal.ToLower().Contains(".pdf"))
                        {
                            OnSaveGalleryPDF(copiedpath, fileNameIfonlyLocal);
                        }
                        else
                        {
                            string filepath = selectedImage.LastPathSegment;

                            if (!filepath.ToLower().Contains(".pdf"))
                            {
                                //reselect path if from uri is not valid
                                string absolutePath = this.view.GetActualPath(selectedImage);

                                if (!absolutePath.ToLower().Contains(".pdf"))
                                {
                                    this.view.ShowError();
                                }
                                else
                                {
                                    OnSaveGalleryPDF(absolutePath, fileName);
                                }
                            }
                            else
                            {
                                OnSaveGalleryPDF(filepath, fileName);
                            }
                        }
                        GC.Collect();
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.LoggingNonFatalError(ex);
            }
        }

        private async void OnSaveCameraImage(string tempImagePath, string fileName)
        {
            this.view.UpdateButtonState(false);
            this.view.ShowLoadingImage();
            string resultFilePath = await this.view.SaveCameraImage(tempImagePath, fileName);
            this.view.UpdateAdapter(resultFilePath, fileName);
            this.view.HideLoadingImage();
        }

        private async void OnSaveGalleryImage(Android.Net.Uri selectedImage, string fileName)
        {
            this.view.UpdateButtonState(false);
            this.view.ShowLoadingImage();
            string resultFilePath = await this.view.SaveGalleryImage(selectedImage, FileUtils.TEMP_IMAGE_FOLDER, fileName);
            this.view.UpdateAdapter(resultFilePath, fileName);
            this.view.HideLoadingImage();
        }

        private void OnSaveGalleryPDF(string filePath, string fileName)
        {
            this.view.UpdateButtonState(false);
            this.view.ShowLoadingImage();

            string result = filePath;

            int cutFiletype = result.IndexOf(':');
            if (cutFiletype != -1)
            {
                filePath = result.Substring(cutFiletype + 1);
            }

            int cut = result.LastIndexOf('/');
            if (cut != -1)
            {
                result = result.Substring(cut + 1);
            }

            string actualPdfName = result;

            this.view.UpdateAdapter(filePath, actualPdfName, fileName);
            this.view.HideLoadingImage();
        }

        public void SetTenancyDocument(string value)
        {
            this.rebateModel.Documents.TenancyAgreement = value;
        }

        public void SetOwnerIC(string value)
        {
            this.rebateModel.Documents.OwnerIC = value;
        }
    }
}
