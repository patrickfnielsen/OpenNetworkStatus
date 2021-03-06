namespace OpenNetworkStatus.Models.Options
{
    public class JwtOptions
    {
        public string IssuerSigningKey { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public int TokenLifetimeMinutes { get; set; } = 60;
    }
}