using Microsoft.AspNetCore.Mvc;
using ParkingLotAPI.Entities;
using ParkingLotAPI.Models.Ticket;
using ParkingLotAPI.Models.Vacancy;
using ParkingLotAPI.Sevices.CarService;
using ParkingLotAPI.Sevices.ParkingService;
using ParkingLotAPI.Sevices.TicketService;
using ParkingLotAPI.Sevices.VacancyService;

namespace ParkingLotAPI.Controllers
{
    /// <summary>
    /// Controller para gerenciar operações relacionadas a tickets de estacionamento.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly ICarService _carService;
        private readonly IVacancyService _vacancyService;
        private readonly IParkingService _parkingService;

        public TicketController(ITicketService ticketService, ICarService carService, IVacancyService vacancyService, IParkingService parkingService)
        {
            _ticketService = ticketService;
            _carService = carService;
            _vacancyService = vacancyService;
            _parkingService = parkingService;
        }

        /// <summary>
        /// Retorna uma lista de todos os tickets cadastrados.
        /// </summary>
        /// <returns>Uma lista de tickets.</returns>
        /// <response code="200">Retorna a lista de tickets.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets()
        {
            var tickets = await _ticketService.GetTiketsAsync();
            FillObjectProperty(tickets);
            return Ok(tickets);
        }

        /// <summary>
        /// Retorna um ticket específico pelo ID.
        /// </summary>
        /// <param name="id">O ID do ticket.</param>
        /// <returns>O ticket correspondente ao ID.</returns>
        /// <response code="200">Retorna o ticket encontrado.</response>
        /// <response code="404">Se o ticket não for encontrado.</response>
        /// <response code="400">Se ocorrer um erro interno.</response>
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

        /// <summary>
        /// Adiciona um novo ticket.
        /// </summary>
        /// <param name="model">Os dados do ticket a ser criado.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Ticket adicionado com sucesso.</response>
        /// <response code="404">Se o carro ou a vaga não forem encontrados.</response>
        /// <response code="400">Se a vaga já estiver ocupada ou ocorrer um erro ao salvar o ticket.</response>
        [HttpPost]
        public async Task<ActionResult> SaveTicket(CreateTicketModel model)
        {
            try
            {
                var car = await _carService.GetCarByIdAsync(model.CarId);
                var vacancy = await _vacancyService.GetVacancyByIdAsync(model.VacancyId);
                var parking = await _parkingService.GetParkingByIdAsync(vacancy.ParkingId);

                if (car is null)
                    return NotFound(new { Message = "Carro não encontrado!" });

                if (vacancy is null)
                    return NotFound(new { Message = "Vaga não encontrada!" });

                if (vacancy.Busy)
                    return BadRequest(new { Message = "Essa vaga já está ocupada" });

                var ticket = new Ticket
                {
                    CarId = car.Id,
                    VacancyId = vacancy.Id,
                    Entry = DateTime.Now,
                    PricePerHourOfParking = parking.PricePerHour,
                    Status = 0,
                };
                await _ticketService.AddTicketAsync(ticket);

                // Alterando o status da vaga para ocupado
                var vacancyModel = new UpdateVacancyInTicket
                {
                    Id = vacancy.Id,
                    Number = vacancy.Number,
                    Busy = true,
                };
                await _vacancyService.UpdateVacancyInTicketAsync(vacancyModel);

                return Ok("Ticket adicionado com sucesso!");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Erro ao salvar o ticket", Error = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza os dados de um ticket existente.
        /// </summary>
        /// <param name="model">Os dados atualizados do ticket.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Ticket atualizado com sucesso.</response>
        /// <response code="404">Se o ticket ou a vaga não forem encontrados.</response>
        /// <response code="400">Se a vaga já estiver ocupada ou ocorrer um erro ao atualizar o ticket.</response>
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

        /// <summary>
        /// Remove um ticket pelo ID.
        /// </summary>
        /// <param name="id">O ID do ticket a ser removido.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        /// <response code="200">Ticket removido com sucesso.</response>
        /// <response code="404">Se o ticket não for encontrado.</response>
        /// <response code="400">Se o ticket ainda não foi processado ou ocorrer um erro ao remover o ticket.</response>
        [HttpDelete]
        public async Task<ActionResult> DeleteTicket(int id)
        {
            try
            {
                var ticket = await _ticketService.GetTicketByIdAsync(id);
                if (ticket != null && ticket.Status == TicketStatus.Pending)
                    return BadRequest(new { Message = "Esse ticket ainda não foi processado!" });

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

        /// <summary>
        /// Processa um ticket, registrando a saída do veículo e calculando o valor a ser pago.
        /// </summary>
        /// <param name="model">Os dados do ticket a ser processado.</param>
        /// <returns>O ticket processado.</returns>
        /// <response code="200">Ticket processado com sucesso.</response>
        /// <response code="404">Se o ticket não for encontrado.</response>
        /// <response code="400">Se o ticket já foi processado ou ocorrer um erro ao processar o ticket.</response>
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