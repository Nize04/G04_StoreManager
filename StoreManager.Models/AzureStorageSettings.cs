﻿namespace StoreManager.Models
{
    public class AzureStorageSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string ContainerName { get; set; } = null!;
    }
}