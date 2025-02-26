namespace StoreManager.Models
{
    public class AzureBlobSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string ContainerName { get; set; } = null!;
    }
}