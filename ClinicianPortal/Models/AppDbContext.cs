using ClinicianPortal.Models.EntityModel;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;

namespace ClinicianPortal.Models
{
    public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public Microsoft.EntityFrameworkCore.DbSet<Clinician> Clinician { get; set; }
    }
}
