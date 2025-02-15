CREATE PROCEDURE [dbo].[sp_GetEmployee]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        E.Id,
        E.FirstName,
        E.LastName,
        E.ReportsTo,
        (SELECT SupplierId
        FROM SupplierEmployeeAssignmentHistory 
        WHERE EmployeeId = @Id AND EndDate IS NULL) AS ResponsibleForSupplierId 
    FROM Employees E
    WHERE Id = @Id and IsActive = 1

    RETURN 0;
END
GO