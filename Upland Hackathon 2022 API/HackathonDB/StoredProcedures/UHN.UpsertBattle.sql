CREATE PROCEDURE [UHN].[UpsertBattle]
(
	@Id                       INT,
	@ContainerId              INT,
	@OpponentBattleAssetId    INT,
	@ChallengerBattleAssetId  INT,
	@UPXPerSide               INT,
	@MustBattleBy             DATETIME,
	@Resolved                 BIT,
	@WinnerBattleAssetId      INT,
	@OpponentSkills           VARCHAR(100),
	@ChallengerSkills         VARCHAR(100)
)
AS
BEGIN
	BEGIN TRY	
		IF(NOT EXISTS(SELECT * FROM [UHN].[Battle] (NOLOCK) WHERE [Id] = @Id))
			BEGIN
				INSERT INTO [UHN].[Battle]
				(
					[ContainerId],
					[OpponentBattleAssetId],
					[ChallengerBattleAssetId],
					[UPXPerSide],
					[MustBattleBy],
					[Resolved],
					[WinnerBattleAssetId],
					[OpponentSkills],
					[ChallengerSkills]
				)
				Values
				(
					@ContainerId,
					@OpponentBattleAssetId,
					@ChallengerBattleAssetId,
					@UPXPerSide,
					@MustBattleBy,
					@Resolved,
					@WinnerBattleAssetId,
					@OpponentSkills,
					@ChallengerSkills
				)

				SELECT CAST(@@IDENTITY AS INT) AS "Id"
			END
		ELSE
			BEGIN
				UPDATE [UHN].[Battle]
				SET
					[ContainerId] = @ContainerId,
					[OpponentBattleAssetId] = @OpponentBattleAssetId,
					[ChallengerBattleAssetId] = @ChallengerBattleAssetId,
					[UPXPerSide] = @UPXPerSide,
					[MustBattleBy] = @MustBattleBy,
					[Resolved] = @Resolved,
					[WinnerBattleAssetId] = @WinnerBattleAssetId,
					[OpponentSkills] = @OpponentSkills,
					[ChallengerSkills] = @ChallengerSkills
				WHERE [Id] = @Id

				SELECT @Id AS "Id"
			END
	END TRY

	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;

		SELECT @ErrorMessage = ERROR_MESSAGE(),
			   @ErrorSeverity = ERROR_SEVERITY(),
			   @ErrorState = ERROR_STATE();

		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
	END CATCH
END
GO