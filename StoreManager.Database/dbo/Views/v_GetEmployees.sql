CREATE VIEW v_GetEmployees AS
SELECT *
FROM Employees
WHERE IsActive = 1;