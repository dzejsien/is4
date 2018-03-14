using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace WebApplication
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
            services.AddMvc();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options => {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme="oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options => {
                options.SignInScheme = "Cookies";
                options.Authority = "https://localhost:44327";
                options.RequireHttpsMetadata = true;
                options.ClientId = "mvc";
                options.ClientSecret = "secret";
                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

                options.SaveTokens = true;
                options.Scope.Add("customclaims");
                options.Scope.Add("offline_access");
                options.Scope.Add("api1");

                // claimy bedzie pobieral przez userinfo tylko wtedy jak bedzie "id_token token" lub "id_token_ code"
                // tutaj jest tylko id_token, wiec zwraca wszystkie clamy w id_token
                // jednakze jezeli to dodam, to mam wiecej claimow ... hmm
                options.GetClaimsFromUserInfoEndpoint = true;
                options.ClaimActions.MapJsonKey("website", "website");
                options.ClaimActions.MapJsonKey("testClaim", "testClaim");
                options.Events = new OpenIdConnectEvents()
                {
                    OnTokenValidated = context =>
                    {
                        return Task.FromResult(0);
                    },

                    OnUserInformationReceived = userInfoReceivedContext => 
                    {
                        return Task.FromResult(0);
                    }
                };
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
