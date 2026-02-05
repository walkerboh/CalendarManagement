using CalendarManagementApi.Models;

namespace CalendarManagementApi.Services;

public interface IBirthdayService
{
    Task<List<Birthday>> GetAllAsync();
    Task<Birthday?> GetByIdAsync(int id);
    Task<Birthday> CreateAsync(Birthday birthday);
    Task<bool> UpdateAsync(Birthday birthday);
    Task<bool> DeleteAsync(int id);
}
