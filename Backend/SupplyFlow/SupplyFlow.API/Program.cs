//using SupplyFlow.Infrastructure;
//using Microsoft.EntityFrameworkCore;
//using SupplyFlow.Infrastructure.Persistence;
//using SupplyFlow.Application.Interfaces.Services;
//using SupplyFlow.Infrastructure.Services.Auth;
//using SupplyFlow.Infrastructure.Services.User;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;
//using Microsoft.OpenApi.Models;
//using SupplyFlow.Infrastructure.Services.Common;
//using SupplyFlow.API.Middleware;
//using SupplyFlow.Infrastructure.Services.CompanyManagement;
//using SupplyFlow.Infrastructure.Services.Role;
//using SupplyFlow.Infrastructure.Services.CategoryManagement;
//using SupplyFlow.Infrastructure.Services.UnitManagement;
//using SupplyFlow.Infrastructure.Services.WarehouseManagement;
//using SupplyFlow.Infrastructure.Services.SupplierManagement;
//using SupplyFlow.Infrastructure.Services.ProductManagement;
//using SupplyFlow.Infrastructure.Services.WarehouseStockManagement;
//using SupplyFlow.Infrastructure.Services.InventoryManagement;
//using SupplyFlow.Infrastructure.Services.PurchaseRequestManagement;
//using SupplyFlow.Infrastructure.Services.PurchaseOrderManagement;
//using SupplyFlow.Infrastructure.Services.GoodsReceiptManagement;
//using SupplyFlow.Infrastructure.Services.Dashboard;
//using SupplyFlow.Infrastructure.Services.Reports;
//using Microsoft.AspNetCore.Mvc;
//using SupplyFlow.Application.DTOs.Common;



//var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddControllers();

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAngularApp", policy =>
//    {
//        policy
//            .WithOrigins("http://localhost:4200")
//            .AllowAnyHeader()
//            .AllowAnyMethod();
//    });
//});
//// Add services to the container.

//builder.Services
//    .AddControllers()
//    .ConfigureApiBehaviorOptions(options =>
//    {
//        options.InvalidModelStateResponseFactory =
//            context =>
//            {
//                var errors =
//                    context.ModelState
//                        .Where(x =>
//                            x.Value?.Errors.Count > 0)
//                        .ToDictionary(
//                            x => x.Key,
//                            x => x.Value!
//                                .Errors
//                                .Select(error =>
//                                    string.IsNullOrWhiteSpace(
//                                        error.ErrorMessage)
//                                        ? "Invalid value."
//                                        : error.ErrorMessage)
//                                .ToArray());

//                var response =
//                    new ApiErrorResponseDto
//                    {
//                        StatusCode = StatusCodes
//                            .Status400BadRequest,

//                        Message =
//                            "One or more validation errors occurred.",

//                        Errors = errors
//                    };

//                return new BadRequestObjectResult(
//                    response);
//            };
//    });

//builder.Services.AddInfrastructure(builder.Configuration);

//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc(
//        "v1",
//        new OpenApiInfo
//        {
//            Title = "SupplyFlow API",
//            Version = "v1",
//            Description =
//                "SupplyFlow - Inventory and Purchase Management System API"
//        });

//    options.AddSecurityDefinition(
//        "Bearer",
//        new OpenApiSecurityScheme
//        {
//            Name = "Authorization",
//            Type = SecuritySchemeType.Http,
//            Scheme = "bearer",
//            BearerFormat = "JWT",
//            In = ParameterLocation.Header,
//            Description =
//                "Enter your JWT token. Example: Bearer {your token}"
//        });

//    options.AddSecurityRequirement(
//        new OpenApiSecurityRequirement
//        {
//            {
//                new OpenApiSecurityScheme
//                {
//                    Reference =
//                        new OpenApiReference
//                        {
//                            Type =
//                                ReferenceType.SecurityScheme,
//                            Id = "Bearer"
//                        }
//                },
//                Array.Empty<string>()
//            }
//        });
//});

//builder.Services.AddDbContext<SupplyFlowDbContext>(options =>
//    options.UseSqlServer(
//        builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddScoped<IAuthService, AuthService>();

//builder.Services.AddScoped<
//    ICurrentUserService,
//    CurrentUserService>();

//builder.Services.AddScoped<
//    ICompanyService,
//    CompanyService>();

//builder.Services.AddScoped<
//    ICategoryService,
//    CategoryService>();

//builder.Services.AddScoped<
//    IUnitService,
//    UnitService>();

//builder.Services.AddScoped<
//    IWarehouseService,
//    WarehouseService>();

//builder.Services.AddScoped<
//    ISupplierService,
//    SupplierService>();

//builder.Services.AddScoped<
//    IProductService,
//    ProductService>();

//builder.Services.AddScoped<
//    IWarehouseStockService,
//    WarehouseStockService>();

//builder.Services.AddScoped<
//    IInventoryTransactionService,
//    InventoryTransactionService>();

//builder.Services.AddScoped<
//    IPurchaseRequestService,
//    PurchaseRequestService>();

//builder.Services.AddScoped<
//    IPurchaseOrderService,
//    PurchaseOrderService>();

//builder.Services.AddScoped<
//    IGoodsReceiptService,
//    GoodsReceiptService>();

//builder.Services.AddScoped<
//    IDashboardService,
//    DashboardService>();

//builder.Services.AddScoped<
//    IReportService,
//    ReportService>();

//var jwtSettings =
//    builder.Configuration.GetSection("JwtSettings");

//var key = Encoding.UTF8.GetBytes(
//    jwtSettings["Key"]!);

//builder.Services
//    .AddAuthentication(options =>
//    {
//        options.DefaultAuthenticateScheme =
//            JwtBearerDefaults.AuthenticationScheme;

