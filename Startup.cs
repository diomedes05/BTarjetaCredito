using BTarjetaCredito.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BTarjetaCredito
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
               c.SwaggerDoc("v1", new OpenApiInfo { Title = "BTarjetaCredito", Version = "v1" });
            });

            //services.AddDbContext<AplicationDbContext>(
            //    options =>
            //   options.UseSqlServer(Configuration.GetConnectionString("DevConnection")));

            services.AddDbContext<AplicationDbContextSqlite>(options =>
                   options.UseSqlite(
                        Configuration.GetConnectionString("DefaultConnection")));

            services.AddCors(options => options.AddPolicy("AllowWebApp",
               builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // if (env.IsDevelopment())
            // {
            //    app.UseDeveloperExceptionPage();
            //    app.UseSwagger();
            //    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BTarjetaCredito v1"));
            // }
            app.UseCors("AllowWebApp");

            app.UseHttpsRedirection();

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AplicationDbContextSqlite>();
                context.Database.Migrate();
            }

            // 
            app.UseRouting();

            app.UseCors(x => x
               .AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());

            // app.UseAuthentication();
            // app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            app.UseSwaggerUI(swag =>
            {
                swag.SwaggerEndpoint("swagger/v1/swagger.json", "IncidentApp Api");
                swag.RoutePrefix = string.Empty;
            });
            // 
        }
    }
}
