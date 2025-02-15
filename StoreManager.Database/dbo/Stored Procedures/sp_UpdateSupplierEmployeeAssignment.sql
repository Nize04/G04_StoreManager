CREATE PROCEDURE [dbo].[sp_UpdateSupplierEmployeeAssignment]
    @Id INT,
    @SupplierId INT,
    @EmployeeId INT,
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
	SET NOCOUNT ON;

    IF dbo.HasPermission(@EmployeeId, '030011') = 0
    BEGIN
        RAISERROR('This employee does not have permission to use this command.', 16, 1)
        RETURN 1;
    END

    BEGIN TRY
        UPDATE SupplierEmployeeAssignmentHistory
        SET SupplierId = @SupplierId,
            EmployeeId = @EmployeeId,
            StartDate = @StartDate,
            EndDate = @EndDate
        WHERE Id = @Id;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
		
	RETURN 0;
END