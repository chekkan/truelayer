using System;

namespace Api.TrueLayer.Persistence
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AuthCode { get; set; }
    }
}