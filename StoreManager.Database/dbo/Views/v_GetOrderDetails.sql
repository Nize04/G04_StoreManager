CREATE VIEW v_GetOrderDetails AS
SELECT *
FROM OrderDetails
WHERE IsActive = 1;