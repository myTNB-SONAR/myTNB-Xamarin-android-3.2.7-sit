using System;
using System.Collections.Generic;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;
using myTNB.Android.Src.SSMR.SubmitMeterReading.MVP;

namespace myTNB.Android.Src.SSMR.Util
{
    public class MeterReadingValuesViewHelper
    {
        List<EditText> editTextList;
        private int currentPosition;
        private SubmitMeterReadingActivity mView;
        private int meterCard;
        string meterValue;
        EditText[] editTextArray = new EditText[8];



        public MeterReadingValuesViewHelper(SubmitMeterReadingActivity view, int meterCardValue)
        {
            mView = view;
            meterCard = meterCardValue;
        }

        public void SetEditTextList(List<EditText> editTextListVal)
        {
            editTextList = editTextListVal;
            currentPosition = editTextList.Count-1;

            editTextArray[0] = editTextListVal[7];
            editTextArray[1] = editTextListVal[6];
            editTextArray[2] = editTextListVal[5];
            editTextArray[3] = editTextListVal[4];
            editTextArray[4] = editTextListVal[3];
            editTextArray[5] = editTextListVal[2];
            editTextArray[6] = editTextListVal[1];
            editTextArray[7] = editTextListVal[0];


        }

        public void SetEvent()
        {
            //editTextList[editTextList.Count-1].AddTextChangedListener(new TextChangeListener(this));
            for(int i=0; i < editTextList.Count; i++)
            {
                editTextList[i].SetOnEditorActionListener(new OnEditorActionChangeListener(this));
                editTextList[i].AddTextChangedListener(new TextChangeListener(this,i));
                //if (i == (editTextList.Count-1))
                //{
                //    editTextList[i].SetOnEditorActionListener(new OnEditorActionChangeListener(this));
                //    editTextList[i].AddTextChangedListener(new TextChangeListener(this)); //For right to left input and validation
                //}
                //else
                //{
                //    editTextList[i].SetOnEditorActionListener(new OnEditorActionChangeListener(this));
                //    //editTextList[i].AddTextChangedListener(new OnValidateTextChangeListener(this)); //For validation
                //    editTextList[i].Enabled = false;
                //}
            }
        }

        public void SetValue(string value)
        {
            currentPosition--;
            editTextList[currentPosition].Text = value;
        }

        public void UpdateMeterValue(string meterValue)
        {
            char[] charArray = meterValue.ToCharArray();
            int totalArray = charArray.Length-1;
            int editTextPosition = editTextList.Count;
            for (int i = totalArray; i >= 0; i--)
            {
                if (i!=0)
                {
                    editTextPosition--;
                    editTextList[editTextPosition].Text = charArray[i].ToString();
                }
            }
        }

        public void ValidateMeterInput()
        {
            //mView.ValidateMeterInput(meterCard);
        }

        public class OnEditorActionChangeListener : Java.Lang.Object, TextView.IOnEditorActionListener
        {
            MeterReadingValuesViewHelper mHelper;
            public OnEditorActionChangeListener(MeterReadingValuesViewHelper helper)
            {
                mHelper = helper;
            }
            public bool OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e)
            {
                if (actionId == ImeAction.ImeNull)
                {
                    mHelper.ValidateMeterInput();
                }
                return false;
            }
        }

        public class OnValidateTextChangeListener : Java.Lang.Object, Android.Text.ITextWatcher
        {
            MeterReadingValuesViewHelper mHelper;
            public OnValidateTextChangeListener(MeterReadingValuesViewHelper helper)
            {
                mHelper = helper;
            }
            public void AfterTextChanged(IEditable s)
            {
                //throw new NotImplementedException();
            }

            public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
            {
                //throw new NotImplementedException();
            }

            public void OnTextChanged(ICharSequence s, int start, int before, int count)
            {
                //mHelper.ValidateMeterInput();
            }
        }

        public class TextChangeListener : Java.Lang.Object, Android.Text.ITextWatcher
        {
            string meterValue = "";
            bool changed = false;
            List<EditText> editTexts;
            bool startChange = false;
            string previousVal;
            int mPosition;

            MeterReadingValuesViewHelper mHelper;

            public TextChangeListener(MeterReadingValuesViewHelper helper, int position)
            {
                mHelper = helper;
                editTexts = mHelper.editTextList;
                mPosition = position;
            }
            public void AfterTextChanged(IEditable s)
            {
                //throw new NotImplementedException();
                //mHelper.UpdateMeterValue(meterValue);
                changed = false;

            }

            public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
            {
                //throw new NotImplementedException();
                //previousVal = editTexts[8].Text;
            }

