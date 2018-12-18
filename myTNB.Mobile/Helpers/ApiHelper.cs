
namespace myTNB.Mobile
{
    public static class ApiHelper
    {
        /// <summary>
        /// Validates the response item.
        /// </summary>
        /// <returns>The response item.</returns>
        /// <param name="item">Item.</param>
        public static string ValidateResponseItem(string item)
        {
            if (string.IsNullOrEmpty(item) || string.IsNullOrWhiteSpace(item))
            {
                return string.Empty;
            }
            return item;
        }
    }
}
