using Microsoft.OpenApi.Models;
using ParkingLotAPI.Data;
using ParkingLotAPI.Repositories;
using ParkingLotAPI.Sevices.CarService;
using ParkingLotAPI.Sevices.ParkingService;
using ParkingLotAPI.Sevices.TicketService;
using ParkingLotAPI.Sevices.VacancyService;

var builder = WebApplication.CreateBuilder(args);

// Add context
builder.Services.AddDbContext<ParkingLotContext>();

// Add Repository generic
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Add services to the container.
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IVacancyService, VacancyService>();
builder.Services.AddScoped<ITicketService, TicketServive>();
builder.Services.AddScoped<IParkingService, ParkingService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Configuração do Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ParkingLotAPI", Version = "v1" });

    // Configura o Swagger para usar os comentários XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ParkingLotAPI v1");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
