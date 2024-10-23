﻿using System;
using Gcpe.Hub.Data.Entity;
using Gcpe.Hub.WebApp.Middleware;
using Gcpe.Hub.WebApp.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Gcpe.Hub.WebApp
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath);
            
            builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc(options => options.EnableEndpointRouting = false).AddNewtonsoftJson();
            // https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-8.0/breaking-changes#sqlserver-contains-compatibility
            services.AddDbContext<HubDbContext>(options => options.UseSqlServer(Configuration["HubDbContext"], o => o.UseCompatibilityLevel(120)));
                                                                  // deprecated in ef core 5.0
                                                                  // .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning)));

            services.AddSingleton(new Func<IServiceProvider, Gcpe.Hub.Services.Legacy.ISubscribeClient>((serviceProvider) =>
            {
                var client = new Gcpe.Hub.Services.Legacy.SubscribeClient()
                {
                    BaseUri = new Uri(Configuration["SubscribeBaseUri"])
                };

                return client;
            }));

            services.Configure<MailProviderSettings>(options => Configuration.GetSection("MailProvider").Bind(options));

            services.AddSingleton<MailProvider>();
            // Add the Configuration object so that it can be used through dependency injection
            services.AddSingleton<IConfiguration>(Configuration);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            ConfigureServices(services);

            // Inject an implementation of ISwaggerProvider with defaulted settings applied
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // not longer needed in netcore 2.0
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            app.UseMiddleware<IntranetMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseApiErrorMiddleware();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "home",
                    defaults: new { controller = "Home", action = "Index" },
                    template: "{action}");

                routes.MapRoute(
                    name: "single-page-application",
                    defaults: new { action = "App" },
                    template: "{controller}/{*url}");
            });

            if (env.IsDevelopment())
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint
                app.UseSwagger();

                // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
                app.UseSwaggerUI();
            }
        }
    }
}