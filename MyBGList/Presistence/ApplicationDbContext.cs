using System.Reflection;

namespace MyBGList.Presistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<BoardGame> BoardGames { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<Mechanic> Mechanics { get; set; }
        public DbSet<BoardGames_Domains> BoardGames_Domains { get; set; }
        public DbSet<BoardGames_Mechanics> BoardGames_Mechanics { get; set; }

        


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Apply all Configurations in ./Presistence/EntitiesConfigurations
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
