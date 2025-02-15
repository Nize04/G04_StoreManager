CREATE PROCEDURE [dbo].[sp_InsertEmployee]
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
	@ReportsTo INT = NULL,
	@AccountId INT,
	@Id int OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	IF dbo.HasPermission(@AccountId,'020005') = 0
	BEGIN
		RAISERROR('This employee does not have permission to use this commnad.', 16, 1)
		RETURN 1;
	END

    INSERT INTO Employees (FirstName, LastName,ReportsTo)
    VALUES (@FirstName, @LastName,@ReportsTo);

	SET @Id = @@IDENTITY

	RETURN 0;
END;