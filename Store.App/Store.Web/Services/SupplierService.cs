using Blazored.LocalStorage;
using Store.Shared.Dto;
using Store.Web.Helpers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Store.Web.Services
{
    public partial class SupplierService : ISupplierService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;

        public SupplierService(HttpClient httpClient, ILocalStorageService localStorageService)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
        }

        public async Task<IEnumerable<SupplierDto>> GetAllSuppliers(bool refreshRequired = false)
        {
            try
            {
                return await JsonSerializer.DeserializeAsync<IEnumerable<SupplierDto>>
                    (await _httpClient.GetStreamAsync($"api/suppliers"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<SupplierDto> AddSupplier(SupplierDto item)
        {
            try
            {
                var itemJson = new StringContent(JsonSerializer.Serialize(item), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsJsonAsync("api/suppliers", itemJson);

                if (response.IsSuccessStatusCode)
                {
                    return await JsonSerializer.DeserializeAsync<SupplierDto>(await response.Content.ReadAsStreamAsync());
                }

                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<SupplierDto> UpdateSupplier(SupplierDto item)
        {
            try
            {
                var itemJson = new StringContent(JsonSerializer.Serialize(item), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"api/suppliers/{item.Id}", itemJson);

                if (response.IsSuccessStatusCode)
                {
                    return await JsonSerializer.DeserializeAsync<SupplierDto>(await response.Content.ReadAsStreamAsync());
                }

                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteSupplier(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/suppliers/{id}");

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

    public interface ISupplierService
    {
        Task<IEnumerable<SupplierDto>> GetAllSuppliers(bool refreshRequired = false);

        Task<SupplierDto> AddSupplier(SupplierDto item);

        Task<SupplierDto> UpdateSupplier(SupplierDto item);

        Task<bool> DeleteSupplier(Guid id);
    }
}
