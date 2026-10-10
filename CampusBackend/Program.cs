var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Allow our upcoming React frontend to communicate with this backend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy => 
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowReact");
app.UseAuthorization();
app.MapControllers();

app.Run();