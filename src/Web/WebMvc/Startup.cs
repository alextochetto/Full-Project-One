using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;

namespace WebMvc
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Home/Index/");
                    options.AccessDeniedPath = new PathString("/Account/Forbidden/");
                    options.Cookie.HttpOnly = true;
                    options.Cookie.Name = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.ExpireTimeSpan = TimeSpan.FromHours(2); //new TimeSpan(2, 0, 0);
                })
                .AddOpenIdConnect(o =>
                {
                    //o.Authority = Configuration["oidc:authority"];
                    //o.ClientId = Configuration["oidc:clientid"];
                    //o.ClientSecret = Configuration["oidc:clientsecret"];

                    o.Authority = "http://localhost:5005";
                    o.ClientId = "mvc-client";
                    o.ClientSecret = "mvc-client-secret";

                    o.RequireHttpsMetadata = false;

                    o.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                    o.SaveTokens = true;
                    o.GetClaimsFromUserInfoEndpoint = true;
                    o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    o.Scope.Add("openid");
                    o.Scope.Add("profile");
                    o.Scope.Add("email");
                    o.Scope.Add("payment");

                    o.ClaimActions.MapAllExcept("aud", "iss", "iat", "nbf", "exp", "aio", "c_hash", "uti", "nonce");

                    o.Events = new OpenIdConnectEvents()
                    {
                        OnAuthenticationFailed = c =>
                        {
                            c.HandleResponse();

                            c.Response.StatusCode = 500;
                            c.Response.ContentType = "text/plain";
                            
                            return c.Response.WriteAsync(c.Exception.ToString());
                        }
                    };
                });

            services.AddHttpClient("payment", options => 
            {
                options.BaseAddress = new Uri("http://localhost:5002");
            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
