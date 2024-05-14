namespace DevEpos.CF.Demo.Common {
    public class ServiceException : Exception {
        public ServiceException(int statusCode, string message, string? detail = null, string? innerMessage = null)
            : base(message) =>
                (StatusCode, Detail, InnerMessage) = (statusCode, detail, innerMessage);
        public string? InnerMessage { get; }
        public int? StatusCode { get; }
        public string? Detail { get; }
    }
}
