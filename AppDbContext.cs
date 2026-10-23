using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace OlympiadProject
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Olympiad> Olympiads { get; set; }
        public DbSet<OlympiadSubject> OlympiadSubjects { get; set; }
        public DbSet<Grade> Grades { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connString = ConfigurationManager.ConnectionStrings["OlympiadConn"].ConnectionString;
            optionsBuilder.UseSqlServer(connString);
        }

        public static void EnsureDatabaseUpdated()
        {
            using (var db = new AppDbContext())
            {
                try { db.Database.ExecuteSqlRaw("ALTER TABLE Olympiads ADD [Date] NVARCHAR(50) NULL;"); } catch { }

                try { db.Database.ExecuteSqlRaw("ALTER TABLE Olympiads ADD [PrizeFund] NVARCHAR(100) NULL;"); } catch { }
            }
        }
    }
}