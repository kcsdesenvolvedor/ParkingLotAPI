using ParkingLotAPI.Entities;
using ParkingLotAPI.Models.Vacancy;

namespace ParkingLotAPI.Sevices.VacancyService
{
    public interface IVacancyService
    {
        Task<IEnumerable<Vacancy>> GetVacanciesAsync();
        Task<Vacancy> GetVacancyByIdAsync(int id);
        Task AddVacancyAsync(Vacancy vacancy);
        Task<bool> UpdateVacancyAsync(UpdateVacancyModel vacancyModel);
        Task<bool> UpdateVacancyInTicketAsync(UpdateVacancyInTicket vacancyModel);
        Task<bool> DeleteVacancyAsync(int id);
    }
}
