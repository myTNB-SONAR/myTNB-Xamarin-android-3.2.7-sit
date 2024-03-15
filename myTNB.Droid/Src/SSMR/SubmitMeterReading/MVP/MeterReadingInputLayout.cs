using System;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using myTNB.AndroidApp.Src.Utils;

namespace myTNB.AndroidApp.Src.SSMR.SubmitMeterReading.MVP
{
    public class MeterReadingInputLayout : LinearLayout
    {
        public int TAG_KEY = 1001;
        public EditText editText1;
        public EditText editText2;
        public EditText editText3;
        public EditText editText4;
        public EditText editText5;
        public EditText editText6;
        public EditText editText7;
        public EditText editText8;
        public EditText[] editTextArray;
        Context mContext;
        public SubmitMeterReadingContract.IView mOwnerView;
        public METER_READING_TYPE mMeterType;
        private string mMeterId;
        OnMeterReadingValueChange onMeterReadingValueChange;

        public MeterReadingInputLayout(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            mContext = context;
            //InitializeInputBoxes();
        }

        public void SetMeterId(string meterId)
        {
            mMeterId = meterId;
        }

        public string GetMeterId()
        {
            return mMeterId;
        }

        public int convertPixelsToDp(float px)
        {
            return (int)(px / ((float)Resources.DisplayMetrics.DensityDpi / (float)DisplayMetricsDensity.Default));
        }

        private LayoutParams GetInputBoxLayoutParams()
        {
            int width = convertPixelsToDp(0);
            LayoutParams layoutParams = new LayoutParams(width, LayoutParams.MatchParent);
            layoutParams.LeftMargin = 5;// convertPixelsToDp(8);
            //layoutParams.TopMargin = 8;// convertPixelsToDp(8);
            layoutParams.Weight = 1;
            return layoutParams;
        }

