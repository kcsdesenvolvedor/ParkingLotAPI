namespace ParkingLotAPI.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public DateTime Entry { get; set; }
        public DateTime? Exit { get; set; }
        public decimal PricePerHourOfParking { get; set; }
        public TicketStatus Status { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; }
        public int VacancyId { get; set; }
        public Vacancy Vacancy { get; set; }

        // Propriedade calculada que retorna o valor atualizado
        public decimal PaidValue
        {
            get
            {
                var exitTime = Exit ?? DateTime.Now;
                var minuteValue = (decimal)(exitTime - Entry).TotalMinutes;
                return Math.Round(minuteValue * (PricePerHourOfParking / 60), 2);
            }
        }
    }

    public enum TicketStatus
    {
        Pending = 0,
        Processed = 1,
    }
}
