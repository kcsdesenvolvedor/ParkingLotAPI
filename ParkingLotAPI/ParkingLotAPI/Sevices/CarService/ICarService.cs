using ParkingLotAPI.Entities;

namespace ParkingLotAPI.Sevices.CarService
{
    public interface ICarService
    {
        Task<IEnumerable<Car>> GetCarsAsync();
        Task<Car> GetCarByIdAsync(int carId);
        Task AddCarAsync(Car car);
        Task<bool> UpdateCarAsync(Car car);
        Task<bool> DeleteCarAsync(int carId);
    }
}
