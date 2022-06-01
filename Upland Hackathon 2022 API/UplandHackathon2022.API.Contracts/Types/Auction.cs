using System;
using System.Collections.Generic;
using System.Text;

namespace UplandHackathon2022.API.Contracts.Types
{
    public class Auction
    {
        public int Id { get; set; }
        public int ContainerId { get; set; }
        public int UserId { get; set; }
        public DateTime AuctionEndDate { get; set; }
        public DateTime ContainerExpirationDate { get; set; }
        public decimal Spark { get; set; }
        public int Reserve { get; set; }
        public bool Ended { get; set; }
        public int Fee { get; set; }

        public List<Bid> Bids { get; set; }
        public List<AuctionAsset> Assets { get; set; }

        public string UplandUsername { get; set; }
    }
}
