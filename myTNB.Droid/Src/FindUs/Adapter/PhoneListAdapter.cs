using Android.Content;


using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.TextField;
using myTNB.Android.Src.Base.Activity;
using myTNB.Android.Src.Utils;
using System;
using System.Collections.Generic;

namespace myTNB.Android.Src.FindUs.Adapter
{
    public class PhoneListAdapter : RecyclerView.Adapter
    {
        BaseAppCompatActivity mActivity;
        List<string> numbers = new List<string>();

        public override int ItemCount => numbers.Count;


        public PhoneListAdapter(BaseAppCompatActivity activity, List<string> data)
        {
            this.mActivity = activity;
            this.numbers.AddRange(data);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                PhoneListViewHolder h = holder as PhoneListViewHolder;
                if (numbers.Count > 1)
                {
                    int count = position + 1;
                    h.PhoneLabel.Hint = Utility.GetLocalizedLabel("LocationDetails", "phone").ToUpper() + count;
                }
                else
                {
                    h.PhoneLabel.Hint = Utility.GetLocalizedLabel("LocationDetails", "phone").ToUpper();
                }
                TextViewUtils.SetMuseoSans300Typeface(h.PhoneLabel);
                h.PhoneNumber.Text = numbers[position];
                TextViewUtils.SetMuseoSans300Typeface(h.PhoneNumber);
                h.PhoneNumber.Enabled = false;
                h.PhoneNumber.Background = null;
                h.PhoneNumber.ClearFocus();
                h.PhoneNumber.SetCursorVisible(false);
                h.PhoneNumber.Focusable = false;

                if (numbers[position].Equals("Not Available"))
                {
                    h.CallButton.Visibility = ViewStates.Gone;
                }
                else
                {
                    h.CallButton.Visibility = ViewStates.Visible;
                    h.CallButton.Click += delegate
                    {
                        try
                        {
                            string call = "tel:" + numbers[position];
                            var geoUri = Android.Net.Uri.Parse(call);
                            var mapIntent = new Intent(Intent.ActionView, geoUri);
                            mActivity.StartActivity(mapIntent);
                        }
                        catch (Exception e)
                        {
                            Utility.LoggingNonFatalError(e);
                        }
                    };
                }
            }
            catch (Exception e)
            {
                Utility.LoggingNonFatalError(e);

            }

        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var id = Resource.Layout.PhoneListItemView;
            var itemView = LayoutInflater.From(parent.Context).Inflate(id, parent, false);
            return new PhoneListViewHolder(itemView);
        }

        public class PhoneListViewHolder : RecyclerView.ViewHolder
        {
            public TextInputLayout PhoneLabel { get; private set; }
            public EditText PhoneNumber { get; private set; }
            public ImageButton CallButton { get; private set; }

            public PhoneListViewHolder(View itemView) : base(itemView)
            {
                PhoneLabel = itemView.FindViewById<TextInputLayout>(Resource.Id.phone_label);
                PhoneNumber = itemView.FindViewById<EditText>(Resource.Id.phone_edittext);
                CallButton = itemView.FindViewById<ImageButton>(Resource.Id.img_call);
            }

        }
    }
}