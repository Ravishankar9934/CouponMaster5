using CouponMaster.DependencyResolver;
using CouponMaster.API.Extensions; // Import your new folder

var builder = WebApplication.CreateBuilder(args);

// --- 1. SERVICE REGISTRATION (The "Ingredients") ---

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.AddCustomSwagger(); // <--- Our new method

// Clean One-Liners
builder.Services.RegisterServices();       // Your DependencyResolver
builder.Services.AddApplicationServices(); // CORS
builder.Services.AddIdentityServices(builder.Configuration); // JWT

// --- 2. PIPELINE CONFIGURATION (The "Instructions") ---

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Strict Order Matters Here:
app.UseCors("CorsPolicy"); // 1. Allow connection
app.UseAuthentication();   // 2. Check ID
app.UseAuthorization();    // 3. Check Permissions

app.MapControllers();

app.Run();