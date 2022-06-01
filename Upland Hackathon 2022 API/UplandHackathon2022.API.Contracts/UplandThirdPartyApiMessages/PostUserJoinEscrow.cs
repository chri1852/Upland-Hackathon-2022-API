using System;
using System.Collections.Generic;
using System.Text;
using UplandHackathon2022.API.Contracts.UplandThirdPartyApiTypes;

namespace UplandHackathon2022.API.Contracts.UplandThirdPartyApiMessages
{
    public class PostUserJoinEscrow
    {
        public int containerId { get; set; }
        public int upxAmount { get; set; }
        public double sparkAmount { get; set; }
        public List<NFTAsset> assets { get; set; }
    }
}
