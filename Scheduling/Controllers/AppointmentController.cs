using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scheduling.Services;
using Scheduling.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Controllers
{
    // to authorize the signed in users only to use the app
    // if not, they'll be redirected to login page
    [Authorize]
    public class AppointmentController : Controller
    {
        // we use appointment service here as well like _db
        private readonly IAppointmentService _appointmentService;
        //ctor + tab tatb
        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }
        public IActionResult Index()
        {
            // regiester the service at startup.cs > configureservices
            ViewBag.DoctorList =_appointmentService.GetDoctorList();
            ViewBag.PatientList = _appointmentService.GetPatientList();
            ViewBag.Duration = Helper.GetTimeDropDown(); // we need to call helper method not appointmentservice.

            return View();
        }
    }
}
