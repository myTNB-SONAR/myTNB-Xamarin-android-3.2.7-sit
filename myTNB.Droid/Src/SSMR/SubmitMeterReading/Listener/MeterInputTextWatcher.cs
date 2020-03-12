using System;
using System.Collections.Generic;
using Android.Text;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.Listener
{
    public class MeterInputTextWatcher : Java.Lang.Object, ITextWatcher
    {
        private View mView;
        EditText editText1, editText2, editText3, editText4, editText5, editText6, editText7, editText8;
        private MeterInputTextWatcher(View view)
        {
            mView = view;
        }

        public void SetEditTextList(List<EditText> editTextList)
        {
            editText1 = editTextList[7];
            editText1.AddTextChangedListener(new MeterInputTextWatcher(editText1));
            editText2 = editTextList[6];
            editText2.AddTextChangedListener(new MeterInputTextWatcher(editText2));
            editText3 = editTextList[5];
            editText3.AddTextChangedListener(new MeterInputTextWatcher(editText3));
            editText4 = editTextList[4];
            editText4.AddTextChangedListener(new MeterInputTextWatcher(editText4));
            editText5 = editTextList[3];
            editText5.AddTextChangedListener(new MeterInputTextWatcher(editText5));
            editText6 = editTextList[2];
            editText6.AddTextChangedListener(new MeterInputTextWatcher(editText6));
            editText7 = editTextList[1];
            editText7.AddTextChangedListener(new MeterInputTextWatcher(editText7));
            editText8 = editTextList[0];
            editText8.AddTextChangedListener(new MeterInputTextWatcher(editText8));
        }
        public void AfterTextChanged(IEditable s)
        {
            if (editText1 == mView)
            {

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
