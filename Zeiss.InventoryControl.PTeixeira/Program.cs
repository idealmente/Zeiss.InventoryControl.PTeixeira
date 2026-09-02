using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Zeiss.InventoryControl.PTeixeira.DbContexts;
using Zeiss.InventoryControl.PTeixeira.Handlers;
using Zeiss.InventoryControl.PTeixeira.Helpers;
using Zeiss.InventoryControl.PTeixeira.Interfaces;
using Zeiss.InventoryControl.PTeixeira.Services;

var builder = WebApplication.CreateBuilder(args);

// Setting in memory Db
builder.Services.AddDbContext<DbContextZeiss>(opt => opt.UseInMemoryDatabase(databaseName: "CarlZeiss-InventoryDB"));

builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    // Seeding the in memory db
    using (var scope = app.Services.CreateScope())
    {
        var service = scope.ServiceProvider;
        var context = service.GetService<DbContextZeiss>();
        if (context != null)
        {
            DbInitializationHelper dbInitializationHelper = new DbInitializationHelper(context);
            dbInitializationHelper.InitDb();
        }
    }
    
    app.UseSwagger();
    app.UseSwaggerUI();
}
    
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();



