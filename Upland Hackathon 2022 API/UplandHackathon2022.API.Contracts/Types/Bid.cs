using System;
using System.Collections.Generic;
using System.Text;

namespace UplandHackathon2022.API.Contracts.Types
{
    public class Bid
    {
        public int Id { get; set; }
        public int ContainerId { get; set; }
        public int UserId { get; set; }
        public int AuctionId { get; set; }
        public DateTime BidDateTime { get; set; }
        public int Amount { get; set; }
        public bool Winner { get; set; }
        public int Fee { get; set; }

        public string UplandUsername { get; set; }
    }
}
