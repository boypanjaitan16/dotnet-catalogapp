using System;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using CatalogApp.Interfaces;
using CatalogApp.Repositories;
using CatalogApp.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace CatalogApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var mongoDbSettings    = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

            BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));

            services.AddSingleton<IMongoClient>(ServiceProvider => {
                
                return new MongoClient(mongoDbSettings.ConnectionString);
            });
            services.AddSingleton<ItemRepositoryInterface, MongoDBItemsRepository>();
            services.AddControllers(options => {
                options.SuppressAsyncSuffixInActionNames = false;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CatalogApp", Version = "v1" });
            });
            services.AddHealthChecks()
            .AddMongoDb(
                mongoDbSettings.ConnectionString, 
                name: "mongodb", 
                timeout: TimeSpan.FromSeconds(5),
                tags: new[]{"ready"}
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CatalogApp v1"));

                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health-check/ready", new HealthCheckOptions{
                    Predicate = (check) => check.Tags.Contains("ready"),
                    ResponseWriter = async (context, report) => {
                        var result = JsonSerializer.Serialize(new{
                            status  = report.Status.ToString(),
                            checks  = report.Entries.Select(entry => new{
                                name    = entry.Key,
                                status  = entry.Value.Status.ToString(),
                                exception   = entry.Value.Exception != null ? entry.Value.Exception.Message.ToString() : null,
                                duration    = entry.Value.Duration.ToString()
                            })
                        });

                        context.Response.ContentType    = MediaTypeNames.Application.Json;
                        await context.Response.WriteAsync(result);
                    }
                });
                endpoints.MapHealthChecks("/health-check/live", new HealthCheckOptions{
                    Predicate = (check) => false
                });
            });
        }
    }
}
