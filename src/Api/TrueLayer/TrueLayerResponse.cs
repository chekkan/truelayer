using System.Collections.Generic;

namespace Api.TrueLayer
{
    public class TrueLayerResponse<T>
    {
        public IEnumerable<T> Results { get; set; }
    }
}