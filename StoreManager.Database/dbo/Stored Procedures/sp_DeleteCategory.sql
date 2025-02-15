create procedure sp_DeleteCategory
	@Id int
as
begin
	set nocount on;
	
	begin try
		begin tran;

		update Categories
		set IsActive = 0
		where Id = @Id and IsActive = 1;

		update Products
		set IsActive = 0
		where CategoryId = @Id and IsActive = 1;

		commit tran;
	end try
	begin catch
		rollback tran;
		throw;
	end catch

	return 0;
end