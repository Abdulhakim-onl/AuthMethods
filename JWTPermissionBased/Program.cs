using JWTPermissionBased.Application.Common.Configs;
using JWTPermissionBased.Application.Common.Constants;
using JWTPermissionBased.Application.Common.Interfaces;
using JWTPermissionBased.Application.Common.Middlewares;
using JWTPermissionBased.Application.Common.Services;
using JWTPermissionBased.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddControllers();

// DB Context
var postgresConnectionString = builder.Configuration
    .GetConnectionString("PostgresConnection");
builder.Services.AddDbContext<ApplicationContext>(opt =>
    opt.UseNpgsql(postgresConnectionString
                  ?? throw new InvalidOperationException(), options => { options.EnableRetryOnFailure(); }));

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddEndpointsApiExplorer();
#region Swagger Configuration

builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(swagger =>
{
    //This is to generate the Default UI of Swagger Documentation
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Lucky API"
    });
    // To Enable authorization using Swagger (JWT)
    swagger.AddSecurityDefinition("Lucky", new OpenApiSecurityScheme()
    {
        Name = "LuckySecurity",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Lucky",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Lucky"
                }
            },
            new string[] { }
        }
    });
});

#endregion

var authcon = builder.Configuration.GetSection("Authentication").GetSection("Google").Get<GoogleExternalOptions>();
builder.Services.AddSingleton(authcon);


var jwtTokenConfig = builder.Configuration.GetSection("jwtTokenConfig").Get<JwtTokenConfig>();
builder.Services.AddSingleton(jwtTokenConfig);

builder.Services.AddAuthentication(
        options =>
        {
            options.DefaultScheme = AuthSchemeConstants.LuckyScheme;
            options.DefaultChallengeScheme = AuthSchemeConstants.LuckyScheme;
            options.DefaultAuthenticateScheme = AuthSchemeConstants.LuckyScheme;
        })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            ValidIssuer = jwtTokenConfig.Issuer,
            ValidAudience = jwtTokenConfig.Audience,
            IssuerSigningKey = jwtTokenConfig.GetSymmetricSecurityKey()
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddTransient<IGoogleAuthenticatorService, GoogleAuthenticatorService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<PermissionsMiddleware>();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();