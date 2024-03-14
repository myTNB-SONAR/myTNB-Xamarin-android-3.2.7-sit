using System;
using Android.Runtime;
using Com.Davemorrissey.Labs.Subscaleview.Decoder;
using Java.IO;

namespace myTNB.Android.Src.Utils.PDFView
{
    public class PDFDecodeFactory : PDFRegionDecoder, IDecoderFactory
    {
        public PDFDecodeFactory() : base()
        {

        }

        public PDFDecodeFactory(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {

        }

        public Java.Lang.Object Make()
        {
            return this;
        }
    }
}
