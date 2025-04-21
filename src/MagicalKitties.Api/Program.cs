using System.Text;
using Asp.Versioning;
using MagicalKitties.Api;
using MagicalKitties.Api.Auth;
using MagicalKitties.Api.Mapping;
using MagicalKitties.Api.Services;
using MagicalKitties.Application;
using MagicalKitties.Application.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager config = builder.Configuration;

// Add services to the container.
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IJwtTokenGeneratorService, JwtTokenGeneratorService>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(x =>
                                   {
                                       x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                       x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                                       x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                                   }).AddJwtBearer(x =>
                                                   {
                                                       x.TokenValidationParameters = new TokenValidationParameters
                                                                                     {
                                                                                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
                                                                                         ValidateIssuerSigningKey = true,
                                                                                         ValidateLifetime = true,
                                                                                         ValidIssuer = config["Jwt:Issuer"],
                                                                                         ValidAudience = config["Jwt:Audience"],
                                                                                         ValidateIssuer = true,
                                                                                         ValidateAudience = true
                                                                                     };
                                                   });

builder.Services.AddAuthorizationBuilder()
       .AddPolicy(AuthConstants.AdminUserPolicyName, p => p.RequireClaim(AuthConstants.AdminUserClaimName, "true"))
       .AddPolicy(AuthConstants.TrustedUserPolicyName, p => p.RequireAssertion(c =>
                                                                                   c.User.HasClaim(m => m is { Type: AuthConstants.AdminUserClaimName, Value: "true" }) ||
                                                                                   c.User.HasClaim(m => m is { Type: AuthConstants.TrustedUserClaimName, Value: "true" })
                                                       ));

builder.Services.AddApiVersioning(x =>
                                  {
                                      x.DefaultApiVersion = new ApiVersion(1.0);
                                      x.AssumeDefaultVersionWhenUnspecified = true;
                                      x.ReportApiVersions = true;
                                      x.ApiVersionReader = new MediaTypeApiVersionReader("api-version");
                                  }).AddMvc();

builder.Services.AddOutputCache(x =>
                                {
                                    x.AddBasePolicy(c=> c.Cache());
                                    
                                    x.AddPolicy(ApiAssumptions.PolicyNames.Flaws, c =>
                                                                                  {
                                                                                      c.Cache()
                                                                                       .Expire(TimeSpan.FromMinutes(5))
                                                                                       .SetVaryByQuery(["sortBy", "page", "pageSize"])
                                                                                       .Tag(ApiAssumptions.TagNames.Flaws);
                                                                                  });
                                    
                                    x.AddPolicy(ApiAssumptions.PolicyNames.Talents, c =>
                                                                                  {
                                                                                      c.Cache()
                                                                                       .Expire(TimeSpan.FromMinutes(5))
                                                                                       .SetVaryByQuery(["sortBy", "page", "pageSize"])
                                                                                       .Tag(ApiAssumptions.TagNames.Talents);
                                                                                  });
                                    
                                    x.AddPolicy(ApiAssumptions.PolicyNames.MagicalPowers, c =>
                                                                                  {
                                                                                      c.Cache()
                                                                                       .Expire(TimeSpan.FromMinutes(5))
                                                                                       .SetVaryByQuery(["sortBy", "page", "pageSize"])
                                                                                       .Tag(ApiAssumptions.TagNames.MagicalPowers);
                                                                                  });
                                });

builder.Services.AddControllers();

builder.Services.AddApplication();
builder.Services.AddDatabase(config["ConnectionStrings:Database"]!);

builder.Services.Configure<HostOptions>(x =>
                                        {
                                            x.ServicesStartConcurrently = true;
                                            x.ServicesStopConcurrently = false;
                                        });

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
                              {
                                  options
                                      .WithTitle("Magical Kitties Save the Day API")
                                      .WithTheme(ScalarTheme.Mars)
                                      .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                              });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseOutputCache();

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    DbInitializer dbInitializer = app.Services.GetRequiredService<DbInitializer>();
    await dbInitializer.InitializeAsync();
}

app.Run();