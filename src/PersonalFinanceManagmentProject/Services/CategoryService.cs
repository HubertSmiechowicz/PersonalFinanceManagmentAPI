using AutoMapper;
using PersonalFinanceManagmentProject.Entities;
using PersonalFinanceManagmentProject.Entities.Dtos.CategoryDto;
using PersonalFinanceManagmentProject.Services.Interfaces;

namespace PersonalFinanceManagmentProject.Services;

public class CategoryService : ICategoryService
{
    private readonly PersonalFinanceManagmentDbContext _dbContext;
    private readonly IMapper _mapper;

    public CategoryService(PersonalFinanceManagmentDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public void AddCategory(CategoryDto categoryDto)
    {
        _dbContext.Categories.Add(_mapper.Map<Category>(categoryDto));
        _dbContext.SaveChanges();
    }
}