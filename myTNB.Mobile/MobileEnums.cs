namespace myTNB.Mobile
{
    public struct MobileEnums
    {
        public enum DBRTypeEnum
        {
            EBill,
            EBillWithCTA,
            Email,
            EmailWithCTA,
            Paper,
            WhatsApp,
            None
        }

        public enum RenderingMethodEnum
        {
            EBill,
            EBill_Email,
            EBill_Email_Paper,
            EBill_Paper,
            Email,
            Email_Paper,
            Paper,
            None
        }

        public enum ServiceEnum
        {
            None = 0,
            SELFMETERREADING,
            SUBMITFEEDBACK,
            PAYBILL,
            VIEWBILL,
            APPLICATIONSTATUS,
            ENERGYBUDGET,
            MYHOME,
            CONNECTMYPREMISE,
            HMO,
            VIEWMORE,
            VIEWLESS
        }
    }
}