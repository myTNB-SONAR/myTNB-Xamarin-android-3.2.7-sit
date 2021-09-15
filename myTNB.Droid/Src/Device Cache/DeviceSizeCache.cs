namespace myTNB_Android.Src
{
    internal static class DeviceSizeCache
    {
        private static float _fontScale = 0;

        internal static float FontScale
        {
            set
            {
                if (_fontScale == 0)
                {
                    _fontScale = value >= 1.3F ? 1.3F : value;
                }
            }
            get
            {
                return _fontScale;
            }
        }
    }
}