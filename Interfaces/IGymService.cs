using userMangment.Models;
namespace userMangment.Interfaces;

public interface IGymService
{
    Task <IEnumerable<GymClass>>  GetAllClassesAsync();
    Task<GymClass?> GetByIdAsync(int id);
    Task<bool> CreateAsync(GymClass gymClass);

    Task<bool> UpdateAsync(GymClass gymClass);

    Task<bool> DeleteAsync(int id);
}