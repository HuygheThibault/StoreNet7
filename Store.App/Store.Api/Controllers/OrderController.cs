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
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderLineRepository _orderLineRepository;
        private readonly IMapper _mapper;

        public OrderController(ILogger<OrderController> logger, IOrderRepository repository, IOrderLineRepository orderLineRepository, IMapper mapper
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orderRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _orderLineRepository = orderLineRepository ?? throw new ArgumentNullException(nameof(orderLineRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                List<Order> items = await _orderRepository.GetAllOrders();
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
                var result = await _orderRepository.GetOrderById(id);
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

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var dbModel = await _orderRepository.UpdateOrder(_mapper.Map<Order>(model));

                return Ok(_mapper.Map<OrderDto>(dbModel));
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

                Order dbItem = await _orderRepository.GetOrderById(request.Id);

                if (dbItem != null)
                {
                    ModelState.AddModelError("Id", "Id already exists");
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                Order newItem = _mapper.Map<Order>(request);

                foreach (OrderLine orderLine in newItem.OrderLines)
                {
                    orderLine.OrderId = newItem.Id;
                    _orderLineRepository.AddOrderLine(orderLine);
                }

                _orderRepository.AddOrder(newItem);

                if (await _orderRepository.SaveChangesAsync())
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
                var dbModel = await _orderRepository.GetOrderById(id);
                if (dbModel == null) return NotFound();

                _orderRepository.Delete(dbModel);

                if (await _orderRepository.SaveChangesAsync())
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
