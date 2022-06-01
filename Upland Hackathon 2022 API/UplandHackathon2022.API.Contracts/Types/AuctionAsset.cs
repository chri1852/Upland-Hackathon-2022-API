using System;
using System.Collections.Generic;
using System.Text;

namespace UplandHackathon2022.API.Contracts.Types
{
    public class AuctionAsset
    {
        public int Id { get; set; }
        public int AuctionId { get; set; }
        public int AssetId { get; set; }
        public string AssetCategory { get; set; }
        public string AssetName { get; set; }
        public string Thumbnail { get; set; }
    }
}
