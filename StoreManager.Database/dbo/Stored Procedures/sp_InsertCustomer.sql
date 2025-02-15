CREATE PROCEDURE [dbo].[sp_InsertCustomer]
	@FullName NVARCHAR(60),
	@PhoneNumber VARCHAR(12),
	@EmployeeId INT,
	@Id INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	IF dbo.HasPermission(@EmployeeId, '020006') = 0
	BEGIN
		RAISERROR('This employee does not have permission to use this commnad.', 16, 1)
		RETURN 1;
	END

	INSERT INTO Customers(FullName,PhoneNumber) 
	VALUES(@FullName,@PhoneNumber)

	SET @Id = @@IDENTITY
	RETURN 0;
END