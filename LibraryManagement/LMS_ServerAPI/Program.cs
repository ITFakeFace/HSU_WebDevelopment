using LMS_ServerAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using LMS_ServerAPI.Models;
//using LMS_ServerAPI.Repositories.BookRepository;
using LMS_ServerAPI.Repositories.AuthorRepositories;
using LMS_ServerAPI.Services.AuthorService;
using LMS_ServerAPI.Services.PublisherService;
using LMS_ServerAPI.Repositories.PublisherRepository;
using LMS_ServerAPI.Repositories.VendorRepository;
using LMS_ServerAPI.Services.VendorService;
using LMS_ServerAPI.Repositories.SeriesRepository;
using LMS_ServerAPI.Services.SeriesService;

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
				"https://localhost:50283"
				)
			.AllowAnyHeader()
			.AllowAnyMethod();
		});
});

// Đăng ký các repository và service

builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<ISeriesRepository, SeriesRepository>();
builder.Services.AddScoped<ISeriesService, SeriesService>();

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
