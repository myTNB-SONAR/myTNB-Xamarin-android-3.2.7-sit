using Android.Text;
using Android.Widget;
using Google.Android.Material.TextField;
using Java.Lang;
using Java.Util.Regex;
using static Android.Views.View;
namespace myTNB.AndroidApp.Src.Utils
{
    public class ResetPasswordNewChangeListener : Java.Lang.Object, ITextWatcher
    {

        private string mPreviousValue;
        private int mCursorPosition;
        private bool mRestoringPreviousValueFlag;
        private int mDigitsAfterZero;
        private EditText mEditText;
        private TextInputLayout _txtLayout;
        private string _hint;

        public ResetPasswordNewChangeListener(EditText editText, TextInputLayout txt)
        {
    
            mEditText = editText;
            mPreviousValue = "";
            mRestoringPreviousValueFlag = false;
            _txtLayout = txt;
            _hint = _txtLayout.Hint;
            mEditText.FocusChange += EditTextOnFocusChanged;
        }


        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            //if (!mRestoringPreviousValueFlag)
            //{

            //    if (!isValid(s.ToString()))
            //    {
            //        mRestoringPreviousValueFlag = true;
            //        restorePreviousValue();
            //    }

            //}
            //else
            //{
            //    mRestoringPreviousValueFlag = false;
            //}

        }


        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
            //if (!mRestoringPreviousValueFlag)
            //{
            //    mPreviousValue = s.ToString();
            //    mCursorPosition = mEditText.SelectionStart;
            //}
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
                    _txtLayout.Hint = Utility.GetLocalizedLabel("ResetPassword", "newPasswordNoEnter").ToUpper();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(_hint))
                {
                    _txtLayout.Hint = "";
                    mEditText.Hint = Utility.GetLocalizedLabel("ResetPassword", "newPassword").ToUpper();
                }
            }
        }

        private void EditTextOnFocusChanged(object sender, FocusChangeEventArgs focusChangedEventArgs)
        {
            if (focusChangedEventArgs.HasFocus)
            {
             
                if (!string.IsNullOrEmpty(_hint) && string.IsNullOrEmpty(mEditText.Text.ToString()))
                {
                    _txtLayout.Hint = Utility.GetLocalizedLabel("ResetPassword", "newPasswordNoEnter").ToUpper();
                    mEditText.Hint = "";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(_hint) && string.IsNullOrEmpty(mEditText.Text.ToString()))
                {
                    _txtLayout.Hint = "";
                    mEditText.Hint = Utility.GetLocalizedLabel("ResetPassword", "newPassword");
                }
            }
        }

    }
}