﻿using Android.Content;
using Android.Graphics;
using Android.Text;
using Java.Text;
using Java.Util;
using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.Database.Model;
using myTNB.AndroidApp.Src.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using static myTNB.AndroidApp.Src.Base.Models.SubmittedFeedbackDetails;

namespace myTNB.AndroidApp.Src.FeedbackDetails.MVP
{
    public class FeedbackDetailsBillRelatedPresenter : FeedbackDetailsContract.BillRelated.IUserActionsListener
    {

        FeedbackDetailsContract.BillRelated.IView mView;
        CancellationTokenSource cts;
        SubmittedFeedbackDetails feedbackDetails;
        private bool isNewScreen;
        private Context context;

        public FeedbackDetailsBillRelatedPresenter(FeedbackDetailsContract.BillRelated.IView mView, SubmittedFeedbackDetails feedbackDetails , bool isNewScreen, Context mContext)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
            this.feedbackDetails = feedbackDetails;
            this.isNewScreen = isNewScreen;
            this.context = mContext;
        }

        public async void Start()
        {
            try
            {
                this.mView.ShowProgressDialog();

                List<AttachedImage> attachImageList = new List<AttachedImage>();

    
                    foreach (ImageResponse image in feedbackDetails.ImageList)
                    {
                        if (!TextUtils.IsEmpty(image.ImageHex))
                        {
                            try
                            {
                            if (image.FileName.ToLower().Contains("pdf"))
                            {
                                //todo hex to file  --done
                                //store at certain path with its name
                                //set as the attached file

                                try
                                {
                                    byte[] byteOfPdf = FileUtils.StringToByteArray(image.ImageHex);
                                    string filePath = await FileUtils.SaveAsyncPDF(this.context, byteOfPdf, FileUtils.IMAGE_FOLDER, image.FileName);
                                    var attachImage = new AttachedImage()
                                    {
                                        Path = filePath,
                                        Name = image.FileName
                                    };
                                    attachImageList.Add(attachImage);
                                }
                                catch (Exception e)
                                {
                                    Utility.LoggingNonFatalError(e);
                                }


                              
                            }
                            else
                            {
                                Bitmap bitmap = await FileUtils.GetImageFromHexAsync(image.ImageHex, image.FileSize);
                                string filePath = await FileUtils.SaveAsync(this.context, bitmap, FileUtils.IMAGE_FOLDER, image.FileName);
                                var attachImage = new AttachedImage()
                                {
                                    Path = filePath,
                                    Name = image.FileName
                                };
                                attachImageList.Add(attachImage);
                            }

                                
                            }
                            catch (Exception e)
                            {
                                Utility.LoggingNonFatalError(e);
                            }
                        }

                    }
                

              

                string dateTime = string.Empty;
                if (!string.IsNullOrEmpty(feedbackDetails.DateCreated))
                {
                    try
                    {
                        dateTime = feedbackDetails.DateCreated;
                        DateTime dateTimeParse = DateTime.ParseExact(dateTime, "dd'/'MM'/'yyyy HH:mm:ss",
                                CultureInfo.InvariantCulture, DateTimeStyles.None);
                        if (LanguageUtil.GetAppLanguage().ToUpper() == "MS")
                        {
                            CultureInfo currCult = CultureInfo.CreateSpecificCulture("ms-MY");
                            dateTime = dateTimeParse.ToString("dd MMM yyyy, h:mm tt", currCult);
                        }
                        else
                        {
                            CultureInfo currCult = CultureInfo.CreateSpecificCulture("en-US");
                            dateTime = dateTimeParse.ToString("dd MMM yyyy, h:mm tt", currCult);
                        }
                    }
                    catch (System.Exception e)
                    {
                        dateTime = "NA";
                        Utility.LoggingNonFatalError(e);
                    }
                }
                else
                {
                    dateTime = "NA";
                }

                string accountNum = feedbackDetails.AccountNum;
                if (UserEntity.IsCurrentlyActive())
                {
                    CustomerBillingAccount customerBillingAccount = CustomerBillingAccount.FindByAccNum(accountNum);
                    if (customerBillingAccount != null)
                    {
                        // accountNum = string.Format("{0} - {1}", accountNum, customerBillingAccount.AccDesc);  
                        accountNum = string.Format("{0}", Utility.GetLocalizedLabel("SubmitEnquiry", "for") + customerBillingAccount.AccDesc);
                    }
                }else
                {  
                   //handle if acc from outside show only CA number 
                }

                
                this.mView.ShowInputData(feedbackDetails.ServiceReqNo, feedbackDetails.StatusDesc, feedbackDetails.StatusCode, dateTime, accountNum, feedbackDetails.FeedbackMessage , feedbackDetails.FeedbackUpdateDetails , feedbackDetails.ContactName,feedbackDetails.ContactEmailAddress,feedbackDetails.ContactMobileNo , feedbackDetails.RelationshipWithCA , feedbackDetails.RelationshipWithCADesc , feedbackDetails.IsOwner, feedbackDetails.EnquiryName);
                this.mView.ShowImages(attachImageList);

                this.mView.HideProgressDialog();
            }
            catch (Exception e)
            {
                this.mView.HideProgressDialog();
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}