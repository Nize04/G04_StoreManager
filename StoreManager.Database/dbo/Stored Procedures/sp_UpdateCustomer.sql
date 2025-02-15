CREATE PROCEDURE [dbo].[sp_UpdateCustomer]
	@Id INT,
	@FullName NVARCHAR(60),
	@AccountId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF dbo.HasPermission(@AccountId,'030006') = 0
	BEGIN
		RAISERROR('This employee doesnt have permission to use this commnad.', 16, 1)
		RETURN 1;
	END

	UPDATE Customers 
	SET FullName = @FullName
	WHERE Id = @Id

	RETURN 0;
END