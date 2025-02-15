CREATE VIEW v_GetOrders AS
SELECT *
FROM Orders
WHERE IsActive = 1;