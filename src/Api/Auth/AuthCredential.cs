using System;

namespace Api.Auth
{
    public class AuthCredential
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}