using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Services;

namespace Shop.Controllers
{
    [Route("users")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get([FromServices] DataContext context)
        {
            var users = await context.Users
                .AsNoTracking()
                .ToArrayAsync();

            return Ok(users);
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        //[Authorize(Roles = "manager")] //Caso queira deixar somente o gerente criar.
        public async Task<ActionResult<User>> Post(
            [FromServices] DataContext context,
            [FromBody] User model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                //Força o usuário a ser sempre "Funcionário"
                model.Role = "employee";
                context.Users.Add(model);
                await context.SaveChangesAsync();
                return model;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o usuário" });
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate( //dynamic because it will return null or with some user 
            [FromServices] DataContext context,
            [FromBody] User model)
        {
            var user = await context.Users
                .AsNoTracking()
                .Where(x => x.Username == model.Username && x.Password == model.Password)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound(new { message = "Usuário ou senha inválidos" });
            }
            var token = TokenService.GenerateToken(user);
            return new
            {
                user = user,
                token = token
            };
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put(
            [FromServices] DataContext context,
            int id,
            [FromBody] User model)
        {
            if (id != model.Id)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                context.Entry<User>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return model;
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Esse usuário já foi atualizado" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível atualizar o usuário" });
            }
        }

        // [HttpGet]
        // [Route("anonimo")]
        // [AllowAnonymous]
        // public string Anonimo() => "Anônimo";

        // [HttpGet]
        // [Route("autenticado")]
        // [Authorize]
        // public string Autenticado() => "Autenticado";

        // [HttpGet]
        // [Route("funcionario")]
        // [Authorize(Roles = "employee")]
        // public string Funcionario() => "Funcionario";

        // [HttpGet]
        // [Route("gerente")]
        // [Authorize(Roles = "manager")]
        // public string Gerente() => "Gerente";
    }
}

