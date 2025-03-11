

The StoreManager.Services project contains the business logic for the StoreManager application. It includes services for managing accounts, tokens, products, orders, and more.

## Features

- Account management
- Token management
- Product and order management
- Two-factor authentication (2FA)

## Services

### AccountCommandService

Handles account registration, login, and updates.

#### Methods

- **RegisterAsync**: Registers a new account.
- **ProcessLoginAsync**: Processes the login for an account.
- **AuthorizeAccountAsync**: Authorizes an account after successful login or 2FA verification.
- **UpdateAccount**: Updates account information.

---------
##### RegisterAsync


public async Task<object> RegisterAsync(Account account)
{

    _logger.LogInformation("Registration Start EmployeeId: {EmployeeId}", account.Id);

    await _unitOfWork.OpenConnectionAsync();
    try
    {
        return await _unitOfWork.AccountRepository.InsertAsync(account);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Registration failed EmployeeId: {EmployeeId}", account.Id);
        throw;
    }
    finally
    {
        await _unitOfWork.CloseConnectionAsync();
    }
}

---------
##### ProcessLoginAsync
 public async Task<LoginResult> ProcessLoginAsync(string email, string password, string clientKey)
 {
 
     // Attempt to retrieve the account associated with the provided email.
    var account = await _unitOfWork.AccountRepository.GetByEmailAsync(email);

    // If no account is found with the provided email, return an invalid credentials response.
    if (account == null)
    {
        _logger.LogWarning("Login failed for Email: {Email}. Account not found.", email);
        return new LoginResult { Status = LoginStatus.InvalidCredentials };
    }

    // Check if the user has been locked out due to too many failed login attempts.
    if (_loginAttemptTracker.IsLoginLockedOut(clientKey))
    {
        // If locked out, return a LockedOut status.
        return new LoginResult { Status = LoginStatus.LockedOut };
    }

    // Validate the entered password against the stored password and salt.
    if (!PasswordHelper.ValidatePassword(password, account.Password, account.Salt))
    {
        // If the password is incorrect, log the failed login attempt and register a failed attempt.
        _logger.LogWarning("Login failed for Email: {Email}. Incorrect password.", email);
        _loginAttemptTracker.RegisterLoginFailedAttempt(clientKey);
        
        // Return an invalid credentials response.
        return new LoginResult { Status = LoginStatus.InvalidCredentials };
    }

    // If the account requires two-factor authentication (2FA), initiate the 2FA process.
    if (account.Requires2FA)
    {
        // Attempt to send the 2FA code to the user's email.
        if (!await Send2FACodeAsync(email))
        {
            // If sending the 2FA code fails, log the error and return a failure status.
            _logger.LogError("Failed to send 2FA code for Email: {Email}", account.Email);
            return new LoginResult { Status = LoginStatus.Failed2FASending };
        }

        // If 2FA is required, return a Requires2FA status along with the account details.
        return new LoginResult { Status = LoginStatus.Requires2FA, Account = account };
    }

    // If login is successful and no 2FA is required, reset any failed login attempts for the client.
    _loginAttemptTracker.ResetLoginAttempts(clientKey);

    // Return a Success status along with the authenticated account.
    return new LoginResult { Status = LoginStatus.Success, Account = account };
 }
 
--------
##### AuthorizeAccountAsync

public async Task AuthorizeAccountAsync(Account account)
{

     // Ensure the account parameter is not null.
     if (account == null)
     {
        throw new ArgumentNullException(nameof(account), "Account cannot be null.");
     }
            
     // Retrieve the user's IP address and device information.
     string ipAddress = _userRequestHelper.GetUserIpAddress()!;
     string deviceInfo = _userRequestHelper.GetDeviceDetails();

     try
        {
            // Extract client information from the device details.
            var clientInfo = _userRequestHelper.GetClientInfoFromDeviceInfo(deviceInfo);

            // Check if the request is from a known bot.
            if (SecurityHelper.IsKnownBot(clientInfo))
            {
                _logger.LogWarning("⚠️ Bot detected! Blocking authorization attempt. IP: {IpAddress}, Device Info: {DeviceInfo}", ipAddress, deviceInfo);
                 throw new SecurityException("Authorization blocked due to bot activity.");
            }

            // Check if the IP address is suspicious (e.g., flagged as malicious or unusual).
            if (SecurityHelper.IsSuspiciousIp(ipAddress))
            {
                 _logger.LogWarning("⚠️ Suspicious IP detected! Additional verification needed. IP: {IpAddress}", ipAddress);
                 throw new SecurityException("Authorization blocked due to suspicious IP address activity.");
             }

            // Generate an authentication token for the user account.
            var tokenResponse = _tokenService.GenerateTokenAsync(account);

            // Check if the token generation failed or the token is invalid.
            if (tokenResponse == null || string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
            {
                _logger.LogError("❌ Failed to generate token for account {UserId}, IP: {IpAddress}", account.Id, ipAddress);
                throw new InvalidOperationException("Failed to generate authentication token.");
            }

            // Store the generated token in the database, including relevant metadata like IP address and device info.
            await _tokenService.InsertAsync(new Token
            {
                AccountId = tokenResponse.AccountId,
                AccessTokenHash = tokenResponse.AccessToken.HashToken(),
                RefreshToken = tokenResponse.RefreshToken,
                AccessTokenExpiresAt = tokenResponse.AccessTokenExpiresAt,
                RefreshTokenExpiresAt = tokenResponse.RefreshTokenExpiresAt,
                IpAddress = ipAddress,
                DeviceInfo = deviceInfo,
                CreateDate = DateTime.UtcNow
            });

            // Log the successful authorization.
            _logger.LogInformation("✅ Account successfully authorized. UserId: {UserId}, IP: {IpAddress}, Device Info: {DeviceInfo}",
                account.Id, ipAddress, deviceInfo);
            }
            catch (SecurityException secEx)
            {
                _logger.LogError(secEx, "❌ Security issue during authorization: {Message}", secEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error during authorization process for UserId: {UserId}", account.Id);
                throw new InvalidOperationException("An error occurred while authorizing the account. Please try again.");
            }
}

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
