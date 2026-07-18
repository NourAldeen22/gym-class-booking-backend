using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using userMangment.Interfaces;
using userMangment.Models;

namespace userMangment.Services;
public class GymService : IGymService
{

private readonly ApplicationDbContext _context;
private readonly ILogger<GymService> _logger;

public GymService(ApplicationDbContext context, ILogger<GymService> logger)
{
        _context = context;
        _logger = logger;
}


public async Task<IEnumerable<GymClass>> GetAllClassesAsync()
{
       return await _context.GymClasses.Include(i=> i.AttendingMembers).ToListAsync(); 
        
}

public async Task<GymClass?> GetByIdAsync(int id) => await _context.GymClasses.FirstOrDefaultAsync(g=> g.Id == id);


public async Task<bool> CreateAsync(GymClass gymClass)
{

    if(gymClass.StartTime <= DateTime.UtcNow)
    {
        _logger.LogWarning("Failed to create class: Start time must be in the future, {ClassName}", gymClass.Name);
        return false;        
    }

    _context.GymClasses.Add(gymClass);
    
    await _context.SaveChangesAsync();
    return true;

}


 public async Task<bool> UpdateAsync(GymClass editClass)
{
   var existClass = await GetByIdAsync(editClass.Id);

   if(existClass == null)
    {
       _logger.LogWarning("Failed to update: Class ID {Id} not found", editClass.Id);
        return false;

    }

    if(editClass.StartTime<= DateTime.UtcNow)
    {
        _logger.LogWarning("Failed to update class: Start time must be in the future, {ClassName}", editClass.Name);
        return false;        
    }

        existClass.Name = editClass.Name;
        existClass.StartTime = editClass.StartTime;
        existClass.Duration = editClass.Duration;
        existClass.Desctiption = editClass.Desctiption;

      await _context.SaveChangesAsync();

      return true;    
}


public async Task<bool> DeleteAsync (int id)
{
    var existClass = await GetByIdAsync(id);

    if(existClass == null)
    {
       _logger.LogError("Faild to delete: Class ID {id} not found ", id);
        return false;        
    }

    _context.Remove(existClass);
    await _context.SaveChangesAsync();
    return true;
}

}