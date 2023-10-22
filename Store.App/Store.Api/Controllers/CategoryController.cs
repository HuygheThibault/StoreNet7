using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Api.Models;
using Store.Api.Repositories;
using Store.Shared.Dto;

namespace Store.Api.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryRepository _CategoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ILogger<CategoryController> logger, ICategoryRepository repository, IMapper mapper
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _CategoryRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                List<Category> items = await _CategoryRepository.GetAllCategories();
                return Ok(_mapper.Map<List<CategoryDto>>(items));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured.");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while getting all Categories.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            try
            {
                var result = await _CategoryRepository.GetCategoryById(id);
                if (result == null) return NotFound();

                return Ok(_mapper.Map<CategoryDto>(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured.");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while getting a Category by id.");
            }
        }

        [HttpGet("{title}")]
        public async Task<IActionResult> GetCategoryByTitle(string title)
        {
            try
            {
                var result = await _CategoryRepository.GetCategoryByTitle(title);
                if (result == null) return NotFound();

                return Ok(_mapper.Map<CategoryDto>(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occured");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while getting a Category by title.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var dbModel = await _CategoryRepository.GetCategoryById(id);
                if (dbModel == null) return NotFound();

                _CategoryRepository.Delete(dbModel);

                if (await _CategoryRepository.SaveChangesAsync())
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occured");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while deleting Category by id.");
            }
            return BadRequest("Failed to delete the item");
        }
    }
}
