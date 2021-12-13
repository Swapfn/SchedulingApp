using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduling.Models;
using Scheduling.Models.ViewModels;
using Scheduling.Services;
using Scheduling.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// this isn't a generic controller
// this is API endpoint
namespace Scheduling.Controllers.Api
{
    // we give it [Route("api/Appointment")]
    [Route("api/Appointment")]
    // define api endpoint using [ApiController]
    [ApiController]
    public class AppointmentApiController : Controller
    {
        // we work with the database using the appointment/iappointment services
        // we inject IAppointmentService, not AppointmentService
        // using DI
        private readonly IAppointmentService _appointmentService; // IAPPOINTMENT SERVICE HOLY FUCKING SHIT
        // injecting IHttpContextAccessor
        private readonly IHttpContextAccessor _httpContextAccessor;
        // get userID and role of the  user
        private readonly string loginUserID;
        private readonly string role;

        public AppointmentApiController(IAppointmentService appointmentService, IHttpContextAccessor httpContextAccessor)
        {
            _appointmentService = appointmentService;
            // we get loginUserID using httpcontextaccessor that as httpcontextobject
            // we inject services.AddHttpContextAccessor(); in startup.cs
            // to get the object
            _httpContextAccessor = httpContextAccessor;
            // claimtypes are built in and are populated when user logs in
            loginUserID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);

        }
        // creates save method and invoking appointmentservice
        [HttpPost]
        [Route("SaveCalendarData")]
        public IActionResult SaveCalendarData(AppointmentViewModel data)
        {
            CommonResponse<int> commonResponse = new CommonResponse<int>();
            try
            {
                // retrieve result and save it at status
                commonResponse.status = _appointmentService.AddUpdate(data).Result;
                if (commonResponse.status == 1)
                {
                    commonResponse.message = Helper.appointmentUpdated;
                }
                if (commonResponse.status == 2)
                {
                    commonResponse.message = Helper.appointmentAdded;
                }
            }
            catch (Exception e)
            {
                commonResponse.message = e.Message;
                commonResponse.status = Helper.failure_code;
            }
            // we return ok with common response, since it's an Api call
            return Ok(commonResponse);
        }

        // we create endpoint here, so first we add Iappointmentservice, then appointmenentservice then here
        [HttpGet]
        [Route("GetCalendarData")]
        public IActionResult GetCalendarData(string doctorID)
        {
            CommonResponse<List<AppointmentViewModel>> commonResponse = new();
            // we call different methods depending on the role signed in
            try
            {
                if (role == Helper.Patient)
                {
                    // this will pass the id of the logged in user (patitent)
                    commonResponse.dataenum = _appointmentService.PatientsEventsByID(loginUserID);
                    commonResponse.status = Helper.success_code;
                }
                else if (role == Helper.Doctor)
                {
                    // this will pass the id of the logged in user (doctor)
                    commonResponse.dataenum = _appointmentService.DoctorsEventsByID(loginUserID);
                    commonResponse.status = Helper.success_code;
                }
                else
                {
                    // this will pass the id of the logged in user
                    // this for admin, they see multiple doctors
                    commonResponse.dataenum = _appointmentService.DoctorsEventsByID(doctorID);
                    commonResponse.status = Helper.success_code;
                }
            }
            catch (Exception e)
            {
                commonResponse.message = e.Message;
                commonResponse.status = Helper.failure_code;
            }
            return Ok(commonResponse);
        }

        // getcalendardatabyid
        // to get the model to display the details when clicking on an appointment
        // it retrieves one appointment only
        // ID is appointment ID
        [HttpGet]
        [Route("GetCalendarDataByID/{ID}")]
        public IActionResult GetCalendarDataByID(int ID)
        {
            CommonResponse<AppointmentViewModel> commonResponse = new();
            // we get one appointment
            try
            {
                    // this will pass the id of the logged in user (patitent)
                    commonResponse.dataenum = _appointmentService.GetByID(ID);
                    commonResponse.status = Helper.success_code;
            }
            catch (Exception e)
            {
                commonResponse.message = e.Message;
                commonResponse.status = Helper.failure_code;
            }
            return Ok(commonResponse);
        }

        // confirm
        [HttpGet]
        [Route("ConfirmAppointment/{ID}")]

        // no Task<> here ?!!
        public IActionResult ConfirmAppointment(int ID)
        {
            CommonResponse<int> commonResponse = new();
            try
            {
                var result = _appointmentService.ConfirmEvent(ID).Result;
                if (result > 0)
                {
                    commonResponse.status = Helper.success_code;
                    commonResponse.message = Helper.appointmentConfirmed;
                }
                else
                {
                    commonResponse.status = Helper.failure_code;
                    commonResponse.message = Helper.appointmentConfirmedError;
                }
            }
            catch (Exception e)
            {
                commonResponse.message = e.Message;
                commonResponse.status = Helper.failure_code;
            }
            return Ok(commonResponse);
        }

        // delete
        [HttpGet]
        [Route("DeleteAppointment/{ID}")]
        public async Task<IActionResult> DeleteAppointment(int ID)
        {
            CommonResponse<int> commonResponse = new();
            try
            {
                // to delete the appointmenet
                commonResponse.status = await _appointmentService.Delete(ID);
                // if commonresponse.status == 1 then do helper.deleted else helper.somethingwentwrong
                commonResponse.message = commonResponse.status == 1 ? Helper.appointmentDeleted : Helper.somethingWentWrong;
            }
            catch (Exception e)
            {
                commonResponse.message = e.Message;
                commonResponse.status = Helper.failure_code;
            }
            return Ok(commonResponse);
        }
    }
}
