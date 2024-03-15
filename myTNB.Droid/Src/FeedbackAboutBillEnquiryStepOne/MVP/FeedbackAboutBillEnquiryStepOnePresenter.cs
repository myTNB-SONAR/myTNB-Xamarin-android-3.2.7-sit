using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Text;
using myTNB.AndroidApp.Src.Utils;


namespace myTNB.AndroidApp.Src.FeedbackAboutBillEnquiryStepOne.MVP
{
    public class FeedbackAboutBillEnquiryStepOnePresenter : FeedbackAboutBillEnquiryStepOneContract.IUserActionsListener
    {

        FeedbackAboutBillEnquiryStepOneContract.IView mView;

        private int countUserPick = 1; 


        public FeedbackAboutBillEnquiryStepOnePresenter(FeedbackAboutBillEnquiryStepOneContract.IView mView)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
        }

       public  void OnAboutBillEnquiry()
        {
            this.mView.ShowAboutBillEnquiry();
        }
        public void OnSelectCategory()
        {
            try
            {
                    this.mView.ShowSelectCategory();
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }

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

        public void CheckRequiredFields(string feedback)
        {
            try
            {
                this.mView.ClearErrors();
                if (!TextUtils.IsEmpty(feedback.Trim()))
                {
                    this.mView.EnableSubmitButton();
                }
                else
                {
                  //  this.mView.ShowEmptyFeedbackError();
                    this.mView.DisableSubmitButton();
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }



        public void OnAttachPhotoCamera()
        {
            this.mView.ShowCamera();
        }

        public void OnAttachPhotoGallery()
        {
            this.mView.ShowGallery();
        }

        public void OnAttachPDF()
        {
            this.mView.ShowPDF();
        }

        private async void OnSaveCameraImage(string tempImagePath, string fileName)
        {
            this.mView.DisableSubmitButton();
            this.mView.ShowLoadingImage();
            string resultFilePath = await this.mView.SaveCameraImage(tempImagePath, fileName);
            this.mView.UpdateAdapter(resultFilePath, fileName);
            this.mView.HideLoadingImage();
            this.mView.EnableSubmitButton();
        }


        private async void OnSaveGalleryImage(Android.Net.Uri selectedImage, string fileName)
        {
            this.mView.DisableSubmitButton();
            this.mView.ShowLoadingImage();
            string resultFilePath = await this.mView.SaveGalleryImage(selectedImage, FileUtils.TEMP_IMAGE_FOLDER, fileName);
          

            this.mView.UpdateAdapter(resultFilePath, fileName);
            this.mView.HideLoadingImage();
            this.mView.EnableSubmitButton();
        }

        private async void OnSaveGalleryPDF(string filePath, string fileName =null)
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
            this.mView.EnableSubmitButton();
        }

    
        public string removeRawtag(string data)
        { string removedString;

            removedString = data.Substring(4);

            return removedString;
        }

        public void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == Constants.REQUEST_ATTACHED_CAMERA_IMAGE)
            {
                if (resultCode == Result.Ok)
                {
                    // TODO : ADD PROGRESS
                    string fileName = string.Format("{0}.jpeg",   this.mView.GetImageName(countUserPick));
                    countUserPick++; //increament picked file number
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
                       // Guid.NewGuid()
                    Android.Net.Uri selectedImage = data.Data;
                    string fileName = string.Format("{0}.pdf", this.mView.GetImageName(countUserPick));
                    countUserPick++;

                    string fileNameIfonlyLocal = fileName;  // temporary file name

                    string tempFilename = this.mView.getFilename(selectedImage); // get filename from content resolver

                    if (tempFilename!=null)
                        if (tempFilename.Contains("pdf"))
                        {
                            fileNameIfonlyLocal = tempFilename;
                        }

                    // reacreate from uri path
                    string copiedpath = this.mView.copyPDFGetFilePath(selectedImage, fileNameIfonlyLocal);


                    //from 
                    if (!string.IsNullOrEmpty(copiedpath)&& copiedpath.ToLower().Contains(".pdf") && fileNameIfonlyLocal.ToLower().Contains(".pdf"))
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








    }
}