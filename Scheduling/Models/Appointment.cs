using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Models
{
    public class Appointment
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public string DoctorID { get; set; }
        public string PatientID { get; set; }
        public bool IsDoctorApproved { get; set; }
        public string AdminID { get; set; }
    }
}
