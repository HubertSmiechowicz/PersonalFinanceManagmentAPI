using Microsoft.AspNetCore.Mvc;
using PersonalFinanceManagmentProject.Entities;
using PersonalFinanceManagmentProject.Entities.Dtos.CategoryDto;
using PersonalFinanceManagmentProject.Services.Interfaces;

namespace PersonalFinanceManagmentProject.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoryController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpPost]
    public ActionResult AddCategory([FromBody] CategoryDto categoryDto)
    {
        _categoryService.AddCategory(categoryDto);
        return Ok();
    }

    [HttpGet("names")]
    public ActionResult<List<CategoryNameDto>> GetCategoryNames()
    {
        return Ok(_categoryService.GetCategoryNames());
    }
}