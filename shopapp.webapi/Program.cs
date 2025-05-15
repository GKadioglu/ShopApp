using Microsoft.EntityFrameworkCore;
using shopapp.data;
using shopapp.data.Abstract;
using shopapp.data.Concrete.EfCore;
using shopapp.business.Abstract;
using shopapp.business.Concrete;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson; // Newtonsoft.Json ekleniyor

var builder = WebApplication.CreateBuilder(args);

const string MyAllowOrigins = "_myAllowOrigins";

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(); // Newtonsoft.Json kullanımı için ekleme

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext ve diğer bağımlılıkları burada ekleyelim.
builder.Services.AddDbContext<ShopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));

// Add scoped services for business logic
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

// Eğer Razor View Engine veya dosya derleme kullanıyorsanız, bu ayarları eklemelisiniz:
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Eğer HTTPS yönlendirmesi kullanmanız gerekiyorsa:
app.UseHttpsRedirection();

app.UseCors(MyAllowOrigins);
// API Controllerları ve diğer yönlendirmeleri buraya ekleyelim:
app.MapControllers();

// Run the application
app.Run();

// Weather forecast örneği
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}