using System;
using System.Threading.Tasks;
using Api.Application;
using Api.Controllers;
using Api.Persistence;
using Microsoft.EntityFrameworkCore;
using User = Api.Controllers.User;

namespace Api.Auth
{
    public class UserService : IUserService
    {
        private readonly PaymentsChallengeContext _dbContext;

        public UserService(PaymentsChallengeContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task UpdateAccessToken(Guid userId, AccessTokenResponse accessToken)
        {
            var userDao = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.Id == userId);
            if (userDao == null)
                return;

            userDao.AccessToken = accessToken.AccessToken;
            userDao.RefreshToken = accessToken.RefreshToken;
            userDao.AccessExpiresAt = DateTime.UtcNow.AddSeconds(accessToken.ExpiresIn);

            _dbContext.Update(userDao);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User> Authenticate(string username, string password)
        {
            var userDao = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.Username == username && u.Password == password);
            return ToUser(userDao);
        }

        public async Task<User> GetById(Guid userId)
        {
            var userDao = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == userId);
            return ToUser(userDao);
        }

        public async Task<AuthCredential> GetAuthCredentials(Guid userId)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == userId);
            return new AuthCredential
            {
                AccessToken = user.AccessToken,
                RefreshToken = user.RefreshToken,
                ExpiresAt = user.AccessExpiresAt
            };
        }

        public async Task SaveCredential(Guid userId, AuthCredential credential)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == userId);
            user.AccessToken = credential.AccessToken;
            user.AccessExpiresAt = credential.ExpiresAt;
            user.RefreshToken = credential.RefreshToken;
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        private static User ToUser(Api.Persistence.User userDao)
        {
            return userDao == null
                ? null
                : new User
                {
                    Id = userDao.Id,
                    Username = userDao.Username
                };
        }
    }
}