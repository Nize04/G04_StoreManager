CREATE PROCEDURE [dbo].[sp_DeleteCustomer]
	@Id INT,
	@AccountId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF dbo.HasPermission(@AccountId,'010006') = 0
	BEGIN
		RAISERROR('This employee does not have permission to use this commnad.', 16, 1)
		RETURN 1;
	END

	UPDATE Customers SET IsActive = 0
	WHERE Id = @Id AND IsActive = 1

	RETURN 0;
END