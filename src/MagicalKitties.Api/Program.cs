using System.IdentityModel.Tokens.Jwt;
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
using Microsoft.Net.Http.Headers;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager config = builder.Configuration;

builder.Services.AddCors(options =>
                         {
                             options.AddPolicy("Default",
                                 policy =>
                                 {
                                     policy.AllowAnyOrigin()
                                           .AllowAnyHeader()
                                           .AllowAnyMethod();
                                 });
                         });

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

builder.Services.AddSerilog(dispose: true);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

builder.Services.AddOpenApi();

builder.Services
       .AddAuthentication(x =>
                          {
                              x.DefaultScheme = "JWT_OR_GOOGLE";
                              x.DefaultChallengeScheme = "JWT_OR_GOOGLE";
                          })
       .AddJwtBearer("JWT", x =>
                               {
                                   x.Audience = config["Jwt:Audience"];
                                   x.TokenValidationParameters = new TokenValidationParameters
                                                                 {
                                                                     // IssuerSigningKeys = [
                                                                     //     new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
                                                                     //     new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Google:ClientSecret"]!))
                                                                     // ],
                                                                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
                                                                     ValidateIssuerSigningKey = true,
                                                                     ValidateLifetime = true,
                                                                     ValidIssuer = config["Jwt:Issuer"],
                                                                     ValidAudience = config["Jwt:Audience"],
                                                                     // ValidIssuers = [config["Jwt:Issuer"], config["Google:Issuer"]],
                                                                     // ValidAudiences = [config["Jwt:Audience"], config["Google:Audience"]],
                                                                     ValidateIssuer = true,
                                                                     ValidateAudience = true
                                                                 };
                               })
       .AddJwtBearer("Google", x =>
                               {
                                   x.Authority = "https://accounts.google.com";
                                   x.Audience = config["Google:Audience"];
                                   x.TokenValidationParameters = new TokenValidationParameters
                                                                 {
                                                                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Google:ClientSecret"]!)),
                                                                     ValidIssuer = config["Google:Issuer"],
                                                                     ValidAudience = config["Google:Audience"],
                                                                     ValidateIssuer = true,
                                                                     ValidateAudience = true
                                                                 };
                                   
                               })
       .AddPolicyScheme("JWT_OR_GOOGLE", "JWT_OR_GOOGLE", x =>
                                                          {
                                                              x.ForwardDefaultSelector = context =>
                                                                                         {
                                                                                             string? authorization = context.Request.Headers[HeaderNames.Authorization];
                                                                                             if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
                                                                                             {
                                                                                                 return "JWT";
                                                                                             }

                                                                                             string token = authorization["Bearer ".Length..].Trim();
                                                                                             JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();

                                                                                             return jwtHandler.CanReadToken(token) && jwtHandler.ReadJwtToken(token).Issuer.Equals(config["Google:Issuer"]) ? "Google" : "JWT";
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
                                    x.AddPolicy(ApiAssumptions.PolicyNames.Rules, c =>
                                                                                  {
                                                                                      c.Cache()
                                                                                       .Expire(TimeSpan.FromMinutes(60))
                                                                                       .Tag(ApiAssumptions.TagNames.Rules);
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
                                            x.ServicesStopConcurrently = true;
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

app.UseCors("Default");
app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.UseOutputCache();

app.MapControllers();
app.UseRateLimiter();

app.Run();