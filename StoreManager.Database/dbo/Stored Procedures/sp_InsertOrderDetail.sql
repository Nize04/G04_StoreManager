CREATE PROCEDURE [dbo].[sp_InsertOrderDetail]
	@OrderId INT,
	@ProductId INT,
	@Quantity SMALLINT,
	@AccountId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF dbo.HasPermission(@AccountId, '020012') = 0
	BEGIN
		RAISERROR('This employee does not have permission to use this commnad.', 16, 1)
		RETURN 1;
	END
	BEGIN TRY
		BEGIN TRANSACTION;
		INSERT OrderDetails (OrderId,ProductId,Quantity,UnitPrice) 
		VALUES(@OrderId,@ProductId,@Quantity,dbo.GetProductPrice(@ProductId))

		UPDATE ProductsQuantity 
		SET Quantity = Quantity - @Quantity, 
			UpdateDate = GETDATE()
		WHERE Id = @ProductId

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION;
		THROW;
	END CATCH

	RETURN 0;
END