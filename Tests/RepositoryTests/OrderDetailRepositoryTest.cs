using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;

namespace StoreManager.Tests.RepositoryTests
{
    [Collection("Database Tests")]
    public class OrderDetailRepositoryTest : RepositoryTestBase
    {
        public OrderDetailRepositoryTest(IUnitOfWork unitOfWork, DatabaseFixture fixture) : base(unitOfWork, fixture)
        {
        }

        [Fact]
        public async Task Insert()
        {
            OrderDetail orderDetail = new OrderDetail() 
            { OrderId = 2, ProductId = 1, Quantity = 5, UnitPrice = 550 };
            await _unitOfWork.OrderDetailRepository.InsertAsync(orderDetail);

            Assert.NotNull(_unitOfWork.OrderDetailRepository.GetByIdAsync(orderDetail.OrderId,orderDetail.ProductId));
        }

        [Fact]
        public async Task Update()
        {
            OrderDetail? orderDetail = await _unitOfWork.OrderDetailRepository.GetByIdAsync(1,1);
            Assert.NotNull(orderDetail);
            orderDetail.Quantity += 5;

            await _unitOfWork.OrderDetailRepository.UpdateAsync(orderDetail);

            OrderDetail updatedOrderDetail = 
                await _unitOfWork.OrderDetailRepository.GetByIdAsync(orderDetail.OrderId, orderDetail.ProductId);

            Assert.True(orderDetail.Quantity == updatedOrderDetail!.Quantity);
        }

        [Fact]
        public async Task Delete()
        {
            OrderDetail? orderDetail = await _unitOfWork.OrderDetailRepository.GetByIdAsync(1,1);
            Assert.NotNull(orderDetail);

            await _unitOfWork.OrderDetailRepository.DeleteAsync(orderDetail.OrderId,orderDetail.ProductId);

            Assert.Null(await _unitOfWork.OrderDetailRepository.GetByIdAsync(orderDetail.OrderId,orderDetail.ProductId));
        }

        [Fact]
        public async Task Insert_NullOrderDetail_ShouldFail()
        {
            OrderDetail? orderDetail = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _unitOfWork.OrderDetailRepository.InsertAsync(orderDetail));
        }

        [Fact]
        public async Task Update_NullOrderDetail_ShouldFail()
        {
            OrderDetail? orderDetail = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _unitOfWork.OrderDetailRepository.UpdateAsync(orderDetail));
        }

        [Fact]
        public async Task Delete_InvalidId_ShouldFail()
        {
            int invalidOrderId = 999;
            int invalidProductId = 999;

            await Assert.ThrowsAsync<ArgumentException>(() => _unitOfWork.OrderDetailRepository.DeleteAsync(invalidOrderId,invalidProductId));
        }
    }
}