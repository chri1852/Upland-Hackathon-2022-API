CREATE PROCEDURE [UHN].[UpsertBattleAssetTraining]
(
	@Id                       INT,
	@BattleAssetId            INT,
	@ContainerId              INT,
	@SkillTraining            VARCHAR(50),
	@FinishedTime             DATETIME,
	@MustAcceptBy             DATETIME,
	@UPXFee                   INT,
	@Resolved                 BIT,
	@Accepted                 BIT
)
AS
BEGIN
	BEGIN TRY	
		IF(NOT EXISTS(SELECT * FROM [UHN].[BattleAssetTraining] (NOLOCK) WHERE [Id] = @Id))
			BEGIN
				INSERT INTO [UHN].[BattleAssetTraining]
				(
					[BattleAssetId],
					[ContainerId],
					[SkillTraining],
					[FinishedTime],
					[MustAcceptBy],
					[UPXFee],
					[Resolved],
					[Accepted]
				)
				Values
				(
					@BattleAssetId,
					@ContainerId,
					@SkillTraining,
					@FinishedTime,
					@MustAcceptBy,
					@UPXFee,
					@Resolved,
					@Accepted
				)

				SELECT CAST(@@IDENTITY AS INT) AS "Id"
			END
		ELSE
			BEGIN
				UPDATE [UHN].[BattleAssetTraining]
				SET
					[BattleAssetId] = @BattleAssetId,
					[ContainerId] = @ContainerId,
					[SkillTraining] = @SkillTraining,
					[FinishedTime] = @FinishedTime,
					[MustAcceptBy] = @MustAcceptBy,
					[UPXFee] = @UPXFee,
					[Resolved] = @Resolved,
					[Accepted] = @Accepted
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