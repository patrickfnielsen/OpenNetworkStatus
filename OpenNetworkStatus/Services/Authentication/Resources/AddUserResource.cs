namespace OpenNetworkStatus.Services.Authentication.Resources
{
    public class AddUserResource
    {
        public bool IsSuccess { get; private set; }

        public string Message { get; private set; }

        public UserResource User { get; private set; }

        public AddUserResource(UserResource user, string message = "")
        {
            IsSuccess = string.IsNullOrEmpty(message);
            User = user;
            Message = message;
        }
    }
}
