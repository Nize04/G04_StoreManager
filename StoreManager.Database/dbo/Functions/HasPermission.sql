CREATE FUNCTION [dbo].[HasPermission](@AccountId int, @PermissionCode varchar(6)) 
	RETURNS BIT
AS
BEGIN
	DECLARE @Result BIT = 0;

	IF EXISTS(SELECT 1
			  FROM AccountRoles AR
					INNER JOIN Roles R ON AR.RoleId = R.Id
					INNER JOIN RolePermissions RP ON R.Id = RP.RoleId
					INNER JOIN Permissions P ON RP.PermissionId = P.Id
			   where AR.AccountId = @AccountId AND P.PermissionCode = @PermissionCode AND P.IsActive = 1) SET @Result = 1;

	RETURN @Result;
END