using AutoMapper;
using StoreManager.DTO;
using StoreManager.Models;

namespace StoreManager.API.MapperProfile
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Category, CategoryModel>().ReverseMap();
            CreateMap<Employee, EmployeeModel>().ReverseMap();
            CreateMap<Account, LoginModel>().ReverseMap();
            CreateMap<Order, OrderModel>().ReverseMap();
            CreateMap<OrderDetail, OrderDetailModel>().ReverseMap();
            CreateMap<Account, RegisterModel>().ReverseMap();
            CreateMap<Role, RoleModel>().ReverseMap();
            CreateMap<Account, AccountModel>().ReverseMap();
            CreateMap<SupplierTransaction, InputSupplierTransactionModel>().ReverseMap();
            CreateMap<Supplier, SupplierModel>().ReverseMap();
            CreateMap<AccountImage, AccountImageModel>().ReverseMap();
        }
    }
}
