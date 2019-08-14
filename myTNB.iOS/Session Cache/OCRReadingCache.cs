using System;
using System.Collections.Generic;
using myTNB.Model;

namespace myTNB
{
    public sealed class OCRReadingCache
    {
        private static readonly Lazy<OCRReadingCache> lazy = new Lazy<OCRReadingCache>(() => new OCRReadingCache());
        public static OCRReadingCache Instance { get { return lazy.Value; } }

        private static List<OCRReadingModel> OCRReadingList = new List<OCRReadingModel>();

        public static void AddOCRReading(GetOCRReadingResponseModel response)
        {
            if (OCRReadingList == null)
            {
                OCRReadingList = new List<OCRReadingModel>();
            }
            if (response != null && response.d != null && response.d.data != null)
            {
                OCRReadingModel data = new OCRReadingModel()
                {
                    RequestReadingUnit = response.d.data.RequestReadingUnit,
                    ImageId = response.d.data.ImageId,
                    OCRUnit = response.d.data.OCRUnit,
                    OCRValue = response.d.data.OCRValue,
                    IsSuccess = response.d.IsSuccess && !string.IsNullOrEmpty(response.d.data.OCRUnit)
                        && !string.IsNullOrEmpty(response.d.data.OCRValue),
                    Message = response?.d?.ErrorMessage ?? string.Empty
                };
                OCRReadingList.Add(data);
            }
        }

        public static void ClearReadings()
        {
            OCRReadingList.Clear();
        }

        public static List<OCRReadingModel> GetOCRReadings()
        {
            if (OCRReadingList != null)
            {
                return OCRReadingList;
            }
            return new List<OCRReadingModel>();
        }
    }
}