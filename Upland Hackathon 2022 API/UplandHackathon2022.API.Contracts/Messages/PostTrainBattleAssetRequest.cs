using System;
using System.Collections.Generic;
using System.Text;
using UplandHackathon2022.API.Contracts.Types;

namespace UplandHackathon2022.API.Contracts.Messages
{
    public class PostTrainBattleAssetRequest
    {
        public BattleAsset BattleAsset { get; set; }
        public string TrainSkill { get; set; }
        public int TimeInHours { get; set; }
    }
}
