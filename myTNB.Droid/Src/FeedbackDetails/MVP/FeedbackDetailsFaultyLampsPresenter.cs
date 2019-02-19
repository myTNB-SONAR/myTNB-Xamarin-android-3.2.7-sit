using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using myTNB_Android.Src.Base.Models;
using static myTNB_Android.Src.Base.Models.SubmittedFeedbackDetails;
using Android.Text;
using Android.Graphics;
using myTNB_Android.Src.Utils;
using Java.Text;
using Java.Util;

namespace myTNB_Android.Src.FeedbackDetails.MVP
{
    public class FeedbackDetailsFaultyLampsPresenter : FeedbackDetailsContract.FaultyLamps.IUserActionsListener
    {

        FeedbackDetailsContract.FaultyLamps.IView mView;
        SubmittedFeedbackDetails feedbackDetails;


        SimpleDateFormat simpleDateTimeParser = new SimpleDateFormat("dd/MM/yyyy HH:mm:ss");
        SimpleDateFormat simpleDateTimeFormat = new SimpleDateFormat("dd MMM yyyy h:mm a");

        public FeedbackDetailsFaultyLampsPresenter(FeedbackDetailsContract.FaultyLamps.IView mView , SubmittedFeedbackDetails submittedFeedback)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
            this.feedbackDetails = submittedFeedback;
        }

  

        public async void Start()
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
            }
  

            this.mView.ShowInputData(feedbackDetails.ServiceReqNo, feedbackDetails.StatusDesc, feedbackDetails.StatusCode, dateTime , feedbackDetails.StateName , feedbackDetails.Location, feedbackDetails.PoleNum, feedbackDetails.FeedbackMessage);

            this.mView.ShowImages(attachImageList);
        }
    }
}