using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using myTNB.Android.Src.Utils;

namespace myTNB.Android.Src.AddAcc_UpdateIdentification_StepThree.MVP
{
    public class AddAccUpdateIdetificationDetailsStepThreePresenter : AddAccUpdateIdetificationDetailsStepThreeContract.IUserActionsListener
    {

        AddAccUpdateIdetificationDetailsStepThreeContract.IView mView;
        private ISharedPreferences mSharedPref;
        private int countUserPick = 1;
        public AddAccUpdateIdetificationDetailsStepThreePresenter(AddAccUpdateIdetificationDetailsStepThreeContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }



        public void Start()
        {
            try
            {

            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

        }

        //public void OninfoLabelProofOfConsent()
        //{
        //    this.mView.ShowinfoLabelProofOfConsent();
        //}

        public void OninfoLabelCopyOfIdentification()
        {
            this.mView.ShowinfoLabelCopyOfIdentification();
        }

        public void OnAttachPhotoCamera()
        {
            this.mView.ShowCamera();
        }


        public void OnAttachPhotoGallery()
        {
            this.mView.ShowGallery();
        }

        public void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == Constants.REQUEST_ATTACHED_CAMERA_IMAGE)
            {
                if (resultCode == Result.Ok)
                {
                    // TODO : ADD PROGRESS
                    string fileName = string.Format("{0}.jpeg", this.mView.GetImageName(countUserPick));
                    countUserPick++;
                    string tempImagePath = this.mView.GetTemporaryImageFilePath(FileUtils.TEMP_IMAGE_FOLDER, string.Format("{0}.jpeg", "temporaryImage"));


                    OnSaveCameraImage(tempImagePath, fileName);

                    GC.Collect();
                }
            }
            else if (requestCode == Constants.RUNTIME_PERMISSION_GALLERY_REQUEST_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    Android.Net.Uri selectedImage = data.Data;
                    string fileName = string.Format("{0}.jpeg", this.mView.GetImageName(countUserPick));
                    countUserPick++;
                    OnSaveGalleryImage(selectedImage, fileName);
                    GC.Collect();
                }
            }

            else if (requestCode == Constants.RUNTIME_PERMISSION_GALLERY_PDF_REQUEST_CODE)
            {
                if (resultCode == Result.Ok)
                {
                    Android.Net.Uri selectedImage = data.Data;
                    string fileName = string.Format("{0}.pdf", this.mView.GetImageName(countUserPick));
                    countUserPick++;


                    string fileNameIfonlyLocal = fileName;
                    string tempFileName = selectedImage.Path;

                    //get filename
                    int cut = tempFileName.LastIndexOf('/');
                    if (cut != -1)
                    {
                        fileNameIfonlyLocal = tempFileName.Substring(cut + 1);
                    }

                    // reacreate from uri path
                    string copiedpath = this.mView.copyPDFGetFilePath(selectedImage, fileNameIfonlyLocal);

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
                            string absolutePath = this.mView.getActualPath(selectedImage);

                            if (!absolutePath.ToLower().Contains(".pdf"))
                            {
                                this.mView.ShowError();
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

        private async void OnSaveCameraImage(string tempImagePath, string fileName)
        {
            this.mView.DisableSubmitButton();
            this.mView.ShowLoadingImage();
            string resultFilePath = await this.mView.SaveCameraImage(tempImagePath, fileName);
            this.mView.UpdateAdapter(resultFilePath, fileName);
            this.mView.HideLoadingImage();
            //  this.mView.EnableSubmitButton();
        }

        private async void OnSaveGalleryImage(Android.Net.Uri selectedImage, string fileName)
        {
            this.mView.DisableSubmitButton();
            this.mView.ShowLoadingImage();
            string resultFilePath = await this.mView.SaveGalleryImage(selectedImage, FileUtils.TEMP_IMAGE_FOLDER, fileName);
            this.mView.UpdateAdapter(resultFilePath, fileName);
            this.mView.HideLoadingImage();

        }

        private async void OnSaveGalleryPDF(string filePath, string fileName)
        {
            this.mView.DisableSubmitButton();
            this.mView.ShowLoadingImage();


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

            this.mView.UpdateAdapter(filePath, actualPdfName, actualPdfName);
            this.mView.HideLoadingImage();

        }


        public void OninfoLabelPermise()
        {

            this.mView.ShowinfoLabelPermise();
        }

        public void OnAttachPDF()
        {
            this.mView.ShowPDF();
        }



    }
}