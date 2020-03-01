using System;
using System.Threading.Tasks;

namespace Api.Auth
{
    public class UserService : IUserService
    {
        public Task UpdateAuthCode(string userId, string code, string scope)
        {
            throw new NotImplementedException();
        }

        public Task<User> Authenticate(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}