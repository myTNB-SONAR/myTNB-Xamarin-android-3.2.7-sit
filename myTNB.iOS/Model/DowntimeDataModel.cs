using System;
using myTNB.Enums;
using Newtonsoft.Json;

namespace myTNB.Model
{
    public class DowntimeDataModel
    {
        public string System { get; set; }
        public string IsDown { get; set; }
        public string DowntimeMessage { get; set; }
        public string DowntimeTextMessage { get; set; }

        [JsonIgnore]
        public bool IsAvailable
        {
            get
            {
                if (SystemType == SystemEnum.BCRM)
                {
                    return true;
                }
                if (SystemType == SystemEnum.PaymentCreditCard)
                {
                    return true;
                }
                if (SystemType == SystemEnum.PaymentFPX)
                {
                    return true;
                }

                bool res = true;
                if (!string.IsNullOrEmpty(IsDown))
                {
                    bool parsed = default(bool);
                    if(bool.TryParse(IsDown, out parsed))
                    {
                        res = !parsed;
                    }
                }
                return res;
            }
        }

        [JsonIgnore]
        public SystemEnum SystemType
        {
            get
            {
                SystemEnum sysType = default(SystemEnum);

                if(!string.IsNullOrWhiteSpace(System))
                {
                    var sysStr = System.ToUpper();

                    if (string.Compare(sysStr, TNBGlobal.SystemCodes.SSP.ToUpper()) == 0)
                    {
                        sysType = SystemEnum.SSP;
                    }
                    else if (string.Compare(sysStr, TNBGlobal.SystemCodes.BCRM.ToUpper()) == 0)
                    {
                        sysType = SystemEnum.BCRM;
                    }
                    else if (string.Compare(sysStr, TNBGlobal.SystemCodes.SmartMeter.ToUpper()) == 0)
                    {
                        sysType = SystemEnum.SmartMeter;
                    }
                    else if (string.Compare(sysStr, TNBGlobal.SystemCodes.PaymentCC.ToUpper()) == 0)
                    {
                        sysType = SystemEnum.PaymentCreditCard;
                    }
                    else if (string.Compare(sysStr, TNBGlobal.SystemCodes.PaymentFPX.ToUpper()) == 0)
                    {
                        sysType = SystemEnum.PaymentFPX;
                    }

                }

                return sysType;
            }
        }
    }
}
