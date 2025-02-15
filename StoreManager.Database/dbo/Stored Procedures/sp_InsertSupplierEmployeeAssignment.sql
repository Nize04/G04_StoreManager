CREATE PROCEDURE [dbo].[sp_InsertSupplierEmployeeAssignment]
    @SupplierId INT,
    @EmployeeId INT,
    @StartDate DATE,
    @EndDate DATE,
	@AccountId INT,
    @Id INT OUTPUT
AS

BEGIN
	SET NOCOUNT ON;

    IF dbo.HasPermission(@AccountId, '020011') = 0
    BEGIN
        RAISERROR('This employee does not have permission to use this command', 16, 1)
        RETURN 1;
    END
        INSERT INTO SupplierEmployeeAssignmentHistory (SupplierId, EmployeeId, StartDate, EndDate)
        VALUES (@SupplierId, @EmployeeId, @StartDate, @EndDate);

        SET @Id = @@IDENTITY

	RETURN 0;
END