# StoreManager.Repositories

The StoreManager.Repositories project contains the data access layer for the StoreManager application. It includes repositories for 
managing accounts, tokens, products, orders, and more.

## Features

- Data access for accounts, tokens, products, orders, and more
- Unit of Work pattern for managing transactions

## Repositories

### AccountRepository

Handles data access for accounts.

### TokenRepository

Handles data access for tokens.

### ProductRepository

Handles data access for products.

### OrderRepository

Handles data access for orders.

### RepositoryBase

Provides a base class for all repositories, implementing common data access methods.

## Usage

### Account Management

- **Get Account by Email**: `AccountRepository.GetByEmailAsync`
- **Insert Account**: `AccountRepository.InsertAsync`
- **Update Account**: `AccountRepository.UpdateAsync`

### Token Management

- **Insert Token**: `TokenRepository.InsertAsync`
- **Validate Token**: `TokenRepository.IsTokenValidAsync`
- **Revoke Token**: `TokenRepository.RevokeTokenAsync`
- **Get Token by Refresh Token**: `TokenRepository.GetByRefreshToken`

### Base Repository

The `RepositoryBase` class provides common data access methods that can be used by all repositories. It includes methods for adding, 
updating, deleting, and retrieving entities.

### Conclusion

The `StoreManager.Repositories` project provides a robust data access layer for the StoreManager application, leveraging the Unit of Work pattern and a 
base repository class to streamline common data access operations. This structure ensures maintainability and scalability for the application's data access needs.
