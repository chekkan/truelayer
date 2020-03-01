using System;
using System.Threading.Tasks;
using Api.Controllers;

namespace Api.Application
{
    public interface IUserService
    {
        Task UpdateAccessToken(Guid userId, AccessTokenResponse accessToken);
        Task<User> Authenticate(string username, string password);
        Task<User> GetById(Guid userId);
        Task<AuthCredential> GetAuthCredentials(Guid userId);
        Task SaveCredential(Guid userId, AuthCredential credential);
    }
}