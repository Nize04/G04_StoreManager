CREATE Procedure [dbo].[sp_InsertSupplierTransactionDetails]
	@SupplierTransactionId Int,
	@ProductId Int,
	@Price money,
	@Quantity Int,
	@ProductionDate Datetime,
	@ExpirationDate Datetime = null,
	@AccountId int
As
Begin
	set nocount on;

	if dbo.HasPermission(@AccountId, '020010') = 0
	Begin
		Raiserror('This employee doesnt have permission to use this command.', 16, 1)
		Return 1;
	End

	Insert into SupplierTransactionDetails(SupplierTransactionId, ProductId, Price, Quantity, ProductionDate, ExpirationDate)
	Values(@SupplierTransactionId, @ProductId, @Price, @Quantity, @ProductionDate, @ExpirationDate)

	return 0;
End