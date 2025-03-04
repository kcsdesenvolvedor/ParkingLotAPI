namespace ParkingLotAPI.Models.Vacancy
{
    public class UpdateVacancyInTicket
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public bool Busy { get; set; }
    }
}
