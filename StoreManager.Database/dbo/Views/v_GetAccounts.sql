CREATE VIEW v_GetAccounts AS
SELECT Id,Email,Password,CreateDate,UpdateDate,IsActive
FROM Accounts
WHERE IsActive = 1;