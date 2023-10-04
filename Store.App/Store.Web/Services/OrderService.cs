using Blazored.LocalStorage;
using Store.Shared.Dto;
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
            try
            {
                return await JsonSerializer.DeserializeAsync<IEnumerable<OrderDto>>
                    (await _httpClient.GetStreamAsync($"api/orders"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OrderDto> GetOrderById(Guid id)
        {
            try
            {
                return await JsonSerializer.DeserializeAsync<OrderDto>
                    (await _httpClient.GetStreamAsync($"api/orderss/{id}"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OrderDto> AddOrder(OrderDto item)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/orders", item);

                if (response.IsSuccessStatusCode)
                {
                    return await JsonSerializer.DeserializeAsync<OrderDto>(await response.Content.ReadAsStreamAsync());
                }

                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<OrderDto> UpdateOrder(OrderDto item)
        {
            try
            {
                var itemJson = new StringContent(JsonSerializer.Serialize(item), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"api/orderss/{item.Id}", itemJson);

                if (response.IsSuccessStatusCode)
                {
                    return await JsonSerializer.DeserializeAsync<OrderDto>(await response.Content.ReadAsStreamAsync());
                }

                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteOrder(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Order/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
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
