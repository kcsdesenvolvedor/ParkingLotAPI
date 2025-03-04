using Microsoft.AspNetCore.Mvc;
using ParkingLotAPI.Entities;
using ParkingLotAPI.Models.Car;
using ParkingLotAPI.Sevices.CarService;

namespace ParkingLotAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarController : Controller
    {
        private readonly ICarService _carService;
        public CarController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> GetCars()
        {
            var cars = await _carService.GetCarsAsync();
            return Ok(cars);
        }

        [HttpGet("GetCar/{id}")]
        public async Task<ActionResult<IEnumerable<Car>>> GetCarById(int id)
        {
            try
            {
                var car = await _carService.GetCarByIdAsync(id);
                if (car != null)
                    return Ok(car);

                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Erro interno", Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveCar(CreateCarModel model)
        {
            try
            {
                var cars = await _carService.GetCarsAsync();
                var plateAlreadyExists = cars.Any(c => c.Plate == model.Plate);

                if (plateAlreadyExists)
                {
                    return BadRequest(new { Message = "Já existe um carro cadastrado com essa placa!" });
                }

                var car = new Car
                {
                    Manufacture = model.Manufacture,
                    Model = model.Model,
                    Plate = model.Plate,
                    Color = model.Color
                };
                await _carService.AddCarAsync(car);
                return Ok("Carro adicionado com sucesso!");
            }
            catch (Exception ex)
            {
                return BadRequest(new {Message = "Erro ao salvar o carro", Error = ex.Message});
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateCar(UpdateCarModel model)
        {
            try
            {
                var carDb = await _carService.GetCarByIdAsync(model.Id);
                if (carDb != null)
                {
                    var carForUpdated = new Car
                    {
                        Manufacture = model.Manufacture,
                        Model = model.Model,
                        Plate = model.Plate,
                        Color = model.Color
                    };
                    await _carService.UpdateCarAsync(carForUpdated);
                    return Ok("Carro adicionado com sucesso!");
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Erro ao atualizar o carro id = {model.Id}", Error = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteCar(int id)
        {
            try
            {
                var isDeletedCar = await _carService.DeleteCarAsync(id);
                if (isDeletedCar)
                {
                    return Ok(new { Message = $"Carro id = {id} removido com sucesso!" });
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new {Message = $"Erro ao tentar remover carro id = {id}", Error = ex.Message});
            }
        }
    }
}
