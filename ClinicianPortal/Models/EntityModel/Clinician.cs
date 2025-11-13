using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;

namespace ClinicianPortal.Models.EntityModel
{
    public class Clinician
    {
        public int Id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public DateTime create_date { get; set; }
        public string Name { get; set; }
        public string lastName { get; set; }
        public DateTime DOJ { get; set; }

    }
}
