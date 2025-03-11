

The StoreManager.Services project contains the business logic for the StoreManager application. It includes services for managing accounts, tokens, products, orders, and more.

## Features

- Account management
- Token management
- Product and order management
- Two-factor authentication (2FA)

## Services

### AccountCommandService

Handles account registration, login, and updates.

### AccountQueryService

Handles retrieving account information.

### TokenService

Handles token generation, validation, and refresh.

### TwoFactorService

Handles sending and verifying 2FA codes.

## Usage

### Account Management

- **Register**: `AccountCommandService.RegisterAsync`
- **Login**: `AccountCommandService.ProcessLoginAsync`
- **Update Account**: `AccountCommandService.UpdateAccount`

### Token Management

- **Generate Token**: `TokenService.GenerateTokenAsync`
- **Validate Token**: `TokenService.IsTokenValidAsync`
- **Revoke Token**: `TokenService.RevokeTokenAsync`
- **Refresh Token**: `TokenService.RefreshToken`

### Azure Blob Storage

The `AzureBlobStorageService` provides methods for storing and retrieving files using Azure Blob Storage.

#### Configuration

Update the `appsettings.json` file with your Azure Storage settings:

### Two-Factor Authentication

- **Send 2FA Code**: `TwoFactorService.Send2FACodeAsync`
- **Verify 2FA Code**: `TwoFactorService.Verify2FACode`
