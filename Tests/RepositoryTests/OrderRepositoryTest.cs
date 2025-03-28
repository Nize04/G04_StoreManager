﻿using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
namespace StoreManager.Tests.RepositoryTests
{
    [Collection("Database Tests")]
    public class OrderRepositoryTest : RepositoryTestBase
    {
        public OrderRepositoryTest(IUnitOfWork unitOfWork, DatabaseFixture fixture) : base(unitOfWork, fixture)
        {
        }

        [Fact]
        public async Task Insert()
        {
            int orderId = (int)await _unitOfWork.OrderRepository.InsertAsync(new Order() { CustomerId = 1, EmployeeId = 2 });

            Assert.True(orderId > 0);
        }

        [Fact]
        public async Task Update()
        {
            Order? order = await _unitOfWork.OrderRepository.GetByIdAsync(2);
            Assert.NotNull(order);
            order.Description = "new" + order.Description;

            await _unitOfWork.OrderRepository.UpdateAsync(order);

            Order updatedOrder = await _unitOfWork.OrderRepository.GetByIdAsync(order.Id)!;

            Assert.True(order.Description == updatedOrder!.Description);
        }

        [Fact]
        public async Task Delete()
        {
            Order? order = await _unitOfWork.OrderRepository.GetByIdAsync(1);
            Assert.NotNull(order);

            await _unitOfWork.OrderRepository.DeleteAsync(order.Id);

            Assert.Null(await _unitOfWork.OrderRepository.GetByIdAsync(1)!);
        }

        [Fact]
        public async Task Insert_NullOrder_ShouldFail()
        {
            Order? order = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _unitOfWork.OrderRepository.InsertAsync(order));
        }

        [Fact]
        public async Task Update_NullOrder_ShouldFail()
        {
            Order? order = null;

            await Assert.ThrowsAsync<ArgumentNullException>(() => _unitOfWork.OrderRepository.UpdateAsync(order));
        }

        [Fact]
        public async Task Delete_InvalidId_ShouldFail()
        {
            int invalidOrderId = 999;

            await Assert.ThrowsAsync<ArgumentException>(() => _unitOfWork.OrderRepository.DeleteAsync(invalidOrderId));
        }
    }
}