using Microsoft.EntityFrameworkCore;

namespace ActivityCenter.Models
{
    public class ActivityCenterContext : DbContext
    {
        public ActivityCenterContext(DbContextOptions options) : base(options) { }
 
        public DbSet<User> Users { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Participate> Participates { get; set; }
    
    }
}