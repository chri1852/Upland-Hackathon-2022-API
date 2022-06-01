using System;
using System.Collections.Generic;
using UplandHackathon2022.API.Contracts.Types;

namespace UplandHackathon2022.API.Contracts.UplandThirdPartyApiMessages
{
    public class GetUserProfileResponse
    {
        public Guid id { get; set; }
        public string eosId { get; set; }
        public string username { get; set; }
        public double networth { get; set; }
        public string level { get; set; }
        public string avatarUrl { get; set; }
        public string initialCity { get; set; }
        public string currentCity { get; set; }

        public List<BattleAsset> battleAssets { get; set; }
    }
}
