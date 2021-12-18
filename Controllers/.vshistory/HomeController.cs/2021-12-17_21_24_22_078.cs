using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Backoffice.Data;
using Backoffice.models;
using Shop.Data;
using Shop.Models;

namespace Backoffice.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<dynamic>> Get([FromServices] DataContext Context)
        {
            var employee = new User { Id = 1, Username = "robin", Password = "robin", Role = "employee" };
            var manager = new User { Id = 2, Username = "batman", Password = "batman", Role = "manager" };
            var category = new Category { Id = 1, Title = "Informática" };
            var product = new Product { Id = 1, Category = category, Title = "Mouse", Price = 299, Description = "Mouse USB"};
            context.Users.Add(employee);
            context.Users.Add(manager);
            context.Categories.Add(category);
            context.Products.Add(product);
            await context.SaveChangesAsync();

            return Ok(new
            {
                message = "Dados configurados"
            });

        }
    }
}