        public void InitializeInputBoxes()
        {
            editTextArray = new EditText[8];
            onMeterReadingValueChange = new OnMeterReadingValueChange(this, mOwnerView, mMeterType);

            editText1 = new EditText(mContext);
            //editText1.SetTag(TAG_KEY, ".editText1");
            editText1.LayoutParameters = GetInputBoxLayoutParams();
            editText1.Gravity = Android.Views.GravityFlags.Center;
            editText1.SetBackgroundColor(Color.ParseColor("#f3f3f3"));
            editText1.SetTextColor(Color.ParseColor("#424141"));
            editText1.SetTextSize(ComplexUnitType.Dip, 14);
            editText1.AddTextChangedListener(onMeterReadingValueChange);
            editText1.SetOnKeyListener(new OnMeterInputKeyListener(this));
            editText1.InputType = InputTypes.ClassNumber;
            editText1.OnFocusChangeListener = new OnMeterInputFocus(this);//SetOnClickListener(new OnMeterInputClick(this));
            editText1.RequestLayout();
            AddView(editText1);
            editTextArray[0] = editText1;

            editText2 = new EditText(mContext);
            //editText2.SetTag(TAG_KEY, ".editText2");
            editText2.LayoutParameters = GetInputBoxLayoutParams();
            editText2.Gravity = Android.Views.GravityFlags.Center;
            editText2.SetBackgroundColor(Color.ParseColor("#f3f3f3"));
            editText2.SetTextColor(Color.ParseColor("#424141"));
            editText2.SetTextSize(ComplexUnitType.Dip, 14);
            editText2.AddTextChangedListener(onMeterReadingValueChange);
            editText2.SetOnKeyListener(new OnMeterInputKeyListener(this));
            editText2.InputType = InputTypes.ClassNumber;
            editText2.OnFocusChangeListener = new OnMeterInputFocus(this);//.SetOnClickListener(new OnMeterInputClick(this));
            AddView(editText2);
            editTextArray[1] = editText2;

            editText3 = new EditText(mContext);
            //editText3.SetTag(TAG_KEY, ".editText3");
            editText3.LayoutParameters = GetInputBoxLayoutParams();
            editText3.Gravity = Android.Views.GravityFlags.Center;
            editText3.SetBackgroundColor(Color.ParseColor("#f3f3f3"));
            editText3.SetTextColor(Color.ParseColor("#424141"));
            editText3.SetTextSize(ComplexUnitType.Dip, 14);
            editText3.AddTextChangedListener(onMeterReadingValueChange);
            editText3.SetOnKeyListener(new OnMeterInputKeyListener(this));
            editText3.InputType = InputTypes.ClassNumber;
            editText3.OnFocusChangeListener = new OnMeterInputFocus(this); //SetOnClickListener(new OnMeterInputClick(this));
            AddView(editText3);
            editTextArray[2] = editText3;

            editText4 = new EditText(mContext);
            //editText4.SetTag(TAG_KEY, ".editText4");
            editText4.LayoutParameters = GetInputBoxLayoutParams();
            editText4.Gravity = Android.Views.GravityFlags.Center;
            editText4.SetBackgroundColor(Color.ParseColor("#f3f3f3"));
            editText4.SetTextColor(Color.ParseColor("#424141"));
            editText4.SetTextSize(ComplexUnitType.Dip, 14);
            editText4.AddTextChangedListener(onMeterReadingValueChange);
            editText4.SetOnKeyListener(new OnMeterInputKeyListener(this));
            editText4.InputType = InputTypes.ClassNumber;
            editText4.OnFocusChangeListener = new OnMeterInputFocus(this); //SetOnClickListener(new OnMeterInputClick(this));
            AddView(editText4);
            editTextArray[3] = editText4;

            editText5 = new EditText(mContext);
            //editText5.SetTag(TAG_KEY, ".editText5");
            editText5.LayoutParameters = GetInputBoxLayoutParams();
            editText5.Gravity = Android.Views.GravityFlags.Center;
            editText5.SetBackgroundColor(Color.ParseColor("#f3f3f3"));
            editText5.SetTextColor(Color.ParseColor("#424141"));
            editText5.SetTextSize(ComplexUnitType.Dip, 14);
            editText5.AddTextChangedListener(onMeterReadingValueChange);
            editText5.SetOnKeyListener(new OnMeterInputKeyListener(this));
            editText5.InputType = InputTypes.ClassNumber;
            editText5.OnFocusChangeListener = new OnMeterInputFocus(this); //SetOnClickListener(new OnMeterInputClick(this));
            AddView(editText5);
            editTextArray[4] = editText5;

            editText6 = new EditText(mContext);
            //editText6.SetTag(TAG_KEY, ".editText6");
            editText6.LayoutParameters = GetInputBoxLayoutParams();
            editText6.Gravity = Android.Views.GravityFlags.Center;
            editText6.SetBackgroundColor(Color.ParseColor("#f3f3f3"));
            editText6.SetTextColor(Color.ParseColor("#424141"));
            editText6.SetTextSize(ComplexUnitType.Dip, 14);
            editText6.AddTextChangedListener(onMeterReadingValueChange);
            editText6.SetOnKeyListener(new OnMeterInputKeyListener(this));
            editText6.InputType = InputTypes.ClassNumber;
            editText6.OnFocusChangeListener = new OnMeterInputFocus(this); //SetOnClickListener(new OnMeterInputClick(this));
            AddView(editText6);
            editTextArray[5] = editText6;

            editText7 = new EditText(mContext);
            //editText7.SetTag(TAG_KEY, ".editText7");
            editText7.LayoutParameters = GetInputBoxLayoutParams();
            editText7.Gravity = Android.Views.GravityFlags.Center;
            editText7.SetBackgroundColor(Color.ParseColor("#f3f3f3"));
            editText7.SetTextColor(Color.ParseColor("#424141"));
            editText7.SetTextSize(ComplexUnitType.Dip, 14);
            editText7.AddTextChangedListener(onMeterReadingValueChange);
            editText7.SetOnKeyListener(new OnMeterInputKeyListener(this));
            editText7.InputType = InputTypes.ClassNumber;
            editText7.OnFocusChangeListener = new OnMeterInputFocus(this); //SetOnClickListener(new OnMeterInputClick(this));
            AddView(editText7);
            editTextArray[6] = editText7;

            editText8 = new EditText(mContext);
            //editText8.SetTag(TAG_KEY, ".editText8");
            editText8.LayoutParameters = GetInputBoxLayoutParams();
            editText8.Gravity = Android.Views.GravityFlags.Center;
            editText8.SetBackgroundColor(Color.ParseColor("#f3f3f3"));
            editText8.SetTextColor(Color.ParseColor("#424141"));
            editText8.SetTextSize(ComplexUnitType.Dip, 14);
            editText8.AddTextChangedListener(onMeterReadingValueChange);
            editText8.SetOnKeyListener(new OnMeterInputKeyListener(this));
            editText8.InputType = InputTypes.ClassNumber;
            //editText8.OnFocusChangeListener = new OnMeterInputFocus(this); //SetOnClickListener(new OnMeterInputClick(this));
            AddView(editText8);
            editTextArray[7] = editText8;

            TextViewUtils.SetMuseoSans500Typeface(editText1, editText2, editText3, editText4, editText5, editText6, editText7, editText8);

            RequestLayout();
        }

