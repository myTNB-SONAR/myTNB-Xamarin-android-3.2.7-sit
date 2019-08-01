using System;
using System.Collections.Generic;
using myTNB.Model;

namespace myTNB
{
    public sealed class OCRReadingCache
    {
        private static readonly Lazy<OCRReadingCache> lazy = new Lazy<OCRReadingCache>(() => new OCRReadingCache());
        public static OCRReadingCache Instance { get { return lazy.Value; } }

        private List<GetOCRReadingModel> OCRReadingList = new List<GetOCRReadingModel>();

        public void AddOCRReading(GetOCRReadingResponseModel response)
        {
            if (OCRReadingList == null)
            {
                OCRReadingList = new List<GetOCRReadingModel>();
            }
            if (response != null && response.d != null && response.d.data != null)
            {
                GetOCRReadingModel data = response.d.data;
                data.IsSuccess = response.d.IsSuccess;
                OCRReadingList.Add(data);
            }
        }

        public void ClearReadings()
        {
            OCRReadingList.Clear();
        }

        public List<GetOCRReadingModel> GetOCRReadings()
        {
            if (OCRReadingList != null)
            {
                return OCRReadingList;
            }
            return new List<GetOCRReadingModel>();
        }
    }
}