using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using SmartCrm.Configurations;
using SmartCrm.Data.DbContexts;
using SmartCrm.Middlewares;
using SmartCrm.Service.Common.Mapping;
using SmartCrm.Service.Helpers;
using SmartCrm.Service.Services.BackUpService;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT configuration
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtIssuer = jwtSection.GetValue<string>("Issuer")!;
var jwtAudience = jwtSection.GetValue<string>("Audience")!;
var jwtKey = jwtSection.GetValue<string>("SecurityKey")!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(
                                     Encoding.UTF8.GetBytes(jwtKey)),
        RoleClaimType = ClaimTypes.Role
    };
});

// Add services to the container.
QuestPDF.Settings.License = LicenseType.Community;
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

builder.ConfigureSwaggerAuth();
builder.ConfigureCORSPolicy();
builder.ConfigureServiceLayer();
builder.ConfigurationValidators();


builder.Services.AddHttpClient<TelegramBotHelper>();
builder.Services.AddHostedService<BackupService>();
builder.Services.AddSingleton<BackupService>();

var app = builder.Build();
app.UseMiddleware<ExceptionHandleMiddleware>();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();