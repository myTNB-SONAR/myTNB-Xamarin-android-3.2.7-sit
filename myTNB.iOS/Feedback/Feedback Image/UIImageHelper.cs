using System;
using System.Diagnostics;
using CoreGraphics;
using Foundation;
using UIKit;

namespace myTNB.Home.Feedback
{
    public class UIImageHelper
    {
        public string ConvertImageToHex(UIImage image)
        {
            try
            {
                NSData imageData = image.AsJPEG();
                string base64String = imageData.GetBase64EncodedString(
                    NSDataBase64EncodingOptions.SixtyFourCharacterLineLength
                );
                byte[] byteArray = Convert.FromBase64String(base64String);
                string hex = BitConverter.ToString(byteArray).Replace("-", string.Empty);
                return hex;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return string.Empty;
            }
        }

        public double GetImageFileSize(UIImage image)
        {
            NSData imageData = image.AsJPEG();
            double fileSize = (Double)imageData.Length / 1024;
            return Math.Ceiling(fileSize);
        }

        public UIImage ConvertHexToUIImage(string hex)
        {
            try
            {
                byte[] byteArray = ConvertHexToByteArray(hex.Replace("-", ""));
                string base64String = Convert.ToBase64String(byteArray);
                NSData imageData = new NSData(
                    base64String
                    , NSDataBase64DecodingOptions.IgnoreUnknownCharacters
                );
                UIImage image = UIImage.LoadFromData(imageData);
                return image;
            }
            catch (MonoTouchException m)
            {
                Debug.WriteLine(m.Message);
                return UIImage.FromBundle(string.Empty);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return UIImage.FromBundle(string.Empty);
            }
        }

        public UIImage ResizeImage(UIImage image)
        {
            UIGraphics.BeginImageContextWithOptions(new CGSize(image.Size.Width / 5, image.Size.Height / 5), false, 0.0f);
            image.Draw(new CGRect(0, 0, image.Size.Width / 5, image.Size.Height / 5));
            UIImage newImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return newImage;
        }

        byte[] ConvertHexToByteArray(string hex)
        {
            if (hex.Length % 2 == 1)
            {
                return new byte[] { };
            }
            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexValue(hex[i << 1]) << 4) + (GetHexValue(hex[(i << 1) + 1])));
            }
            return arr;
        }

        int GetHexValue(char hex)
        {
            int val = (int)hex;
            return val - (val < 58 ? 48 : 55);
        }
    }
}