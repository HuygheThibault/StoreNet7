using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.Api.Models;
using Store.Api.Repositories;
using Store.Shared.Dto;

namespace Store.Api.Controllers
{
    [ApiController]
    [Route("api/suppliers")]
    public class SupplierController : ControllerBase
    {
        private readonly ILogger<SupplierController> _logger;
        private readonly ISupplierRepository _SupplierRepository;
        private readonly IMapper _mapper;

        public SupplierController(ILogger<SupplierController> logger, ISupplierRepository repository, IMapper mapper
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _SupplierRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSuppliers()
        {
            try
            {
                List<Supplier> items = await _SupplierRepository.GetAllSuppliers();
                return Ok(_mapper.Map<List<SupplierDto>>(items));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured.");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while getting all Suppliers.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplierById(Guid id)
        {
            try
            {
                var result = await _SupplierRepository.GetSupplierById(id);
                if (result == null) return NotFound();

                return Ok(_mapper.Map<SupplierDto>(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured.");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while getting a Supplier by id.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] SupplierDto model)
        {
            try
            {
                if (id != model.Id) ModelState.AddModelError("Supplier id", "Id in uri and body must be equal and cannot be changed");

                var dbModel = await _SupplierRepository.GetSupplierById(id);

                if (dbModel == null) ModelState.AddModelError("Supplier", $"Could not find item: {model}");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _mapper.Map(model, dbModel); // map model to dbmodel (destination)

                if (await _SupplierRepository.SaveChangesAsync())
                {
                    return Ok(_mapper.Map<SupplierDto>(dbModel));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occured");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while updating a Supplier by id.");
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult<SupplierDto>> Add([FromBody] SupplierDto request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Please privde a valid Supplier");
                }

                Supplier dbItem = await _SupplierRepository.GetSupplierById(request.Id);

                if (dbItem != null)
                {
                    ModelState.AddModelError("Id", "Id already exists");
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                Supplier newItem = _mapper.Map<Supplier>(request);

                _SupplierRepository.Add(newItem);

                if (await _SupplierRepository.SaveChangesAsync())
                {
                    _logger.LogInformation($"Created new Supplier {newItem.Id}");
                    return Created($"/api/Supplier/{newItem.Id}", _mapper.Map<SupplierDto>(newItem));
                }

                return BadRequest("Faild to create Supplier");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occured");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while adding a Supplier");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var dbModel = await _SupplierRepository.GetSupplierById(id);
                if (dbModel == null) return NotFound();

                _SupplierRepository.Delete(dbModel);

                if (await _SupplierRepository.SaveChangesAsync())
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occured");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while deleting Supplier by id.");
            }
            return BadRequest("Failed to delete the item");
        }
    }
}
