using Microsoft.EntityFrameworkCore;
using src.features.user.entities;

namespace src.contexts;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}
