namespace StoreManager.Models
{
    public class AzureBlobStorageSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string ContainerName { get; set; } = null!;
    }
}
