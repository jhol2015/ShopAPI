using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.EntityFrameworkCore;
using Shop.Data;

namespace Shop
{
    public class Startup //Classe Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) //Metodos
        {
            services.AddControllers();
            //informa para aplicação que tem um dbcontex
            services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("Database"));
            //Garante que terá somente um dbcontext por conexao.
            services.AddScoped<DataContext, DataContext>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) //so exibe os detalhes do erro se tiver em dev.
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection(); //redireciona toda app para https
            app.UseRouting(); //padrao de rotas
            app.UseAuthorization();//autenticação com rotes
            app.UseEndpoints(endpoints => // mapeamento da rotas
            {
                endpoints.MapControllers();
            });
        }
    }
}