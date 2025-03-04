using ParkingLotAPI.Entities;
using ParkingLotAPI.Models.Parking;

namespace ParkingLotAPI.Sevices.ParkingService
{
    public interface IParkingService
    {
        Task<IEnumerable<Parking>> GetParkingsAsync();
        Task<Parking> GetParkingByIdAsync(int parkingId);
        Task AddParkingAsync(Parking parking);
        Task<bool> UpdateParkingrAsync(UpdateParkingModel parkingViewModel);
        Task<bool> DeleteParkingAsync(int parkingId);
    }
}
