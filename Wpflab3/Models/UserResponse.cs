namespace Wpflab3.Models
{
    class UserResponse
    {
        public string? access_token { get; set; }
        public string? username { get; set; }

        public UserResponse(string? access_token, string? username)
        {
            this.access_token = access_token;
            this.username = username;
        }
    }
}