create procedure sp_UpdateCategory
	@Id int,
	@Name nvarchar(50),
	@Description nvarchar(1000)
as
begin
	set nocount on;

	if exists(select 1 from Categories where Id = @Id and IsActive = 0) 
	begin
		raiserror('Category is deleted', 16, 1);
		return 1;
	end

	update Categories
	set Name = @Name,
		Description = @Description,
		UpdateDate = GETDATE()
	where Id = @Id and IsActive = 1;

	return 0;
end
