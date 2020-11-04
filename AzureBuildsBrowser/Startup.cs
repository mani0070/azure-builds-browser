using AzureBuildsBrowser.Clients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

namespace AzureBuildsBrowser
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
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"));

            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddRazorPages().AddMicrosoftIdentityUI();
            services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme,
            options =>
            {
                var redirectToIdpHandler = options.Events.OnRedirectToIdentityProvider;
                options.Events.OnRedirectToIdentityProvider = async context =>
                {
                    // Call what Microsoft.Identity.Web is doing
                    await redirectToIdpHandler(context);

                    // Override the redirect URI to be what you want
                    if (context.ProtocolMessage?.RedirectUri?.StartsWith("http://") ?? false)
                    {
                        context.ProtocolMessage.RedirectUri = context.ProtocolMessage.RedirectUri.Replace("http://", "https://");
                    }
                };
            });
            services.Configure<DevopClientOptions>(Configuration.GetSection("DevopsClient"));
            services.AddHttpClient<IDevopsClient, DevopsClient>();
            services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseForwardedHeaders();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseForwardedHeaders();
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}");
            });

            ConfigureForwardedHeaders(app);
            app.UsePathBase(Configuration.GetValue<string>("PathBase"));
            app.Use(async (context, next) =>
            {
                context.Request.Scheme = "https";
                await next.Invoke();
            });
        }

        private static void ConfigureForwardedHeaders(IApplicationBuilder app)
        {
            var options = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
            app.UseForwardedHeaders(options);
        }
    }
}
