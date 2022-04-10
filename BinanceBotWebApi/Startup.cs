using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BinanceBotApp.DataValidators;
using BinanceBotInfrastructure;
using BinanceBotWebApi.Middlewares;
using BinanceBotWebApi.SignalR;

namespace BinanceBotWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwagger();
            services.AddValidators();
            services.AddInfrastructure(Configuration);
            services.AddJWTAuthentication();
            services.AddSignalR();
            services.AddCors(options =>
            {
                options.AddPolicy("ClientPermission", policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins(
                            "http://0.0.0.0:3000",
                            "http://*:3000",
                            "http://localhost:3000",
                            "http://0.0.0.0:5000",
                            "http://*:5000",
                            "http://localhost:5000"
                        )
                        .AllowCredentials();
                });
            });
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BinanceBotWebApi v1"));
            
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();
     
            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCors("ClientPermission");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseMiddleware<RequestTrackerMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<PricesHub>("/hubs/prices");
                endpoints.MapHub<ConnectionStatusHub>("/hubs/connection");
            });

            app.UseSpa(spa =>
                spa.Options.SourcePath = "wwwroot");
        }
    }
}