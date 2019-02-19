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
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;
using myTNB_Android.Src.Base.Activity;
using myTNB_Android.Src.Utils;

namespace myTNB_Android.Src.FindUs.Adapter
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
            PhoneListViewHolder h = holder as PhoneListViewHolder;
            if (numbers.Count > 1)
            {
                int count = position + 1;
                h.PhoneLabel.Hint = "PHONE" + count;
            }
            else
            {
                h.PhoneLabel.Hint = "PHONE";
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
                h.CallButton.Click += delegate {
                    try
                    {
                        string call = "tel:" + numbers[position];
                        var geoUri = Android.Net.Uri.Parse(call);
                        var mapIntent = new Intent(Intent.ActionView, geoUri);
                        mActivity.StartActivity(mapIntent);
                    }
                    catch (Exception e)
                    {

                    }
                };
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