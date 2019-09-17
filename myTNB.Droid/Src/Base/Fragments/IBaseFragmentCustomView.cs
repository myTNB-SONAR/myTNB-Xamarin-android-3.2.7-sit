using System;
using Android.Views;

namespace myTNB_Android.Src.Base.Fragments
{
    public interface IBaseFragmentCustomView
    {
        void ShowGenericExceptionSnackBar();
        bool IsActive();
    }
}
