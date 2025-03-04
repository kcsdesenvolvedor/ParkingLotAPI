using ParkingLotAPI.Entities;
using ParkingLotAPI.Models.Ticket;
using ParkingLotAPI.Repositories;

namespace ParkingLotAPI.Sevices.TicketService
{
    public class TicketServive : ITicketService
    {
        private readonly IRepository<Ticket> _repository;
        public TicketServive(IRepository<Ticket> repository)
        {
            _repository = repository;
        }
        public async Task AddTicketAsync(Ticket ticket)
        {
            await _repository.AddAsync(ticket);
        }

        public async Task<bool> DeleteTicketAsync(int tickeId)
        {
            var ticket = await _repository.GetByIdAsync(tickeId);
            if (ticket != null)
            {
                await _repository.DeleteAsync(ticket);
                return true;
            }
            return false;
        }

        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            return await _repository.GetByIdAsync(ticketId);
        }


        public Task<IEnumerable<Ticket>> GetTiketsAsync()
        {
            return _repository.GetAllAsync();
        }

        public async Task<bool> UpdateTicketAsync(UpdateTicketModel ticketViewModel)
        {
            var ticketDb = await _repository.GetByIdAsync(ticketViewModel.Id);
            if (ticketDb != null)
            {
                ticketDb.Entry = ticketViewModel.Entry;
                ticketDb.VacancyId = ticketViewModel.VacancyId;
                await _repository.UpdateAsync(ticketDb);
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByParkingIdAsync(int parkingId)
        {
            var tickets = await _repository.GetAllAsync();
            return tickets.Where(x => x.Vacancy.ParkingId == parkingId);
        }

        public async Task ProcessedTicket(Ticket processedTicket)
        {
            await _repository.UpdateAsync(processedTicket);
        }   
    }
}
