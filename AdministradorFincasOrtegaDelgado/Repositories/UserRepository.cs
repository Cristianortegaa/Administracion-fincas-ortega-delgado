using AdministradorFincasOrtegaDelgado.Data;
using AdministradorFincasOrtegaDelgado.Models;
using Microsoft.EntityFrameworkCore;

namespace AdministradorFincasOrtegaDelgado.Repositories;

public class UserRepository(ApplicationDbContext db) : IUserRepository
{
    public async Task<IEnumerable<User>> GetAllAsync() =>
        await db.Users.OrderBy(u => u.Name).ToListAsync();

    public Task<User?> GetByIdAsync(int id) =>
        db.Users.FirstOrDefaultAsync(u => u.Id == id);

    public Task<User?> GetByEmailAsync(string email) =>
        db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

    public Task<bool> AnyAsync() => db.Users.AnyAsync();

    public async Task AddAsync(User user) => await db.Users.AddAsync(user);

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null) return false;
        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return true;
    }

    public Task SaveAsync() => db.SaveChangesAsync();
}
