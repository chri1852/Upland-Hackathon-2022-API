CREATE PROCEDURE [UHN].[UpsertBattleAsset]
(
	@Id                       INT,
	@AssetId                  INT,
	@AssetCategory            VARCHAR(50),
	@AssetName                VARCHAR(2000),
	@Thumbnail                VARCHAR(2000),
	@RockSkill                INT,
	@PaperSkill               INT,
	@SissorsSkill             INT
)
AS
BEGIN
	BEGIN TRY	
		IF(NOT EXISTS(SELECT * FROM [UHN].[BattleAsset] (NOLOCK) WHERE [Id] = @Id))
			BEGIN
				INSERT INTO [UHN].[BattleAsset]
				(
					[AssetId],
					[AssetCategory],
					[AssetName],
					[Thumbnail],
					[RockSkill],
					[PaperSkill],
					[SissorsSkill]
				)
				Values
				(
					@AssetId,
					@AssetCategory,
					@AssetName,
					@Thumbnail,
					@RockSkill,
					@PaperSkill,
					@SissorsSkill
				)
			END
		ELSE
			BEGIN
				UPDATE [UHN].[BattleAsset]
				SET
					[AssetId] = @AssetId,
					[AssetCategory] = @AssetCategory,
					[AssetName] = @AssetName,
					[Thumbnail] = @Thumbnail,
					[RockSkill] = @RockSkill,
					[PaperSkill] = @PaperSkill,
					[SissorsSkill] = @SissorsSkill
				WHERE [Id] = @Id
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