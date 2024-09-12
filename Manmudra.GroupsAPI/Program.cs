using Manmudra.Data.Context;
using Manmudra.GroupsAPI.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.InjectService(builder.Configuration);

var app = builder.Build();
app.Use(async (context, next) =>
{
    context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = long.MaxValue; // 50 MB
    await next.Invoke();
});
app.InjectConfiguration(app);

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<ManmudraContext>();
await dbContext.Database.MigrateAsync();

app.Run();
