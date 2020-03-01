using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Models;

namespace Api.Controllers
{
    public interface IAccountReader
    {
        Task<ResourceCollection<Account>> GetAll(Guid userId);
    }

    public class Account
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string ProviderId { get; set; }
        public string ProviderName { get; set; }
    }

    public interface ITransactionReader
    {
        Task<ResourceCollection<Transaction>> GetAll(Guid userId, string accountId, DateTimeOffset from,
            DateTimeOffset to);

        Task<ResourceCollection<Transaction>> GetAll(Guid userId, DateTimeOffset from, DateTimeOffset to);
        Task<IDictionary<string, decimal>> GetSummary(Guid userId, DateTimeOffset from, DateTimeOffset to);
    }

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