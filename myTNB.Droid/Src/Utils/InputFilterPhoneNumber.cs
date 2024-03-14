using Android.Text;
using Java.Lang;

namespace myTNB.Android.Src.Utils
{
    public class InputFilterPhoneNumber : Java.Lang.Object, IInputFilter
    {
        ICharSequence blank = new Java.Lang.String("");
        ICharSequence plus = new Java.Lang.String("+");
        ICharSequence six = new Java.Lang.String("6");
        ICharSequence zero = new Java.Lang.String("0");

        public ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
        {
            if (start == 0 && end == 0 && dend <= 3)
            {
                if (dend == 0)
                {
                    return blank;
                }
                if (dstart == 0)
                {
                    return plus;
                }
                if (dstart == 1)
                {
                    return six;
                }
                if (dstart == 2)
                {
                    return zero;
                }
            }
            return null;
        }
    }
}