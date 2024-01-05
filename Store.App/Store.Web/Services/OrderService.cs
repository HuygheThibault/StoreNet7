using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Store.Shared.Dto;
using Store.Shared.Modals;
using Store.Web.Exceptions;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace Store.Web.Services
{
    public partial class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;

        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Tuple<IEnumerable<OrderDto>, PaginationMetadata>> GetAllOrders(string? name = null, string? searchQuery = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(name))
            {
                query["name"] = name;
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query["searchQuery"] = searchQuery;
            }

            query["pageNumber"] = pageNumber.ToString();
            query["pageSize"] = pageSize.ToString();

            var request = await _httpClient.GetAsync(QueryHelpers.AddQueryString($"api/orders", query));

            if (request != null)
            {
                if (request.IsSuccessStatusCode)
                {
                    request.Headers.TryGetValues("X-Pagination", out IEnumerable<string> headerValue);

                    return Tuple.Create(await request.Content.ReadFromJsonAsync<IEnumerable<OrderDto>>(), JsonConvert.DeserializeObject<PaginationMetadata>(headerValue.FirstOrDefault()));
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
            var itemJson = new StringContent(System.Text.Json.JsonSerializer.Serialize(item), Encoding.UTF8, "application/json");
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
        Task<Tuple<IEnumerable<OrderDto>, PaginationMetadata>> GetAllOrders(string? name = null, string? searchQuery = null, int pageNumber = 1, int pageSize = 10);

        Task<OrderDto> GetOrderById(Guid id);

        Task<OrderDto> AddOrder(OrderDto item);

        Task<OrderDto> UpdateOrder(OrderDto item);

        Task<bool> DeleteOrder(Guid id);
    }
}
