using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWT.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

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
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters.NameClaimType = "username"; //will map claim username to identity name
                    //good with AuthController.SecurityKey - my own key!
                     o.TokenValidationParameters.IssuerSigningKey = AuthController.AsymetricSecurityKey();
                    //o.TokenValidationParameters.ValidateIssuerSigningKey = true;

                    o.TokenValidationParameters.ValidAudience = "vit";
                    //o.TokenValidationParameters.ValidateAudience = true;

                    o.TokenValidationParameters.ValidateIssuer = false;
                    //o.TokenValidationParameters.ValidIssuer = "vit";

                 //   o.Authority = "https://api.preprod.fusionfabric.cloud/login/v1/sandbox/";
                    o.TokenValidationParameters.ValidateAudience = false;
                })
                .AddGoogle(o =>
                {
                    o.ClientId = Configuration["google-clientId"]; //set via dotnet user-secrets set google-clientId
                    o.ClientSecret = Configuration["google-clientSecret"]; //set via dotnet user-secrets set google-clientSecret
                    o.ClaimActions.MapAll();
                })
                .AddCookie(o => { });
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
