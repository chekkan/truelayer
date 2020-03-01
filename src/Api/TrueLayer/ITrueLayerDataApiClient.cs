using System.Threading.Tasks;
using Api.Auth;
using Api.Controllers;

namespace Api.TrueLayer
{
    public interface ITrueLayerDataApiClient
    {
        Task<AccessTokenResponse> GetAccessToken(string code);
    }
}