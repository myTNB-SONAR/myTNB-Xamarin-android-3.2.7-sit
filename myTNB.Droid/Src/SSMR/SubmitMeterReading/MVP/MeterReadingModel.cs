using System;
namespace myTNB.Android.Src.SSMR.SubmitMeterReading.MVP
{
    public class MeterReadingModel
    {
        //public string previousMeterReadingValue, currentMeterReadingValue, meterReadingUnit;
        //public bool isValidated;

        //meterReading.MroID = validatedMeter.mroID;
        //            meterReading.RegisterNumber = validatedMeter.registerNumber;
        //            MeterReadingInputLayout readingInput = meterReadingInputLayoutList.Find(meterInput => meterInput.GetMeterId() == validatedMeter.meterId);
        //meterReading.MeterReadingResult = readingInput.GetMeterReadingInput();
        //            meterReading.Channel = "MyTNBAPP";
        //            meterReading.MeterReadingDate = "";
        //            meterReading.MeterReadingTime = "";

        public string mroID { get; set; }
        public string registerNumber { get; set; }

        public string previousMeterReadingValue { get; set; }
        public string currentMeterReadingValue { get; set; }
        public string meterReadingUnit { get; set; }
        public string meterReadingUnitDisplay { get; set; }
        public bool isValidated { get; set; }
    }
}
