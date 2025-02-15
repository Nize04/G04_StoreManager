using System.Diagnostics.CodeAnalysis;

namespace StoreManager.Models
{
    public class OrderModel
    {
        public int EmployeeId { get; set; }

        [AllowNull]
        public int? CustomerId { get; set; }

        [AllowNull]
        public string? Description { get; set; }
    }

    public class OrderDetailModel
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}