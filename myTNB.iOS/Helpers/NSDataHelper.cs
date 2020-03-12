using System;
using System.Diagnostics;
using Foundation;

namespace myTNB
{
    public static class NSDataHelper
    {
        public static byte[] ToByteArray(this NSData data)
        {
            byte[] dataBytes = new byte[data.Length];
            System.Runtime.InteropServices.Marshal.Copy(data.Bytes, dataBytes, 0, Convert.ToInt32(data.Length));
            return dataBytes;
        }

        public static NSData ToNSData(this byte[] byteArray)
        {
            try
            {
                return NSData.FromArray(byteArray);
            }
            catch (MonoTouchException m) { Debug.WriteLine("Error in parsing byte array to nsdata: " + m.Message); }
            catch (Exception e)
            {
                Debug.WriteLine("Error in parsing byte array to nsdata: " + e.Message);
            }
            return null;
        }
    }
}