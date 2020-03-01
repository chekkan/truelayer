using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Controllers
{
    [Controller]
    [Route("/v1/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionReader _transactionReader;

        public TransactionsController(ITransactionReader transactionReader)
        {
            _transactionReader = transactionReader;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions(
            [FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to)
        {
            if (!to.HasValue)
            {
                to = DateTimeOffset.Now;
            }

            if (!from.HasValue)
            {
                from = to.Value.AddMonths(-3);
            }

            if (to.Value < from.Value)
            {
                ModelState.AddModelError("from", "`to` must be after `from`");
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var transactions = await _transactionReader
                .GetAll(new Guid(userId), from.Value, to.Value);
            Response.Headers["X-Total-Count"] = transactions.Count.ToString();
            return Ok(transactions.Items);
        }
    }
}