CREATE TABLE [UHN].[Battle]
(
	[Id]                       INT IDENTITY(1,1) NOT NULL,
	[ContainerId]              INT                       ,
	[OpponentBattleAssetId]    INT                       ,
	[ChallengerBattleAssetId]  INT                       ,
	[UPXPerSide]               INT                       ,
	[MustBattleBy]             DATETIME                  ,
	[Resolved]                 BIT                       ,
	[WinnerBattleAssetId]      INT                       ,
	[OpponentSkills]           VARCHAR(100)              ,
	[ChallengerSkills]         VARCHAR(100)              ,

	CONSTRAINT pk_BattleId PRIMARY KEY(Id)
)
GO