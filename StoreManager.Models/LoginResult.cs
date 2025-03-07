﻿using StoreManager.DTO;

namespace StoreManager.Models
{
    public class LoginResult
    {
        public Account Account { get; set; } = null!;
        public LoginStatus Status { get; set; }
    }

    public enum LoginStatus
    {
        Success,
        Requires2FA,
        LockedOut,
        InvalidCredentials,
        Failed2FASending
    }
    public enum TwoFAResult
    {
        Success,
        InvalidCode,
        LockedOut,
        AccountNotFound
    }
}