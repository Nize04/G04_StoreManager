CREATE PROCEDURE [dbo].[sp_DeleteEmployee]
	@Id INT,
	@AccountId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF dbo.HasPermission(@AccountId,'010005') = 0
	BEGIN
		RAISERROR('This employee does not have permission to use this command', 16, 1)
		RETURN 1;
	END

	UPDATE Employees 
	SET IsActive = 0
	WHERE Id = @Id AND IsActive = 1

	UPDATE EmployeeAccounts 
	SET IsActive = 0
	WHERE Id = @Id AND IsActive = 1

	RETURN 0;
END