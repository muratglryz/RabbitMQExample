using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQExample.Models;
using RabbitMQExample.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ")), DispatchConsumersAsync = true });
builder.Services.AddSingleton<RabbitMQClient>();
builder.Services.AddSingleton<RabbitMQPublisher>();
builder.Services.AddHostedService<ProductImageBackgroudService>();
builder.Services.AddDbContext<AppDbContext>(options => {
    options.UseInMemoryDatabase(databaseName: "ProductsDb");
});
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
