using AutoMapper;
using PersonalFinanceManagmentProject.Entities.Dtos;
using PersonalFinanceManagmentProject.Entities.Dtos.CategoryDto;
using PersonalFinanceManagmentProject.Entities.Dtos.TransactionDto;

namespace PersonalFinanceManagmentProject.Entities
{
    public class FinanceMapperProfile : Profile
    {
        public FinanceMapperProfile()
        {
            CreateMap<BillShortDto, Bill>();
                
            CreateMap<Bill, BillShortDto>();

            CreateMap<Bill, BillExpandedDto>()
                .ForMember(m => m.CreateDate, c => c.MapFrom(s => s.CreateDate.Value.Day.ToString() + "." + s.CreateDate.Value.Month.ToString() + "." + s.CreateDate.Value.Year.ToString()))
                .ForMember(m => m.LastUpdateDate, c => c.MapFrom(s => s.LastUpdateDate.Value.Day.ToString() + "." + s.LastUpdateDate.Value.Month.ToString() + "." + s.LastUpdateDate.Value.Year.ToString()));

            CreateMap<TransactionAddDto, Transaction>();

            CreateMap<CategoryDto, Category>();

            CreateMap<Bill, BillNameDto>();

            CreateMap<Category, CategoryNameDto>();
        }
    }
}
