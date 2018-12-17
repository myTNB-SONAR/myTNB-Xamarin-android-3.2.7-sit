using System;
using Android.Views.InputMethods;
using Android.Text.Method;
using Java.Lang;

namespace myTNB_Android.Src.Utils
{
    public class MoneyValueFilter : DigitsKeyListener
    {


        private int beforeDecimal;
        private int afterDecimal;




        public MoneyValueFilter(int beforeDecimal, int afterDecimal) : base((bool)Java.Lang.Boolean.False, (bool)Java.Lang.Boolean.True)
        {
            
            this.beforeDecimal = beforeDecimal;
            this.afterDecimal = afterDecimal;
        }




    public override Java.Lang.ICharSequence FilterFormatted(Java.Lang.ICharSequence source, int start, int end, Android.Text.ISpanned dest, int dstart, int dend)
    {


        StringBuilder builder = new StringBuilder(dest);
        builder.Insert(dstart, source);
        string temp = builder.ToString();

            //if (temp.Equals("."))
            //{
            //    return '0'+'.';
            //}
            //else if (temp.IndexOf('.') == -1)
            //{
            //    if (temp.Length > beforeDecimal)
            //    {
            //        return '';
            //    }
            //}
            //else
            //{
            Java.Lang.ICharSequence ch = null;
            if (temp.Substring(0, temp.IndexOf('.')).Length > beforeDecimal 
                || temp.Substring(temp.IndexOf('.') + 1, temp.Length).Length > afterDecimal)
            {
                return ch;
            }
        //}SSSS



        return base.FilterFormatted(source, start, end, dest, dstart, dend);
    }
}
    
}
