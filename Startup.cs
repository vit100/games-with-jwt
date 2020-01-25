using System;
using System.Linq;
using System.Threading.Tasks;
using JWT.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JWT
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

            services.AddAuthentication(o =>
                {
                    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;

                }) //JwtBearerDefaults.AuthenticationScheme)

                //adds more functionality, like caching
                //.AddIdentityServerAuthentication(o =>
                //{
                //    o.TokenRetriever
                //})
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters.NameClaimType = "username"; //will map claim username to identity name
                    o.TokenValidationParameters.RoleClaimType = "scope";


                    //good with AuthController.SecurityKey - my own key!
                    //  o.TokenValidationParameters.IssuerSigningKey = AuthController.AsymetricSecurityKey();
                    //o.TokenValidationParameters.ValidateIssuerSigningKey = true;

                    o.TokenValidationParameters.ValidAudience = "vit";
                    //o.TokenValidationParameters.ValidateAudience = true;

                    o.TokenValidationParameters.ValidateIssuer = false;
                    //o.TokenValidationParameters.ValidIssuer = "vit";

                    o.Authority = "https://api.preprod.fusionfabric.cloud/login/v1/sandbox/";
                    o.TokenValidationParameters.ValidateAudience = false;

                    o.Events = new JwtBearerEvents()
                    {
                        OnMessageReceived = (c) =>
                        {
                            return Task.CompletedTask;
                        },
                        //OnTokenValidated = c =>
                        //{
                        //    return Task.CompletedTask;
                        //}
                    };
                    o.SaveToken = true;
                })
                .AddGoogle(o =>
                {
                    o.ClientId = Configuration["google-clientId"]; //set via dotnet user-secrets set google-clientId
                    o.ClientSecret = Configuration["google-clientSecret"]; //set via dotnet user-secrets set google-clientSecret
                    o.ClaimActions.MapAll();
                })
                .AddCookie(o => { });

            services.AddAuthorization(c =>
                c.AddPolicy("mustBeInScope_Something", b =>
                {
                    //like this
                    //b.RequireAssertion(c =>
                    //{
                    //    var scopeClaim = c.User.FindFirst("scope");
                    //    if (scopeClaim != null && !string.IsNullOrEmpty(scopeClaim.Value))
                    //    {
                    //        return scopeClaim.Value.Contains("b2c-p2p-v1-0ff75e33-5086-40d8-acb2-85d6a4a07698");
                    //    }

                    //    return false;

                    //});

                    //or via Requirements
                    b.AddRequirements(new ScopeRequirement("b2c-p2p-v1-0ff75e33-5086-40d8-acb2-85d6a4a07698"));

                 }));
            services.AddSingleton<IAuthorizationHandler, ScopeAuthorizationHandler>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
