using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnlineShopConsoleApp
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ItemsCount { get; set; }
    }

    public class CreateCategoryDto
    {
        public string Title { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
    }

    class Program
    {
        private static readonly HttpClient client;
        private static readonly string baseUrl = "https://localhost:5001/api/Category";

        static Program()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            client = new HttpClient(handler);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        static async Task Main(string[] args)
        {
            bool running = true;

            while (running)
            {
                DisplayMainMenu();
                string option = Console.ReadLine();
                Console.WriteLine();

                try
                {
                    switch (option)
                    {
                        case "1": await ListCategories(); break;
                        case "2": await GetCategoryDetails(); break;
                        case "3": await CreateCategory(); break;
                        case "4": await DeleteCategory(); break;
                        case "5": await UpdateCategoryTitle(); break;
                        case "6": await CreateProduct(); break;
                        case "7": await ListProductsByCategory(); break;
                        case "0":
                            running = false;
                            Console.WriteLine("Multumim ca ai folosit sistemul nostru!");
                            break;
                        default:
                            Console.WriteLine("Optiune invalida. Incearca din nou.");
                            break;
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Eroare server: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Eroare: {ex.Message}");
                }

                if (running)
                {
                    Console.WriteLine("\nApasa orice tasta pentru a continua...");
                    Console.ReadKey();
                }
            }
        }

        static void DisplayMainMenu()
        {
            Console.Clear();
            Console.WriteLine("=== SISTEM DE GESTIONARE MAGAZIN ONLINE ===");
            Console.WriteLine($"Timpul curent: {DateTime.Now}");
            Console.WriteLine();
            Console.WriteLine("1. Afiseaza toate categoriile");
            Console.WriteLine("2. Vezi detalii categorie");
            Console.WriteLine("3. Creeaza categorie noua");
            Console.WriteLine("4. Sterge categorie");
            Console.WriteLine("5. Actualizeaza numele categoriei");
            Console.WriteLine("6. Adauga produs nou");
            Console.WriteLine("7. Afiseaza produsele dintr-o categorie");
            Console.WriteLine("0. Iesire");
            Console.Write("Alege o optiune: ");
        }

        static async Task ListCategories()
        {
            Console.WriteLine("LISTA CATEGORII");
            HttpResponseMessage response = await client.GetAsync($"{baseUrl}/categories");

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<Category>>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (categories.Count == 0)
                {
                    Console.WriteLine("Nu exista categorii.");
                }
                else
                {
                    foreach (var c in categories)
                    {
                        Console.WriteLine($"ID: {c.Id} | Nume: {c.Name} | Produse: {c.ItemsCount}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Eroare: {response.StatusCode}");
            }
        }

        static async Task GetCategoryDetails()
        {
            Console.Write("Introdu ID-ul categoriei: ");
            if (int.TryParse(Console.ReadLine(), out int categoryId))
            {
                HttpResponseMessage response = await client.GetAsync($"{baseUrl}/categories/{categoryId}");

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var categories = JsonSerializer.Deserialize<List<Category>>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (categories?.Count > 0)
                    {
                        var c = categories[0];
                        Console.WriteLine($"ID: {c.Id} | Nume: {c.Name} | Produse: {c.ItemsCount}");
                    }
                    else
                    {
                        Console.WriteLine("Categoria nu a fost gasita.");
                    }
                }
                else
                {
                    Console.WriteLine("Categoria nu a fost gasita.");
                }
            }
            else
            {
                Console.WriteLine("Format ID invalid.");
            }
        }

        static async Task CreateCategory()
        {
            Console.Write("Introdu numele categoriei: ");
            string title = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Numele nu poate fi gol.");
                return;
            }

            var newCategory = new CreateCategoryDto { Title = title };
            var json = JsonSerializer.Serialize(newCategory);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync($"{baseUrl}/categories", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Categoria a fost creata.");
            }
            else
            {
                Console.WriteLine($"Eroare: {response.StatusCode}");
            }
        }

        static async Task DeleteCategory()
        {
            Console.Write("Introdu ID-ul categoriei de sters: ");
            if (int.TryParse(Console.ReadLine(), out int categoryId))
            {
                Console.Write("Esti sigur? (y/n): ");
                string confirmation = Console.ReadLine().ToLower();

                if (confirmation != "y") return;

                HttpResponseMessage response = await client.DeleteAsync($"{baseUrl}/categories/{categoryId}");

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Categoria a fost stearsa.");
                }
                else
                {
                    Console.WriteLine("Eroare la stergerea categoriei.");
                }
            }
            else
            {
                Console.WriteLine("ID invalid.");
            }
        }

        static async Task UpdateCategoryTitle()
        {
            Console.Write("Introdu ID-ul categoriei: ");
            if (int.TryParse(Console.ReadLine(), out int categoryId))
            {
                HttpResponseMessage response = await client.GetAsync($"{baseUrl}/categories/{categoryId}");
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Categoria nu a fost gasita.");
                    return;
                }

                Console.Write("Introdu noul nume: ");
                string newTitle = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(newTitle))
                {
                    Console.WriteLine("Numele nu poate fi gol.");
                    return;
                }

                var updateDto = new CreateCategoryDto { Title = newTitle };
                var json = JsonSerializer.Serialize(updateDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage putResponse = await client.PutAsync($"{baseUrl}/{categoryId}", content);

                if (putResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine("Categoria a fost actualizata.");
                }
                else
                {
                    Console.WriteLine("Eroare la actualizare.");
                }
            }
            else
            {
                Console.WriteLine("ID invalid.");
            }
        }

        static async Task CreateProduct()
        {
            Console.Write("Introdu ID-ul categoriei: ");
            if (!int.TryParse(Console.ReadLine(), out int categoryId)) { Console.WriteLine("ID invalid."); return; }

            HttpResponseMessage check = await client.GetAsync($"{baseUrl}/categories/{categoryId}");
            if (!check.IsSuccessStatusCode) { Console.WriteLine("Categoria nu a fost gasita."); return; }

            Console.Write("Introdu numele produsului: ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Numele este obligatoriu."); return; }

            Console.Write("Introdu pretul produsului: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price)) { Console.WriteLine("Pret invalid."); return; }

            var product = new Product { Title = name, Price = price, CategoryId = categoryId };
            var json = JsonSerializer.Serialize(product);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync($"{baseUrl}/categories/{categoryId}/products", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Produsul a fost creat.");
            }
            else
            {
                Console.WriteLine("Eroare la crearea produsului.");
            }
        }

        static async Task ListProductsByCategory()
        {
            Console.Write("Introdu ID-ul categoriei: ");
            if (!int.TryParse(Console.ReadLine(), out int categoryId)) { Console.WriteLine("ID invalid."); return; }

            HttpResponseMessage response = await client.GetAsync($"{baseUrl}/categories/{categoryId}/products");
            if (!response.IsSuccessStatusCode) { Console.WriteLine("Nu au fost gasite produse."); return; }

            string body = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<List<Product>>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            foreach (var p in products)
            {
                Console.WriteLine($"ID: {p.Id} | Nume: {p.Title} | Pret: {p.Price}");
            }
        }
    }
}
