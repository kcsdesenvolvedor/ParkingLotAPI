using Microsoft.AspNetCore.Mvc;
using ParkingLotAPI.Entities;
using ParkingLotAPI.Models.Parking;
using ParkingLotAPI.Models.Ticket;
using ParkingLotAPI.Sevices.ParkingService;
using ParkingLotAPI.Sevices.TicketService;

namespace ParkingAPI.Controllers
{
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Parking>>> GetParkings()
        {
            try
            {
                var pakings = await _parkingService.GetParkingsAsync();
                return Ok(pakings.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(new {Message = "Erro interno do sistema", Error = ex.Message});
            }
        }

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
                return Ok(new {Message = "Estacionamento adicionado com sucesso!"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Erro ao salvar o estacionamento", Error = ex.Message });
            }
        }

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

        [HttpDelete]
        public async Task<ActionResult> DeleteParking(int id)
        {
            try
            {
                var isDeletedParking = await _parkingService.DeleteParkingAsync(id);
                if (isDeletedParking)
                {
                    return Ok(new { Message = "Estacionamento deletado com sucesso!"});
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
