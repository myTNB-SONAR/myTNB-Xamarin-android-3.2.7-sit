using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Database.Model;
using myTNB_Android.Src.Utils;
using static myTNB_Android.Src.Base.Models.SubmittedFeedbackDetails;

namespace myTNB_Android.Src.SubmittedNewEnquiry.MVP
{
    public class SubmittedNewEnquiryPresenter : SubmittedNewEnquiryContract.IUserActionsListener
    {

        SubmittedNewEnquiryContract.IView mView;
        CancellationTokenSource cts;
        SubmittedFeedbackDetails feedbackDetails;
        Context context;

        public SubmittedNewEnquiryPresenter(SubmittedNewEnquiryContract.IView mView, SubmittedFeedbackDetails feedbackDetails, Context mContext)
        {

            this.mView = mView;
            this.mView.SetPresenter(this);
            this.feedbackDetails = feedbackDetails;
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
                            Bitmap bitmap = await FileUtils.GetImageFromHexAsync(image.ImageHex, image.FileSize);
                            string filePath = await FileUtils.SaveAsync(this.context, bitmap, FileUtils.IMAGE_FOLDER, image.FileName);
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
                        accountNum = string.Format("{0} - {1}", accountNum, customerBillingAccount.AccDesc);
                    }
                }

                this.mView.ShowInputData(feedbackDetails.ServiceReqNo, feedbackDetails.StatusDesc, feedbackDetails.StatusCode, dateTime, accountNum, feedbackDetails.FeedbackMessage);
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