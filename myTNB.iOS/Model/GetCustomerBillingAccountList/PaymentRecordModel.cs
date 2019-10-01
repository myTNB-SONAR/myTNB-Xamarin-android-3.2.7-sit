using System.Globalization;

namespace myTNB.Model
{
    public class PaymentRecordModel : CustomerAccountRecordModel
    {
        public MandatoryChargesModel MandatoryCharges { set; get; } = new MandatoryChargesModel();
        public double MinimumAmount
        {
            get
            {
                return MandatoryCharges.TotalAmount > 0 ? MandatoryCharges.TotalAmount : TNBGlobal.PaymentMinAmnt;
            }
        }
        public bool HasMandatory
        {
            get
            {
                return MandatoryCharges != null && MandatoryCharges.TotalAmount > 0;
            }
        }
        public bool IsValidAmount
        {
            get
            {
                return Amount >= MinimumAmount;
            }
        }
        public string InlineValidationMessage
        {
            get
            {
                string inlineErrorMessage = LanguageUtility.GetErrorI18NValue("minimumPayAmount");
                if (HasMandatory)
                {
                    inlineErrorMessage = string.Format(LanguageUtility.GetErrorI18NValue("minimumMandatoryPayment")
                        , string.Format("{0} {1}", TNBGlobal.UNIT_CURRENCY, MandatoryCharges.TotalAmount.ToString("N2", CultureInfo.InvariantCulture)));
                }
                return inlineErrorMessage;
            }
        }
    }
}