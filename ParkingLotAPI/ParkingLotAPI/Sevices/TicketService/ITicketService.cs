using ParkingLotAPI.Entities;
using ParkingLotAPI.Models.Ticket;

namespace ParkingLotAPI.Sevices.TicketService
{
    public interface ITicketService
    {
        Task<IEnumerable<Ticket>> GetTiketsAsync();
        Task<Ticket> GetTicketByIdAsync(int ticketId);
        Task<IEnumerable<Ticket>> GetTicketsByParkingIdAsync(int parkingId);
        Task AddTicketAsync(Ticket ticket);
        Task<bool> UpdateTicketAsync(UpdateTicketModel ticketViewModel);
        Task<bool> DeleteTicketAsync(int ticketId);
        Task ProcessedTicket(Ticket processedTicket);
    }
}
