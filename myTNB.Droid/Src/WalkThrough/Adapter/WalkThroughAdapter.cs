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

using Android.Support.V4.View;
using Android.Support.V4.App;
using myTNB.Droid.Models;
using myTNB_Android.Src.WalkThrough.Fragment;

namespace myTNB_Android.Src.WalkThrough.Adapter
{
    class WalkThroughAdapter : FragmentPagerAdapter
    {

        Context context;
        public WalkThroughDeck walkThroughDeck;

        public WalkThroughAdapter(Android.Support.V4.App.FragmentManager fm, WalkThroughDeck deck) : base(fm)
        {
            this.walkThroughDeck = deck;
        }

        public override int Count
        {
            get {  return this.walkThroughDeck.NumCards; }
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return WalkThroughFragment.newInstance(walkThroughDeck[position].imageId, walkThroughDeck[position].imageUrl,
            walkThroughDeck[position].Heading, walkThroughDeck[position].Content);
        }
    }
}