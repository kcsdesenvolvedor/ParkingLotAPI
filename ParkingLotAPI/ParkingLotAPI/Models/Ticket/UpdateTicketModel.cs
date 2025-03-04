using ParkingLotAPI.Entities;

namespace ParkingLotAPI.Models.Ticket
{
    public class UpdateTicketModel
    {
        public int Id { get; set; }
        public DateTime Entry { get; set; }
        public int VacancyId { get; set; }
    }
}
