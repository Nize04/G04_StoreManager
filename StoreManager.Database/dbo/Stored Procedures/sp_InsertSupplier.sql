CREATE PROCEDURE [dbo].[sp_InsertSupplier]
	@CompanyName NVARCHAR(50),
	@Country NVARCHAR(35),
	@City NVARCHAR(35),
	@AccountId INT,
	@Id INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	IF dbo.HasPermission(@AccountId,'020003') = 0
	BEGIN
		RAISERROR('This employee does not have permission to use this commnad.', 16, 1)
		RETURN 1;
	END

		INSERT INTO Suppliers (CompanyName, Country,City)
		VALUES (@CompanyName, @Country,@City);

		SET @Id = @@IDENTITY

	RETURN 0;
END