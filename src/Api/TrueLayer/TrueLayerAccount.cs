namespace Api.TrueLayer
{
    public class TrueLayerAccount
    {
        public string AccountId { get; set; }

        public string AccountType { get; set; }

        public string DisplayName { get; set; }

        public string Currency { get; set; }

        public AccountProvider Provider { get; set; }

        public class AccountProvider
        {
            public string DisplayName { get; set; }
            public string ProviderId { get; set; }
        }
    }
}