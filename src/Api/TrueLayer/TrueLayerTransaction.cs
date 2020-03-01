using System;
using System.Collections.Generic;

namespace Api.TrueLayer
{
    public class TrueLayerTransaction
    {
        public string TransactionId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string TransactionType { get; set; }
        public string TransactionCategory { get; set; }
        public string MerchantName { get; set; }
        public IEnumerable<string> TransactionClassification { get; set; }
    }
}