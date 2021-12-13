using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Models.ViewModels
{
    public class AppointmentViewModel
    {
        // ID will be nullable, cause when creating it won't be present
        public int? ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int Duration { get; set; }
        public string DoctorID { get; set; }
        public string PatientID { get; set; }
        public bool IsDoctorApproved { get; set; }
        public string AdminID { get; set; }

        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public string AdminName { get; set; }
        public bool IsForClient { get; set; }

        // after finishing we go to appdbcontext and add db set 
        // this is the code:         public DbSet<Appointment> Appointments { get; set; }
    }
}
