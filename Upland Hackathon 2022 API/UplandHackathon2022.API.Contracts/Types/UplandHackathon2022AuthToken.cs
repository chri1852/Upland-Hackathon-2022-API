namespace UplandHackathon2022.API.Contracts.Types
{
    // TODO CHANGE ME
    public class UplandHackathon2022AuthToken
    {
        public int RegisteredUserId { get; set; }
        public string UplandUsername { get; set; }
        public string PasswordHash { get; set; }
        public bool WebVerified { get; set; }
    }
}
