using DataEntity.Data;
using Microsoft.EntityFrameworkCore;

namespace DataEntity
{
    public class SkladContext : DbContext
    {
        
        public DbSet<Vozidla> Vozidla { get; set; }
        
        public DbSet<Zakaznici> Zakaznici { get; set; }

        public DbSet<Pronajmy> Pronajmy { get; set; }
        public DbSet<Pokuty> Pokuty { get; set; }
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SPv1;Integrated Security=True;TrustServerCertificate=True";
                optionsBuilder.UseSqlServer(connectionString, opts => opts.EnableRetryOnFailure())
                              .UseLazyLoadingProxies();
            }


        }
    }
}
