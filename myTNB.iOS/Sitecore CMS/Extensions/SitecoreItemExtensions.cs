using System;
using Sitecore.MobileSDK.API.Items;
using System.Linq;
using Sitecore.MobileSDK.API.Request.Parameters;
using System.Collections.Generic;
using System.Xml.Linq;
using myTNB.SitecoreCMS.Services;
using System.Diagnostics;

namespace myTNB.SitecoreCMS.Extensions
{
    public static class SitecoreItemExtensions
    {
        private static readonly string[] SizeList = { "main", "2x", "3x", "hdpi", "mdpi", "xhdpi", "xxhdpi", "xxxhdpi" };

        public static int GetIntValueFromField(this ISitecoreItem item, string fieldName)
        {
            if (item == null)
                return 0;

            int.TryParse(item[fieldName].RawValue, out int parsedValue);
            return parsedValue;
        }

        public static string GetValueFromField(this ISitecoreItem item, string fieldName)
        {
            if (item == null)
                return string.Empty;

            return item[fieldName].RawValue;
        }

        public static string GetTemplateName(this ISitecoreItem item)
        {
            if (item == null)
                return string.Empty;

            return item.Template.Substring(item.Template.LastIndexOf("/", System.StringComparison.Ordinal) + 1);
        }

        public static bool GetCheckBoxValueFromField(this ISitecoreItem item, string fieldName)
        {
            if (item == null)
                return false;

            return item[fieldName].RawValue == "1";
        }

        public static string GetDateValueFromField(this ISitecoreItem item, string fieldName)
        {
            if (item == null)
                return string.Empty;

            var str = item[fieldName].RawValue;
            int dateLength = 8;

            if (!string.IsNullOrEmpty(str) && str.Length >= dateLength)
            {
                return item[fieldName].RawValue.Substring(0, dateLength);
            }

            return string.Empty;
        }

        public static string GetImageIDFromMediaField(this ISitecoreItem item, string mediafieldName)
        {
            XElement xmlElement = GetXElement(item, mediafieldName);

            if (xmlElement == null)
                return String.Empty;

            XAttribute attribute = xmlElement.Attributes().FirstOrDefault(attr => attr.Name == "mediaid");

            string mediaId = attribute.Value;

            Guid id = Guid.Parse(mediaId);

            return mediaId;
        }

        public static string GetImageUrlFromMediaField(this ISitecoreItem item, string fieldNameOrSize, string websiteUrl = null, bool hasSize = true)
        {
            string mediafieldName = hasSize ? GetImageFieldName(fieldNameOrSize) : fieldNameOrSize;
            XElement xmlElement = GetXElement(item, mediafieldName);

            if (xmlElement == null)
                return String.Empty;

            XAttribute attribute = xmlElement.Attributes().FirstOrDefault(attr => attr.Name == "mediaid");

            string mediaId = attribute.Value;

            Guid id = Guid.Parse(mediaId);

            if (string.IsNullOrWhiteSpace(websiteUrl))
                return String.Format("-/media/{0}.ashx", id.ToString("N")).Replace(" ", "%20");

            return String.Format("{0}/-/media/{1}.ashx", websiteUrl, id.ToString("N")).Replace(" ", "%20");
        }

        public static string GetImageUrlFromMediaId(this string mediaId, string websiteUrl = null)
        {
            Guid id = Guid.Parse(mediaId);

            if (string.IsNullOrWhiteSpace(websiteUrl))
                return String.Format("-/media/{0}.ashx", id.ToString("N"));

            return String.Format("{0}/-/media/{1}.ashx", websiteUrl, id.ToString("N"));
        }

        public static string GetImageUrlFromMediaPath(this string mediaPath, string websiteUrl = null)
        {
            string theString = mediaPath;
            var array = theString.Split('/', '2');
            var newPath = String.Join("/", array);

            if (string.IsNullOrWhiteSpace(websiteUrl))
                return String.Format("-/media/{0}.ashx", newPath);

            return String.Format("{0}/-/media/{1}.ashx", websiteUrl, newPath);
        }