        public void UpdateMeterReadingInput(string updatedReading)
        {
            char[] readingInArray = updatedReading.ToCharArray();
            int MAX_METER_DIGIT = 7;

            editTextArray[7].RemoveTextChangedListener(onMeterReadingValueChange);
            editTextArray[6].RemoveTextChangedListener(onMeterReadingValueChange);
            editTextArray[5].RemoveTextChangedListener(onMeterReadingValueChange);
            editTextArray[4].RemoveTextChangedListener(onMeterReadingValueChange);
            editTextArray[3].RemoveTextChangedListener(onMeterReadingValueChange);
            editTextArray[2].RemoveTextChangedListener(onMeterReadingValueChange);
            editTextArray[1].RemoveTextChangedListener(onMeterReadingValueChange);
            editTextArray[0].RemoveTextChangedListener(onMeterReadingValueChange);

            editTextArray[7].Text = "";
            editTextArray[6].Text = "";
            editTextArray[5].Text = "";
            editTextArray[4].Text = "";
            editTextArray[3].Text = "";
            editTextArray[2].Text = "";
            editTextArray[1].Text = "";
            editTextArray[0].Text = "";

            for (int i = (readingInArray.Length - 1); i >= 0; i--)
            {
                editTextArray[MAX_METER_DIGIT].Text = readingInArray[i].ToString();
                MAX_METER_DIGIT--;
            }

            editTextArray[7].AddTextChangedListener(onMeterReadingValueChange);
            editTextArray[6].AddTextChangedListener(onMeterReadingValueChange);
            editTextArray[5].AddTextChangedListener(onMeterReadingValueChange);
            editTextArray[4].AddTextChangedListener(onMeterReadingValueChange);
            editTextArray[3].AddTextChangedListener(onMeterReadingValueChange);
            editTextArray[2].AddTextChangedListener(onMeterReadingValueChange);
            editTextArray[1].AddTextChangedListener(onMeterReadingValueChange);
            editTextArray[0].AddTextChangedListener(onMeterReadingValueChange);
        }

        public string GetMeterReadingInput()
        {
            return (editText1.Text + editText2.Text + editText3.Text + editText4.Text +
                editText5.Text + editText6.Text + editText7.Text + editText8.Text);
        }

        public void SetOnValidateInput(SubmitMeterReadingContract.IView view, METER_READING_TYPE meterType)
        {
            mOwnerView = view;
            mMeterType = meterType;
        }

        public bool HasReadingInput()
        {
            return (editText1.Text != "" || editText2.Text != "" || editText3.Text != "" || editText4.Text != ""
                || editText5.Text != "" || editText6.Text != "" || editText7.Text != "" || editText8.Text != "");
        }

        public void SetInputColor(Color color)
        {
            editText1.SetTextColor(color);
            editText2.SetTextColor(color);
            editText3.SetTextColor(color);
            editText4.SetTextColor(color);
            editText5.SetTextColor(color);
            editText6.SetTextColor(color);
            editText7.SetTextColor(color);
            editText8.SetTextColor(color);
        }

        public void UpdateSubmitReadingButton()
        {
            mOwnerView.OnUpdateSubmitMeterButton();
        }

        class OnMeterReadingValueChange : Java.Lang.Object, ITextWatcher
        {
            MeterReadingInputLayout mContainer;
            SubmitMeterReadingContract.IView mOwnerView;
            METER_READING_TYPE mMeterType;

            public OnMeterReadingValueChange(MeterReadingInputLayout container, SubmitMeterReadingContract.IView ownerView, METER_READING_TYPE meterType)
            {
                mContainer = container;
                mOwnerView = ownerView;
                mMeterType = meterType;
            }

