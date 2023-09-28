using AutoMapper;
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] CategoryDto model)
        {
            try
            {
                if (id != model.Id) ModelState.AddModelError("Category id", "Id in uri and body must be equal and cannot be changed");

                var dbModel = await _CategoryRepository.GetCategoryById(id);

                if (dbModel == null) ModelState.AddModelError("Category", $"Could not find item: {model}");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _mapper.Map(model, dbModel); // map model to dbmodel (destination)

                if (await _CategoryRepository.SaveChangesAsync())
                {
                    return Ok(_mapper.Map<CategoryDto>(dbModel));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occured");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while updating a Category by id.");
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> Add([FromBody] CategoryDto request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Please privde a valid Category");
                }

                Category dbItem = await _CategoryRepository.GetCategoryByTitle(request.Title);

                if (dbItem != null)
                {
                    ModelState.AddModelError("Category title", "Category already exists");
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                //handle image upload
                //string currentUrl = _httpContextAccessor.HttpContext.Request.Host.Value;
                //var path = $"{_webHostEnvironment.WebRootPath}\\uploads\\{request.Title}.jpg";
                //var fileStream = System.IO.File.Create(path);
                //fileStream.Write(request.Image, 0, request.Image.Length);
                //fileStream.Close();

                //request.ImageName = $"https://{currentUrl}/uploads/{request.Title}.jpg";

                Category newItem = _mapper.Map<Category>(request);

                _CategoryRepository.Add(newItem);

                if (await _CategoryRepository.SaveChangesAsync())
                {
                    _logger.LogInformation($"Created new Category {newItem.Id}");
                    return Created($"/api/Category/{newItem.Id}", _mapper.Map<CategoryDto>(newItem));
                }

                return BadRequest("Faild to create Category");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occured");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while adding a Category");
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
