using System;
using System.Collections.Generic;
using System.Text;
using UplandHackathon2022.API.Contracts.Types;

namespace UplandHackathon2022.API.Contracts.Messages
{
    public class PostJoinBattleRequest
    {
        public BattleAsset BattleAsset { get; set; }
        public int BattleId { get; set; }
    }
}
