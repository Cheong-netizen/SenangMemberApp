using SenangMemberApp.Services;
using SenangMemberApp.Shared.ApiClient;
using SenangMemberApp.Shared.Repositories.IRepository;
using SenangMemberApp.Shared.Repositories.Repository;
using SenangMemberApp.Shared.Services;
using SenangMemberApp.Shared.Services.ConcreteService;
using SenangMemberApp.Shared.Services.IService;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SenangMemberApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });


            // Add device-specific services used by the SenangMemberApp.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            //all the services are build here
            //all the services are build here
            builder.Services.AddScoped<IServiceProducts, ServiceProducts>();
            //builder.Services.AddScoped<IStoreService, StoreService>();
            builder.Services.AddScoped<IAppointmentState, AppointmentState>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddScoped<IShopState, ShopState>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ICreditService, CreditService>();
            builder.Services.AddScoped<ICompanyService, CompanyService>();
            builder.Services.AddScoped<IShopStateLocalManagement, MobileShopStateLocalManagement>();
            builder.Services.AddScoped<IAppointmentDetailState, AppointmentDetailState>();
            builder.Services.AddScoped<IUserProfileService, UserProfileService>();
            builder.Services.AddScoped<IUrlLauncher, MobileUrlLauncher>();
            builder.Services.AddScoped<ITokenService, MobileTokenService>();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            //repository
            builder.Services.AddScoped<IStoreOutletRepository, StoreOutletRepository>();
            builder.Services.AddScoped<IProductServicesRepository, ProductServicesRepository>();
            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            builder.Services.AddScoped<IOutletRepository, OutletRepository>();
            builder.Services.AddScoped<IStaffRepository, StaffRepository>();

            // Plain HttpClient used by Chat.razor for the chatbot API at https://60.49.248.21:43250.
            // That endpoint serves a certificate a raw IP can't validate, so trust the same
            // specific hosts SenangMemberApp.Web does. SocketsHttpHandler (managed) is used
            // because iOS's native handler doesn't support a validation callback.
            builder.Services.AddScoped(sp =>
            {
                var handler = new SocketsHttpHandler
                {
                    SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                    {
                        RemoteCertificateValidationCallback = (sender, cert, chain, errors) =>
                        {
                            if (errors == System.Net.Security.SslPolicyErrors.None) return true;
                            var host = (sender as System.Net.Security.SslStream)?.TargetHostName ?? "";
                            return host == "localhost"
                                || host == "127.0.0.1"
                                || host == "60.49.248.21"
                                || host.StartsWith("192.168.")
                                || host.StartsWith("10.");
                        }
                    }
                };
                return new HttpClient(handler);
            });

            //get base url to be reused in the API client
            var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
                             ?? "https://ebisoftware.com.my:9088/";

            //AC
            builder.Services.AddHttpClient<AuthAC>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });

            builder.Services.AddHttpClient<CreditBalanceAC>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });

            builder.Services.AddHttpClient<CompanyAC>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });

            builder.Services.AddHttpClient<AppointmentAC>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });

            builder.Services.AddHttpClient<PurchaseHistoryAC>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });

            builder.Services.AddHttpClient<ProfileAC>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
