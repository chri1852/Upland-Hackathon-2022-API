using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using UplandHackathon2022.API.Contracts.Types;
using UplandHackaton2022.Api.Abstractions;

namespace UplandHackathon2022.API.Infrastructure.Repositories
{
    public class LocalRepository : ILocalRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string DbConnectionString;

        public LocalRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            DbConnectionString = _configuration["AppSettings:DatabaseConnectionString"];
        }

        public RegisteredUser GetUserByUplandUsername(string uplandUsername)
        {
            SqlConnection sqlConnection = GetSQLConnector();
            RegisteredUser user = null;
            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "SELECT TOP(1) * FROM [UHN].[User] (NOLOCK) WHERE UplandUsername = '" + uplandUsername + "'";
                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user = new RegisteredUser();

                            user.Id = (int)reader["Id"];
                            user.UplandUsername = (string)reader["UplandUsername"];
                            user.UplandId = ReadNullParameterSafe<Guid>(reader, "UplandId");
                            user.PasswordSalt = ReadNullParameterSafe<string>(reader, "PasswordSalt");
                            user.PasswordHash = ReadNullParameterSafe<string>(reader, "PasswordHash");
                            user.UplandAccessToken = ReadNullParameterSafe<string>(reader, "UplandAccessToken");
                            user.AccessCode = (string)reader["AccessCode"];
                        }
                        reader.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return user;
        }

        public RegisteredUser GetUserByAccessCode(string accessCode)
        {
            SqlConnection sqlConnection = GetSQLConnector();
            RegisteredUser user = null;
            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "SELECT TOP(1) * FROM [UHN].[User] (NOLOCK) WHERE AccessCode = '" + accessCode + "'";
                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user = new RegisteredUser();

                            user.Id = (int)reader["Id"];
                            user.UplandUsername = (string)reader["UplandUsername"];
                            user.UplandId = ReadNullParameterSafe<Guid>(reader, "UplandId");
                            user.PasswordSalt = ReadNullParameterSafe<string>(reader, "PasswordSalt");
                            user.PasswordHash = ReadNullParameterSafe<string>(reader, "PasswordHash");
                            user.UplandAccessToken = ReadNullParameterSafe<string>(reader, "UplandAccessToken");
                            user.AccessCode = (string)reader["AccessCode"];
                        }
                        reader.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return user;
        }

        public void UpsertUser(RegisteredUser registeredUser)
        {
            SqlConnection sqlConnection = GetSQLConnector();

            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.CommandText = "[UHN].[UpsertUser]";
                    sqlCmd.Parameters.Add(new SqlParameter("UplandUsername", registeredUser.UplandUsername));
                    sqlCmd.Parameters.Add(AddNullParmaterSafe<Guid?>("UplandId", registeredUser.UplandId));
                    sqlCmd.Parameters.Add(AddNullParmaterSafe<string>("PasswordSalt", registeredUser.PasswordSalt));
                    sqlCmd.Parameters.Add(AddNullParmaterSafe<string>("PasswordHash", registeredUser.PasswordHash));
                    sqlCmd.Parameters.Add(AddNullParmaterSafe<string>("UplandAccessToken", registeredUser.UplandAccessToken));
                    sqlCmd.Parameters.Add(new SqlParameter("AccessCode", registeredUser.AccessCode));

                    sqlCmd.ExecuteNonQuery();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        public void UpsertAuction(Auction auction)
        {
            SqlConnection sqlConnection = GetSQLConnector();

            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.CommandText = "[UHN].[UpsertAuction]";
                    sqlCmd.Parameters.Add(new SqlParameter("Id", auction.Id));
                    sqlCmd.Parameters.Add(new SqlParameter("ContainerId", auction.ContainerId));
                    sqlCmd.Parameters.Add(new SqlParameter("UserId", auction.UserId));
                    sqlCmd.Parameters.Add(new SqlParameter("AuctionEndDate", auction.AuctionEndDate));
                    sqlCmd.Parameters.Add(new SqlParameter("ContainerExpirationDate", auction.ContainerExpirationDate));
                    sqlCmd.Parameters.Add(new SqlParameter("Spark", auction.Spark));
                    sqlCmd.Parameters.Add(new SqlParameter("Reserve", auction.Reserve));
                    sqlCmd.Parameters.Add(new SqlParameter("Ended", auction.Ended));
                    sqlCmd.Parameters.Add(new SqlParameter("Fee", auction.Fee));

                    sqlCmd.ExecuteNonQuery();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        public void UpsertAuctionAsset(AuctionAsset auctionAsset)
        {
            SqlConnection sqlConnection = GetSQLConnector();

            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.CommandText = "[UHN].[UpsertAuctionAsset]";
                    sqlCmd.Parameters.Add(new SqlParameter("Id", auctionAsset.Id));
                    sqlCmd.Parameters.Add(new SqlParameter("AuctionId", auctionAsset.AuctionId));
                    sqlCmd.Parameters.Add(new SqlParameter("AssetId", auctionAsset.AssetId));
                    sqlCmd.Parameters.Add(new SqlParameter("AssetCategory", auctionAsset.AssetCategory));
                    sqlCmd.Parameters.Add(new SqlParameter("AssetName", auctionAsset.AssetName));
                    sqlCmd.Parameters.Add(new SqlParameter("Thumbnail", auctionAsset.Thumbnail));

                    sqlCmd.ExecuteNonQuery();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        public void UpserBid(Bid bid)
        {
            SqlConnection sqlConnection = GetSQLConnector();

            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.CommandText = "[UHN].[UpsertBid]";
                    sqlCmd.Parameters.Add(new SqlParameter("Id", bid.Id));
                    sqlCmd.Parameters.Add(new SqlParameter("ContainerId", bid.ContainerId));
                    sqlCmd.Parameters.Add(new SqlParameter("UserId", bid.UserId));
                    sqlCmd.Parameters.Add(new SqlParameter("AuctionId", bid.AuctionId));
                    sqlCmd.Parameters.Add(new SqlParameter("BidDateTime", bid.BidDateTime));
                    sqlCmd.Parameters.Add(new SqlParameter("Amount", bid.Amount));
                    sqlCmd.Parameters.Add(new SqlParameter("Winner", bid.Winner));
                    sqlCmd.Parameters.Add(new SqlParameter("Fee", bid.Fee));

                    sqlCmd.ExecuteNonQuery();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        public Auction GetAuctionById(int auctionId)
        {
            SqlConnection sqlConnection = GetSQLConnector();
            Auction auction = null;

            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.CommandText = "[UHN].[GetAuctionById]";
                    sqlCmd.Parameters.Add(new SqlParameter("Id", auctionId));
                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            auction = new Auction();

                            auction.Id = (int)reader["Id"];
                            auction.ContainerId = (int)reader["ContainerId"];
                            auction.UserId = (int)reader["UserId"];
                            auction.AuctionEndDate = (DateTime)reader["AuctionEndDate"];
                            auction.ContainerExpirationDate = (DateTime)reader["ContainerExpirationDate"];
                            auction.Spark = (decimal)reader["Spark"];
                            auction.Reserve = (int)reader["Reserve"];
                            auction.Ended = (bool)reader["Ended"];
                            auction.Fee = (int)reader["Fee"];

                            auction.UplandUsername = (string)reader["UplandUsername"];
                        }

                        reader.NextResult();

                        while (reader.Read())
                        {
                            Bid bid = new Bid();

                            bid.Id = (int)reader["Id"];
                            bid.ContainerId = (int)reader["ContainerId"];
                            bid.UserId = (int)reader["UserId"];
                            bid.AuctionId = (int)reader["AuctionId"];
                            bid.BidDateTime = (DateTime)reader["BidDateTime"];
                            bid.Amount = (int)reader["Amount"];
                            bid.Winner = (bool)reader["Winner"];
                            bid.Fee = (int)reader["Fee"];

                            bid.UplandUsername = (string)reader["UplandUsername"];

                            auction.Bids.Add(bid);
                        }

                        reader.NextResult();

                        while (reader.Read())
                        {
                            AuctionAsset auctionAsset = new AuctionAsset();

                            auctionAsset.Id = (int)reader["Id"];
                            auctionAsset.AuctionId = (int)reader["AuctionId"];
                            auctionAsset.AssetId = (int)reader["AssetId"];
                            auctionAsset.AssetCategory = (string)reader["AssetCategory"];
                            auctionAsset.AssetName = (string)reader["AssetName"];
                            auctionAsset.Thumbnail = (string)reader["Thumbnail"];

                            auction.Assets.Add(auctionAsset);
                        }

                        reader.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return auction;
        }

        public List<int> GetAllOpenAuctionIds()
        {
            SqlConnection sqlConnection = GetSQLConnector();
            List<int> auctionIds = new List<int>();

            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.CommandText = "[UHN].[GetAllOpenAuctionIds]";
                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            auctionIds.Add((int)reader["Id"]);
                        }

                        reader.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return auctionIds;
        }

        public int GetAuctionIdByContainerId(int containerId)
        {
            SqlConnection sqlConnection = GetSQLConnector();
            int auctionId = -1;
            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "SELECT TOP(1) * FROM [UHN].[Auction] (NOLOCK) WHERE ContainerId = " + containerId;
                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            auctionId = (int)reader["Id"];
                        }
                        reader.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return auctionId;
        }

        public int UpsertBattleAsset(BattleAsset asset)
        {
            int newId = -1;
            SqlConnection sqlConnection = GetSQLConnector();

            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.CommandText = "[UHN].[UpsertBattleAsset]";
                    sqlCmd.Parameters.Add(new SqlParameter("Id", asset.Id));
                    sqlCmd.Parameters.Add(new SqlParameter("AssetId", asset.AssetId));
                    sqlCmd.Parameters.Add(new SqlParameter("AssetCategory", asset.AssetCategory));
                    sqlCmd.Parameters.Add(new SqlParameter("AssetName", asset.AssetName));
                    sqlCmd.Parameters.Add(new SqlParameter("Thumbnail", asset.Thumbnail));
                    sqlCmd.Parameters.Add(new SqlParameter("RockSkill", asset.RockSkill));
                    sqlCmd.Parameters.Add(new SqlParameter("PaperSkill", asset.PaperSkill));
                    sqlCmd.Parameters.Add(new SqlParameter("SissorsSkill", asset.SissorsSkill));

                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            newId = (int)reader["Id"];
                        }
                        reader.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return newId;
        }

        public BattleAsset GetBattleAssetByAssetId(int assetId)
        {
            SqlConnection sqlConnection = GetSQLConnector();
            BattleAsset battleAsset = null;

            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "SELECT TOP(1) * FROM [UHN].[BattleAsset] (NOLOCK) WHERE AssetId = " + assetId;
                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            battleAsset = new BattleAsset();

                            battleAsset.Id = (int)reader["Id"];
                            battleAsset.AssetId = (int)reader["AssetId"];
                            battleAsset.AssetCategory = (string)reader["AssetCategory"];
                            battleAsset.AssetName = (string)reader["AssetName"];
                            battleAsset.Thumbnail = (string)reader["Thumbnail"];
                            battleAsset.RockSkill = (int)reader["RockSkill"];
                            battleAsset.PaperSkill = (int)reader["PaperSkill"];
                            battleAsset.SissorsSkill = (int)reader["SissorsSkill"];
                        }
                        reader.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return battleAsset;
        }

        public BattleAsset GetBattleAssetByBattleAssetId(int battleAssetId)
        {
            SqlConnection sqlConnection = GetSQLConnector();
            BattleAsset battleAsset = null;

            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "SELECT TOP(1) * FROM [UHN].[BattleAsset] (NOLOCK) WHERE Id = " + battleAssetId;
                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            battleAsset = new BattleAsset();

                            battleAsset.Id = (int)reader["Id"];
                            battleAsset.AssetId = (int)reader["AssetId"];
                            battleAsset.AssetCategory = (string)reader["AssetCategory"];
                            battleAsset.AssetName = (string)reader["AssetName"];
                            battleAsset.Thumbnail = (string)reader["Thumbnail"];
                            battleAsset.RockSkill = (int)reader["RockSkill"];
                            battleAsset.PaperSkill = (int)reader["PaperSkill"];
                            battleAsset.SissorsSkill = (int)reader["SissorsSkill"];
                        }
                        reader.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return battleAsset;
        }

        public int UpsertBattleAssetTraining(BattleAssetTraining battleAssetTraining)
        {
            int newId = -1;
            SqlConnection sqlConnection = GetSQLConnector();

            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.CommandText = "[UHN].[UpsertBattleAssetTraining]";
                    sqlCmd.Parameters.Add(new SqlParameter("Id", battleAssetTraining.Id));
                    sqlCmd.Parameters.Add(new SqlParameter("BattleAssetId", battleAssetTraining.BattleAssetId));
                    sqlCmd.Parameters.Add(new SqlParameter("ContainerId", battleAssetTraining.ContainerId));
                    sqlCmd.Parameters.Add(new SqlParameter("SkillTraining", battleAssetTraining.SkillTraining));
                    sqlCmd.Parameters.Add(new SqlParameter("FinishedTime", battleAssetTraining.FinishedTime));
                    sqlCmd.Parameters.Add(new SqlParameter("MustAcceptBy", battleAssetTraining.MustAcceptBy));
                    sqlCmd.Parameters.Add(new SqlParameter("UPXFee", battleAssetTraining.UPXFee));
                    sqlCmd.Parameters.Add(new SqlParameter("Resolved", battleAssetTraining.Resolved));
                    sqlCmd.Parameters.Add(new SqlParameter("Accepted", battleAssetTraining.Accepted));

                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            newId = (int)reader["Id"];
                        }
                        reader.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return newId;
        }

        public int IsBattleAssetTraining(int battleAssetId)
        {
            SqlConnection sqlConnection = GetSQLConnector();
            int isTraining = -1;

            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "SELECT TOP(1) Id FROM [UHN].[BattleAssetTraining] (NOLOCK) WHERE Resolved = 0 AND BattleAssetId = " + battleAssetId;
                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            isTraining = (int)reader["Id"];
                        }
                        reader.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return isTraining;
        }

        public BattleAssetTraining GetBattleAssetTrainingById(int battleAssetTrainingId)
        {
            SqlConnection sqlConnection = GetSQLConnector();
            BattleAssetTraining battleAssetTraining = null;

            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "SELECT TOP(1) * FROM [UHN].[BattleAssetTraining] (NOLOCK) WHERE Id = " + battleAssetTrainingId;
                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            battleAssetTraining = new BattleAssetTraining();

                            battleAssetTraining.Id = (int)reader["Id"];
                            battleAssetTraining.BattleAssetId = (int)reader["BattleAssetId"];
                            battleAssetTraining.ContainerId = (int)reader["ContainerId"];
                            battleAssetTraining.SkillTraining = (string)reader["SkillTraining"];
                            battleAssetTraining.FinishedTime = (DateTime)reader["FinishedTime"];
                            battleAssetTraining.MustAcceptBy = (DateTime)reader["MustAcceptBy"];
                            battleAssetTraining.UPXFee = (int)reader["UPXFee"];
                            battleAssetTraining.Resolved = (bool)reader["Resolved"];
                            battleAssetTraining.Accepted = (bool)reader["Accepted"];
                        }
                        reader.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return battleAssetTraining;
        }

        public List<BattleAssetTraining> GetAllExpiredBattleAssetTrainings()
        {
            SqlConnection sqlConnection = GetSQLConnector();
            List<BattleAssetTraining> expiredTrainings = new List<BattleAssetTraining>();
            string nowUTC = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "SELECT * FROM [UHN].[BattleAssetTraining] (NOLOCK) WHERE Resolved = 0 AND Accepted = 0 AND MustAcceptBy < '" + nowUTC + "'";
                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BattleAssetTraining entry = new BattleAssetTraining();

                            entry.Id = (int)reader["Id"];
                            entry.BattleAssetId = (int)reader["BattleAssetId"];
                            entry.ContainerId = (int)reader["ContainerId"];
                            entry.SkillTraining = (string)reader["SkillTraining"];
                            entry.FinishedTime = (DateTime)reader["FinishedTime"];
                            entry.MustAcceptBy = (DateTime)reader["MustAcceptBy"];
                            entry.UPXFee = (int)reader["UPXFee"];
                            entry.Resolved = (bool)reader["Resolved"];

                            expiredTrainings.Add(entry);
                        }
                        reader.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return expiredTrainings;
        }

        public List<BattleAssetTraining> GetAllFinishedBattleAssetTrainings()
        {
            SqlConnection sqlConnection = GetSQLConnector();
            List<BattleAssetTraining> finishedTrainings = new List<BattleAssetTraining>();
            string nowUTCPlusSix = DateTime.UtcNow.AddMinutes(6).ToString("yyyy-MM-dd hh:mm:ss");

            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "SELECT * FROM [UHN].[BattleAssetTraining] (NOLOCK) WHERE Resolved = 0 AND Accepted = 1 AND FinishedTime < '" + nowUTCPlusSix + "'";
                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BattleAssetTraining entry = new BattleAssetTraining();

                            entry.Id = (int)reader["Id"];
                            entry.BattleAssetId = (int)reader["BattleAssetId"];
                            entry.ContainerId = (int)reader["ContainerId"];
                            entry.SkillTraining = (string)reader["SkillTraining"];
                            entry.FinishedTime = (DateTime)reader["FinishedTime"];
                            entry.MustAcceptBy = (DateTime)reader["MustAcceptBy"];
                            entry.UPXFee = (int)reader["UPXFee"];
                            entry.Resolved = (bool)reader["Resolved"];

                            finishedTrainings.Add(entry);
                        }
                        reader.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return finishedTrainings;
        }

        public List<BattleAssetTraining> GetAllApprovedNotFinishedBattleAssetTrainings()
        {
            SqlConnection sqlConnection = GetSQLConnector();
            List<BattleAssetTraining> finishedTrainings = new List<BattleAssetTraining>();

            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "SELECT * FROM [UHN].[BattleAssetTraining] (NOLOCK) WHERE Resolved = 0 AND Accepted = 1";
                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BattleAssetTraining entry = new BattleAssetTraining();

                            entry.Id = (int)reader["Id"];
                            entry.BattleAssetId = (int)reader["BattleAssetId"];
                            entry.ContainerId = (int)reader["ContainerId"];
                            entry.SkillTraining = (string)reader["SkillTraining"];
                            entry.FinishedTime = (DateTime)reader["FinishedTime"];
                            entry.MustAcceptBy = (DateTime)reader["MustAcceptBy"];
                            entry.UPXFee = (int)reader["UPXFee"];
                            entry.Resolved = (bool)reader["Resolved"];

                            finishedTrainings.Add(entry);
                        }
                        reader.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return finishedTrainings;
        }

        public int UpsertBattle(Battle battle)
        {
            int newId = -1;
            SqlConnection sqlConnection = GetSQLConnector();

            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.CommandText = "[UHN].[UpsertBattle]";
                    sqlCmd.Parameters.Add(new SqlParameter("Id", battle.Id));
                    sqlCmd.Parameters.Add(new SqlParameter("ContainerId", battle.ContainerId));
                    sqlCmd.Parameters.Add(new SqlParameter("OpponentBattleAssetId", battle.OpponentBattleAssetId));
                    sqlCmd.Parameters.Add(AddNullParmaterSafe<int?>("ChallengerBattleAssetId", battle.ChallengerBattleAssetId));
                    sqlCmd.Parameters.Add(new SqlParameter("UPXPerSide", battle.UPXPerSide));
                    sqlCmd.Parameters.Add(new SqlParameter("MustBattleBy", battle.MustBattleBy));
                    sqlCmd.Parameters.Add(new SqlParameter("Resolved", battle.Resolved));
                    sqlCmd.Parameters.Add(AddNullParmaterSafe<int?>("WinnerBattleAssetId", battle.WinnerBattleAssetId));
                    sqlCmd.Parameters.Add(new SqlParameter("OpponentSkills", battle.OpponentSkills));
                    sqlCmd.Parameters.Add(AddNullParmaterSafe<string?>("ChallengerSkills", battle.ChallengerSkills));

                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            newId = (int)reader["Id"];
                        }
                        reader.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return newId;
        }

        public List<Battle> GetAllUnresolvedBattles()
        {
            SqlConnection sqlConnection = GetSQLConnector();
            List<Battle> expiredBattles = new List<Battle>();
            string nowUTC = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "SELECT * FROM [UHN].[Battle] (NOLOCK) WHERE Resolved = 0";
                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Battle entry = new Battle();

                            entry.Id = (int)reader["Id"];
                            entry.ContainerId = (int)reader["ContainerId"];
                            entry.OpponentBattleAssetId = (int)reader["OpponentBattleAssetId"];
                            entry.ChallengerBattleAssetId = ReadNullParameterSafe<int?>(reader, "ChallengerBattleAssetId");
                            entry.UPXPerSide = (int)reader["UPXPerSide"];
                            entry.MustBattleBy = (DateTime)reader["MustBattleBy"];
                            entry.Resolved = (bool)reader["Resolved"];
                            entry.WinnerBattleAssetId = ReadNullParameterSafe<int?>(reader, "WinnerBattleAssetId");
                            entry.OpponentSkills = (string)reader["OpponentSkills"];
                            entry.ChallengerSkills = ReadNullParameterSafe<string?>(reader, "ChallengerSkills");

                            expiredBattles.Add(entry);
                        }
                        reader.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return expiredBattles;
        }

        public List<Battle> GetAllNeedingChallengers()
        {
            SqlConnection sqlConnection = GetSQLConnector();
            List<Battle> expiredBattles = new List<Battle>();
            string nowUTC = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "SELECT * FROM [UHN].[Battle] (NOLOCK) WHERE Resolved = 0 AND MustBattleBy > '" + nowUTC + "'";
                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Battle entry = new Battle();

                            entry.Id = (int)reader["Id"];
                            entry.ContainerId = (int)reader["ContainerId"];
                            entry.OpponentBattleAssetId = (int)reader["OpponentBattleAssetId"];
                            entry.ChallengerBattleAssetId = ReadNullParameterSafe<int?>(reader, "ChallengerBattleAssetId");
                            entry.UPXPerSide = (int)reader["UPXPerSide"];
                            entry.MustBattleBy = (DateTime)reader["MustBattleBy"];
                            entry.Resolved = (bool)reader["Resolved"];
                            entry.WinnerBattleAssetId = ReadNullParameterSafe<int?>(reader, "WinnerBattleAssetId");
                            entry.OpponentSkills = (string)reader["OpponentSkills"];
                            entry.ChallengerSkills = ReadNullParameterSafe<string?>(reader, "ChallengerSkills");

                            expiredBattles.Add(entry);
                        }
                        reader.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return expiredBattles;
        }

        public bool IsBattling(int battleAssetId)
        {
            SqlConnection sqlConnection = GetSQLConnector();
            List<Battle> expiredBattles = new List<Battle>();
            string nowUTC = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            int isBattling = -1;

            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "SELECT TOP(1) Id FROM [UHN].[Battle] (NOLOCK) WHERE Resolved = 0  AND (OpponentBattleAssetId = " + battleAssetId + " OR ChallengerBattleAssetId = " + battleAssetId + ")";
                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            isBattling = (int)reader["Id"];
                        }
                        reader.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return isBattling != -1;
        }


        public List<Battle> GetAllBattlesResolvedByBattleAssetId(int battleAssetId)
        {
            SqlConnection sqlConnection = GetSQLConnector();
            List<Battle> expiredBattles = new List<Battle>();
            string nowUTC = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "SELECT * FROM [UHN].[Battle] (NOLOCK) WHERE Resolved = 1 AND ChallengerBattleAssetId IS NOT NULL AND (OpponentBattleAssetId = " + battleAssetId + " OR ChallengerBattleAssetId = " + battleAssetId + ") ORDER BY MustBattleBy DESC";
                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Battle entry = new Battle();

                            entry.Id = (int)reader["Id"];
                            entry.ContainerId = (int)reader["ContainerId"];
                            entry.OpponentBattleAssetId = (int)reader["OpponentBattleAssetId"];
                            entry.ChallengerBattleAssetId = ReadNullParameterSafe<int?>(reader, "ChallengerBattleAssetId");
                            entry.UPXPerSide = (int)reader["UPXPerSide"];
                            entry.MustBattleBy = (DateTime)reader["MustBattleBy"];
                            entry.Resolved = (bool)reader["Resolved"];
                            entry.WinnerBattleAssetId = ReadNullParameterSafe<int?>(reader, "WinnerBattleAssetId");
                            entry.OpponentSkills = (string)reader["OpponentSkills"];
                            entry.ChallengerSkills = ReadNullParameterSafe<string?>(reader, "ChallengerSkills");

                            expiredBattles.Add(entry);
                        }
                        reader.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return expiredBattles;
        }

        public List<Battle> GetAllNeedingApproval()
        {
            SqlConnection sqlConnection = GetSQLConnector();
            List<Battle> expiredBattles = new List<Battle>();

            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "SELECT * FROM [UHN].[Battle] (NOLOCK) WHERE Resolved = 0 AND ChallengerBattleAssetId IS NOT NULL";
                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Battle entry = new Battle();

                            entry.Id = (int)reader["Id"];
                            entry.ContainerId = (int)reader["ContainerId"];
                            entry.OpponentBattleAssetId = (int)reader["OpponentBattleAssetId"];
                            entry.ChallengerBattleAssetId = ReadNullParameterSafe<int?>(reader, "ChallengerBattleAssetId");
                            entry.UPXPerSide = (int)reader["UPXPerSide"];
                            entry.MustBattleBy = (DateTime)reader["MustBattleBy"];
                            entry.Resolved = (bool)reader["Resolved"];
                            entry.WinnerBattleAssetId = ReadNullParameterSafe<int?>(reader, "WinnerBattleAssetId");
                            entry.OpponentSkills = (string)reader["OpponentSkills"];
                            entry.ChallengerSkills = ReadNullParameterSafe<string?>(reader, "ChallengerSkills");

                            expiredBattles.Add(entry);
                        }
                        reader.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return expiredBattles;
        }

        public Battle GetBattleById(int battleId)
        {
            SqlConnection sqlConnection = GetSQLConnector();
            Battle battle = null;
            string nowUTC = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            using (sqlConnection)
            {
                sqlConnection.Open();

                try
                {
                    SqlCommand sqlCmd = new SqlCommand();
                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.CommandText = "SELECT TOP(1) * FROM [UHN].[Battle] (NOLOCK) WHERE Id = " + battleId;
                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            battle = new Battle();

                            battle.Id = (int)reader["Id"];
                            battle.ContainerId = (int)reader["ContainerId"];
                            battle.OpponentBattleAssetId = (int)reader["OpponentBattleAssetId"];
                            battle.ChallengerBattleAssetId = ReadNullParameterSafe<int?>(reader, "ChallengerBattleAssetId");
                            battle.UPXPerSide = (int)reader["UPXPerSide"];
                            battle.MustBattleBy = (DateTime)reader["MustBattleBy"];
                            battle.Resolved = (bool)reader["Resolved"];
                            battle.WinnerBattleAssetId = ReadNullParameterSafe<int?>(reader, "WinnerBattleAssetId");
                            battle.OpponentSkills = (string)reader["OpponentSkills"];
                            battle.ChallengerSkills = ReadNullParameterSafe<string?>(reader, "ChallengerSkills");
                        }
                        reader.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return battle;
        }

        private SqlParameter AddNullParmaterSafe<T>(string parameterName, T value)
        {
            if (value == null)
            {
                return new SqlParameter(parameterName, DBNull.Value);
            }
            else
            {
                return new SqlParameter(parameterName, value);
            }
        }

        private T ReadNullParameterSafe<T>(SqlDataReader reader, string parameter)
        {
            if (reader[parameter] != DBNull.Value)
            {
                return (T)reader[parameter];
            }
            else
            {
                return default(T);
            }
        }

        private SqlConnection GetSQLConnector()
        {
            string connectionString = DbConnectionString;

            if (connectionString == null)
            {
                throw new Exception("Invalid DB Connection");
            }
            else
            {
                return new SqlConnection(connectionString);
            }
        }
    }
}
