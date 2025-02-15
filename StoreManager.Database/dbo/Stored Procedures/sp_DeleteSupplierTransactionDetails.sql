CREATE procedure [dbo].[sp_DeleteSupplierTransactionDetails]
	@SupplierTransactionId Int,
	@ProductId int,
	@AccountId Int
As
Begin
	set nocount on;

	if dbo.HasPermission(@AccountId, '010010') = 0
	Begin
		Raiserror('This employee doesnt have permission to use this command.', 16, 1) 
		Return 1;
	End

	delete from SupplierTransactionDetails
	where SupplierTransactionId = @SupplierTransactionId and ProductId = @ProductId

	Return 0;
End