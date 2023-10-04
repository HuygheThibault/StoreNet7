using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.Api.Models;
using Store.Api.Repositories;
using Store.Shared.Dto;

namespace Store.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderRepository _OrderRepository;
        private readonly IMapper _mapper;

        public OrderController(ILogger<OrderController> logger, IOrderRepository repository, IMapper mapper
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _OrderRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                List<Order> items = await _OrderRepository.GetAllOrders();
                return Ok(_mapper.Map<List<OrderDto>>(items));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured.");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while getting all Orders.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            try
            {
                var result = await _OrderRepository.GetOrderById(id);
                if (result == null) return NotFound();

                return Ok(_mapper.Map<OrderDto>(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured.");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while getting a Order by id.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] OrderDto model)
        {
            try
            {
                if (id != model.Id) ModelState.AddModelError("Order id", "Id in uri and body must be equal and cannot be changed");

                var dbModel = await _OrderRepository.GetOrderById(id);

                if (dbModel == null) ModelState.AddModelError("Order", $"Could not find item: {model}");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _mapper.Map(model, dbModel); // map model to dbmodel (destination)

                if (await _OrderRepository.SaveChangesAsync())
                {
                    return Ok(_mapper.Map<OrderDto>(dbModel));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occured");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while updating a Order by id.");
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> Add([FromBody] OrderDto request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Please privde a valid Order");
                }

                Order dbItem = await _OrderRepository.GetOrderById(request.Id);

                if (dbItem != null)
                {
                    ModelState.AddModelError("Id", "Id already exists");
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                Order newItem = _mapper.Map<Order>(request);

                _OrderRepository.Add(newItem);

                if (await _OrderRepository.SaveChangesAsync())
                {
                    _logger.LogInformation($"Created new Order {newItem.Id}");
                    return Created($"/api/Order/{newItem.Id}", _mapper.Map<OrderDto>(newItem));
                }

                return BadRequest("Faild to create Order");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occured");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while adding a Order");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var dbModel = await _OrderRepository.GetOrderById(id);
                if (dbModel == null) return NotFound();

                _OrderRepository.Delete(dbModel);

                if (await _OrderRepository.SaveChangesAsync())
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occured");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while deleting Order by id.");
            }
            return BadRequest("Failed to delete the item");
        }
    }
}
