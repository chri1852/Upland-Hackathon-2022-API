CREATE TABLE [UHN].[BattleAssetTraining]
(
	[Id]                       INT IDENTITY(1,1) NOT NULL,
	[BattleAssetId]            INT                       ,
	[ContainerId]              INT                       ,
	[SkillTraining]            VARCHAR(10)               ,
	[FinishedTime]             DateTime                  ,
	[MustAcceptBy]             DateTime                  ,
	[UPXFee]                   INT                       ,
	[Resolved]                 BIT                       ,
	[Accepted]                 BIT                       ,

	CONSTRAINT pk_BattleAssetTrainingId PRIMARY KEY(Id),
	CONSTRAINT FK_TrainingBattleAssetId FOREIGN KEY ([BattleAssetId]) REFERENCES [UHN].[BattleAsset](Id)
)
GO