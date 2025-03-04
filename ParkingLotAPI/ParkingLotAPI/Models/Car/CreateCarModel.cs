using System.ComponentModel.DataAnnotations;

namespace ParkingLotAPI.Models.Car
{
    public class CreateCarModel
    {
        public CreateCarModel(string manufacture, string model, string plate, string color)
        {
            Manufacture = manufacture;
            Model = model;
            Plate = plate;
            Color = color;
        }

        [Required(ErrorMessage = "O campo fabricante é obrigatório")]
        public string Manufacture { get; set; }

        [Required(ErrorMessage = "O campo modelo é obrigatório")]
        public string Model { get; set; }

        [Required(ErrorMessage = "O campo placa é obrigatório")]
        public string Plate { get; set; }

        public string Color { get; set; }
    }
}
