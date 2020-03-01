using System;
using System.Threading.Tasks;
using Api.TrueLayer.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Api.TrueLayer
{
    public class UserService : IUserService
    {
        private readonly PaymentsChallengeContext _dbContext;

        public UserService(PaymentsChallengeContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task UpdateAuthCode(Guid userId, string code, string scope)
        {
            var userDao = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.Id == userId);
            if (userDao == null)
                return;

            userDao.AuthCode = code;

            _dbContext.Update(userDao);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User> Authenticate(string username, string password)
        {
            var userDao = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.Username == username && u.Password == password);
            if (userDao == null)
                return null;
            return new User
            {
                Id = userDao.Id,
                Username = userDao.Username
            };
        }
    }
}