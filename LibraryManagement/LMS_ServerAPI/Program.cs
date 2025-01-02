using LMS_ServerAPI.Models;
using LMS_ServerAPI.Repositories;
using LMS_ServerAPI.Repositories.AddressRepositories;
using LMS_ServerAPI.Repositories.AgeRepositories;

using LMS_ServerAPI.Repositories.AuthorRepositories;
using LMS_ServerAPI.Repositories.CategoryRepository;
using LMS_ServerAPI.Repositories.PublisherRepository;
using LMS_ServerAPI.Repositories.SeriesRepository;
using LMS_ServerAPI.Repositories.VendorRepository;
using LMS_ServerAPI.Services.AddressService;
using LMS_ServerAPI.Services.AgeService;
using LMS_ServerAPI.Services.AuthorService;
using LMS_ServerAPI.Services.CategoryService;
using LMS_ServerAPI.Services.PublisherService;
using LMS_ServerAPI.Services.SeriesService;
using LMS_ServerAPI.Services.VendorService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ DbContext với connection string
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LibrarySqlServer"))
);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Thêm CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins(
                "http://127.0.0.1:5500",
                "https://localhost",
                "http://localhost",
                "https://localhost:50283",
                "http://localhost:5057",
                "https://localhost:51917"
                )
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

// Đăng ký các repository và service

// Author 
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
// Age
builder.Services.AddScoped<IAgeRepository, AgeRepository>();
builder.Services.AddScoped<IAgeService, AgeService>();
// Publisher
builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
// Category
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
//Vendor
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
//Series
builder.Services.AddScoped<ISeriesRepository, SeriesRepository>();
builder.Services.AddScoped<ISeriesService, SeriesService>();
// Address
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IDistrictRepository, DistrictRepository>();
builder.Services.AddScoped<IWardRepository, WardRepository>();
builder.Services.AddScoped<IStreetRepository, StreetRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IAddressService, AddressService>();

// Thêm dịch vụ controller
builder.Services.AddControllers();

// Cấu hình Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Cấu hình HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();
app.UseCors("_myAllowSpecificOrigins");
app.MapControllers();

app.Run();
