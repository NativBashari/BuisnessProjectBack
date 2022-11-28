using BuisnessLogic;
using BuisnessProjectAPI.HubConfig;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
    .WithOrigins("http://localhost:4200")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()); 
}); 

// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddSingleton<MainLogic>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
app.UseCors("CorsPolicy");


app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ServiceStationsHub>("/ServiceStations");
    endpoints.MapHub<OrdersToPrepareHub>("/OrdersToPrepare");
    endpoints.MapHub<OrdersToDelieveryHub>("/OrdersToDelievery");
}
) ;

app.MapControllers();

app.Run();
