﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.Api.Models;
using Store.Api.Repositories;
using Store.Shared.Dto;

namespace Store.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductController(ILogger<ProductController> logger, IProductRepository repository, IMapper mapper
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _productRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                List<Product> items = await _productRepository.GetAllProducts();
                return Ok(_mapper.Map<List<ProductDto>>(items));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured.");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while getting all products.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            try
            {
                var result = await _productRepository.GetProductById(id);
                if (result == null) return NotFound();

                return Ok(_mapper.Map<ProductDto>(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured.");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while getting a product by id.");
            }
        }

        [HttpGet("{title}")]
        public async Task<IActionResult> GetProductByTitle(string title)
        {
            try
            {
                var result = await _productRepository.GetProductByTitle(title);
                if (result == null) return NotFound();

                return Ok(_mapper.Map<ProductDto>(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occured");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while getting a product by title.");
            }
        }

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
        public async Task<ActionResult<ProductDto>> Add([FromBody] ProductDto request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Please privde a valid product");
                }

                Product dbItem = await _productRepository.GetProductByTitle(request.Title);

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

                Product newItem = _mapper.Map<Product>(request);

                _productRepository.Add(newItem);

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
