using Android.Text;
using Android.Widget;
using Java.Lang;

namespace myTNB_Android.Src.Utils
{
    public class InputFilterExpDate : Java.Lang.Object, IInputFilter
    {
        private readonly EditText _editText;
        private ICharSequence empty = new Java.Lang.String("");
        private ICharSequence slash = new Java.Lang.String("/");

        public InputFilterExpDate(EditText edt)
        {
            _editText = edt;
        }

        public ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
        {
            if (source.Length() > 0)
            {
                if (!Character.IsDigit(source.CharAt(0)))
                {
                    return empty;
                }
                else
                {
                    int currentDigit = Integer.ParseInt(source.ToString());

                    if (dstart == 0)
                    {
                        if (currentDigit > 1)
                        {
                            return empty;
                        }
                        return source;
                    }
                    else if (dstart == 1)
                    {

                        int firstDigit = Integer.ParseInt(_editText.Text.ToString().Substring(0, 1));

                        if (firstDigit == 0 && currentDigit == 0)
                        {
                            return empty;
                        }
                        else if (firstDigit == 1 && currentDigit > 2)
                        {
                            return empty;
                        }

                        string temp = source.ToString().Insert(dend, value: "/");
                        return new Java.Lang.String(temp);
                    }
                    else if (dstart == 2)
                    {
                        if (currentDigit < 1)
                        {
                            return empty;
                        }
                        int edtLength = _editText.Text.ToString().Length;
                        if (edtLength >= dstart)
                        {
                            dstart = edtLength - dstart;
                        }
                        else
                        {
                            if (dstart > 0)
                            {
                                dstart = dstart - 1;
                            }
                            else if (dstart < 0)
                            {
                                dstart = 0;
                            }

                        }
                        string temp = source.ToString().Insert(dstart, value: "/");
                        return new Java.Lang.String(temp);
                    }
                    else if (dstart == 3)
                    {
                        if (currentDigit < 1)
                        {
                            return empty;
                        }
                        return source;
                    }
                }
            }
            return null;
        }
    }
}