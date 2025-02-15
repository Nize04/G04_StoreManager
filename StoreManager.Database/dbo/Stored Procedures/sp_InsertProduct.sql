CREATE PROCEDURE [dbo].[sp_InsertProduct]
	@Name nvarchar(50),
	@CategoryID int,
	@Price decimal(10,2),
	@EmployeeId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF dbo.HasPermission(@EmployeeId,'020002') = 0
	BEGIN
		RAISERROR('This employee does not have permission to use this command.', 16, 1)
		RETURN 1;
	END

	BEGIN TRY
		BEGIN TRANSACTION;
		INSERT INTO Products (Name, CategoryID)
		VALUES (@Name, @CategoryID);
		
		DECLARE @InsertedProduct INT = SCOPE_IDENTITY();
		INSERT INTO PriceHistory (ProductId,Price) VALUES(@InsertedProduct,@Price)

		INSERT INTO ProductsQuantity(Id) VALUES(@InsertedProduct)

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION;
		THROW;
	END CATCH

	RETURN 0;
END;