CREATE procedure [dbo].[sp_UpdateSupplierTransactionDetails]
	@SupplierTransactionId Int,
	@ProductId Int,
	@Price money,
	@Quantity Int,
	@ProductionDate Datetime,
	@ExpirationDate Datetime,
	@AccountId int
As
Begin
	set nocount on;

	If dbo.HasPermission(@AccountId, '030010') = 0
	Begin
		Raiserror('This employee doesnt have permissions to use this command.', 16, 1)
		Return 1;
	End

	Update SupplierTransactionDetails 
	Set ProductId = @ProductId, Price = @Price, Quantity = @Quantity, 
		ProductionDate = @ProductionDate, ExpirationDate = @ExpirationDate
	Where SupplierTransactionId = @SupplierTransactionId and ProductId = @ProductId

	Return 0;
End