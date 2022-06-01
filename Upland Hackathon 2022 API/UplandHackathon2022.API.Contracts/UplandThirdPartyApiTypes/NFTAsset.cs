using System;
using System.Collections.Generic;
using System.Text;

namespace UplandHackathon2022.API.Contracts.UplandThirdPartyApiTypes
{
    public class NFTAsset
    {
        public int id { get; set; }
        public string category { get; set; }
        public string name { get; set; }
        public string thumbnail { get; set; }

        public int transactionId { get; set; }
        public string ownerEosId { get; set; }
        public string status { get; set; }
        public int assetId { get; set; }
    }
}
