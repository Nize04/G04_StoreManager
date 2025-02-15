﻿using Microsoft.Extensions.Logging;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using StoreManager.Facade.Interfaces.Services;

namespace StoreManager.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SupplierService> _logger;

        public SupplierService(IUnitOfWork unitOfWork, ILogger<SupplierService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> AddSupplierAsync(Supplier supplier)
        {
            _logger.LogInformation("adding Supplier in db: {@Supplier}", supplier);

            await _unitOfWork.OpenConnectionAsync();
            try
            {
                int supplierId = (int)await _unitOfWork.SupplierRepository.InsertAsync(supplier);
                _logger.LogInformation("Employee added successfully with SupplierId: {SupplierId}", supplierId);
                return supplierId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding Supplier {@Supplier}", supplier);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }

        public async Task<Supplier?> GetSupplierById(int id)
        {
            _logger.LogInformation("Retrieving supplier by ID: {SupplierId}", id);

            await _unitOfWork.OpenConnectionAsync();

            try
            {
                var supplier = await _unitOfWork.SupplierRepository.GetByIdAsync(id);

                if (supplier == null)
                {
                    _logger.LogWarning("No supplier found with ID: {SupplierId}", id);
                }
                else
                {
                    _logger.LogInformation("Supplier retrieved successfully: {SupplierId}", id);
                }

                return supplier;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving supplier with ID: {SupplierId}", id);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }

        public async Task<IEnumerable<Supplier>> GetSuppliersByCountryAsync(string country)
        {
            _logger.LogInformation("Retrieving suppliers from country: {Country}", country);

            await _unitOfWork.OpenConnectionAsync();

            try
            {
                var suppliers = await _unitOfWork.SupplierRepository.GetAsync(s => s.Country == country);

                if (!suppliers.Any())
                {
                    _logger.LogWarning("No suppliers found for country: {Country}", country);
                }
                else
                {
                    _logger.LogInformation("Suppliers retrieved successfully for country: {Country}", country);
                }

                return suppliers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving suppliers from country: {Country}", country);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }
    }
}