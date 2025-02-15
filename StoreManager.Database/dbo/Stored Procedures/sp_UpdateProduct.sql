CREATE PROCEDURE [dbo].[sp_UpdateProduct]
	@Id int,
	@Name nvarchar(50),
	@CategoryID int,
	@Price decimal(10,2),
	@AccountId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF dbo.HasPermission(@AccountId,'030002') = 0
	BEGIN
		RAISERROR('This employee doesnt have permission to use this command.', 16, 1)
		RETURN 1;
	END

	BEGIN TRY
		BEGIN TRANSACTION;
		UPDATE Products
		SET Name = @Name,
			CategoryID = @CategoryID,
			UpdateDate = GETDATE()
		WHERE ID = @ID;

		IF dbo.GetProductPrice(@Id) != @Price
		BEGIN
			INSERT INTO PriceHistory (ProductId, Price) VALUES(@Id,@Price)
		END

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION;
		THROW;
	END CATCH

  RETURN 0;
END