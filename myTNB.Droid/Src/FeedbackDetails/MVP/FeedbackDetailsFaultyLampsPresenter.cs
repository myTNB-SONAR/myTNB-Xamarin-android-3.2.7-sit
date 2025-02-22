﻿using Android.Content;
using Android.Graphics;
using Android.Text;
using Java.Text;
using Java.Util;
using myTNB.AndroidApp.Src.Base.Models;
using myTNB.AndroidApp.Src.Utils;
using System;
using System.Collections.Generic;
using static myTNB.AndroidApp.Src.Base.Models.SubmittedFeedbackDetails;

namespace myTNB.AndroidApp.Src.FeedbackDetails.MVP
{
    public class FeedbackDetailsFaultyLampsPresenter : FeedbackDetailsContract.FaultyLamps.IUserActionsListener
    {

        FeedbackDetailsContract.FaultyLamps.IView mView;
        SubmittedFeedbackDetails feedbackDetails;


        SimpleDateFormat simpleDateTimeParser = new SimpleDateFormat("dd/MM/yyyy HH:mm:ss");
        SimpleDateFormat simpleDateTimeFormat = new SimpleDateFormat("dd MMM yyyy h:mm a");
        private Context context;

        public FeedbackDetailsFaultyLampsPresenter(FeedbackDetailsContract.FaultyLamps.IView mView, SubmittedFeedbackDetails submittedFeedback, Context mContext)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
            this.feedbackDetails = submittedFeedback;
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


                this.mView.ShowInputData(feedbackDetails.ServiceReqNo, feedbackDetails.StatusDesc, feedbackDetails.StatusCode, dateTime, feedbackDetails.StateName, feedbackDetails.Location, feedbackDetails.PoleNum, feedbackDetails.FeedbackMessage);

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