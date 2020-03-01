using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    // [Controller]
    // [Route("/v1/accounts")]
    // [Authorize]
    // public class AccountsController : ControllerBase
    // {
    //     private readonly ITransactionReader _transactionTransactionReader;
    //     private readonly IAccountReader _accountReader;
    //
    //     public AccountsController(ITransactionReader transactionReader, IAccountReader accountReader)
    //     {
    //         _transactionTransactionReader = transactionReader;
    //         _accountReader = accountReader;
    //     }
    //
    //     [HttpGet]
    //     public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
    //     {
    //         var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //         var collection = await _accountReader.GetAll(new Guid(userId));
    //         Response.Headers["X-Total-Count"] = collection.Count.ToString();
    //         return Ok(collection.Items);
    //     }
    //
    //     [HttpGet("{accountId}/transactions")]
    //     public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions(string accountId)
    //     {
    //         var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //         var collection = await _transactionTransactionReader.GetAll(new Guid(userId), accountId);
    //         Response.Headers["X-Total-Count"] = collection.Count.ToString();
    //         return Ok(collection.Items);
    //     }
    // }

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