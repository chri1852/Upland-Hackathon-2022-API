using System;
using System.Collections.Generic;
using System.Text;

namespace UplandHackathon2022.API.Contracts.UplandThirdPartyApiTypes
{
    public class EscrowAction
    {
        public int? assetId { get; set; }
        public int? amount { get; set; }
        public string category { get; set; }
        public string targetEosId { get; set; }
    }
}
