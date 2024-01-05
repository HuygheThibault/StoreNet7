using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Store.Api.Models;
using Store.Api.Repositories;
using Store.Shared.Dto;
using System.Text.Json;

namespace Store.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        const int maxPageSize = 20;

        public OrderController(ILogger<OrderController> logger, IOrderRepository repository, IMapper mapper
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orderRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders(string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                if (pageSize > maxPageSize)
                {
                    pageSize = maxPageSize;
                }

                var (entities, paginationMetadata) = await _orderRepository.GetAllOrders(name, searchQuery, pageNumber, pageSize);

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

                //User.Claims.FirstOrDefault(x => x.Type == "UserName")?.Value;

                return Ok(_mapper.Map<IEnumerable<OrderDto>>(entities));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured.");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while getting all Orders.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(Guid id)
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
        public async Task<ActionResult<OrderDto>> Put(Guid id, [FromBody] OrderDto request)
        {
            try
            {
                if (id != request.Id) ModelState.AddModelError("Order id", "Id in uri and body must be equal and cannot be changed");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                request.ModifiedBy = User.Identity.Name ?? "Unknown";
                request.ModifiedOn = DateTime.Now;
                request.OrderLines.ToList().ForEach(x => x.ModifiedBy = User.Identity.Name ?? "Unknown");
                request.OrderLines.ToList().ForEach(x => x.ModifiedOn = DateTime.Now);

                var dbModel = await _orderRepository.UpdateOrder(_mapper.Map<Order>(request));

                return Ok(_mapper.Map<OrderDto>(dbModel));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occured");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure while updating a Order by id.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> Add([FromBody] OrderDto request)
        {
            try
            {
                if (await _orderRepository.GetOrderById(request.Id) != null)
                {
                    ModelState.AddModelError("Id", "Id already exists");
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                request.ModifiedBy = User.Identity.Name ?? "Unknown";
                request.ModifiedOn = DateTime.Now;
                request.OrderLines.ToList().ForEach(x => x.ModifiedBy = User.Identity.Name ?? "Unknown");
                request.OrderLines.ToList().ForEach(x => x.ModifiedOn = DateTime.Now);
                request.OrderLines.ToList().ForEach(x => x.CreatedBy = User.Identity.Name ?? "Unknown");
                request.OrderLines.ToList().ForEach(x => x.CreatedOn = DateTime.Now);

                Order newItem = _mapper.Map<Order>(request);

                await _orderRepository.AddOrder(newItem);

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
        public async Task<ActionResult> Delete(Guid id)
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
