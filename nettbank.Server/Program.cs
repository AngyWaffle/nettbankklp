using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins, policy =>
    {
        policy.SetIsOriginAllowed(origin => true) // Allow all origins dynamically
              .AllowAnyMethod()  // Allow GET, POST, PUT, DELETE
              .AllowAnyHeader()  // Allow all headers
              .AllowCredentials(); // Allow cookies & authentication headers
    });
});

builder.Services.AddAntiforgery(options => options.SuppressXFrameOptionsHeader = true);

// Add DbContext and other services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=database.db"));

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Apply CORS middleware
app.UseCors(MyAllowSpecificOrigins);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();