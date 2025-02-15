namespace StoreManager.DTO.Interfaces;

public interface IDto<T>
{
    T Id { get; set; }
    bool IsActive { get; set; }
}

public interface IDto : IDto<int> { }