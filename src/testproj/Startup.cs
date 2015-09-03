using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Authentication.OpenIdConnect;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Dnx.Runtime;
using testproj.Contexts;

namespace testproj
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            // Setup configuration sources.

            var builder = new ConfigurationBuilder(appEnv.ApplicationBasePath)
                .AddJsonFile("config.json")
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // This reads the configuration keys from the secret store.
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookieAuthenticationOptions>(options =>
            {
                options.AutomaticAuthentication = true;
            });

            services.Configure<OpenIdConnectAuthenticationOptions>(options =>
            {
                options.AutomaticAuthentication = true;
                options.ClientId = Configuration["Authentication:AzureAd:ClientId"];
                options.Authority = Configuration["Authentication:AzureAd:AADInstance"] + Configuration["Authentication:AzureAd:TenantId"];
                options.PostLogoutRedirectUri = Configuration["Authentication:AzureAd:PostLogoutRedirectUri"];
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });

            services.ConfigureAuthorization(options =>
            {
                options.AddPolicy("Admin", policyBuilder =>
                {
                    policyBuilder.RequireClaim(
                        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
                        Configuration["Authorization:AdminUsers"].Split(',')
                    );
                });
            });

            // Add MVC services to the services container.
            services.AddMvc();
            var context = new AzureTableContext(Configuration["Authentication:AzureStorageAccount:StorageConnectionString"], Configuration["Authentication:AzureStorageAccount:version"]);
            services.AddInstance<ITableContext>(context);
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ITableContext tableContext)
        {
            loggerFactory.MinimumLevel = LogLevel.Information;
            loggerFactory.AddConsole();

            // Configure the HTTP request pipeline.

            // Add the following to the request pipeline only in development environment.
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseErrorPage();
            }
            else
            {
                // Add Error handling middleware which catches all application specific errors and
                // send the request to the following path or controller action.
                app.UseErrorHandler("error");
            }

            // Add static files to the request pipeline.
            app.UseStaticFiles();

            // Add cookie-based authentication to the request pipeline.
            app.UseCookieAuthentication();

            // Add OpenIdConnect middleware so you can login using Azure AD.
            app.UseOpenIdConnectAuthentication();

            app.Use((context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/ping"))
                {
                    return context.Response.WriteAsync("pong");
                }
                return next();
            });

            // Add MVC to the request pipeline.
            app.UseMvc();

            // Initialize DB context
            tableContext.Configure();
        }
    }
}
