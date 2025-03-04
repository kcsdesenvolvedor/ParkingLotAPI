using Microsoft.AspNetCore.Mvc;
using ParkingLotAPI.Entities;
using ParkingLotAPI.Models.Vacancy;
using ParkingLotAPI.Sevices.ParkingService;
using ParkingLotAPI.Sevices.VacancyService;

namespace ParkingLotAPI.Controllers
{
    /// <summary>
    /// Controller para gerenciar operações relacionadas a vagas de estacionamento.
    /// </summary>
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

        /// <summary>
        /// Retorna uma lista de todas as vagas cadastradas.
        /// </summary>
        /// <returns>Uma lista de vagas.</returns>
        /// <response code="200">Retorna a lista de vagas.</response>
        /// <response code="400">Se ocorrer um erro interno.</response>
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

        /// <summary>
        /// Retorna uma vaga específica pelo ID.
        /// </summary>
        /// <param name="id">O ID da vaga.</param>
        /// <returns>A vaga correspondente ao ID.</returns>
        /// <response code="200">Retorna a vaga encontrada.</response>
        /// <response code="404">Se a vaga não for encontrada.</response>
        /// <response code="400">Se ocorrer um erro interno.</response>
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

        /// <summary>
        /// Adiciona uma nova vaga.
        /// </summary>
        /// <param name="model">Os dados da vaga a ser criada.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Vaga adicionada com sucesso.</response>
        /// <response code="400">Se ocorrer um erro ao salvar a vaga.</response>
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

        /// <summary>
        /// Atualiza os dados de uma vaga existente.
        /// </summary>
        /// <param name="model">Os dados atualizados da vaga.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Vaga atualizada com sucesso.</response>
        /// <response code="404">Se a vaga não for encontrada.</response>
        /// <response code="400">Se ocorrer um erro ao atualizar a vaga.</response>
        [HttpPut]
        public async Task<ActionResult<Vacancy>> UpdateVacancy(UpdateVacancyModel model)
        {
            try
            {
                var isUpdatedVacancy = await _vacancyService.UpdateVacancyAsync(model);
                if (isUpdatedVacancy)
                {
                    return Ok(new { Message = "Vaga atualizada com sucesso!" });
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Erro ao atualizar a vaga", Error = ex.Message });
            }
        }

        /// <summary>
        /// Remove uma vaga pelo ID.
        /// </summary>
        /// <param name="id">O ID da vaga a ser removida.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Vaga removida com sucesso.</response>
        /// <response code="404">Se a vaga não for encontrada.</response>
        /// <response code="400">Se ocorrer um erro ao remover a vaga.</response>
        [HttpDelete]
        public async Task<ActionResult> DeleteVacancy(int id)
        {
            try
            {
                var isDeletedVacancy = await _vacancyService.DeleteVacancyAsync(id);
                if (isDeletedVacancy)
                {
                    return Ok(new { Message = "Vaga deletada com sucesso!" });
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