            public void AfterTextChanged(IEditable s)
            {
                string val = s.ToString();
                char[] charArray = val.ToCharArray();
                if (charArray.Length > 1)
                {
                    ((EditText)mContainer.GetChildAt(7)).RemoveTextChangedListener(this);
                    ((EditText)mContainer.GetChildAt(6)).RemoveTextChangedListener(this);
                    ((EditText)mContainer.GetChildAt(5)).RemoveTextChangedListener(this);
                    ((EditText)mContainer.GetChildAt(4)).RemoveTextChangedListener(this);
                    ((EditText)mContainer.GetChildAt(3)).RemoveTextChangedListener(this);
                    ((EditText)mContainer.GetChildAt(2)).RemoveTextChangedListener(this);
                    ((EditText)mContainer.GetChildAt(1)).RemoveTextChangedListener(this);
                    ((EditText)mContainer.GetChildAt(0)).RemoveTextChangedListener(this);

                    if (mContainer.GetChildAt(7).HasFocus)
                    {
                        ((EditText)mContainer.GetChildAt(6)).ClearFocus();
                        ((EditText)mContainer.GetChildAt(5)).ClearFocus();
                        ((EditText)mContainer.GetChildAt(4)).ClearFocus();
                        ((EditText)mContainer.GetChildAt(3)).ClearFocus();
                        ((EditText)mContainer.GetChildAt(2)).ClearFocus();
                        ((EditText)mContainer.GetChildAt(1)).ClearFocus();
                        ((EditText)mContainer.GetChildAt(0)).ClearFocus();

                        ((EditText)mContainer.GetChildAt(7)).Text = charArray[1].ToString();
                        ((EditText)mContainer.GetChildAt(7)).SetSelection(1);

                        if (((EditText)mContainer.GetChildAt(6)).Text != "")
                        {
                            ((EditText)mContainer.GetChildAt(0)).Text = (((EditText)mContainer.GetChildAt(1)).Text != "") ? ((EditText)mContainer.GetChildAt(1)).Text : "";
                            ((EditText)mContainer.GetChildAt(1)).Text = (((EditText)mContainer.GetChildAt(2)).Text != "") ? ((EditText)mContainer.GetChildAt(2)).Text : "";
                            ((EditText)mContainer.GetChildAt(2)).Text = (((EditText)mContainer.GetChildAt(3)).Text != "") ? ((EditText)mContainer.GetChildAt(3)).Text : "";
                            ((EditText)mContainer.GetChildAt(3)).Text = (((EditText)mContainer.GetChildAt(4)).Text != "") ? ((EditText)mContainer.GetChildAt(4)).Text : "";
                            ((EditText)mContainer.GetChildAt(4)).Text = (((EditText)mContainer.GetChildAt(5)).Text != "") ? ((EditText)mContainer.GetChildAt(5)).Text : "";
                            ((EditText)mContainer.GetChildAt(5)).Text = ((EditText)mContainer.GetChildAt(6)).Text;
                            ((EditText)mContainer.GetChildAt(6)).Text = charArray[0].ToString();
                        }
                        else
                        {
                            ((EditText)mContainer.GetChildAt(6)).Text = charArray[0].ToString();
                        }

                    }

                    if (mContainer.GetChildAt(6).HasFocus)
                    {
                        ((EditText)mContainer.GetChildAt(6)).Text = charArray[1].ToString();
                        ((EditText)mContainer.GetChildAt(6)).SetSelection(1);

                        if (((EditText)mContainer.GetChildAt(5)).Text != "")
                        {
                            ((EditText)mContainer.GetChildAt(0)).Text = (((EditText)mContainer.GetChildAt(1)).Text != "") ? ((EditText)mContainer.GetChildAt(1)).Text : "";
                            ((EditText)mContainer.GetChildAt(1)).Text = (((EditText)mContainer.GetChildAt(2)).Text != "") ? ((EditText)mContainer.GetChildAt(2)).Text : "";
                            ((EditText)mContainer.GetChildAt(2)).Text = (((EditText)mContainer.GetChildAt(3)).Text != "") ? ((EditText)mContainer.GetChildAt(3)).Text : "";
                            ((EditText)mContainer.GetChildAt(3)).Text = (((EditText)mContainer.GetChildAt(4)).Text != "") ? ((EditText)mContainer.GetChildAt(4)).Text : "";
                            ((EditText)mContainer.GetChildAt(4)).Text = ((EditText)mContainer.GetChildAt(5)).Text;
                            ((EditText)mContainer.GetChildAt(5)).Text = charArray[0].ToString();
                        }
                        else
                        {
                            ((EditText)mContainer.GetChildAt(5)).Text = charArray[0].ToString();
                        }

                    }

                    if (mContainer.GetChildAt(5).HasFocus)
                    {
                        ((EditText)mContainer.GetChildAt(5)).Text = charArray[1].ToString();
                        ((EditText)mContainer.GetChildAt(5)).SetSelection(1);

                        if (((EditText)mContainer.GetChildAt(4)).Text != "")
                        {
                            ((EditText)mContainer.GetChildAt(0)).Text = (((EditText)mContainer.GetChildAt(1)).Text != "") ? ((EditText)mContainer.GetChildAt(1)).Text : "";
                            ((EditText)mContainer.GetChildAt(1)).Text = (((EditText)mContainer.GetChildAt(2)).Text != "") ? ((EditText)mContainer.GetChildAt(2)).Text : "";
                            ((EditText)mContainer.GetChildAt(2)).Text = (((EditText)mContainer.GetChildAt(3)).Text != "") ? ((EditText)mContainer.GetChildAt(3)).Text : "";
                            ((EditText)mContainer.GetChildAt(3)).Text = ((EditText)mContainer.GetChildAt(4)).Text;
                            ((EditText)mContainer.GetChildAt(4)).Text = charArray[0].ToString();
                        }
                        else
                        {
                            ((EditText)mContainer.GetChildAt(4)).Text = charArray[0].ToString();
                        }

                    }

                    if (mContainer.GetChildAt(4).HasFocus)
                    {
                        ((EditText)mContainer.GetChildAt(4)).Text = charArray[1].ToString();
                        ((EditText)mContainer.GetChildAt(4)).SetSelection(1);

                        if (((EditText)mContainer.GetChildAt(3)).Text != "")
                        {
                            ((EditText)mContainer.GetChildAt(0)).Text = (((EditText)mContainer.GetChildAt(1)).Text != "") ? ((EditText)mContainer.GetChildAt(1)).Text : "";
                            ((EditText)mContainer.GetChildAt(1)).Text = (((EditText)mContainer.GetChildAt(2)).Text != "") ? ((EditText)mContainer.GetChildAt(2)).Text : "";
                            ((EditText)mContainer.GetChildAt(2)).Text = ((EditText)mContainer.GetChildAt(3)).Text;
                            ((EditText)mContainer.GetChildAt(3)).Text = charArray[0].ToString();
                        }
                        else
                        {
                            ((EditText)mContainer.GetChildAt(3)).Text = charArray[0].ToString();
                        }

                    }

                    if (mContainer.GetChildAt(3).HasFocus)
                    {
                        ((EditText)mContainer.GetChildAt(3)).Text = charArray[1].ToString();
                        ((EditText)mContainer.GetChildAt(3)).SetSelection(1);

                        if (((EditText)mContainer.GetChildAt(2)).Text != "")
                        {
                            ((EditText)mContainer.GetChildAt(0)).Text = (((EditText)mContainer.GetChildAt(1)).Text != "") ? ((EditText)mContainer.GetChildAt(1)).Text : "";
                            ((EditText)mContainer.GetChildAt(1)).Text = ((EditText)mContainer.GetChildAt(2)).Text;
                            ((EditText)mContainer.GetChildAt(2)).Text = charArray[0].ToString();
                        }
                        else
                        {
                            ((EditText)mContainer.GetChildAt(2)).Text = charArray[0].ToString();
                        }

                    }

                    if (mContainer.GetChildAt(2).HasFocus)
                    {
                        ((EditText)mContainer.GetChildAt(2)).Text = charArray[1].ToString();
                        ((EditText)mContainer.GetChildAt(2)).SetSelection(1);

                        if (((EditText)mContainer.GetChildAt(1)).Text != "")
                        {
                            ((EditText)mContainer.GetChildAt(0)).Text = ((EditText)mContainer.GetChildAt(1)).Text;
                            ((EditText)mContainer.GetChildAt(1)).Text = charArray[0].ToString();
                        }
                        else
                        {
                            ((EditText)mContainer.GetChildAt(1)).Text = charArray[0].ToString();
                        }

                    }

                    if (mContainer.GetChildAt(1).HasFocus)
                    {
                        ((EditText)mContainer.GetChildAt(1)).Text = charArray[1].ToString();
                        ((EditText)mContainer.GetChildAt(1)).SetSelection(1);

                        if (((EditText)mContainer.GetChildAt(0)).Text == "")
                        {
                            ((EditText)mContainer.GetChildAt(0)).Text = charArray[0].ToString();
                        }

                    }

                    ((EditText)mContainer.GetChildAt(7)).AddTextChangedListener(this);
                    ((EditText)mContainer.GetChildAt(6)).AddTextChangedListener(this);
                    ((EditText)mContainer.GetChildAt(5)).AddTextChangedListener(this);
                    ((EditText)mContainer.GetChildAt(4)).AddTextChangedListener(this);
                    ((EditText)mContainer.GetChildAt(3)).AddTextChangedListener(this);
                    ((EditText)mContainer.GetChildAt(2)).AddTextChangedListener(this);
                    ((EditText)mContainer.GetChildAt(1)).AddTextChangedListener(this);
                    ((EditText)mContainer.GetChildAt(0)).AddTextChangedListener(this);
                }

                mOwnerView.ClearMeterCardValidationError(mMeterType);

                mContainer.UpdateSubmitReadingButton();
            }

