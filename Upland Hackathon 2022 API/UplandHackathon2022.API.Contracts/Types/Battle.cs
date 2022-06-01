using System;
using System.Collections.Generic;
using System.Text;

namespace UplandHackathon2022.API.Contracts.Types
{
    public class Battle
    {
        public int Id { get; set; }
        public int ContainerId { get; set; }
        public int OpponentBattleAssetId { get; set; }
        public int? ChallengerBattleAssetId { get; set; }
        public int UPXPerSide { get; set; }
        public DateTime MustBattleBy { get; set; }
        public bool Resolved { get; set; }
        public int? WinnerBattleAssetId { get; set; }
        public string OpponentSkills { get; set; }
        public string? ChallengerSkills { get; set; }
    }
}
