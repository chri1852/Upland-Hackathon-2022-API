using System;
using System.Collections.Generic;
using System.Text;

namespace UplandHackathon2022.API.Contracts.Types
{
    public class BattleAsset
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public string AssetCategory { get; set; }
        public string AssetName { get; set; }
        public string Thumbnail { get; set; }
        public int RockSkill { get; set; }
        public int PaperSkill { get; set; }
        public int SissorsSkill { get; set; }

        public bool IsTraining { get; set; }
        public bool IsBattling { get; set; }
    }
}
