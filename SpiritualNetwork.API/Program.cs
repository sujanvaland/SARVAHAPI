
using Microsoft.OpenApi.Models;
using SpiritualNetwork.API.AppContext;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.API.Services;
using SpiritualNetwork.Entities.CommonModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.API.Helper;
using SpiritualNetwork.API.Controllers;
using SpiritualNetwork.API.Hubs;
using RestSharp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();



builder.Services.AddDbContext<AppDbContext>((serviceProvider, dbContextBuilder) =>
{
    var ConnectionString = builder.Configuration.GetConnectionString("Default");
    dbContextBuilder.UseSqlServer(ConnectionString,dbContextBuilder => dbContextBuilder.EnableRetryOnFailure());
});


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .SetIsOriginAllowed((host) => true)
            .AllowCredentials()
            .AllowAnyMethod();
        });

    //options.AddPolicy("AllowSpecificOrigin", builder =>
    //{
    //    builder.WithOrigins("https://backoffice.generositymatrix.net", "http://localhost:3000")
    //           .AllowAnyHeader()
    //           .AllowAnyMethod();
    //});
});




builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        In = ParameterLocation.Header,
        Description = "JWT Autherization",
        Name = "Autherization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IGlobalSettingService, GlobalSettingService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<ISubcriptionService, SubcriptionService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IChatService,ChatService>();
builder.Services.AddScoped<IRestClient, RestClient>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:Secret"])),
    };
});

var app = builder.Build();

app.UseCors();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseSwagger();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
// specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
    // ^ This line sets the URL for the Swagger JSON file.
    // You can adjust the path and version as per your setup.
    // For example, if you have multiple versions, you can change "v1" to "v2".
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<NotificationHub>("/chathub");

app.Run();

