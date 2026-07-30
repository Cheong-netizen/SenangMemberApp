using SenangMemberApp.Services;
using SenangMemberApp.Shared.ApiClient;
using SenangMemberApp.Shared.Repositories.IRepository;
using SenangMemberApp.Shared.Repositories.Repository;
using SenangMemberApp.Shared.Services;
using SenangMemberApp.Shared.Services.ConcreteService;
using SenangMemberApp.Shared.Services.IService;
using SenangMemberApp.Web.Components;
using SenangMemberApp.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(options =>
    {
        // Voice recordings are returned from JavaScript as Base64. Even a short
        // recording exceeds SignalR's small default incoming-message limit and
        // otherwise disconnects the interactive server circuit.
        options.MaximumReceiveMessageSize = 10 * 1024 * 1024;
    });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // This overrides the default "/Account/Login" path
        options.LoginPath = "/";

        // Optional: If you ever add roles and someone tries to access an admin page, 
        // they will be sent here instead of crashing
        options.AccessDeniedPath = "/access-denied";
    });

// Add device-specific services used by the SenangMemberApp.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

//all the services are build here
builder.Services.AddScoped<IServiceProducts, ServiceProducts>();
//builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IAppointmentState, AppointmentState>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IShopState, ShopState>();
builder.Services.AddScoped<ITokenService, WebStoreTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICreditService, CreditService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IShopStateLocalManagement, WebShopStateLocalManagement>();
builder.Services.AddScoped<IAppointmentDetailState, AppointmentDetailState>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

//repository
builder.Services.AddScoped<IStoreOutletRepository, StoreOutletRepository>();
builder.Services.AddScoped<IProductServicesRepository, ProductServicesRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IOutletRepository, OutletRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();

builder.Services.AddScoped(sp =>
{
    var handler = new System.Net.Http.HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
        {
            if (message.RequestUri != null && (
                message.RequestUri.Host == "localhost" ||
                message.RequestUri.Host == "127.0.0.1" ||
                message.RequestUri.Host == "60.49.248.21" ||
                message.RequestUri.Host.StartsWith("192.168.") ||
                message.RequestUri.Host.StartsWith("10.")))
            {
                return true;
            }
            return errors == System.Net.Security.SslPolicyErrors.None;
        }
    };
    return new System.Net.Http.HttpClient(handler);
});

//get base url to be reused in the API client
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
                 ?? "https://ebisoftware.com.my:5014/";

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(
        typeof(SenangMemberApp.Shared._Imports).Assembly);

app.Run();
