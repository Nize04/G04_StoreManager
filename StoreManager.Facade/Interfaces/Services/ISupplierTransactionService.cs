using StoreManager.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Facade.Interfaces.Services
{
    public interface ISupplierTransactionService
    {
        Task CreateTransactionAsync(SupplierTransaction supplierTransaction, IEnumerable<SupplierTransactionDetail> supplierTransactionDetails);
        Task<IEnumerable<SupplierTransactionDetail>?> GetTransactionDetailsAsync(int supplierTransactionId);
        Task<SupplierTransaction?> GetTransactionByIdAsync(int id);
    }
}