            public void BeforeTextChanged(ICharSequence s, int start, int count, int after){}

            public void OnTextChanged(ICharSequence s, int start, int before, int count)
            {}
        }

        class OnMeterInputClick : Java.Lang.Object, IOnClickListener
        {
            MeterReadingInputLayout mContainer;
            public OnMeterInputClick(MeterReadingInputLayout container)
            {
                mContainer = container;
            }
            public void OnClick(View v)
            {
                if (((EditText)v).Text == "")
                {
                    mContainer.editText8.RequestFocus();
                }
            }
        }

        class OnMeterInputFocus : Java.Lang.Object, IOnFocusChangeListener
        {
            MeterReadingInputLayout mContainer;

            public OnMeterInputFocus(MeterReadingInputLayout container)
            {
                mContainer = container;
            }

            public void OnFocusChange(View v, bool hasFocus)
            {
                if (hasFocus)
                {
                    if (((EditText)v).Text == "")
                    {
                        mContainer.editText8.RequestFocus();
                        ((EditText)v).SetCursorVisible(false);
                    }
                    else
                    {
                        ((EditText)v).SetCursorVisible(true);
                        ((EditText)v).SetSelection(1);
                    }
                }
                //mContainer.UpdateSubmitReadingButton();
            }
        }

        class OnMeterInputKeyListener : Java.Lang.Object, IOnKeyListener
        {
            MeterReadingInputLayout mContainer;

