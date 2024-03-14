using Android.Text;
using Android.Widget;
using Java.Lang;
using myTNB.Android.Src.MultipleAccountPayment.Model;
using static myTNB.Android.Src.MultipleAccountPayment.Adapter.SelectAccountListAdapter;

namespace myTNB.Android.Src.Utils
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