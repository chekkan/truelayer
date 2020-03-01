using Microsoft.EntityFrameworkCore;

namespace Api.TrueLayer.Persistence
{
    public class PaymentsChallengeContext : DbContext
    {
        public PaymentsChallengeContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}