            public OnMeterInputKeyListener(MeterReadingInputLayout container)
            {
                mContainer = container;
            }

            public bool OnKey(View v, [GeneratedEnum] Keycode keyCode, KeyEvent e)
            {
                if (keyCode == Keycode.Del && e.Action == KeyEventActions.Up)
                {
                    if (mContainer.editText8.Text == "" && mContainer.editText7.Text == "" && mContainer.editText6.Text == ""
                        && mContainer.editText5.Text == "" && mContainer.editText4.Text == "" && mContainer.editText3.Text == ""
                        && mContainer.editText2.Text == "" && mContainer.editText1.Text == "")
                        return true;

                    string meterReading = mContainer.editText1.Text +
                        mContainer.editText2.Text +
                        mContainer.editText3.Text +
                        mContainer.editText4.Text +
                        mContainer.editText5.Text +
                        mContainer.editText6.Text +
                        mContainer.editText7.Text +
                        mContainer.editText8.Text;
                    mContainer.UpdateMeterReadingInput(meterReading);
                    if (mContainer.editText8 == v && mContainer.editText8.Text != "")
                    {
                        mContainer.editText8.SetSelection(1);
                    }
                    if (mContainer.editText7 == v && mContainer.editText7.Text != "")
                    {
                        mContainer.editText7.SetSelection(1);
                    }
                    if (mContainer.editText6 == v && mContainer.editText6.Text != "")
                    {
                        mContainer.editText6.SetSelection(1);
                    }
                    if (mContainer.editText5 == v && mContainer.editText5.Text != "")
                    {
                        mContainer.editText5.SetSelection(1);
                    }
                    if (mContainer.editText4 == v && mContainer.editText4.Text != "")
                    {
                        mContainer.editText4.SetSelection(1);
                    }
                    if (mContainer.editText3 == v && mContainer.editText3.Text != "")
                    {
                        mContainer.editText3.SetSelection(1);
                    }
                    if (mContainer.editText2 == v && mContainer.editText2.Text != "")
                    {
                        mContainer.editText2.SetSelection(1);
                    }
                    if (mContainer.editText1 == v && mContainer.editText1.Text != "")
                    {
                        mContainer.editText1.SetSelection(1);
                    }
                    return false;
                }
                return false;
            }
        }
    }
}
