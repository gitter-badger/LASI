﻿using System;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using LASI.WebApp.Authentication;
using LASI.WebApp.Logging;
using LASI.WebApp.Models;
using LASI.WebApp.Models.User;
using LASI.WebApp.Persistence;
using LASI.WebApp.Persistence.MongoDB.Extensions;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Authentication.JwtBearer;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace LASI.WebApp
{
    public class Startup
    {

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            isDevelopment = env.IsDevelopment();
            if (isDevelopment)
            {
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            ConfigureLASIComponents(fileName: Path.Combine(Directory.GetParent(env.WebRootPath).FullName, "appsettings.json"), subkey: "Resources");

        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"))
                    .AddSingleton<JwtSecurityTokenHandler>()
                    .AddSingleton<ILookupNormalizer, UpperInvariantLookupNormalizer>()
                    .AddSingleton<IWorkItemsService, WorkItemsService>()
                    .AddMongoDB(Configuration)
                    .AddRouting(options =>
                    {
                        options.LowercaseUrls = true;
                    });

            services.AddMvc()
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                        options.SerializerSettings.Formatting = isDevelopment ? Formatting.Indented : Formatting.None;
                        //options.SerializerSettings.Error = (s, e) => { throw e.ErrorContext.Error; };
                        options.SerializerSettings.Converters.Add(new StringEnumConverter
                        {
                            AllowIntegerValues = false,
                            CamelCaseText = true
                        });
                    });

            services.AddSingleton(provider => TokenAuthorizationOptions)
                    .AddAuthorization(options =>
                    {
                        options.AddPolicy("Bearer", new AuthorizationPolicyBuilder("Bearer")
                            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                            .RequireAuthenticatedUser().Build()
                        );
                    })
                    .AddIdentity<ApplicationUser, UserRole>(options =>
                    {
                        options.Lockout = new LockoutOptions
                        {
                            AllowedForNewUsers = true,
                            DefaultLockoutTimeSpan = TimeSpan.FromDays(1),
                            MaxFailedAccessAttempts = 10
                        };
                        options.User = new UserOptions
                        {
                            RequireUniqueEmail = true
                        };
                        options.SignIn = new SignInOptions
                        {
                            RequireConfirmedEmail = false,
                            RequireConfirmedPhoneNumber = false
                        };
                        options.Password = new PasswordOptions
                        {
                            RequiredLength = 8,
                            RequireDigit = true,
                            RequireLowercase = true,
                            RequireUppercase = true,
                            RequireNonLetterOrDigit = true
                        };
                    })
                    .AddRoleStore<CustomUserStore<UserRole>>()
                    .AddUserStore<CustomUserStore<UserRole>>();
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            RsaKey = new RsaSecurityKey(RSAFactory.LoadRSAKey(Configuration["AppSettings:KeyFileName"]));

            TokenAuthorizationOptions = new TokenAuthorizationOptions
            {
                Audience = "LASI",
                Issuer = "LASI",
                SigningCredentials = new SigningCredentials(key: RsaKey, algorithm: SecurityAlgorithms.RsaSha256Signature)
            };

            loggerFactory
                .AddConsole(LogLevel.Debug)
                .AddLASIOutput(LogLevel.Debug);

            if (env.IsDevelopment())
            {
                app.UseBrowserLink()
                   .UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseIISPlatformHandler(options =>
               {
                   options.AuthenticationDescriptions.Clear();
               })
               .UseJwtBearerAuthentication(options =>
               {
                   options.Events = new JwtBearerEvents
                   {
                       OnAuthenticationFailed = context =>
                       {
                           System.Diagnostics.Debug.WriteLine(context.Request);
                           return Task.FromResult(0);
                       }
                   };
                   options.RequireHttpsMetadata = false;
                   options.TokenValidationParameters.IssuerSigningKey = RsaKey;
                   options.TokenValidationParameters.ValidAudience = TokenAuthorizationOptions.Audience;
                   options.TokenValidationParameters.ValidIssuer = TokenAuthorizationOptions.Issuer;
                   options.TokenValidationParameters.ValidateSignature = true;
                   options.TokenValidationParameters.ValidateLifetime = true;
                   options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(0);
                   options.TokenValidationParameters.NameClaimType = "unique_name";
                   options.Configuration = new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration
                   {
                       HttpLogoutSupported = true
                   };
                   options.TokenValidationParameters.SaveSigninToken = true;
               })
               .UseCors(policy =>
               {
                   policy.AllowAnyHeader()
                         .AllowAnyMethod()
                         .AllowAnyOrigin()
                         .AllowCredentials()
                         .WithExposedHeaders("Access-Control-Allow-Origin");
               })
               .UseStaticFiles()
               .UseIdentity()
               .UseMvc(routes =>
               {
                   routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}")
                         .MapRoute(name: "ChildApi", template: "api/{parentController}/{parentId?}/{controller}/{id?}")
                         .MapRoute(name: "DefaultApi", template: "api/{controller}/{id?}");
               });
        }

        private void ConfigureLASIComponents(string fileName, string subkey)
        {
            Interop.ResourceUsageManager.SetPerformanceLevel(Interop.PerformanceProfile.High);
            Interop.Configuration.Initialize(fileName, Interop.ConfigFormat.Json, subkey);
        }


        public static void Main(string[] args) => WebApplication.Run<Startup>(args);

        RsaSecurityKey RsaKey { get; set; }
        private readonly bool isDevelopment;
        private TokenAuthorizationOptions TokenAuthorizationOptions { get; set; }
    }
}