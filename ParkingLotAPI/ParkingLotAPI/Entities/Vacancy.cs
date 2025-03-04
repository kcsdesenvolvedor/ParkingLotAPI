namespace ParkingLotAPI.Entities
{
    public class Vacancy
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public bool Busy { get; set; }
        public int ParkingId { get; set; }
        public Parking Parking { get; set; }
    }
}
