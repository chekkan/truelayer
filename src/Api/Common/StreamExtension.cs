using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Api.Common
{
    public static class StreamExtension
    {
        public static ValueTask<T> ReadAsJson<T>(this Stream stream)
        {
            return JsonSerializer.DeserializeAsync<T>(stream, new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeCaseNamingPolicy() 
            });
        }
    }
}