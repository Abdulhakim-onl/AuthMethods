using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using IdentityAuth.Infrastructure;
using IdentityAuth.Models;
using System.Text;
using IdentityAuth.Interfaces;
using IdentityAuth.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddControllers();
//Swagger
#region Swagger Configuration

builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(swagger =>
{
    //This is to generate the Default UI of Swagger Documentation
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "TestAuth API"
    });
    // To Enable authorization using Swagger (JWT)
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
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
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                    }
                });
});

#endregion

// DB Context
builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("WebApiDatabase"));
    options.EnableSensitiveDataLogging();
});

builder.Services.AddIdentity<User, IdentityRole>(config =>
{
    config.User.RequireUniqueEmail = true;
    config.SignIn.RequireConfirmedEmail = false;
    config.Password.RequiredLength = 4;
    config.Password.RequireDigit = false;
    config.Password.RequireUppercase = false;
    config.Password.RequireLowercase = false;
    config.Password.RequireNonAlphanumeric = false;
})
                    .AddDefaultTokenProviders()
                    .AddEntityFrameworkStores<ApplicationContext>();

//.AddFacebook(options =>
//{
//    IConfigurationSection FBAuthNSection =
//     builder.Configuration.GetSection("Authentication:FB");
//    options.ClientId = FBAuthNSection["ClientId"];
//    options.ClientSecret = FBAuthNSection["ClientSecret"];
//});
//.AddMicrosoftAccount(microsoftOptions =>
//{
//    microsoftOptions.ClientId = config["Authentication:Microsoft:ClientId"];
//    microsoftOptions.ClientSecret = config["Authentication:Microsoft:ClientSecret"];
//})
//.AddTwitter(twitterOptions =>
//{
//    twitterOptions.ConsumerKey = config["Authentication:Twitter:ConsumerAPIKey"];
//    twitterOptions.ConsumerSecret = config["Authentication:Twitter:ConsumerSecret"];
//    twitterOptions.RetrieveUserDetails = true;
//});


// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    //options.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme;
    //options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings.GetSection("validIssuer").Value,
        ValidAudience = jwtSettings.GetSection("validAudience").Value,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.GetSection("securityKey").Value))
    };
})
    .AddGoogle(options =>
{
    IConfigurationSection googleAuthNSection =
    builder.Configuration.GetSection("Authentication:Google");
    options.ClientId = googleAuthNSection["ClientId"];
    options.ClientSecret = googleAuthNSection["ClientSecret"];

    //this function is get user google profile image
    options.Scope.Add("profile");
    options.SignInScheme = IdentityConstants.ExternalScheme;
});

//Cookie Policy needed for External Auth
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
