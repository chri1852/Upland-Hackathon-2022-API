using System;
using System.Collections.Generic;
using System.Text;
using UplandHackathon2022.API.Contracts.Types;

namespace UplandHackaton2022.Api.Abstractions
{
    public interface ILocalRepository
    {
        RegisteredUser GetUserByUplandUsername(string uplandUsername);
        RegisteredUser GetUserByAccessCode(string accessCode);
        void UpsertUser(RegisteredUser registeredUser);
        void UpsertAuction(Auction auction);
        void UpsertAuctionAsset(AuctionAsset auctionAsset);
        void UpserBid(Bid bid);
        Auction GetAuctionById(int auctionId);
        List<int> GetAllOpenAuctionIds();
        int GetAuctionIdByContainerId(int containerId);
        int UpsertBattleAsset(BattleAsset asset);
        BattleAsset GetBattleAssetByAssetId(int assetId);
        BattleAsset GetBattleAssetByBattleAssetId(int battleAssetId);
        int UpsertBattleAssetTraining(BattleAssetTraining battleAssetTraining);
        BattleAssetTraining GetBattleAssetTrainingById(int battleAssetTrainingId);
        List<BattleAssetTraining> GetAllExpiredBattleAssetTrainings();
        List<BattleAssetTraining> GetAllFinishedBattleAssetTrainings();
        List<BattleAssetTraining> GetAllApprovedNotFinishedBattleAssetTrainings();
        int IsBattleAssetTraining(int battleAssetId);
        int UpsertBattle(Battle battle);
        List<Battle> GetAllUnresolvedBattles();
        List<Battle> GetAllNeedingChallengers();
        List<Battle> GetAllBattlesResolvedByBattleAssetId(int battleAssetId);
        List<Battle> GetAllNeedingApproval();
        Battle GetBattleById(int battleId);
        bool IsBattling(int battleAssetId);
    }
}
