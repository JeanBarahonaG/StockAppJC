using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockAppJC.Models;
using StockAppJC.Services;

namespace StockAppJC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategorieService _categorieService;

        public CategoryController(ICategorieService categorieService)
        {
            _categorieService = categorieService;
        }

        [HttpGet("GetCategories")]
        [Authorize(Roles = "Admin, Regulate")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _categorieService.GetCategories();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("GetCategory")]
        [Authorize(Roles = "Admin, Regulate")]
        public async Task<IActionResult> GetCategory(int id)
        {
            try
            {
                var category = await _categorieService.GetCategory(id);
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("CreateCategory")]
        [Authorize(Roles = "Admin, Regulate")]
        public async Task<IActionResult> CreateCategory(CategoryViewModel category)
        {
            try
            {
                var result = await _categorieService.CreateCategory(category);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("UpdateCategory")]
        [Authorize(Roles = "Admin, Regulate")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryViewModel category)
        {
            try
            {
                var result = await _categorieService.UpdateCategory(id, category);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("DeleteCategory")]
        [Authorize(Roles = "Admin, Regulate")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categorieService.DeleteCategory(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
