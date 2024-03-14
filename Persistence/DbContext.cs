 using Microsoft.EntityFrameworkCore;
using SimpleApi.Persistence.Entities;


namespace SimpleApi.Persistence;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public virtual DbSet<SmallGroupDTO> SmallGroups { get; set; }

    public DbContext(DbContextOptions<DbContext> options) : base(options) {
        Database.EnsureCreated();
    }

}
