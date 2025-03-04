using ParkingLotAPI.Entities;
using ParkingLotAPI.Models.Parking;
using ParkingLotAPI.Repositories;

namespace ParkingLotAPI.Sevices.ParkingService
{
    public class ParkingService : IParkingService
    {
        private readonly IRepository<Parking> _repository;
        public ParkingService(IRepository<Parking> repository)
        {
            _repository = repository;
        }
        public async Task AddParkingAsync(Parking parking)
        {
            await _repository.AddAsync(parking);
        }

        public async Task<bool> DeleteParkingAsync(int parkingId)
        {
            var parking = await _repository.GetByIdAsync(parkingId);
            if (parking != null)
            {
                await _repository.DeleteAsync(parking);
                return true;
            }
            return false;
        }

        public async Task<Parking> GetParkingByIdAsync(int parkingId)
        {
            return await _repository.GetByIdAsync(parkingId);
        }

        public async Task<IEnumerable<Parking>> GetParkingsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<bool> UpdateParkingrAsync(UpdateParkingModel parkingViewModel)
        {
            var parkingDb = await _repository.GetByIdAsync(parkingViewModel.Id);
            if (parkingDb != null)
            {
                var parkingForUpdated = new Parking
                {
                    Id = parkingViewModel.Id,
                    ParkingName = parkingViewModel.ParkingName,
                    PricePerHour = parkingViewModel.PricePerHour
                };
                await _repository.UpdateAsync(parkingForUpdated);
                return true;
            }
            return false;
        }
    }
}
