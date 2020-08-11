using Android.Graphics.Drawables;
using Android.Text;
using Android.Widget;
using Google.Android.Material.TextField;
using Java.Lang;
using System;
using static Android.Views.View;

namespace myTNB_Android.Src.Utils
{

    public class InputFilterFormField : Java.Lang.Object, ITextWatcher
    {
        private readonly TextInputLayout _txtLayout;
        private readonly EditText _editText;
        private Drawable _leftDrawable;
        private Drawable _rightDrawable;
        private string _hint;
        private static char space = ' ';

        const int DRAWABLE_LEFT = 0;
        const int DRAWABLE_TOP = 1;
        const int DRAWABLE_RIGHT = 2;
        const int DRAWABLE_BOTTOM = 3;

        public InputFilterFormField(EditText edt, TextInputLayout txt)
        {
            _editText = edt;
            _txtLayout = txt;
            Drawable[] drawables = _editText.GetCompoundDrawables();
            _leftDrawable = drawables[DRAWABLE_LEFT];
            _rightDrawable = drawables[DRAWABLE_RIGHT];
            _hint = _txtLayout.Hint;
            _editText.FocusChange += EditTextOnFocusChanged;
        }

        public InputFilterFormField(EditText edt, TextInputLayout txt, EventHandler<TextChangedEventArgs> listener)
        {
            _editText = edt;
            _txtLayout = txt;
            Drawable[] drawables = _editText.GetCompoundDrawables();
            _leftDrawable = drawables[0];
            _rightDrawable = drawables[DRAWABLE_RIGHT];
            _hint = _txtLayout.Hint;
            _editText.FocusChange += EditTextOnFocusChanged;
            _editText.TextChanged += new EventHandler<TextChangedEventArgs>(listener);
        }

        public void AfterTextChanged(IEditable s)
        {
            if (_editText == null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(s.ToString()))
            {
                _editText.SetCompoundDrawables(null, null, _rightDrawable, null);
                if (!string.IsNullOrEmpty(_hint))
                {
                    _txtLayout.Hint = _hint.ToUpper();
                }
            }
            else
            {
                _editText.SetCompoundDrawables(null , null, _rightDrawable, null);
                if (!string.IsNullOrEmpty(_hint))
                {
                    _txtLayout.Hint = "";
                    _editText.Hint = _hint;
                }
            }

        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {

        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {

        }

        private void EditTextOnFocusChanged(object sender, FocusChangeEventArgs focusChangedEventArgs)
        {
            if (focusChangedEventArgs.HasFocus)
            {
                //if (_leftDrawable != null)
                //{
                //    _editText.SetCompoundDrawables(_leftDrawable, null, _rightDrawable, null);
                //}
                _editText.SetCompoundDrawables(null, null, _rightDrawable, null);
                if (!string.IsNullOrEmpty(_hint) && string.IsNullOrEmpty(_editText.Text.ToString()))
                {
                    _txtLayout.Hint = _hint.ToUpper();
                    _editText.Hint = "";
                }
            }
            else
            {
                if (_leftDrawable != null && string.IsNullOrEmpty(_editText.Text.ToString()))
                {
                    _editText.SetCompoundDrawables(_leftDrawable, null, _rightDrawable, null);
                }
                if (!string.IsNullOrEmpty(_hint) && string.IsNullOrEmpty(_editText.Text.ToString()))
                {
                    _txtLayout.Hint = "";
                    _editText.Hint = _hint;
                }
            }
        }

    }
}