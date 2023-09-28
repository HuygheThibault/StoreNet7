using Blazored.LocalStorage;
using Store.Shared.Dto;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Store.Web.Services
{
    public partial class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;

        public CategoryService(HttpClient httpClient, ILocalStorageService localStorageService)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategorys()
        {
            try
            {
                var list = await JsonSerializer.DeserializeAsync<IEnumerable<CategoryDto>>
                    (await _httpClient.GetStreamAsync($"api/categories"), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                return list;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<CategoryDto> AddCategory(CategoryDto item)
        {
            try
            {
                var itemJson = new StringContent(JsonSerializer.Serialize(item), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsJsonAsync("api/categories", itemJson);

                if (response.IsSuccessStatusCode)
                {
                    return await JsonSerializer.DeserializeAsync<CategoryDto>(await response.Content.ReadAsStreamAsync());
                }

                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<CategoryDto> UpdateCategory(CategoryDto item)
        {
            try
            {
                var itemJson = new StringContent(JsonSerializer.Serialize(item), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"api/categories/{item.Id}", itemJson);

                if (response.IsSuccessStatusCode)
                {
                    return await JsonSerializer.DeserializeAsync<CategoryDto>(await response.Content.ReadAsStreamAsync());
                }

                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteCategory(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/categories/{id}");

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

    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategorys();

        Task<CategoryDto> AddCategory(CategoryDto item);

        Task<CategoryDto> UpdateCategory(CategoryDto item);

        Task<bool> DeleteCategory(Guid id);
    }
}
