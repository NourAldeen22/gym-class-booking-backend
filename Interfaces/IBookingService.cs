namespace userMangment.Interfaces;
public interface IBookingService
{
    Task<bool> ToggleBookingAsync(int id, string userId);
}