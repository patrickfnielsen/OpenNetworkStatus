namespace OpenNetworkStatus.Data.Entities
{
    public class User : AuditEntityBase
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }
}
