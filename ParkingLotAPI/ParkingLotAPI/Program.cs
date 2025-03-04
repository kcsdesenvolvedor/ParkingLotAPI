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
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
