using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using kino.Crypto;
using kino.Database;
using System;
using System.Text;

namespace kino
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
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOriginsHeadersAndMethods",
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            services.AddDbContext<KinoContext>(options =>
                options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=kinoDB;Trusted_Connection=True;MultipleActiveResultSets=true"));

            services.AddIdentity<User, IdentityRole>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<KinoContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(ConfigureAuthenticationOptions)
                .AddJwtBearer(ConfigureJwtBearerOptions);
            services.AddAuthorization(ConfigureAuthorizationOptions);

            services.AddSingleton<IEncoder, CaesarEncoder>();

            services.AddTransient<KinoContext>();
            services.AddScoped<JwtTokenGenerator>();
            services.AddTransient<SeedData>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SeedData seedData)
        {
            // todo sprawdziæ czy w .net 3.0 dziala
            //workaround for https://github.com/aspnet/AspNetCore/issues/4398
            app.Use(async (ctx, next) =>
            {
                await next();
                if (ctx.Response.StatusCode == 204)
                {
                    ctx.Response.ContentLength = 0;
                }
            });
            app.UseCors("AllowAllOriginsHeadersAndMethods");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            seedData.EnsurePopulated();
        }
        private void ConfigureJwtBearerOptions(JwtBearerOptions options)
        {
            string issuer = "http://localhost:3000";
            string audience = "http://localhost:3000";
            string key = "dcf205a7-6207-48bf-8489-62b8a77ec00a";

            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.ClaimsIssuer = issuer;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidIssuer = issuer,

                ValidateAudience = false,
                ValidAudience = audience,

                ValidateIssuerSigningKey = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

                RequireExpirationTime = true,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };
        }

        private void ConfigureAuthenticationOptions(AuthenticationOptions options)
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }

        private void ConfigureAuthorizationOptions(AuthorizationOptions options)
        {
            foreach (string role in Role.AllRoles)
            {
                options.AddPolicy(role, authBuilder =>
                {
                    authBuilder.RequireRole(role);
                });

            }
        }

    }
}
