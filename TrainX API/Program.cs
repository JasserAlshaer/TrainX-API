using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using TrainX_API.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TrainX API", Version = "v1" });
});
builder.Services.Configure<FormOptions>(options =>
{
    // Set max body size limit (e.g., 10 MB)
    options.MultipartBodyLengthLimit = 512 * 1024 * 1024; // 10 MB
});
builder.Services.AddSignalR();
var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();
//app.UseSwaggerUI(c =>
//{
//    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Market Analysis and Monitoring API");
//    c.RoutePrefix = string.Empty;
//});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
                            Path.Combine(Directory.GetCurrentDirectory(), "Attachments")),
    RequestPath = "/attachments"
});
app.MapHub<TrackingHub>("/hubs/tracking");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
