
using StoreManager.DTO;

namespace StoreManager.Models
{
    public class MakeSupplierTransactionRequest
    {
        public InputSupplierTransactionModel InputSupplierTransactionModel { get; set; } = null!;
        public IEnumerable<SupplierTransactionDetail> SupplierTransactionDetails { get; set; } = null!;
    }
}
