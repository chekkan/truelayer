using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Models;

namespace Api.Application
{
    public interface ITransactionReader
    {
        Task<ResourceCollection<Transaction>> GetAll(Guid userId, string accountId, DateTimeOffset from,
            DateTimeOffset to);

        Task<ResourceCollection<Transaction>> GetAll(Guid userId, DateTimeOffset from, DateTimeOffset to);
        Task<IDictionary<string, decimal>> GetSummary(Guid userId, DateTimeOffset from, DateTimeOffset to);
    }
}