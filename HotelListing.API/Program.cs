using HotelListing.API.Configurations;
using HotelListing.API.Contracts;
using HotelListing.API.Data;
using HotelListing.API.Repository;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.--------------------------------------------------------------------------

//Get connection string from config file (appsettings.json)
var connectionString = builder.Configuration.GetConnectionString("HotelListingDbConnectionString");

//Add DbContext with connection string in options
builder.Services.AddDbContext<HotelListingDbContext>(options => {
    options.UseSqlServer(connectionString);
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//configure API to allow requests that are not on the same server that the API is running
builder.Services.AddCors(options =>
{
    //Add cors policy with name AllowAll and these restrictions
    options.AddPolicy("AllowAllow", b =>
    b.AllowAnyHeader()
    .AllowAnyOrigin()
    .AllowAnyMethod());
});

//use Serilog in application for logging purposes
//ctx is builder context, lc is logger configuration
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

//register configuration for automapper, allows the injection of automapper anywhere in the program
builder.Services.AddAutoMapper(typeof(MapperConfig));

//register repositories in program, associates abstract delaration with implementation of the abstraction
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//use Cors policy with the name AllowAll
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
