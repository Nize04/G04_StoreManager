using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyAttributes;
using StoreManager.DTO;
using StoreManager.Models;

namespace StoreManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeJwt("Manager")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryCommandService _categoryCommandService;
        private readonly ICategoryQueryService _categoryQueryService;
        private readonly ILogger<CategoryController> _logger;
        private readonly IMapper _mapper;

        public CategoryController(
            ICategoryCommandService categoryCommandService,
            ICategoryQueryService categoryQueryService,
            ILogger<CategoryController> logger,
            IMapper mapper)
        {
            _categoryCommandService = categoryCommandService ?? throw new ArgumentNullException(nameof(categoryCommandService));
            _categoryQueryService = categoryQueryService ?? throw new ArgumentNullException(nameof(categoryQueryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("GetCategoryByName")]
        public async Task<IActionResult> GetCategoryByName(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                _logger.LogWarning("GetCategoryByName called with an empty or null categoryName.");
                return BadRequest("Category name is required.");
            }

            try
            {
                _logger.LogInformation("Attempting to retrieve category with name: {CategoryName}", categoryName);
                var category = await _categoryQueryService.GetByNameAsync(categoryName);

                if (category == null)
                {
                    _logger.LogWarning("Category not found for name: {CategoryName}", categoryName);
                    return NotFound($"Category '{categoryName}' not found.");
                }

                _logger.LogInformation("Successfully retrieved category with name: {CategoryName}", categoryName);
                return Ok(_mapper.Map<CategoryModel>(category));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving category by name: {CategoryName}", categoryName);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryModel categoryModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("AddCategory called with invalid model state.");
                return BadRequest(ModelState);
            }

            try
            {
                var category = _mapper.Map<Category>(categoryModel);

                _logger.LogInformation("Adding a new category with name: {CategoryName}", category.Name);
                await _categoryCommandService.AddCategoryAsync(category);

                _logger.LogInformation("Successfully added a new category: {CategoryName}", category.Name);
                return CreatedAtAction(nameof(GetCategoryByName), new { categoryName = category.Name }, "Category added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new category: {CategoryName}", categoryModel?.Name);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}