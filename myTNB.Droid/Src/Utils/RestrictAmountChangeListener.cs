
using Android.Text;
using Android.Widget;
using Google.Android.Material.TextField;
using Java.Lang;
using Java.Util.Regex;
using static Android.Views.View;
namespace myTNB.Android.Src.Utils
{
    public class RestrictAmountChangeListener : Java.Lang.Object, ITextWatcher
    {
        private string mPreviousValue;
        private int mCursorPosition;
        private bool mRestoringPreviousValueFlag;
        private int mDigitsAfterZero;
        private EditText mEditText;
        private TextInputLayout _txtLayout;
        private string _hint;

        public RestrictAmountChangeListener(EditText editText, TextInputLayout txt, int digitsAfterZero)
        {
            mDigitsAfterZero = digitsAfterZero;
            mEditText = editText;
            mPreviousValue = "";
            mRestoringPreviousValueFlag = false;
            _txtLayout = txt;
            _hint = _txtLayout.Hint;
            mEditText.FocusChange += EditTextOnFocusChanged;
        }


        public void AfterTextChanged(IEditable s)
        {
            if (mEditText == null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(s.ToString()))
            {

                if (!string.IsNullOrEmpty(_hint))
                {
                    _txtLayout.Hint = Utility.GetLocalizedLabel("SelectBills", "iamPaying");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(_hint))
                {
                    _txtLayout.Hint = "";
                    mEditText.Hint = Utility.GetLocalizedLabel("SelectBills", "enterAmount");
                }
            }
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
            if (!mRestoringPreviousValueFlag)
            {
                mPreviousValue = s.ToString();
                mCursorPosition = mEditText.SelectionStart;
            }
        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            if (!mRestoringPreviousValueFlag)
            {

                if (!isValid(s.ToString()))
                {
                    mRestoringPreviousValueFlag = true;
                    restorePreviousValue();
                }

            }
            else
            {
                mRestoringPreviousValueFlag = false;
            }

        }

        private void restorePreviousValue()
        {
            mEditText.Text = mPreviousValue;
            mEditText.SetSelection(mCursorPosition);
        }

        private bool isValid(string s)
        {
            Pattern patternWithDot = Pattern.Compile("^[\\-\\+\\s]*[0-9]*((\\.[0-9]{0," + mDigitsAfterZero + "})?)||(\\.)?");
            Pattern patternWithComma = Pattern.Compile("^[\\-\\+\\s]*[0-9]*((,[0-9]{0," + mDigitsAfterZero + "})?)||(,)?");

            Matcher matcherDot = patternWithDot.Matcher(s);
            Matcher matcherComa = patternWithComma.Matcher(s);

            return matcherDot.Matches() || matcherComa.Matches();
        }

        private void EditTextOnFocusChanged(object sender, FocusChangeEventArgs focusChangedEventArgs)
        {
            if (focusChangedEventArgs.HasFocus)
            {
                //if (_leftDrawable != null)
                //{
                //    _editText.SetCompoundDrawables(_leftDrawable, null, _rightDrawable, null);
                //}
                if (!string.IsNullOrEmpty(_hint) && string.IsNullOrEmpty(mEditText.Text.ToString()))
                {
                    _txtLayout.Hint = Utility.GetLocalizedLabel("SelectBills", "iamPaying");
                    mEditText.Hint = "";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(_hint) && string.IsNullOrEmpty(mEditText.Text.ToString()))
                {
                    _txtLayout.Hint = "";
                    mEditText.Hint = Utility.GetLocalizedLabel("SelectBills", "enterAmount");
                }
            }
        }
    }
}
