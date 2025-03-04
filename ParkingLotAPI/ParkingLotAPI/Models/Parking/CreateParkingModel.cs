using System.ComponentModel.DataAnnotations;

namespace ParkingLotAPI.Models.Parking
{
    public class CreateParkingModel
    {
        [Required(ErrorMessage = "O nome do estacionamento é obrigatório")]
        public string ParkingName { get; set; }

        [Required(ErrorMessage = "O campo preço por hora é obrigatório")]
        public decimal PricePerHour { get; set; }
    }
}
