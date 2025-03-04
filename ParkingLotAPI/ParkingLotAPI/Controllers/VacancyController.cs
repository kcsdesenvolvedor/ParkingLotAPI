using Microsoft.AspNetCore.Mvc;
using ParkingLotAPI.Entities;
using ParkingLotAPI.Models.Vacancy;
using ParkingLotAPI.Sevices.ParkingService;
using ParkingLotAPI.Sevices.VacancyService;

namespace ParkingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VacancyController : Controller
    {
        private readonly IVacancyService _vacancyService;
        private readonly IParkingService _parkingService;
        public VacancyController(IVacancyService vacancyService, IParkingService parkingService)
        {
            _vacancyService = vacancyService;
            _parkingService = parkingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vacancy>>> GetVacancies()
        {
            try
            {
                var vacancies = await _vacancyService.GetVacanciesAsync();
                return Ok(vacancies.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Erro interno do sistema", Error = ex.Message });
            }
        }

        [HttpGet("GetVacancy/{id}")]
        public async Task<ActionResult<Vacancy>> GetVacancyById(int id)
        {
            try
            {
                var vacancy = await _vacancyService.GetVacancyByIdAsync(id);
                if (vacancy != null)
                {
                    return Ok(vacancy);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Erro interno do sistema", Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveVacancy(CreateVacancyModel model)
        {
            try
            {
                var parking = await _parkingService.GetParkingByIdAsync(model.ParkingId);

                var vacancy = new Vacancy
                {
                    Number = model.Number,
                    Busy = false,
                    ParkingId = parking.Id,
                };
                await _vacancyService.AddVacancyAsync(vacancy);
                return Ok(new { Message = "Vaga adicionada com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Erro ao salvar a vaga", Error = ex.Message });
            }
        }

        [HttpPut]
        public async Task<ActionResult<Vacancy>> UpdateVacancy(UpdateVacancyModel model)
        {
            try
            {
                var isUpdatedVacancy = await _vacancyService.UpdateVacancyAsync(model);
                if (isUpdatedVacancy)
                {
                    return Ok(new { Message = "Vaga atualizada com sucesso!"});
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Erro ao atualizar a vaga", Error = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteVacancy(int id)
        {
            try
            {
                var isDeletedVacancy = await _vacancyService.DeleteVacancyAsync(id);
                if (isDeletedVacancy)
                {
                    return Ok(new { Message = "Vaga deletado com sucesso!" });
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Erro ao deletar a vaga", Error = ex.Message });
            }
        }
    }
}
