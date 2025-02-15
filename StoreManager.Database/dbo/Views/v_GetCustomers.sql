CREATE VIEW v_GetCustomers AS
SELECT *
FROM Customers
WHERE IsActive = 1;
