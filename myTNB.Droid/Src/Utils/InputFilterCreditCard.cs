using Android.Text;
using Android.Widget;
using Java.Lang;

namespace myTNB.Android.Src.Utils
{

    public class InputFilterCreditCard : Java.Lang.Object, ITextWatcher
    {
        private readonly EditText _editText;
        private static char space = ' ';

        public InputFilterCreditCard(EditText edt)
        {
            _editText = edt;
        }

        public void AfterTextChanged(IEditable s)
        {
            int pos = 0;
            while (true)
            {
                if (pos >= s.Length()) break;
                if (space == s.CharAt(pos) && (((pos + 1) % 5) != 0 || pos + 1 == s.Length()))
                {
                    s.Delete(pos, pos + 1);
                }
                else
                {
                    pos++;
                }
            }

            // Insert char where needed.
            pos = 4;
            while (true)
            {
                if (pos >= s.Length()) break;
                char c = s.CharAt(pos);
                // Only if its a digit where there should be a space we insert a space
                if ("0123456789".IndexOf(c) >= 0)
                {
                    s.Insert(pos, "" + space);
                }
                pos += 5;
            }

        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {

        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {

        }

    }
}