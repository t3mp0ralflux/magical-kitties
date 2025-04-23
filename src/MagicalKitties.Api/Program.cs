using System.Text;
using Asp.Versioning;
using MagicalKitties.Api;
using MagicalKitties.Api.Auth;
using MagicalKitties.Api.Services;
using MagicalKitties.Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager config = builder.Configuration;

builder.Logging.ClearProviders();

Log.Logger = new LoggerConfiguration()
             .Enrich.FromLogContext()
             .WriteTo.Console()
             .WriteTo.OpenTelemetry(x =>
                                    {
                                        x.Endpoint = config["Logging:Settings:Endpoint"]!;
                                        x.Protocol = OtlpProtocol.HttpProtobuf;
                                        x.Headers = new Dictionary<string, string>
                                                    {
                                                        ["X-Seq-ApiKey"] = config["Logging:Settings:ApiKey"]!
                                                    };
                                        x.ResourceAttributes = new Dictionary<string, object>
                                                               {
                                                                   ["service.name"] = "Magical Kitties API",
                                                                   ["deployment.environment"] = builder.Environment.EnvironmentName
                                                               };
                                    })
             .CreateLogger();

builder.Services.AddSerilog();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IJwtTokenGeneratorService, JwtTokenGeneratorService>();

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
                                    x.AddBasePolicy(c => c.Cache());

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
builder.Services.AddRateLimiter(options =>
                                {
                                    options.AddFixedWindowLimiter(ApiAssumptions.PolicyNames.RateLimiter, windowOptions =>
                                                                                                          {
                                                                                                              windowOptions.PermitLimit = 3;
                                                                                                              windowOptions.Window = TimeSpan.FromSeconds(5);
                                                                                                          });
                                    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                                    options.OnRejected = async (context, token) =>
                                                         {
                                                             ProblemDetails problemDetails = new()
                                                                                             {
                                                                                                 Status = StatusCodes.Status429TooManyRequests,
                                                                                                 Title = "Too Many Requests",
                                                                                                 Detail = "Too many requests were received. Please wait before submitting again."
                                                                                             };

                                                             await context.HttpContext.Response.WriteAsJsonAsync(problemDetails, token);
                                                         };
                                });

builder.Services.AddDatabase(config["ConnectionStrings:Database"]!);
builder.Services.AddApplication();

builder.Services.Configure<HostOptions>(x =>
                                        {
                                            x.ServicesStartConcurrently = true;
                                            x.ServicesStopConcurrently = false;
                                            x.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore; // will log error, but don't want complete death.
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

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.UseOutputCache();

app.MapControllers();
app.UseRateLimiter();

// if (app.Environment.IsDevelopment())
// {
//     DbInitializer dbInitializer = app.Services.GetRequiredService<DbInitializer>();
//     await dbInitializer.InitializeAsync();
// }

app.Run();