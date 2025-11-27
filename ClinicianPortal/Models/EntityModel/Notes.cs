using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ClinicianPortal.Models.EntityModel
{
    public class Notes
    {
        public int Id { get; set; }
        public string Patient_Id { get; set; }
        public string notes { get; set; }
        public DateTime create_date { get; set; }
    }
}
