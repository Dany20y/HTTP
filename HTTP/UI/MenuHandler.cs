using HTTP.Services;
using HTTP.Models;

namespace HTTP.UI
{
    public class MenuHandler
    {
        private readonly ShopApiService api = new ShopApiService();

        public async Task RunAsync()
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
                catch (Exception ex)
                {
                    Console.WriteLine($"Eroare: {ex.Message}");
                }

                if (running)
                {
                    Console.WriteLine("\nApasa orice tasta pentru a continua...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        private void DisplayMainMenu()
        {
            Console.WriteLine("===== MENIU PRINCIPAL =====");
            Console.WriteLine("1. Lista categorii");
            Console.WriteLine("2. Detalii categorie");
            Console.WriteLine("3. Creeaza categorie");
            Console.WriteLine("4. Sterge categorie");
            Console.WriteLine("5. Actualizeaza titlu categorie");
            Console.WriteLine("6. Adauga produs");
            Console.WriteLine("7. Lista produse din categorie");
            Console.WriteLine("0. Iesire");
            Console.Write("Alege o optiune: ");
        }

        private async Task ListCategories()
        {
            var categories = await api.GetCategoriesAsync();
            foreach (var c in categories)
            {
                Console.WriteLine($"ID: {c.Id}, Nume: {c.Name}, Produse: {c.ItemsCount}");
            }
        }

        private async Task GetCategoryDetails()
        {
            Console.Write("ID categorie: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var cat = await api.GetCategoryAsync(id);
                if (cat != null)
                    Console.WriteLine($"ID: {cat.Id}, Nume: {cat.Name}, Produse: {cat.ItemsCount}");
                else
                    Console.WriteLine("Categoria nu a fost gasita.");
            }
        }

        private async Task CreateCategory()
        {
            Console.Write("Titlu nou categorie: ");
            var title = Console.ReadLine();
            bool success = await api.CreateCategoryAsync(title);
            Console.WriteLine(success ? "Categorie creata!" : "Eroare la creare.");
        }

        private async Task DeleteCategory()
        {
            Console.Write("ID categorie de sters: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                bool success = await api.DeleteCategoryAsync(id);
                Console.WriteLine(success ? "Categorie stearsa." : "Eroare la stergere.");
            }
        }

        private async Task UpdateCategoryTitle()
        {
            Console.Write("ID categorie: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Console.Write("Titlu nou: ");
                string newTitle = Console.ReadLine();
                bool success = await api.UpdateCategoryAsync(id, newTitle);
                Console.WriteLine(success ? "Categorie actualizata." : "Eroare la actualizare.");
            }
        }

        private async Task CreateProduct()
        {
            Console.Write("Titlu produs: ");
            string title = Console.ReadLine();
            Console.Write("Pret: ");
            decimal.TryParse(Console.ReadLine(), out decimal price);
            Console.Write("ID categorie: ");
            int.TryParse(Console.ReadLine(), out int categoryId);

            var product = new Product
            {
                Title = title,
                Price = price,
                CategoryId = categoryId
            };

            bool success = await api.CreateProductAsync(product);
            Console.WriteLine(success ? "Produs adaugat!" : "Eroare la adaugare.");
        }

        private async Task ListProductsByCategory()
        {
            Console.Write("ID categorie: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var products = await api.GetProductsByCategoryAsync(id);
                if (products != null && products.Count > 0)
                {
                    foreach (var p in products)
                        Console.WriteLine($"ID: {p.Id}, Titlu: {p.Title}, Pret: {p.Price} MDL");
                }
                else
                {
                    Console.WriteLine("Nu exista produse.");
                }
            }
        }
    }
}
