using ParkingLotAPI.Entities;
using ParkingLotAPI.Repositories;

namespace ParkingLotAPI.Sevices.CarService
{
    public class CarService : ICarService
    {
        private readonly IRepository<Car> _repository;
        public CarService(IRepository<Car> repository)
        {
            _repository = repository;
        }
        public async Task AddCarAsync(Car car)
        {
            await _repository.AddAsync(car);
        }

        public async Task<bool> DeleteCarAsync(int carId)
        {
            var car = await _repository.GetByIdAsync(carId);
            if (car != null)
            {
                await _repository.DeleteAsync(car);
                return true;
            }
            return false;
        }

        public async Task<Car> GetCarByIdAsync(int carId)
        {
            return await _repository.GetByIdAsync(carId);
        }

        public async Task<IEnumerable<Car>> GetCarsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<bool> UpdateCarAsync(Car car)
        {
            var carDb = await _repository.GetByIdAsync(car.Id);
            if (carDb != null)
            {
                await _repository.UpdateAsync(car);
                return true;
            }
            return false;
        }
    }
}
