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
using Java.Util;
using Java.Text;

namespace myTNB_Android.Src.FeedbackDetails.MVP
{
    public class FeedbackDetailsOthersPresenter : FeedbackDetailsContract.Others.IUserActionsListener
    {
        FeedbackDetailsContract.Others.IView mView;
        SubmittedFeedbackDetails feedbackDetails;

        SimpleDateFormat simpleDateTimeParser = new SimpleDateFormat("dd/MM/yyyy HH:mm:ss");
        SimpleDateFormat simpleDateTimeFormat = new SimpleDateFormat("dd MMM yyyy h:mm a");

        public FeedbackDetailsOthersPresenter(FeedbackDetailsContract.Others.IView mView , SubmittedFeedbackDetails feedbackDetails)
        {
            this.mView = mView;
            this.mView.SetPresenter(this);
            this.feedbackDetails = feedbackDetails;
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

            this.mView.ShowInputData(feedbackDetails.ServiceReqNo, feedbackDetails.StatusDesc, feedbackDetails.StatusCode, dateTime, feedbackDetails.FeedbackTypeName , feedbackDetails.FeedbackMessage);

            this.mView.ShowImages(attachImageList);
        }
    }
}