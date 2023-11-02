using Blazored.LocalStorage;
using Store.Shared.Dto;
using Store.Web.Exceptions;
using Store.Web.Helpers;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Store.Web.Services
{
    public partial class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;

        public ProductService(HttpClient httpClient, ILocalStorageService localStorageService)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProducts(bool refreshRequired = false)
        {
            try
            {
                if (!refreshRequired)
                {
                    bool productExpirationExists = await _localStorageService.ContainKeyAsync(LocalStorageConstants.ProductListExpirationKey);
                    if (productExpirationExists)
                    {
                        DateTime productListExpiration = await _localStorageService.GetItemAsync<DateTime>(LocalStorageConstants.ProductListExpirationKey);
                        if (productListExpiration > DateTime.Now) //get from local storage
                        {
                            if (await _localStorageService.ContainKeyAsync(LocalStorageConstants.ProductListKey))
                            {
                                return await _localStorageService.GetItemAsync<List<ProductDto>>(LocalStorageConstants.ProductListKey);
                            }
                        }
                    }
                }

                //otherwise refresh the list locally from the API and set expiration to 1 minute in future

                var list = await JsonSerializer.DeserializeAsync<IEnumerable<ProductDto>>
                    (await _httpClient.GetStreamAsync($"api/products"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                await _localStorageService.SetItemAsync(LocalStorageConstants.ProductListKey, list);
                await _localStorageService.SetItemAsync(LocalStorageConstants.ProductListExpirationKey, DateTime.Now.AddMinutes(1));

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ProductDto> GetProductById(Guid id)
        {
            var request = await _httpClient.GetAsync($"api/products/{id}");

            if (request != null)
            {
                if (request.IsSuccessStatusCode)
                {
                    return await request.Content.ReadFromJsonAsync<ProductDto>();
                }
                else if (request.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
            }

            throw new HttpRequestFailedException(message: $"Request failed: {request?.StatusCode}, {request}");
        }

        public async Task<ProductDto> AddProduct(ProductDto item)
        {
            var response = await _httpClient.PostAsJsonAsync("api/products", item);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProductDto>();
            }

            throw new HttpRequestFailedException(message: $"Request failed: {response?.StatusCode}, {response}");
        }

        public async Task<ProductDto> UpdateProduct(ProductDto item)
        {
            try
            {
                var itemJson = new StringContent(JsonSerializer.Serialize(item), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"api/products/{item.Id}", itemJson);

                if (response.IsSuccessStatusCode)
                {
                    return await JsonSerializer.DeserializeAsync<ProductDto>(await response.Content.ReadAsStreamAsync());
                }

                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteProduct(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/products/{id}");

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

    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProducts(bool refreshRequired = false);

        Task<ProductDto> GetProductById(Guid id);

        Task<ProductDto> AddProduct(ProductDto item);

        Task<ProductDto> UpdateProduct(ProductDto item);

        Task<bool> DeleteProduct(Guid id);
    }
}
