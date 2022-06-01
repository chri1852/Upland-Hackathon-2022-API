CREATE PROCEDURE [UHN].[GetAuctionById]
(
	@Id INT
)
AS
BEGIN
	BEGIN TRY		
		SELECT A.*, U.UplandUsername
		FROM UHN.Auction A (NOLOCK)
			JOIN UHN.[User] U (NOLOCK)
				ON A.UserId = U.Id
		WHERE A.Id = @Id

		SELECT B.*, U.UplandUsername
		FROM UHN.Bid B (NOLOCK)
			JOIN UHN.[User] U (NOLOCK)
				ON B.UserId = U.Id
		WHERE B.AuctionId = @Id
		ORDER BY B.BidDateTime DESC

		SELECT AA.*
		FROM UHN.AuctionAsset AA (NOLOCK)
		WHERE AA.AuctionId = @Id
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