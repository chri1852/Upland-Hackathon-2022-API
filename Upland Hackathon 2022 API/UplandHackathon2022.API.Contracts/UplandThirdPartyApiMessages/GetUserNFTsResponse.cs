using System.Collections.Generic;
using UplandHackathon2022.API.Contracts.UplandThirdPartyApiTypes;

namespace UplandHackathon2022.API.Contracts.UplandThirdPartyApiMessages
{
    public class GetUserNFTsResponse
    {
        public int currentPage { get; set; }
        public int pageSize { get; set; }
        public int totalResults { get; set; }
        public List<NFTAsset> results { get; set; }
    }
}
