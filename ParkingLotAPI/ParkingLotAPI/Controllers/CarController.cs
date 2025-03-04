using Microsoft.AspNetCore.Mvc;
using ParkingLotAPI.Entities;
using ParkingLotAPI.Models.Car;
using ParkingLotAPI.Sevices.CarService;

namespace ParkingLotAPI.Controllers
{
    /// <summary>
    /// Controller para gerenciar operações relacionadas a carros.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CarController : Controller
    {
        private readonly ICarService _carService;
        public CarController(ICarService carService)
        {
            _carService = carService;
        }

        /// <summary>
        /// Retorna uma lista de todos os carros cadastrados.
        /// </summary>
        /// <returns>Uma lista de carros.</returns>
        /// <response code="200">Retorna a lista de carros.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> GetCars()
        {
            var cars = await _carService.GetCarsAsync();
            return Ok(cars);
        }

        /// <summary>
        /// Retorna um carro específico pelo ID.
        /// </summary>
        /// <param name="id">O ID do carro.</param>
        /// <returns>O carro correspondente ao ID.</returns>
        /// <response code="200">Retorna o carro encontrado.</response>
        /// <response code="404">Se o carro não for encontrado.</response>
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

        /// <summary>
        /// Adiciona um novo carro.
        /// </summary>
        /// <param name="model">Os dados do carro a ser criado.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Carro adicionado com sucesso.</response>
        /// <response code="400">Se a placa do carro já estiver cadastrada ou ocorrer um erro.</response>
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
                return BadRequest(new { Message = "Erro ao salvar o carro", Error = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza os dados de um carro existente.
        /// </summary>
        /// <param name="model">Os dados atualizados do carro.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Carro atualizado com sucesso.</response>
        /// <response code="404">Se o carro não for encontrado.</response>
        /// <response code="400">Se ocorrer um erro ao atualizar o carro.</response>
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

        /// <summary>
        /// Remove um carro pelo ID.
        /// </summary>
        /// <param name="id">O ID do carro a ser removido.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Carro removido com sucesso.</response>
        /// <response code="404">Se o carro não for encontrado.</response>
        /// <response code="400">Se ocorrer um erro ao remover o carro.</response>
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
                return BadRequest(new { Message = $"Erro ao tentar remover carro id = {id}", Error = ex.Message });
            }
        }
    }
}