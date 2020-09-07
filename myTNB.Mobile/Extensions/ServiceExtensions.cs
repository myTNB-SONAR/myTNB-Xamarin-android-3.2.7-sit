namespace myTNB
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// This is a service name extension
        /// </summary>
        /// <param name="serviceName">Name of the service</param>
        /// <param name="statusCode">Status code returned from the Service Response</param>
        /// <returns>Mapped status code details</returns>
        public static ServiceStatus GetStatusDetails(this string serviceName, string statusCode)
        {
            return ServiceMappingManager.Instance.GetStatusDetails(serviceName, statusCode);
        }
    }
}