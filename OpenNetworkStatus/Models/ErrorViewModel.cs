namespace OpenNetworkStatus.Models
{
    public struct ErrorViewModel
    {
        public int StatusCode { get; }

        public string Title { get; }

        public string Message { get; }

        public string RequestId { get; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public ErrorViewModel(int statusCode, string title, string message, string requestId = null)
        {
            StatusCode = statusCode;
            Title = title;
            Message = message;
            RequestId = requestId;
        }
    }
}