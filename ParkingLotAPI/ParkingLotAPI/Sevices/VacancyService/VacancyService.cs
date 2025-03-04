using ParkingLotAPI.Entities;
using ParkingLotAPI.Models.Vacancy;
using ParkingLotAPI.Repositories;

namespace ParkingLotAPI.Sevices.VacancyService
{
    public class VacancyService : IVacancyService
    {
        private readonly IRepository<Vacancy> _repository;
        public VacancyService(IRepository<Vacancy> repository)
        {
            _repository = repository;
        }
        public async Task AddVacancyAsync(Vacancy vacancy)
        {
            await _repository.AddAsync(vacancy);
        }

        public async Task<bool> DeleteVacancyAsync(int id)
        {
            var vacancy = await _repository.GetByIdAsync(id);
            if (vacancy != null)
            {
                await _repository.DeleteAsync(vacancy);
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Vacancy>> GetVacanciesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Vacancy> GetVacancyByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateVacancyAsync(UpdateVacancyModel vacancyModel)
        {
            var vacancyDb = await _repository.GetByIdAsync(vacancyModel.Id);
            if (vacancyDb != null)
            {
                var vacancy = new Vacancy
                {
                    Id = vacancyDb.Id,
                    Number = vacancyModel.Number,
                    ParkingId = vacancyDb.ParkingId,
                    Parking = vacancyDb.Parking,
                };
                await _repository.UpdateAsync(vacancy);
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateVacancyInTicketAsync(UpdateVacancyInTicket vacancyModel)
        {
            var vacancyDb = await _repository.GetByIdAsync(vacancyModel.Id);
            if (vacancyDb != null)
            {
                var vacancy = new Vacancy
                {
                    Id = vacancyDb.Id,
                    Number = vacancyDb.Number,
                    Busy = vacancyModel.Busy,
                    ParkingId = vacancyDb.ParkingId,
                    Parking = vacancyDb.Parking,
                };
                await _repository.UpdateAsync(vacancy);
                return true;
            }
            return false;
        }
    }
}
