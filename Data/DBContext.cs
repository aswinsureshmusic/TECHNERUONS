using Microsoft.EntityFrameworkCore;
using TECHNERUONS.Models;

namespace TECHNERUONS.Data
{
    public class DBContext:DbContext
    {
        public DBContext(DbContextOptions options):base(options) 
        {
            
        }

        public DbSet<Employee>Employees { get; set; }   
    }
}
