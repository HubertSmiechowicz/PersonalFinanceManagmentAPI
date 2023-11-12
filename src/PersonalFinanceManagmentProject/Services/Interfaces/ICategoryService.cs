using PersonalFinanceManagmentProject.Entities;
using PersonalFinanceManagmentProject.Entities.Dtos.CategoryDto;

namespace PersonalFinanceManagmentProject.Services.Interfaces;

public interface ICategoryService
{
    void AddCategory(CategoryDto categoryDto);
}