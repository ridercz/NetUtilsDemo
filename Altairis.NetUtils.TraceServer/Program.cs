var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<NetUtilsDbContext>(options => {
    options.UseSqlServer("SERVER=.\\SqlExpress;TRUSTED_CONNECTION=yes;DATABASE=TraceServer");
});
builder.Services.AddRazorPages();
builder.Services.AddHostedService<QueueProcessor>();
var app = builder.Build();

// Create or migrate database
using (var scope = app.Services.CreateScope()) {
    using var dc = scope.ServiceProvider.GetRequiredService<NetUtilsDbContext>();
    await dc.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.Run();
