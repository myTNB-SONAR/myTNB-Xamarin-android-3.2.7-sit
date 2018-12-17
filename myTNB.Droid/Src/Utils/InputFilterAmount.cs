using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Text;
using Java.Lang;
using myTNB_Android.Src.MultipleAccountPayment.Model;
using static myTNB_Android.Src.MultipleAccountPayment.Adapter.SelectAccountListAdapter;

namespace myTNB_Android.Src.Utils
{
    public class InputFilterAmount : Java.Lang.Object, ITextWatcher
    {
        private readonly EditText _editText;
        private TextChangedCallBack _callBack;

        public delegate void TextChangedCallBack(MPAccount item, int position, SelectAccountListViewHolder vh);

        public void OnTextChanged(TextChangedCallBack callback)
        {
           
        }

        public InputFilterAmount(EditText edt, TextChangedCallBack callBack)
        {
            _editText = edt;
            _callBack = callBack;
        }

        public void AfterTextChanged(IEditable s)
        {
            if (!TextUtils.IsEmpty(s))
            {
                OnTextChanged(_callBack);
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