CREATE PROCEDURE [UHN].[UpsertAuctionAsset]
(
	@Id                       INT,
	@AuctionId                INT,
	@AssetId                  INT,
	@AssetCategory            VARCHAR(50),
	@AssetName                VARCHAR(2000),
	@Thumbnail                VARCHAR(2000)
)
AS
BEGIN
	BEGIN TRY	
		IF(NOT EXISTS(SELECT * FROM [UHN].[AuctionAsset] (NOLOCK) WHERE [Id] = @Id))
			BEGIN
				INSERT INTO [UHN].[AuctionAsset]
				(
					[AuctionId],
					[AssetId],
					[AssetCategory],
					[AssetName],
					[Thumbnail]
				)
				Values
				(
					@AuctionId,
					@AssetId,
					@AssetCategory,
					@AssetName,
					@Thumbnail
				)
			END
		ELSE
			BEGIN
				UPDATE [UHN].[AuctionAsset]
				SET
					[AuctionId] = @AuctionId,
					[AssetId] = @AssetId,
					[AssetCategory] = @AssetCategory,
					[AssetName] = @AssetName,
					[Thumbnail] = @Thumbnail
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