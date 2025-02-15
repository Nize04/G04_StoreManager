CREATE FUNCTION [dbo].[HashPassword]
(
    @Password NVARCHAR(100),
    @Salt NVARCHAR(100)
)
RETURNS NVARCHAR(128)
AS
BEGIN
    DECLARE @CombinedString NVARCHAR(200) = @Password + @Salt;
    DECLARE @HashedPassword VARBINARY(64) = HASHBYTES('SHA2_512', @CombinedString);
    DECLARE @HexHashedPassword NVARCHAR(128) = CONVERT(NVARCHAR(128), @HashedPassword, 2);
    RETURN @HexHashedPassword;
END;
