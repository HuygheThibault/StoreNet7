using Store.Shared.Dto;
using Store.Web.Exceptions;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Store.Web.Services
{
    public partial class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;

        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrders()
        {
            var request = await _httpClient.GetAsync($"api/orders");

            if (request != null)
            {
                if (request.IsSuccessStatusCode)
                {
                    return await request.Content.ReadFromJsonAsync<IEnumerable<OrderDto>>();
                }
                else if (request.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
            }

            throw new HttpRequestFailedException(message: $"Request failed: {request?.StatusCode}, {request}");
        }

        public async Task<OrderDto> GetOrderById(Guid id)
        {
            var request = await _httpClient.GetAsync($"api/orders/{id}");

            if (request != null)
            {
                if (request.IsSuccessStatusCode)
                {
                    return await request.Content.ReadFromJsonAsync<OrderDto>();
                }
                else if (request.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
            }

            throw new HttpRequestFailedException(message: $"Request failed: {request?.StatusCode}, {request}");
        }

        public async Task<OrderDto> AddOrder(OrderDto item)
        {
            var response = await _httpClient.PostAsJsonAsync("api/orders", item);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<OrderDto>();
            }

            throw new HttpRequestFailedException(message: $"Request failed: {response?.StatusCode}, {response}");
        }

        public async Task<OrderDto> UpdateOrder(OrderDto item)
        {
            var itemJson = new StringContent(JsonSerializer.Serialize(item), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/orders/{item.Id}", itemJson);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<OrderDto>();
            }

            throw new HttpRequestFailedException(message: $"Request failed: {response?.StatusCode}, {response}");
        }

        public async Task<bool> DeleteOrder(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/orders/{id}");

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            throw new HttpRequestFailedException(message: $"Request failed: {response?.StatusCode}, {response}");
        }
    }

    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrders();

        Task<OrderDto> GetOrderById(Guid id);

        Task<OrderDto> AddOrder(OrderDto item);

        Task<OrderDto> UpdateOrder(OrderDto item);

        Task<bool> DeleteOrder(Guid id);
    }
}
