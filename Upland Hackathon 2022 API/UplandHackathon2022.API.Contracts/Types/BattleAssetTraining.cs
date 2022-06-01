using System;
using System.Collections.Generic;
using System.Text;

namespace UplandHackathon2022.API.Contracts.Types
{
    public class BattleAssetTraining
    {
        public int Id { get; set; }
        public int BattleAssetId { get; set; }
        public int ContainerId { get; set; }
        public string SkillTraining { get; set; }
        public DateTime FinishedTime { get; set; }
        public DateTime MustAcceptBy { get; set; }
        public int UPXFee { get; set; }
        public bool Resolved { get; set; }
        public bool Accepted { get; set; }
    }
}