            public void OnTextChanged(ICharSequence s, int start, int before, int count)
            {
                //if (!changed)
                //{
                //    changed = !changed;
                //    if (editTexts[0].Text == "" && s.Length() > 0)
                //    {
                //        meterValue = meterValue + s.ToString();
                //        string val = s.CharAt(0).ToString();
                //        editTexts[8].RemoveTextChangedListener(this);
                //        editTexts[8].Text = "";
                //        editTexts[8].Text = val;
                //        editTexts[8].AddTextChangedListener(this);

                //        if (startChange && previousVal != "")
                //        {
                //            editTexts[0].Text = editTexts[1].Text;
                //            editTexts[1].Text = editTexts[2].Text;
                //            editTexts[2].Text = editTexts[3].Text;
                //            editTexts[3].Text = editTexts[4].Text;
                //            editTexts[4].Text = editTexts[5].Text;
                //            editTexts[5].Text = editTexts[6].Text;
                //            editTexts[6].Text = editTexts[7].Text;
                //            editTexts[7].Text = previousVal;
                //        }
                //        if (editTexts[7].Text == "")
                //        {
                //            startChange = !startChange;
                //        }
                //    }
                //    else
                //    {
                //        editTexts[8].RemoveTextChangedListener(this);
                //        editTexts[8].Text = "";
                //        editTexts[8].AddTextChangedListener(this);

                //    }

                //    for (int i = 0; i < mHelper.editTextList.Count; i++)
                //    {
                //        if (i != (mHelper.editTextList.Count - 1))
                //        {
                //            if (mHelper.editTextList[i].Text == "")
                //            {
                //                mHelper.editTextList[i].Enabled = false;
                //            }
                //            else
                //            {
                //                mHelper.editTextList[i].Enabled = true;
                //            }
                //        }
                //    }
                //}
                if (s.Length() > 1)
                {
                    //if (mHelper.editTextList[mPosition - 1].Text == "")
                    //{
                    //    mHelper.editTextList[mPosition].Text = s.CharAt(1).ToString();
                    //    mHelper.editTextList[mPosition - 1].Text = s.CharAt(0).ToString();
                    //}
                    //if (mHelper.editTextList[mPosition - 2].Text == "")
                    //{
                    //    mHelper.editTextList[mPosition].Text = s.CharAt(1).ToString();
                    //    mHelper.editTextList[mPosition - 1].Text = s.CharAt(0).ToString();
                    //}
                    //int startCursorPosition = mPosition;
                    //for (int i = startCursorPosition; i < mHelper.editTextList.Count; i++)
                    //{
                    //    if ()
                    //    {

                    //    }
                    //}




                    //if (mHelper.meterValue == null)
                    //{
                    //    mHelper.meterValue = mHelper.meterValue + s.ToString();
                    //}
                    //else
                    //{
                    //    mHelper.meterValue = mHelper.meterValue + s.CharAt(0);
                    //}
                    //int meterValueLength = mHelper.meterValue.ToCharArray().Length;
                    //for (int i = 0; i <= mPosition; i++)
                    //{
                    //    int pos = (meterValueLength-1) - i;

                    //    mHelper.editTextList[mPosition - i].RemoveTextChangedListener(this);
                    //    mHelper.editTextList[mPosition - i].Text = mHelper.meterValue.ToCharArray()[pos].ToString();
                    //    mHelper.editTextList[mPosition - i].AddTextChangedListener(this);

                    //    if (pos == 0)
                    //    {
                    //        break;
                    //    }
                    //}



                    //string value = mHelper.meterValue;
                    //string t = "";
                    //if (mHelper.editTextList[mPosition - 1].Text == "")
                    //{
                    //    mHelper.editTextList[mPosition].Text = s.CharAt(1).ToString();
                    //    mHelper.editTextList[mPosition - 1].Text = s.CharAt(0).ToString();
                    //}
                    //else
                    //{

                    //   List<EditText> et = mHelper.editTextList.GetRange(0,mPosition-1);
                    //    foreach (EditText editText in et)
                    //    {
                    //        t = t + editText.Text;
                    //    }


                    //}
                    //string result = t;

                    //mHelper.meterValue = mHelper.editTextList[0].Text
                    //    + mHelper.editTextList[1].Text
                    //    + mHelper.editTextList[2].Text
                    //    + mHelper.editTextList[3].Text
                    //    + mHelper.editTextList[4].Text
                    //    + mHelper.editTextList[5].Text
                    //    + mHelper.editTextList[6].Text
                    //    + mHelper.editTextList[7].Text;
                }
            }
        }
    }
}
