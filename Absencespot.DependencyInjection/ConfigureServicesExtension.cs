using Absencespot.Azure;
using Absencespot.Business.Abstractions;
using Absencespot.Clients.Sendgrid;
using Absencespot.Domain;
using Absencespot.Infrastructure.Abstractions;
using Absencespot.Infrastructure.Abstractions.Clients;
using Absencespot.Services;
using Absencespot.SqlServer;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Absencespot.DependencyInjection
{
    public static class ConfigureServicesExtension
    {

        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((provider, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    options =>
                    {
                        options.EnableRetryOnFailure(5, new(0, 0, 30), null);
                        options.CommandTimeout(60);
                    });
                options.UseLoggerFactory(provider.GetRequiredService<ILoggerFactory>());
                options.EnableDetailedErrors();
            });
            services.AddAzureClients(builder =>
             {
                 builder.AddBlobServiceClient(configuration.GetConnectionString("BlobStorageConnection"));
                 builder.ConfigureDefaults(options =>
                 {
                     options.Retry.Delay = TimeSpan.FromSeconds(30);
                     options.Retry.MaxDelay = TimeSpan.FromSeconds(40);
                     options.Retry.NetworkTimeout = TimeSpan.FromSeconds(60);
                     options.Retry.MaxRetries = 5;
                 });
             });

            return services;
        }
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConfiguration>(configuration);
            services.AddScoped<IUnitOfWork, Absencespot.UnitOfWork.UnitOfWork>();
            services.AddTransient<IEmailClient, SendridClient>();
            services.AddTransient<ICompanyService, CompanyService>();
            services.AddTransient<IRequestService, RequestService>();
            services.AddTransient<ILeaveService, LeaveService>();
            services.AddTransient<IOfficeService, OfficeService>();
            services.AddTransient<ITeamService, TeamService>();
            services.AddTransient<IWorkScheduleService, WorkScheduleService>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<ITrackRecordService, TrackRecordService>();
            services.AddTransient<IAvailableLeaveService, AvailableLeaveService>();
            services.AddTransient<ISettingsService, SettingsService>();
            services.AddTransient<IIntegrationService, IntegrationService>();
            services.AddSingleton(x => new BlobServiceClient(configuration.GetConnectionString("BlobStorageConnection")));
            services.AddSingleton<BlobStorageClient>();

            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequiredLength = 7;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = true;
                options.SignIn.RequireConfirmedEmail = true;
                //options.SignIn.RequireConfirmedAccount = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddUserStore<UserStore<User, Role, ApplicationDbContext, int>>()
                .AddRoleStore<RoleStore<Role, ApplicationDbContext, int>>();

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(1);
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                //options.AddPolicy("TwoFactorEnabled", x => x.RequireClaim("amr", "mfa")); 
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Secret_key"]))
                };
            })
            .AddCookie()
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = configuration["GoogleAuth:ClientId"];
                googleOptions.ClientSecret = configuration["GoogleAuth:ClientSecret"];
                //googleOptions.SignInScheme = GoogleDefaults.AuthenticationScheme;
                //googleOptions.AuthorizationEndpoint

                googleOptions.Scope.Add("profile");
                googleOptions.SignInScheme = Microsoft.AspNetCore.Identity.IdentityConstants.ExternalScheme;
            })
            .AddMicrosoftAccount(options =>
           {
               options.ClientId = configuration["MicrosoftAuth:ClientId"];
               options .ClientSecret = configuration["MicrosoftAuth:ClientSecret"];
           });
             
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
            //});
            ////services.Configure<CookiePolicyOptions>(options =>
            ////{
            ////    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
            ////    options.OnAppendCookie = cookieContext =>
            ////        CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            ////    options.OnDeleteCookie = cookieContext =>
            ////        CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            ////});

            return services;
        }

    }
}