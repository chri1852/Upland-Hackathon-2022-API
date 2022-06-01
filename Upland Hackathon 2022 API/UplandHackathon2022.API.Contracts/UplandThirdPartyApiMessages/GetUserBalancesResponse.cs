namespace UplandHackathon2022.API.Contracts.UplandThirdPartyApiMessages
{
    public class GetUserBalancesResponse
    {
        public int availableUpx { get; set; }
        public double availableSpark { get; set; }
        public double stakedSpark { get; set; }
    }
}
