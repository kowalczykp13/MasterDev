using Client.Models;
using Microsoft.EntityFrameworkCore;

namespace Client.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Klient> Klient { get; set; }
    }
}
