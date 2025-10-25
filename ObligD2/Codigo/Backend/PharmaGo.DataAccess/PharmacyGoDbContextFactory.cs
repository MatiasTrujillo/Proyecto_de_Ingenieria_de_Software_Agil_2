using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PharmaGo.DataAccess
{
    public class PharmacyGoDbContextFactory : IDesignTimeDbContextFactory<PharmacyGoDbContext>
    {
        public PharmacyGoDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PharmacyGoDbContext>();

            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=PharmaGoDb;Trusted_Connection=True;");

            return new PharmacyGoDbContext(optionsBuilder.Options);
        }
    }
}