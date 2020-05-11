using IdentityModel;
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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, o =>
                {
                    o.Events.OnRedirectToIdentityProvider = context =>
                    {
                        // only modify requests to the authorization endpoint
                        if (context.ProtocolMessage.RequestType != OpenIdConnectRequestType.Authentication) 
                            return Task.CompletedTask;
                        
                        // generate code_verifier
                        var codeVerifier = CryptoRandom.CreateUniqueId(32);

                        // store codeVerifier for later use
                        context.Properties.Items.Add("code_verifier", codeVerifier);

                        // create code_challenge
                        string codeChallenge;
                        using (var sha256 = SHA256.Create())
                        {
                            var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                            codeChallenge = Base64Url.Encode(challengeBytes);
                        }

                        // add code_challenge and code_challenge_method to request
                        context.ProtocolMessage.Parameters.Add("code_challenge", codeChallenge);
                        context.ProtocolMessage.Parameters.Add("code_challenge_method", "S256");

                        return Task.CompletedTask;
                    };

                    o.Events.OnAuthorizationCodeReceived = context =>
                    {
                        // only when authorization code is being swapped for tokens
                        if (context.TokenEndpointRequest?.GrantType != OpenIdConnectGrantTypes.AuthorizationCode) 
                            return Task.CompletedTask;
                        
                        // get stored code_verifier
                        if (context.Properties.Items.TryGetValue("code_verifier", out var codeVerifier))
                        {
                            // add code_verifier to token request
                            context.TokenEndpointRequest.Parameters.Add("code_verifier", codeVerifier);
                        }

                        return Task.CompletedTask;
                    };

                    o.Authority = "http://localhost:5005";
                    o.ClientId = "pkce-client";
                    o.ClientSecret = "pkce-client-secret";
                    // Enable PKCE(authorization code flow only)
                    o.UsePkce = true;

                    o.RequireHttpsMetadata = false;

                    o.ResponseType = OpenIdConnectResponseType.Code;
                    o.ResponseMode = OpenIdConnectResponseMode.FormPost;
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
