using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.Api.Models;
using Store.Api.Repositories;
using Store.Shared.Dto;
using System.Text.Json;

namespace Store.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        const int maxPageSize = 20;

        public ProductController(ILogger<ProductController> logger, IProductRepository repository, IMapper mapper
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _productRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts(string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                if (pageSize > maxPageSize)
                {
                    pageSize = maxPageSize;
                }

                var (entities, paginationMetadata) = await _productRepository.GetAllProducts(name, searchQuery, pageNumber, pageSize);

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

                return Ok(_mapper.Map<IEnumerable<ProductDto>>(entities));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured.");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while getting all products.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            try
            {
                Guid guidId;
                bool isGuid = Guid.TryParse(id, out guidId);

                if (isGuid)
                {
                    var result = await _productRepository.GetProductById(guidId);
                    if (result == null) return NotFound();
                    return Ok(_mapper.Map<ProductDto>(result));
                }
                else
                {
                    var result = await _productRepository.GetProductByTitle(id);
                    if (result == null) return NotFound();
                    return Ok(_mapper.Map<ProductDto>(result));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured.");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while getting a product by id.");
            }
        }

        //[HttpGet("{title}")]
        //public async Task<IActionResult> GetProductByTitle(string title)
        //{
        //    try
        //    {
        //        var result = await _productRepository.GetProductByTitle(title);
        //        if (result == null) return NotFound();

        //        return Ok(_mapper.Map<ProductDto>(result));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "error occured");
        //        return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while getting a product by title.");
        //    }
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] ProductDto model)
        {
            try
            {
                if (id != model.Id) ModelState.AddModelError("Product id", "Id in uri and body must be equal and cannot be changed");

                var dbModel = await _productRepository.GetProductById(id);

                if (dbModel == null) ModelState.AddModelError("Product", $"Could not find item: {model}");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                model.ModifiedOn = DateTime.Now;
                model.ModifiedBy = User.Identity.Name ?? "Unknown";
                model.Category = null;

                _mapper.Map(model, dbModel); // map model to dbmodel (destination)

                if (await _productRepository.SaveChangesAsync())
                {
                    return Ok(_mapper.Map<ProductDto>(dbModel));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occured");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while updating a product by id.");
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> Add([FromBody] ProductDto model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest("Please privde a valid product");
                }

                Product dbItem = await _productRepository.GetProductByTitle(model.Title);

                if (dbItem != null)
                {
                    ModelState.AddModelError("Product title", "Product already exists");
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

                model.ModifiedOn = DateTime.Now;
                model.ModifiedBy = User.Identity.Name ?? "Unknown";
                model.CreatedOn = DateTime.Now;
                model.CreatedBy = User.Identity.Name ?? "Unknown";

                Product newItem = _mapper.Map<Product>(model);

                _productRepository.AddProduct(newItem);

                if (await _productRepository.SaveChangesAsync())
                {
                    _logger.LogInformation($"Created new product {newItem.Id}");
                    return Created($"/api/product/{newItem.Id}", _mapper.Map<ProductDto>(newItem));
                }

                return BadRequest("Faild to create product");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occured");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while adding a product");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var dbModel = await _productRepository.GetProductById(id);
                if (dbModel == null) return NotFound();

                _productRepository.Delete(dbModel);

                if (await _productRepository.SaveChangesAsync())
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occured");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while deleting product by id.");
            }
            return BadRequest("Failed to delete the item");
        }
    }
}
