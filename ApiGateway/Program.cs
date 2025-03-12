
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddOcelot();
            builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

            // Load additional Ocelot JSON files dynamically from "ocelot-configs" folder
            var configFiles = Directory.GetFiles("ocelot-configs", "*.json", SearchOption.AllDirectories);
            foreach (var file in configFiles)
            {
                builder.Configuration.AddJsonFile(file, optional: false, reloadOnChange: true);
            }

            builder.Logging.AddConsole();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1"));
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.UseOcelot().Wait(); 

            app.Run();
        }
    }
}
