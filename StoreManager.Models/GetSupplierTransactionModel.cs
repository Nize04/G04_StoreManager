
using StoreManager.DTO;

namespace StoreManager.Models
{
    public class GetSupplierTransactionModel
    {
        public SupplierTransaction SupplierTransaction { get; set; } = null!;
        public IEnumerable<SupplierTransactionDetail> SupplierTransactionDetails { get; set; } = null!;
    }
}
