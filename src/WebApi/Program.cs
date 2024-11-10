using Application;
using Domain.Modules.Users;
using Infrastructure;
using Persistence;
using Serilog;
using WebApi.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure()
    .AddPersistence()
    .AddApplication();

builder.Services.AddAuthentication();

builder.AddLogging();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddSession();

builder.Services.AddCors(options =>
{
    options.AddPolicy("UserInfoPolicy", o =>
    {
        o.AllowAnyOrigin();
        o.AllowAnyHeader();
        o.AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("UserInfoPolicy");

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();