//        options.DefaultChallengeScheme =
//            JwtBearerDefaults.AuthenticationScheme;
//    })
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters =
//            new TokenValidationParameters
//            {
//                ValidateIssuer = true,
//                ValidateAudience = true,
//                ValidateLifetime = true,
//                ValidateIssuerSigningKey = true,

//                ValidIssuer = jwtSettings["Issuer"],
//                ValidAudience = jwtSettings["Audience"],

//                IssuerSigningKey =
//                    new SymmetricSecurityKey(key),

//                ClockSkew = TimeSpan.Zero
//            };
//    });
//builder.Services.AddHttpContextAccessor();

//builder.Services.AddScoped<IRoleService, RoleService>();

//builder.Services.AddScoped<IUserService, UserService>();



//var app = builder.Build();
//app.UseGlobalExceptionMiddleware(); 
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;

//    var dbContext =
//        services.GetRequiredService<SupplyFlowDbContext>();

//    await DatabaseSeeder.SeedAsync(dbContext);
//}

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseCors("AllowAngularApp");

//app.UseMiddleware<ExceptionHandlingMiddleware>();

//app.UseAuthentication();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();
using SupplyFlow.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SupplyFlow.Infrastructure.Persistence;
using SupplyFlow.Application.Interfaces.Services;
using SupplyFlow.Infrastructure.Services.Auth;
using SupplyFlow.Infrastructure.Services.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using SupplyFlow.Infrastructure.Services.Common;
using SupplyFlow.API.Middleware;
using SupplyFlow.Infrastructure.Services.CompanyManagement;
using SupplyFlow.Infrastructure.Services.Role;
using SupplyFlow.Infrastructure.Services.CategoryManagement;
using SupplyFlow.Infrastructure.Services.UnitManagement;
using SupplyFlow.Infrastructure.Services.WarehouseManagement;
using SupplyFlow.Infrastructure.Services.SupplierManagement;
using SupplyFlow.Infrastructure.Services.ProductManagement;
using SupplyFlow.Infrastructure.Services.WarehouseStockManagement;
using SupplyFlow.Infrastructure.Services.InventoryManagement;
using SupplyFlow.Infrastructure.Services.PurchaseRequestManagement;
using SupplyFlow.Infrastructure.Services.PurchaseOrderManagement;
using SupplyFlow.Infrastructure.Services.GoodsReceiptManagement;
using SupplyFlow.Infrastructure.Services.Dashboard;
using SupplyFlow.Infrastructure.Services.Reports;
using Microsoft.AspNetCore.Mvc;
using SupplyFlow.Application.DTOs.Common;

using SupplyFlow.Infrastructure.Services.SalesReturn;
using SupplyFlow.Infrastructure.Services.Customer;
using SupplyFlow.Infrastructure.Services.SaleManagement;



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
// Add services to the container.

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory =
            context =>
            {
                var errors =
                    context.ModelState
                        .Where(x =>
                            x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!
                                .Errors
                                .Select(error =>
                                    string.IsNullOrWhiteSpace(
                                        error.ErrorMessage)
                                        ? "Invalid value."
                                        : error.ErrorMessage)
                                .ToArray());

                var response =
                    new ApiErrorResponseDto
                    {
                        StatusCode = StatusCodes
                            .Status400BadRequest,

                        Message =
                            "One or more validation errors occurred.",

                        Errors = errors
                    };

                return new BadRequestObjectResult(
                    response);
            };
    });

builder.Services.AddInfrastructure(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "SupplyFlow API",
            Version = "v1",
            Description =
                "SupplyFlow - Inventory and Purchase Management System API"
        });

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "Enter your JWT token. Example: Bearer {your token}"
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type =
                                ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                },
                Array.Empty<string>()
            }
        });
});

builder.Services.AddDbContext<SupplyFlowDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<
    ICurrentUserService,
    CurrentUserService>();

builder.Services.AddScoped<
    ICompanyService,
    CompanyService>();

builder.Services.AddScoped<
    ICategoryService,
    CategoryService>();

builder.Services.AddScoped<
    IUnitService,
    UnitService>();

builder.Services.AddScoped<
    IWarehouseService,
    WarehouseService>();

builder.Services.AddScoped<
    ISupplierService,
    SupplierService>();

builder.Services.AddScoped<
    IProductService,
    ProductService>();


builder.Services.AddScoped<
    ISalesReturnService,
    SalesReturnService>();


builder.Services.AddScoped<
    IWarehouseStockService,
    WarehouseStockService>();

builder.Services.AddScoped<
    IInventoryTransactionService,
    InventoryTransactionService>();

builder.Services.AddScoped<
    IPurchaseRequestService,
    PurchaseRequestService>();

builder.Services.AddScoped<
    IPurchaseOrderService,
    PurchaseOrderService>();

builder.Services.AddScoped<
    IGoodsReceiptService,
    GoodsReceiptService>();

builder.Services.AddScoped<
    IDashboardService,
    DashboardService>();

builder.Services.AddScoped<
    IReportService,
    ReportService>();

builder.Services.AddScoped<
    ICustomerService,
    CustomerService>();

builder.Services.AddScoped<ISaleService, SaleService>();

var jwtSettings =
    builder.Configuration.GetSection("JwtSettings");

var key = Encoding.UTF8.GetBytes(
    jwtSettings["Key"]!);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(key),

                ClockSkew = TimeSpan.Zero
            };
    });
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddScoped<IUserService, UserService>();



var app = builder.Build();
app.UseGlobalExceptionMiddleware();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var dbContext =
        services.GetRequiredService<SupplyFlowDbContext>();

    await DatabaseSeeder.SeedAsync(dbContext);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("AllowAngularApp");

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
