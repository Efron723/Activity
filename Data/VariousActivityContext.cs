using Activity.Models;
using Microsoft.EntityFrameworkCore;

namespace Activity.Data
{
    public class VariousActivityContext : DbContext
    {
        public VariousActivityContext(DbContextOptions<VariousActivityContext> options)
            : base(options)
        {
        }

        public DbSet<VariousActivity> VariousActivities { get; set; } = default!;
        public DbSet<Giveaway> Giveaways { get; set; } = default!;
        public DbSet<Member> Members { get; set; } = default!;

        // 新增一筆 Member 資料
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Member>().HasData(
                new Member
                {
                    Id = new Guid("4444B02E-5546-469F-B8CD-05101FE7BCEE"),
                    Account = "Efron",
                    Password = "0"
                }
            );
        }
    }
}
