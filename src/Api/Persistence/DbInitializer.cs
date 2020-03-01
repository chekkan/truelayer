using System;
using System.Linq;

namespace Api.Persistence
{
    public static class DbInitializer
    {
        public static void Initialize(PaymentsChallengeContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return; // DB has been seeded
            }

            var users = new[]
            {
                new User {Id = new Guid("16D9A0FB-BAC6-4859-93D9-49FE019126F6"), Username = "john", Password = "doe"},
                new User {Id = new Guid("68223EDD-015B-40CD-A107-B6D43609B12C"), Username = "john1", Password = "doe1"},
                new User {Id = new Guid("8D025351-35C9-4B0E-8F51-1F48A8D007F2"), Username = "john2", Password = "doe2"}
            };
            foreach (var user in users)
            {
                context.Users.Add(user);
            }

            context.SaveChanges();
        }
    }
}