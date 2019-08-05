using System;
using System.Collections.Generic;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;
using myTNB_Android.Src.SSMR.SubmitMeterReading.MVP;

namespace myTNB_Android.Src.SSMR.Util
{
    public class MeterReadingValuesViewHelper
    {
        List<EditText> editTextList;
        private int currentPosition;
        private SubmitMeterReadingActivity mView;
        private int meterCard;
        public MeterReadingValuesViewHelper(SubmitMeterReadingActivity view, int meterCardValue)
        {
            mView = view;
            meterCard = meterCardValue;
        }

        public void SetEditTextList(List<EditText> editTextListVal)
        {
            editTextList = editTextListVal;
            currentPosition = editTextList.Count-1;
        }

        public void SetEvent()
        {
            //editTextList[editTextList.Count-1].AddTextChangedListener(new TextChangeListener(this));
            for(int i=0; i < editTextList.Count; i++)
            {
                if (i == (editTextList.Count-1))
                {
                    editTextList[i].SetOnEditorActionListener(new OnEditorActionChangeListener(this));
                    editTextList[i].AddTextChangedListener(new TextChangeListener(this)); //For right to left input and validation
                }
                else
                {
                    editTextList[i].SetOnEditorActionListener(new OnEditorActionChangeListener(this));
                    //editTextList[i].AddTextChangedListener(new OnValidateTextChangeListener(this)); //For validation
                    editTextList[i].Enabled = false;
                }
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

        public void UpdateLastDigit(string val)
        {
            editTextList[8].RemoveTextChangedListener(new TextChangeListener(this));
            editTextList[8].Text = val;
            editTextList[8].AddTextChangedListener(new TextChangeListener(this));
        }

        public void ValidateMeterInput()
        {
            mView.ValidateMeterInput(meterCard);
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

            MeterReadingValuesViewHelper mHelper;

            public TextChangeListener(MeterReadingValuesViewHelper helper)
            {
                mHelper = helper;
                editTexts = mHelper.editTextList;
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
                previousVal = editTexts[8].Text;
            }

            public void OnTextChanged(ICharSequence s, int start, int before, int count)
            {
                if (!changed)
                {
                    changed = !changed;
                    if (editTexts[0].Text == "" && s.Length() > 0)
                    {
                        meterValue = meterValue + s.ToString();
                        string val = s.CharAt(0).ToString();
                        editTexts[8].RemoveTextChangedListener(this);
                        editTexts[8].Text = "";
                        editTexts[8].Text = val;
                        editTexts[8].AddTextChangedListener(this);

                        if (startChange && previousVal != "")
                        {
                            editTexts[0].Text = editTexts[1].Text;
                            editTexts[1].Text = editTexts[2].Text;
                            editTexts[2].Text = editTexts[3].Text;
                            editTexts[3].Text = editTexts[4].Text;
                            editTexts[4].Text = editTexts[5].Text;
                            editTexts[5].Text = editTexts[6].Text;
                            editTexts[6].Text = editTexts[7].Text;
                            editTexts[7].Text = previousVal;
                        }
                        if (editTexts[7].Text == "")
                        {
                            startChange = !startChange;
                        }
                    }
                    else
                    {
                        editTexts[8].RemoveTextChangedListener(this);
                        editTexts[8].Text = "";
                        editTexts[8].AddTextChangedListener(this);

                    }

                    for (int i=0; i < mHelper.editTextList.Count; i++)
                    {
                        if (i != (mHelper.editTextList.Count-1))
                        {
                            if (mHelper.editTextList[i].Text == "")
                            {
                                mHelper.editTextList[i].Enabled = false;
                            }
                            else
                            {
                                mHelper.editTextList[i].Enabled = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
