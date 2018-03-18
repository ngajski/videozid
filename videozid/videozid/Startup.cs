using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using videozid.Utils;

namespace videozid
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
 
            Configuration = builder.Build();

            env.ConfigureNLog("nlog.config");
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMemoryCache();
            services.AddSession();
            services.AddMvc();
            services.AddScoped<LogFilter>();
            //Sve dodano

            //application settings
            services.AddOptions();
            var appSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSection);

            string connectionString = appSection.GetValue<string>("ConnectionString");

            services.AddDbContext<Models.RPPP15Context>(options => options.UseSqlServer(connectionString));

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.DocInclusionPredicate((name, apidescription) => apidescription.RelativePath.Contains($"api"));

                c.SwaggerDoc("v1", new Info { Title = "Videozid.Mvc API", Version = "v1" });

                //Set the comments path for the swagger json and ui.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "videozid.xml");
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            loggerFactory.AddNLog();
            app.AddNLogWeb();

            //loggerFactory.AddZidLogger(serviceProvider, level => level >= LogLevel.Warning);

            LogManager.Configuration.Variables["connectionString"] = Configuration.GetConnectionString("ConnectionString");
            LogManager.Configuration.Variables["configDir"] = "C:\\git\\damienbod\\AspNetCoreNlog\\Logs";

            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

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
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "videozid.Mvc API V1");
            });
        }
    }
}
