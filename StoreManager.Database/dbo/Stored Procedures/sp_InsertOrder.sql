CREATE PROCEDURE [dbo].[sp_InsertOrder]
	@EmployeeId INT,
	@CustomerId INT = NULL,
	@Id INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	IF dbo.HasPermission(@EmployeeId,'020007') = 0
	BEGIN
		RAISERROR('This employee does not have permission to use this commnad.', 16, 1)
		RETURN 1;
	END

	INSERT INTO Orders(CustomerId,EmployeeId) 
	VALUES(@CustomerId,@EmployeeId)

	SET @Id = @@IDENTITY

	RETURN 0;
END