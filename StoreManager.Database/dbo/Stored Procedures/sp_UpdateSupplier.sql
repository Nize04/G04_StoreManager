CREATE PROCEDURE [dbo].[sp_UpdateSupplier]
	@Id INT,
	@CompanyName NVARCHAR(50),
	@Country NVARCHAR(35),
	@City NVARCHAR(35),
	@AccountId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF dbo.HasPermission(@AccountId,'030003') = 0
	BEGIN
		RAISERROR('This employee doesnt have permission to use this commnad', 16, 1)
		RETURN 1;
	END

	UPDATE Suppliers 
	SET CompanyName = @CompanyName, 
		Country = @Country, City = @City,
		UpdateDate = GETDATE()
	WHERE Id = @Id

	RETURN 0;
END