        public static string GetImageUrlFromItemWithSize(this ISitecoreItem item, string mediafieldName, string OS, string imageSize = null, string websiteUrl = null, string language = "en")
        {
            var imageUrl = String.Empty;
            bool isIos = false;
            if (OS.ToLower() == "ios")
            {
                isIos = true;
            }

            bool isMain = false;
            if (imageSize.ToLower() == "main")
            {
                isMain = true;
            }

            if (String.IsNullOrEmpty(imageSize))
            {
                return item.GetImageUrlFromMediaField(mediafieldName);
            }

            var imageId = item.GetImageIDFromMediaField(mediafieldName);
            try
            {
                SitecoreService sitecoreService = new SitecoreService();
                if (!String.IsNullOrEmpty(imageId))
                {
                    var imageRequest = sitecoreService.GetItemById(imageId, PayloadType.Content, new List<ScopeType> { ScopeType.Parent, ScopeType.Self }, websiteUrl, language);
                    var imagePath = String.Empty;
                    if (imageRequest.Result.TotalCount > 0)
                    {
                        if (isIos)
                        {
                            imagePath = imageRequest.Result[1].Path;
                            if (!isMain)
                            {
                                if (imagePath.Contains("main"))
                                {
                                    imagePath = imagePath.Replace("main", imageSize.ToLower());
                                }
                                else
                                {
                                    var imagePathArray = imagePath.Split('/');
                                    var imageSizeReplace = imagePathArray.Skip(imagePathArray.Length - 2).First();
                                    if (SizeList.Any(imageSizeReplace.Contains))
                                    {
                                        imagePath = imagePath.Replace(imageSizeReplace, imageSize.ToLower());
                                    }
                                }
                            }
                        }
                        else
                        {
                            imagePath = imageRequest.Result[1].Path.ToLower();
                            imagePath = imagePath.Replace("ios", OS.ToLower());
                            var imagePathArray = imagePath.Split('/');
                            var imageSizeReplace = imagePathArray.Skip(imagePathArray.Length - 2).First();
                            if (SizeList.Any(imageSizeReplace.Contains))
                            {
                                imagePath = imagePath.Replace(imageSizeReplace, imageSize.ToLower());
                            }
                        }

                        var mediaUrlArray = imagePath.Split('/');
                        mediaUrlArray = mediaUrlArray.Skip(3).ToArray();
                        imageUrl = String.Join("/", mediaUrlArray);

                        if (string.IsNullOrWhiteSpace(websiteUrl))
                            return String.Format("-/media/{0}.ashx", imageUrl);

                        return String.Format("{0}/-/media/{1}.ashx", websiteUrl, imageUrl);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: " + e.Message);
                return null;
            }

            return item.GetImageUrlFromMediaField("Image");
        }

        public static string GetItemIdFromLinkField(this ISitecoreItem item, string linkFieldName)
        {
            XElement xmlElement = GetXElement(item, linkFieldName);

            if (xmlElement == null)
                return string.Empty;

            XAttribute linkTypeAttribute = xmlElement.Attributes().FirstOrDefault(attr => attr.Name == "linktype");

            if (linkTypeAttribute.Value == "internal")
            {
                XAttribute attribute = xmlElement.Attributes().FirstOrDefault(attr => attr.Name == "id");

                return attribute.Value;
            }
            else if (linkTypeAttribute.Value == "external")
            {
                XAttribute attribute = xmlElement.Attributes().FirstOrDefault(attr => attr.Name == "url");

                return attribute.Value;
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetTextFromLinkField(this ISitecoreItem item, string linkFieldName)
        {
            XElement xmlElement = GetXElement(item, linkFieldName);

            if (xmlElement == null)
                return string.Empty;

            XAttribute attribute = xmlElement.Attributes().FirstOrDefault(attr => attr.Name == "text");

            return attribute.Value;
        }

        public static string GetFieldValueFromDropLink(this ISitecoreItem item, string fieldName, string droplinkFieldName, string websiteUrl = null, string language = "en")
        {
            var ID = item.GetValueFromField(fieldName);
            var droplinkFieldNameValue = String.Empty;
            SitecoreService sitecoreService = new SitecoreService();
            try
            {
                var itemReq = sitecoreService.GetItemById(ID, PayloadType.Content, new List<ScopeType> { ScopeType.Self }, websiteUrl, language);
                foreach (var itemDropLink in itemReq.Result)
                {
                    droplinkFieldNameValue = itemDropLink.GetValueFromField(droplinkFieldName);
                }

                return droplinkFieldNameValue;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: " + e.Message);
                return string.Empty;
            }
        }

        public static List<string> GetFieldValueFromMultilist(this ISitecoreItem item, string fieldName, string multilistFieldName, string websiteUrl = null, string language = "en")
        {
            var IDs = item.GetValueFromField(fieldName);
            var IDsSplit = IDs.Split('|');
            var multilistFieldNameValue = String.Empty;
            List<string> multilistFieldNameList = new List<string>();

            foreach (var ID in IDsSplit)
            {
                SitecoreService sitecoreService = new SitecoreService();

                try
                {
                    var itemReq = sitecoreService.GetItemById(ID, PayloadType.Content, new List<ScopeType> { ScopeType.Self }, websiteUrl, language);
                    foreach (var itemMultilist in itemReq.Result)
                    {
                        multilistFieldNameValue = itemMultilist.GetValueFromField(multilistFieldName);
                    }

                    multilistFieldNameList.Add(multilistFieldNameValue);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception: " + e.Message);
                    return new List<string>();
                }
            }
            return multilistFieldNameList;
        }

        private static XElement GetXElement(this ISitecoreItem item, string fieldName)
        {
            if (item == null)
                return null;

            if (item.Fields.All(f => f.Name != fieldName))
                return null;

            string fieldValue = item[fieldName].RawValue;

            if (string.IsNullOrWhiteSpace(fieldValue))
                return null;

            return XElement.Parse(fieldValue);
        }

        public static string GetFileURLFromFieldName(this ISitecoreItem item, string fieldName, string websiteUrl = null)
        {
            XElement xmlElement = GetXElement(item, fieldName);

            if (xmlElement == null) { return string.Empty; }

            XAttribute attribute = xmlElement.Attributes().FirstOrDefault(attr => attr.Name == "mediaid");
            string mediaId = attribute.Value;
            Guid id = Guid.Parse(mediaId);

            if (string.IsNullOrWhiteSpace(websiteUrl))
            {
                return string.Format("-/media/{0}.ashx", id.ToString("N")).Replace(" ", "%20");
            }
            return string.Format("{0}/-/media/{1}.ashx", websiteUrl, id.ToString("N")).Replace(" ", "%20");
        }

        public static string GetImageUrlFromExtractedUrl(this ISitecoreItem item, string extractedUrl, string websiteUrl = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(websiteUrl))
                    return extractedUrl.Replace(" ", "%20");
                return String.Format("{0}/{1}", websiteUrl, extractedUrl).Replace(" ", "%20");
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string GetImageFieldName(string imgSize)
        {
            switch (imgSize)
            {
                case "2x":
                    return Constants.Sitecore.Fields.ImageName.Image_2X;
                case "3x":
                    return Constants.Sitecore.Fields.ImageName.Image_3X;
                case "hdpi":
                    return Constants.Sitecore.Fields.ImageName.Image_HDPI;
                case "mdpi":
                    return Constants.Sitecore.Fields.ImageName.Image_MDPI;
                case "xhdpi":
                    return Constants.Sitecore.Fields.ImageName.Image_XHDPI;
                case "xxhdpi":
                    return Constants.Sitecore.Fields.ImageName.Image_XXHDPI;
                case "xxxhdpi":
                    return Constants.Sitecore.Fields.ImageName.Image_XXXHDPI;
                default:
                    return Constants.Sitecore.Fields.ImageName.Image;
            }
        }
    }
}