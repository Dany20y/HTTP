using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using HTTP.Models;

namespace HTTP.Services
{
    public class ShopApiService
    {
        private readonly HttpClient client;
        private readonly string baseUrl = "https://localhost:5001/api/Category";

        public ShopApiService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            client = new HttpClient(handler);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var response = await client.GetAsync($"{baseUrl}/categories");
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Category>>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<Category> GetCategoryAsync(int id)
        {
            var response = await client.GetAsync($"{baseUrl}/categories/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var body = await response.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<List<Category>>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return list?[0];
        }

        public async Task<bool> CreateCategoryAsync(string title)
        {
            var dto = new CreateCategoryDto { Title = title };
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{baseUrl}/categories", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var response = await client.DeleteAsync($"{baseUrl}/categories/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCategoryAsync(int id, string newTitle)
        {
            var dto = new CreateCategoryDto { Title = newTitle };
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{baseUrl}/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateProductAsync(Product product)
        {
            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{baseUrl}/categories/{product.CategoryId}/products", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            var response = await client.GetAsync($"{baseUrl}/categories/{categoryId}/products");
            if (!response.IsSuccessStatusCode) return null;
            var body = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Product>>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
