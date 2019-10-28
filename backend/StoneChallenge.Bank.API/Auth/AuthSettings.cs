namespace StoneChallenge.Bank.API.Auth
{
    public class AuthSettings
    {
        public string Secret { get; set; }
        public int ExpirationHours { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
