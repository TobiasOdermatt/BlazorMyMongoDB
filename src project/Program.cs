using BlazorServerMyMongo.Data.DB;
using BlazorServerMyMongo.Data.Helpers;
using BlazorServerMyMongo.Data.OTP;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();

IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("config.json", optional: false, reloadOnChange: false).Build();

ConfigManager configManager = new(config);
builder.Services.AddSingleton<ConfigManager>(configManager);
OTPFileManagement OTP = new();
OTP.CleanUpOTPFiles();
builder.Services.AddTransient<DBController>();
builder.Services.AddSingleton<AppData>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
});

app.Run();