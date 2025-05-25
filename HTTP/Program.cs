using System.Threading.Tasks;
using HTTP.UI;

namespace HTTP
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var menu = new MenuHandler();
            await menu.RunAsync();
        }
    }
}