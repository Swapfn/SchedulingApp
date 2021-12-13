using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Utility
{
    public static class Helper
    {
        public static string Admin = "Admin";
        public static string Doctor = "Doctor";
        public static string Patient = "Patient";
        public static string appointmentAdded = "Appointment Added Successfuly";
        public static string appointmentUpdated = "Appointment Updated Successfuly";
        public static string appointmentDeleted = "Appointment Deleted Successfuly";
        public static string appointmentExists = "Appointment for selected date already exists";
        public static string appointmentNotExists = "Appointment doesn't exist";
        public static string appointmentAddError = "Something went wrong. Please try again";
        public static string appointmentUpdateError = "Something went wrong. Please try again";
        public static string somethingWentWrong = "Something went wrong. Please try again";
        public static string appointmentConfirmed = "Appointment Confirmed";
        public static string appointmentConfirmedError = "Appointment Confirm Error";
        public static int success_code = 1;
        public static int failure_code = 0;


        public static List<SelectListItem> GetRolesForDropdown(bool isAdmin) //getrolesfordropdown is what its called
        {
            // if the user is admin, can make more admins, if not, only patients and doctors could be added
            if (isAdmin)
            {
                return new List<SelectListItem> //return new seleclistitems
            {
                new SelectListItem{Value=Helper.Admin, Text=Helper.Admin }
            };
            }
            else
            {
                return new List<SelectListItem> //return new seleclistitems
            {
                new SelectListItem{Value=Helper.Doctor, Text=Helper.Doctor },
                new SelectListItem{Value=Helper.Patient, Text=Helper.Patient }
            };
            }

        }
        // helper method for duration
        // this creates a list from 1 hour to 12 hours with increaments of 30 minutes
        public static List<SelectListItem> GetTimeDropDown()
        {
            int minute = 60;
            List<SelectListItem> duration = new();
            for (int i = 1; i <= 12; i++)
            {
                //if (minute < 60)
                //{
                //    duration.Add(new SelectListItem { Value = minute.ToString(), Text = minute + " min" });
                //    minute += 30;
                //}
                duration.Add(new SelectListItem { Value = minute.ToString(), Text = i + " Hr" });
                minute += 30;
                duration.Add(new SelectListItem { Value = minute.ToString(), Text = i + " Hr 30 min" });
                minute += 30;
            }
            return duration;
        }
    }
}
