
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

            ////Add Ocelot configuration
            //builder.Configuration
            //    .SetBasePath(Directory.GetCurrentDirectory()) // Ensure correct base path
            //    .AddJsonFile("Properties/ocelot.json", optional: false, reloadOnChange: true);

            builder.Services.AddOcelot();

            builder.Configuration.AddJsonFile("Properties/ocelot.json", optional: false, reloadOnChange: true);

            // Load additional Ocelot JSON files dynamically from "ocelot-configs" folder
            //var configFiles = Directory.GetFiles("ocelot-configs", "*.json", SearchOption.AllDirectories);
            //foreach (var file in configFiles)
            //{
            //    builder.Configuration.AddJsonFile(file, optional: false, reloadOnChange: true);
            //}

            //string ocelotConfigFile = Environment.GetEnvironmentVariable("OCELOT__JSON__FILE") ?? "ocelot.json";


            builder.Logging.AddConsole();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();

            app.Urls.Add("http://0.0.0.0:8080");
            //app.Urls.Add("https://0.0.0.0:8081");

            app.MapControllers();

            app.UseOcelot().Wait();

            app.Run();
        }
    }
}
