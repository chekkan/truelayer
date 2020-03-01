using System;
using System.Collections.Generic;

namespace Api.Models
{
    public class Transaction
    {
        public string Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public string MerchantName { get; set; }
        public IEnumerable<string> Classifications { get; set; }
    }
}