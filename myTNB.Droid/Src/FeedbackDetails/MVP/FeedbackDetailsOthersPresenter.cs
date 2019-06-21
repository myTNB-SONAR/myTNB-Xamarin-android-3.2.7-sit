﻿using Android.Graphics;
using Android.Text;
using Java.Text;
using Java.Util;
using myTNB_Android.Src.Base.Models;
using myTNB_Android.Src.Utils;
using System;
using System.Collections.Generic;
using static myTNB_Android.Src.Base.Models.SubmittedFeedbackDetails;

namespace myTNB_Android.Src.FeedbackDetails.MVP
{
    public class FeedbackDetailsOthersPresenter : FeedbackDetailsContract.Others.IUserActionsListener
    {
        FeedbackDetailsContract.Others.IView mView;
        SubmittedFeedbackDetails feedbackDetails;

        SimpleDateFormat simpleDateTimeParser = new SimpleDateFormat("dd/MM/yyyy HH:mm:ss");
        SimpleDateFormat simpleDateTimeFormat = new SimpleDateFormat("dd MMM yyyy h:mm a");

        public FeedbackDetailsOthersPresenter(FeedbackDetailsContract.Others.IView mView, SubmittedFeedbackDetails feedbackDetails)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
            this.feedbackDetails = feedbackDetails;
        }



        public async void Start()
        {
            try
            {
                List<AttachedImage> attachImageList = new List<AttachedImage>();
                foreach (ImageResponse image in feedbackDetails.ImageList)
                {
                    if (!TextUtils.IsEmpty(image.ImageHex))
                    {
                        try
                        {


                            Bitmap bitmap = await FileUtils.GetImageFromHexAsync(image.ImageHex, image.FileSize);
                            string filePath = await FileUtils.SaveAsync(bitmap, FileUtils.IMAGE_FOLDER, image.FileName);
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


                Date d = null;
                string dateTime = string.Empty;
                try
                {
                    d = simpleDateTimeParser.Parse(feedbackDetails.DateCreated);
                    dateTime = simpleDateTimeFormat.Format(d);
                }
                catch (Java.Text.ParseException e)
                {
                    dateTime = "NA";
                    Utility.LoggingNonFatalError(e);
                }

                this.mView.ShowInputData(feedbackDetails.ServiceReqNo, feedbackDetails.StatusDesc, feedbackDetails.StatusCode, dateTime, feedbackDetails.FeedbackTypeName, feedbackDetails.FeedbackMessage);

                this.mView.ShowImages(attachImageList);
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);
            }
        }
    }
}