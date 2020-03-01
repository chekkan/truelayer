using System.Threading.Tasks;
using Api.Auth;

namespace Api.TrueLayer
{
    public interface ITrueLayerDataApiClient
    {
        Task<AccessTokenResponse> GetAccessToken(string code);
    }
}