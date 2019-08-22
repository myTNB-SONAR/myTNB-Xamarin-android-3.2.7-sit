using System;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace myTNB_Android.Src.SSMR.SubmitMeterReading.MVP
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
        Context mContext;


        //public MeterReadingInputLayout(Context context) : base(context)
        //{
        //    mContext = context;
        //    LayoutParameters = LayoutParameters;
        //    Orientation = Orientation.Horizontal;
        //    WeightSum = 8;
        //    InitializeInputBoxes();
        //}

        public MeterReadingInputLayout(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            mContext = context;
            //LayoutParameters = LayoutParameters;
            //Orientation = Orientation.Horizontal;
            //WeightSum = 8;
            InitializeInputBoxes();
        }

        //public MeterReadingInputLayout(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        //{
        //    mContext = context;
        //    LayoutParameters = LayoutParameters;
        //    Orientation = Orientation.Horizontal;
        //    WeightSum = 8;
        //    InitializeInputBoxes();
        //}

        //public MeterReadingInputLayout(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        //{
        //    mContext = context;
        //    LayoutParameters = LayoutParameters;
        //    Orientation = Orientation.Horizontal;
        //    WeightSum = 8;
        //    InitializeInputBoxes();
        //}

        //protected MeterReadingInputLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        //{
        //    //mContext = context;
        //    LayoutParameters = LayoutParameters;
        //    Orientation = Orientation.Horizontal;
        //    WeightSum = 8;
        //    InitializeInputBoxes();
        //}

        public int convertPixelsToDp(float px)
        {
            return (int)(px / ((float)Resources.DisplayMetrics.DensityDpi / (float)DisplayMetrics.DensityDefault));
        }

        private LayoutParams GetInputBoxLayoutParams()
        {
            int width = convertPixelsToDp(0);
            LayoutParams layoutParams = new LayoutParams(width, LayoutParams.MatchParent);
            layoutParams.LeftMargin = 5;// convertPixelsToDp(8);
            layoutParams.TopMargin = 8;// convertPixelsToDp(8);
            layoutParams.Weight = 1;
            return layoutParams;
        }

        private void InitializeInputBoxes()
        {
            editText1 = new EditText(mContext);
            //editText1.SetTag(TAG_KEY, ".editText1");
            editText1.LayoutParameters = GetInputBoxLayoutParams();
            editText1.Gravity = Android.Views.GravityFlags.Center;
            editText1.SetBackgroundColor(Color.ParseColor("#f3f3f3"));
            editText1.AddTextChangedListener(new OnMeterReadingValueChange(this));
            editText1.SetOnKeyListener(new OnMeterInputKeyListener(this));
            editText1.InputType = InputTypes.ClassNumber;
            editText1.OnFocusChangeListener = new OnMeterInputFocus(this);//SetOnClickListener(new OnMeterInputClick(this));
            editText1.RequestLayout();
            AddView(editText1);

            editText2 = new EditText(mContext);
            //editText2.SetTag(TAG_KEY, ".editText2");
            editText2.LayoutParameters = GetInputBoxLayoutParams();
            editText2.Gravity = Android.Views.GravityFlags.Center;
            editText2.SetBackgroundColor(Color.ParseColor("#f3f3f3"));
            editText2.AddTextChangedListener(new OnMeterReadingValueChange(this));
            editText2.SetOnKeyListener(new OnMeterInputKeyListener(this));
            editText2.InputType = InputTypes.ClassNumber;
            editText2.OnFocusChangeListener = new OnMeterInputFocus(this);//.SetOnClickListener(new OnMeterInputClick(this));
            AddView(editText2);

            editText3 = new EditText(mContext);
            //editText3.SetTag(TAG_KEY, ".editText3");
            editText3.LayoutParameters = GetInputBoxLayoutParams();
            editText3.Gravity = Android.Views.GravityFlags.Center;
            editText3.SetBackgroundColor(Color.ParseColor("#f3f3f3"));
            editText3.AddTextChangedListener(new OnMeterReadingValueChange(this));
            editText3.SetOnKeyListener(new OnMeterInputKeyListener(this));
            editText3.InputType = InputTypes.ClassNumber;
            editText3.OnFocusChangeListener = new OnMeterInputFocus(this); //SetOnClickListener(new OnMeterInputClick(this));
            AddView(editText3);

            editText4 = new EditText(mContext);
            //editText4.SetTag(TAG_KEY, ".editText4");
            editText4.LayoutParameters = GetInputBoxLayoutParams();
            editText4.Gravity = Android.Views.GravityFlags.Center;
            editText4.SetBackgroundColor(Color.ParseColor("#f3f3f3"));
            editText4.AddTextChangedListener(new OnMeterReadingValueChange(this));
            editText4.SetOnKeyListener(new OnMeterInputKeyListener(this));
            editText4.InputType = InputTypes.ClassNumber;
            editText4.OnFocusChangeListener = new OnMeterInputFocus(this); //SetOnClickListener(new OnMeterInputClick(this));
            AddView(editText4);

            editText5 = new EditText(mContext);
            //editText5.SetTag(TAG_KEY, ".editText5");
            editText5.LayoutParameters = GetInputBoxLayoutParams();
            editText5.Gravity = Android.Views.GravityFlags.Center;
            editText5.SetBackgroundColor(Color.ParseColor("#f3f3f3"));
            editText5.AddTextChangedListener(new OnMeterReadingValueChange(this));
            editText5.SetOnKeyListener(new OnMeterInputKeyListener(this));
            editText5.InputType = InputTypes.ClassNumber;
            editText5.OnFocusChangeListener = new OnMeterInputFocus(this); //SetOnClickListener(new OnMeterInputClick(this));
            AddView(editText5);

            editText6 = new EditText(mContext);
            //editText6.SetTag(TAG_KEY, ".editText6");
            editText6.LayoutParameters = GetInputBoxLayoutParams();
            editText6.Gravity = Android.Views.GravityFlags.Center;
            editText6.SetBackgroundColor(Color.ParseColor("#f3f3f3"));
            editText6.AddTextChangedListener(new OnMeterReadingValueChange(this));
            editText6.SetOnKeyListener(new OnMeterInputKeyListener(this));
            editText6.InputType = InputTypes.ClassNumber;
            editText6.OnFocusChangeListener = new OnMeterInputFocus(this); //SetOnClickListener(new OnMeterInputClick(this));
            AddView(editText6);

            editText7 = new EditText(mContext);
            //editText7.SetTag(TAG_KEY, ".editText7");
            editText7.LayoutParameters = GetInputBoxLayoutParams();
            editText7.Gravity = Android.Views.GravityFlags.Center;
            editText7.SetBackgroundColor(Color.ParseColor("#f3f3f3"));
            editText7.AddTextChangedListener(new OnMeterReadingValueChange(this));
            editText7.SetOnKeyListener(new OnMeterInputKeyListener(this));
            editText7.InputType = InputTypes.ClassNumber;
            editText7.OnFocusChangeListener = new OnMeterInputFocus(this); //SetOnClickListener(new OnMeterInputClick(this));
            AddView(editText7);

            editText8 = new EditText(mContext);
            //editText8.SetTag(TAG_KEY, ".editText8");
            editText8.LayoutParameters = GetInputBoxLayoutParams();
            editText8.Gravity = Android.Views.GravityFlags.Center;
            editText8.SetBackgroundColor(Color.ParseColor("#f3f3f3"));
            editText8.AddTextChangedListener(new OnMeterReadingValueChange(this));
            editText8.SetOnKeyListener(new OnMeterInputKeyListener(this));
            editText8.InputType = InputTypes.ClassNumber;
            //editText8.OnFocusChangeListener = new OnMeterInputFocus(this); //SetOnClickListener(new OnMeterInputClick(this));
            AddView(editText8);

            RequestLayout();
        }

        class OnMeterReadingValueChange : Java.Lang.Object, ITextWatcher
        {
            MeterReadingInputLayout mContainer;

            public OnMeterReadingValueChange(MeterReadingInputLayout container)
            {
                mContainer = container;
            }

            public void AfterTextChanged(IEditable s)
            {
                //bool e1 = mContainer.GetChildAt(0).HasFocus;
                //bool e2 = mContainer.GetChildAt(1).HasFocus;
                //bool e3 = mContainer.GetChildAt(2).HasFocus;
                //bool e4 = mContainer.GetChildAt(3).HasFocus;
                //bool e5 = mContainer.GetChildAt(4).HasFocus;
                //bool e6 = mContainer.GetChildAt(5).HasFocus;
                //bool e7 = mContainer.GetChildAt(6).HasFocus;
                //bool e8 = mContainer.GetChildAt(7).HasFocus;

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
            }

            public void BeforeTextChanged(ICharSequence s, int start, int count, int after){}

            public void OnTextChanged(ICharSequence s, int start, int before, int count){}
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
                if (e.Action != KeyEventActions.Down)
                    return true;

                if (keyCode == Keycode.Del)
                {
                    if (mContainer.editText8 == v && mContainer.editText8.Text == "")
                    {
                        mContainer.editText8.Text = mContainer.editText7.Text;
                        mContainer.editText7.Text = mContainer.editText6.Text;
                        mContainer.editText6.Text = mContainer.editText5.Text;
                        mContainer.editText5.Text = mContainer.editText4.Text;
                        mContainer.editText4.Text = mContainer.editText3.Text;
                        mContainer.editText3.Text = mContainer.editText2.Text;
                        mContainer.editText2.Text = mContainer.editText1.Text;
                        mContainer.editText1.Text = "";

                        mContainer.editText8.SetSelection(1);
                    }

                    if (mContainer.editText7 == v && mContainer.editText7.Text == "")
                    {
                        mContainer.editText7.Text = mContainer.editText6.Text;
                        mContainer.editText6.Text = mContainer.editText5.Text;
                        mContainer.editText5.Text = mContainer.editText4.Text;
                        mContainer.editText4.Text = mContainer.editText3.Text;
                        mContainer.editText3.Text = mContainer.editText2.Text;
                        mContainer.editText2.Text = mContainer.editText1.Text;
                        mContainer.editText1.Text = "";

                        mContainer.editText7.SetSelection(1);
                    }

                    if (mContainer.editText6 == v && mContainer.editText6.Text == "")
                    {
                        mContainer.editText6.Text = mContainer.editText5.Text;
                        mContainer.editText5.Text = mContainer.editText4.Text;
                        mContainer.editText4.Text = mContainer.editText3.Text;
                        mContainer.editText3.Text = mContainer.editText2.Text;
                        mContainer.editText2.Text = mContainer.editText1.Text;
                        mContainer.editText1.Text = "";

                        mContainer.editText6.SetSelection(1);
                    }

                    if (mContainer.editText5 == v && mContainer.editText5.Text == "")
                    {
                        mContainer.editText5.Text = mContainer.editText4.Text;
                        mContainer.editText4.Text = mContainer.editText3.Text;
                        mContainer.editText3.Text = mContainer.editText2.Text;
                        mContainer.editText2.Text = mContainer.editText1.Text;
                        mContainer.editText1.Text = "";

                        mContainer.editText5.SetSelection(1);
                    }

                    if (mContainer.editText4 == v && mContainer.editText4.Text == "")
                    {
                        mContainer.editText4.Text = mContainer.editText3.Text;
                        mContainer.editText3.Text = mContainer.editText2.Text;
                        mContainer.editText2.Text = mContainer.editText1.Text;
                        mContainer.editText1.Text = "";

                        mContainer.editText4.SetSelection(1);
                    }

                    if (mContainer.editText3 == v && mContainer.editText3.Text == "")
                    {
                        mContainer.editText3.Text = mContainer.editText2.Text;
                        mContainer.editText2.Text = mContainer.editText1.Text;
                        mContainer.editText1.Text = "";

                        mContainer.editText3.SetSelection(1);
                    }

                    if (mContainer.editText2 == v && mContainer.editText2.Text == "")
                    {
                        mContainer.editText2.Text = mContainer.editText1.Text;
                        mContainer.editText1.Text = "";

                        mContainer.editText2.SetSelection(1);
                    }

                    if (mContainer.editText1 == v && mContainer.editText1.Text == "")
                    {
                        mContainer.editText1.Text = "";

                        mContainer.editText1.SetSelection(1);
                    }
                }
                return false;
            }
        }
    }
}
