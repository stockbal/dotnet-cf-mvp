namespace DevEpos.CF.Demo.Common {
    public class HttpResponseException : Exception {
        public HttpResponseException(int statusCode, string? message = null)
            : base(message, null) =>
                (StatusCode) = (statusCode);

        public HttpResponseException(int statusCode, string? taskId = null, string? message = null)
            : base(message, null) =>
                (StatusCode, TaskId) = (statusCode, taskId);

        public HttpResponseException(int statusCode, string? message = null, Exception? innerException = null)
            : base(message, innerException) =>
                (StatusCode) = (statusCode);

        public HttpResponseException(int statusCode, string? taskId = null, string? message = null, Exception? innerException = null)
            : base(message, innerException) =>
                (StatusCode, TaskId) = (statusCode, taskId);

        public string? TaskId { get; }
        public int StatusCode { get; }

    }
}
