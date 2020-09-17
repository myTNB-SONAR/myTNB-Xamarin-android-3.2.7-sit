namespace myTNB.Mobile.API.Managers.ApplicationStatus
{
    internal static class GetApplicationStatusUtility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="applicationTypeTitle"></param>
        /// <param name="searchTypeTitle"></param>
        /// <param name="searchTerm"></param>
        internal static void Parse(this GetApplicationStatusResponse response
            , string applicationTypeID
            , string applicationTypeTitle
            , string searchTypeTitle
            , string searchTerm)
        {
            response.Content.ApplicationType = applicationTypeTitle;
            //Mark: Add search type and number data
            response.Content.AdditionalInfoList.Add(new TitleValueModel
            {
                Title = searchTypeTitle.ToUpper(),
                Value = searchTerm
            });
            //Mark: Add created on data
            response.Content.AdditionalInfoList.Add(new TitleValueModel
            {
                Title = LanguageManager.Instance.GetPageValueByKey("ApplicationStatusDetails", "creationDate").ToUpper(),
                Value = response.Content.ApplicationDetail.CreatedDateDisplay ?? string.Empty
            });

            response.Content.ApplicationTypeID = applicationTypeID;
        }
    }
}