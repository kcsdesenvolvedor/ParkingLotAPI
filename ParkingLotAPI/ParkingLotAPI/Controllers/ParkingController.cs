using Microsoft.AspNetCore.Mvc;
using ParkingLotAPI.Entities;
using ParkingLotAPI.Models.Parking;
using ParkingLotAPI.Models.Ticket;
using ParkingLotAPI.Sevices.ParkingService;
using ParkingLotAPI.Sevices.TicketService;

namespace ParkingLotAPI.Controllers
{
    /// <summary>
    /// Controller para gerenciar operações relacionadas a estacionamentos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ParkingController : Controller
    {
        private readonly IParkingService _parkingService;
        private readonly ITicketService _ticketService;
        public ParkingController(IParkingService parkingService, ITicketService ticketService)
        {
            _parkingService = parkingService;
            _ticketService = ticketService;
        }

        /// <summary>
        /// Retorna uma lista de todos os estacionamentos cadastrados.
        /// </summary>
        /// <returns>Uma lista de estacionamentos.</returns>
        /// <response code="200">Retorna a lista de estacionamentos.</response>
        /// <response code="400">Se ocorrer um erro interno.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Parking>>> GetParkings()
        {
            try
            {
                var parkings = await _parkingService.GetParkingsAsync();
                return Ok(parkings.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Erro interno do sistema", Error = ex.Message });
            }
        }

        /// <summary>
        /// Retorna um estacionamento específico pelo ID.
        /// </summary>
        /// <param name="id">O ID do estacionamento.</param>
        /// <returns>O estacionamento correspondente ao ID.</returns>
        /// <response code="200">Retorna o estacionamento encontrado.</response>
        /// <response code="404">Se o estacionamento não for encontrado.</response>
        /// <response code="400">Se ocorrer um erro interno.</response>
        [HttpGet("GetParking/{id}")]
        public async Task<ActionResult<Parking>> GetParkingById(int id)
        {
            try
            {
                var parking = await _parkingService.GetParkingByIdAsync(id);
                if (parking != null)
                {
                    return Ok(parking);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Erro interno do sistema", Error = ex.Message });
            }
        }

        /// <summary>
        /// Adiciona um novo estacionamento.
        /// </summary>
        /// <param name="model">Os dados do estacionamento a ser criado.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Estacionamento adicionado com sucesso.</response>
        /// <response code="400">Se ocorrer um erro ao salvar o estacionamento.</response>
        [HttpPost]
        public async Task<ActionResult> SaveParking(CreateParkingModel model)
        {
            try
            {
                var parking = new Parking
                {
                    ParkingName = model.ParkingName,
                    PricePerHour = model.PricePerHour
                };
                await _parkingService.AddParkingAsync(parking);
                return Ok(new { Message = "Estacionamento adicionado com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Erro ao salvar o estacionamento", Error = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza os dados de um estacionamento existente.
        /// </summary>
        /// <param name="model">Os dados atualizados do estacionamento.</param>
        /// <returns>O estacionamento atualizado.</returns>
        /// <response code="200">Estacionamento atualizado com sucesso.</response>
        /// <response code="404">Se o estacionamento não for encontrado.</response>
        /// <response code="400">Se ocorrer um erro ao atualizar o estacionamento.</response>
        [HttpPut]
        public async Task<ActionResult<Parking>> UpdateParking(UpdateParkingModel model)
        {
            try
            {
                var parkingDb = await _parkingService.GetParkingByIdAsync(model.Id);

                var isUpdatedParking = await _parkingService.UpdateParkingrAsync(model);
                if (!isUpdatedParking)
                    return NotFound();

                if (parkingDb.PricePerHour != model.PricePerHour)
                {
                    var tickets = await _ticketService.GetTicketsByParkingIdAsync(parkingDb.Id);
                    foreach (var ticket in tickets)
                    {
                        var ticketViewModel = new UpdateTicketModel
                        {
                            Id = ticket.Id,
                            Entry = ticket.Entry,
                            VacancyId = ticket.VacancyId,
                        };
                        await _ticketService.UpdateTicketAsync(ticketViewModel);
                    }
                }
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Erro ao atualizar o estacionamento", Error = ex.Message });
            }
        }

        /// <summary>
        /// Remove um estacionamento pelo ID.
        /// </summary>
        /// <param name="id">O ID do estacionamento a ser removido.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Estacionamento removido com sucesso.</response>
        /// <response code="404">Se o estacionamento não for encontrado.</response>
        /// <response code="400">Se ocorrer um erro ao remover o estacionamento.</response>
        [HttpDelete]
        public async Task<ActionResult> DeleteParking(int id)
        {
            try
            {
                var isDeletedParking = await _parkingService.DeleteParkingAsync(id);
                if (isDeletedParking)
                {
                    return Ok(new { Message = "Estacionamento deletado com sucesso!" });
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Erro ao deletar o estacionamento", Error = ex.Message });
            }
        }
    }
}