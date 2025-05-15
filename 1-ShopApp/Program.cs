using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using shopapp.business.Abstract;
using shopapp.business.Concrete;
using shopapp.data;
using shopapp.data.Abstract;
using shopapp.data.Concrete.EfCore;
using _1_ShopApp.EmailServices;
using _1_ShopApp.Extensions;
using _1_ShopApp.Identity;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// **CORS policy** değişkeni sınıf seviyesine taşındı
const string MyAllowOrigins = "_myAllowOrigins";

// Razor Runtime Compilation ekleniyor
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();  

builder.Services.AddRazorPages();  

// Veritabanı bağlantısı ayarları
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("SqlConnection")));
builder.Services.AddDbContext<ShopContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("SqlConnection")));

// Identity ayarları
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();

// Identity konfigürasyonu
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.AllowedForNewUsers = true;

    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedPhoneNumber = false;
});

// Cookie ayarları
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.LogoutPath = "/account/logout";
    options.AccessDeniedPath = "/account/accessdenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.Cookie = new CookieBuilder
    {
        HttpOnly = true,
        Name = ".ShopApp.Security.Cookie",
        SameSite = SameSiteMode.Strict
    };
});

// E-posta servisi
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>(i =>
    new SmtpEmailSender(
        configuration["EmailSender:Host"],
        configuration.GetValue<int>("EmailSender:Port"),
        configuration.GetValue<bool>("EmailSender:EnableSSL"),
        configuration["EmailSender:UserName"],
        configuration["EmailSender:Password"]
    ));

// Uygulama servisleri
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrderService, OrderManager>();
builder.Services.AddScoped<ICartService, CartManager>();
builder.Services.AddScoped<IProductService, ProductManager>();
builder.Services.AddScoped<ICategoryService, CategoryManager>();

// CORS ayarları
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowOrigins, builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Kestrel yapılandırması
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5001, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});

var app = builder.Build();

// Hata ayıklama ayarları
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    SeedDatabase.Seed();
    app.UseDeveloperExceptionPage();
}

app.MigrateDatabase();
app.UseHttpsRedirection();
app.UseStaticFiles();

// node_modules klasörü için statik dosya desteği
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "node_modules")),
    RequestPath = "/modules"
});

app.UseRouting();
app.UseCors(MyAllowOrigins);
app.UseAuthentication();
app.UseAuthorization();

// Rota tanımlamaları (yukarıdan aşağı doğru sıralı)
app.MapControllerRoute(name: "orders", pattern: "orders", defaults: new { controller = "Cart", action = "GetOrders" });
app.MapControllerRoute(name: "checkout", pattern: "checkout", defaults: new { controller = "Cart", action = "Checkout" });
app.MapControllerRoute(name: "cart", pattern: "cart", defaults: new { controller = "Cart", action = "Index" });
app.MapControllerRoute(name: "adminuser", pattern: "admin/user/list", defaults: new { controller = "Admin", action = "UserList" });
app.MapControllerRoute(name: "search", pattern: "search", defaults: new { controller = "Shop", action = "Search" });
app.MapControllerRoute(name: "products", pattern: "products/{category?}", defaults: new { controller = "Shop", action = "List" });
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(name: "productdetails", pattern: "{url}", defaults: new { controller = "Shop", action = "details" });

// Razor Pages
app.MapRazorPages();

// Identity Seed işlemi
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    await SeedIdentity.Seed(userManager, roleManager, configuration);
}

app.Run();