using System;
using System.Collections.Generic;
using System.Text;
using UplandHackathon2022.API.Contracts.UplandThirdPartyApiTypes;

namespace UplandHackathon2022.API.Contracts.UplandThirdPartyApiMessages
{
    public class PostEscrowResolveRequest
    {
        public List<EscrowAction> actions { get; set; }
    }
}
