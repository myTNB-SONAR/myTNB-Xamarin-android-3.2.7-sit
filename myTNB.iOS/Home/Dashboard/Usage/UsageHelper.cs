namespace myTNB.Home.Dashboard.Usage
{
    public static class UsageHelper
    {
        public static string GetRMkWhValueStringForEnum(RMkWhEnum rMkWhEnum)
        {
            string valueString = string.Empty;
            switch (rMkWhEnum)
            {
                case RMkWhEnum.RM:
                    valueString = "RM";
                    break;
                case RMkWhEnum.kWh:
                    valueString = "kWh";
                    break;
            }
            return valueString;
        }
    }
}
