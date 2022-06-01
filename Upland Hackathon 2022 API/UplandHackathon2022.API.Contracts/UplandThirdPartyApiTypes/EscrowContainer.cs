using System;
using System.Collections.Generic;
using System.Text;

namespace UplandHackathon2022.API.Contracts.UplandThirdPartyApiTypes
{
    public class EscrowContainer
    {
        public int id { get; set; }
        public int appId { get; set; }
        public string status { get; set; }
        public DateTime expirationDate { get; set; }
        public int upx { get; set; }
        public double spark { get; set; }
        public List<NFTAsset> assets { get; set; }
    }
}
