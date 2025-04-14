
using DotNetEnv;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Prometheus;

namespace ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddLogging();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Check if the environment is Docker (from GitHub CI/CD pipeline)
            var dockerEnv = Environment.GetEnvironmentVariable("DOCKER_ENV");
            Console.WriteLine($"DOCKER_ENV: {dockerEnv}");

            if (dockerEnv == "true")
            {
                // DOCKER ENVIRONMENT
                string configPath = Environment.GetEnvironmentVariable("DOCKER_OCELOT_CONFIG_PATH") ?? "Properties/ocelot.json";
                builder.Configuration.AddJsonFile(configPath, optional: false, reloadOnChange: true);
            }
            else
            {
                // LOCAL ENVIRONMENT
                var envFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
                Env.Load(envFilePath);

                // Load additional Ocelot JSON files dynamically from "ocelot-configs" folder
                builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
            }

            builder.Services.AddOcelot();
            builder.Logging.AddConsole();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // if you need cookies/auth
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseCors("AllowFrontend");
            app.UseRouting();
            app.UseAuthorization();

            //app.UseHttpsRedirection();

            //// Enable Prometheus scraping at /metrics

            //app.UseHttpMetrics(); // middleware that tracks request durations, status codes, etc.
            //                      // Add the /metrics endpoint for Prometheus

            //app.MapMetrics();  // This exposes metrics at the /metrics endpoint



            if (dockerEnv == "true")
            {
                // DOCKER ONLY
                app.Urls.Add("http://0.0.0.0:8086");
                //app.Urls.Add("https://0.0.0.0:8087");
            }

            app.MapControllers();

            app.Use(async (context, next) =>
            {
                if (context.Request.Method == HttpMethods.Options)
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:3000");
                    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                    context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
                    context.Response.StatusCode = 200;
                    await context.Response.CompleteAsync();
                }
                else
                {
                    await next();
                }
            });

            app.UseOcelot().Wait();

            app.Run();
        }
    }
}
