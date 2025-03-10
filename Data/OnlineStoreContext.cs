using Microsoft.EntityFrameworkCore;
using OnlineStoreBackend.Models;

namespace OnlineStoreBackend.Data
{
    public class OnlineStoreContext : DbContext
    {
        public OnlineStoreContext(DbContextOptions<OnlineStoreContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
