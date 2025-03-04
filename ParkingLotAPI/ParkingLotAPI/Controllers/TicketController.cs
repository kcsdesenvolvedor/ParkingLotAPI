using Microsoft.AspNetCore.Mvc;
using ParkingLotAPI.Entities;
using ParkingLotAPI.Models.Ticket;
using ParkingLotAPI.Models.Vacancy;
using ParkingLotAPI.Sevices.CarService;
using ParkingLotAPI.Sevices.ParkingService;
using ParkingLotAPI.Sevices.TicketService;
using ParkingLotAPI.Sevices.VacancyService;
using System.Net.Sockets;

namespace ParkingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly ICarService _carService;
        private readonly IVacancyService _vacancyService;
        private readonly IParkingService _parkingService;
        public TicketController(ITicketService ticketService, ICarService car, IVacancyService vacancyService, IParkingService parkingService)
        {
            _ticketService = ticketService;
            _carService = car;
            _vacancyService = vacancyService;
            _parkingService = parkingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets()
        {
            var tickets = await _ticketService.GetTiketsAsync();
            FillObjectProperty(tickets);
            return Ok(tickets);
        }

        [HttpGet("GetTicket/{id}")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTicketById(int id)
        {
            try
            {
                var ticket = await _ticketService.GetTicketByIdAsync(id);
                if (ticket != null)
                {
                    FillObjectProperty(new List<Ticket> { ticket });
                    return Ok(ticket);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Erro interno", Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveTicket(CreateTicketModel model)
        {
            try
            {
                var car = await _carService.GetCarByIdAsync(model.CarId);
                var vacancy = await _vacancyService.GetVacancyByIdAsync(model.VacancyId);
                var parking = await _parkingService.GetParkingByIdAsync(vacancy.ParkingId);

                if (car is null)
                    return NotFound(new {Message = "Carro não encontrado!"});

                if (vacancy is null)
                    return NotFound(new { Message = "Vaga não encontrada!" });

                if (vacancy.Busy)
                    return BadRequest(new {Message = "Essa vaga já está ocupada"});

                var ticket = new Ticket
                {
                    CarId = car.Id,
                    VacancyId = vacancy.Id,
                    Entry = DateTime.Now,
                    PricePerHourOfParking = parking.PricePerHour,
                    Status = 0,
                };
                await _ticketService.AddTicketAsync(ticket);

                // Aterando o status da vaga para ocupado
                var vacancyModel = new UpdateVacancyInTicket
                {
                    Id = vacancy.Id,
                    Number = vacancy.Number,
                    Busy = false,
                };
                await _vacancyService.UpdateVacancyInTicketAsync(vacancyModel);

                return Ok("Ticket adicionado com sucesso!");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Erro ao salvar o ticket", Error = ex.Message });
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateTicket(UpdateTicketModel model)
        {
            try
            {
                var ticketDb = await _ticketService.GetTicketByIdAsync(model.Id);
                if (ticketDb is null)
                    return NotFound(new { Message = "Ticket não encontrado!" });

                var isChangedVacancy = ticketDb.VacancyId != model.VacancyId;

                if (isChangedVacancy)
                {
                    var newVacancy = await _vacancyService.GetVacancyByIdAsync(model.VacancyId);
                    if (newVacancy is null)
                        return NotFound(new { Message = "Vaga não encontrada!" });

                    if (newVacancy.Busy)
                        return BadRequest(new { Message = "Essa vaga já está ocupada!" });
                }

                var isUpdatedTicket = await _ticketService.UpdateTicketAsync(model);
                if (!isUpdatedTicket)
                    return NotFound();

                if (ticketDb.VacancyId != model.VacancyId)
                {
                    var vacancy = await _vacancyService.GetVacancyByIdAsync(ticketDb.VacancyId);
                    var vacancyModel = new UpdateVacancyInTicket
                    {
                        Id = vacancy.Id,
                        Number = vacancy.Number,
                        Busy = false,
                    };
                    await _vacancyService.UpdateVacancyInTicketAsync(vacancyModel);
                }
                return Ok(new { Message = "Ticket atualizado com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Erro ao atualizar o ticket", Error = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteTicket(int id)
        {
            try
            {
                var ticket = await _ticketService.GetTicketByIdAsync(id);
                if (ticket != null && ticket.Status == TicketStatus.Pending)
                    return BadRequest(new {Message = "Esse ticket ainda não foi processado!"});

                var isDeletedTicket = await _ticketService.DeleteTicketAsync(id);
                if (isDeletedTicket)
                {
                    return Ok(new { Message = $"Ticket id = {id} removido com sucesso!" });
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Erro ao deletar o ticket", Error = ex.Message });
            }
        }

        [HttpPost("ProcessedTicket")]
        public async Task<ActionResult> ProcessedTicketAsync(ProcessedTicketModel model)
        {
            try
            {
                var ticket = await _ticketService.GetTicketByIdAsync(model.TicketId);
                if (ticket.Status == TicketStatus.Processed)
                    return BadRequest(new { Message = "Esse ticket já foi processado!" });

                if (ticket != null)
                {
                    ticket.Exit = model.Exit;
                    ticket.Status = TicketStatus.Processed;

                    await _ticketService.ProcessedTicket(ticket);

                    // Atualizando status da vaga para não ocupada
                    var vacancy = await _vacancyService.GetVacancyByIdAsync(ticket.VacancyId);
                    if (vacancy != null)
                    {
                        var vacancyModel = new UpdateVacancyInTicket
                        {
                            Id = vacancy.Id,
                            Number = vacancy.Number,
                            Busy = false,
                        };
                        await _vacancyService.UpdateVacancyInTicketAsync(vacancyModel);
                    }

                    return Ok(ticket);
                }
                return NotFound(new { Message = "Ticket não encontrado!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Erro ao processar o ticket", Error = ex.Message });
            }
        }

        private async void FillObjectProperty(IEnumerable<Ticket> tickets)
        {
            foreach (var ticket in tickets)
            {
                ticket.Car = await _carService.GetCarByIdAsync(ticket.CarId);
                ticket.Vacancy = await _vacancyService.GetVacancyByIdAsync(ticket.VacancyId);
            }
        }
    }
}
