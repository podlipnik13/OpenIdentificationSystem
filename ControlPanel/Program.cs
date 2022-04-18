using Microsoft.EntityFrameworkCore;
using ControlPanel.Development;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<OISContext>(options =>
  options.UseNpgsql(builder.Configuration.GetConnectionString("npgsql")));


var app = builder.Build();



using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<OISContext>();
    context.Database.EnsureCreated();

    DbInitializer.Init(context);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
