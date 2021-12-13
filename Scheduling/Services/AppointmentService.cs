using Scheduling.Models;
using Scheduling.Models.ViewModels;
using Scheduling.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Services
{
    // make appointmentservice inherit the interface Iappointmentservice
    public class AppointmentService : IAppointmentService // implement interface
    {
        // to link the db and be able to access it
        private readonly AppDBContext _db;

        // ctor + tab tab
        public AppointmentService(AppDBContext db)
        {
            _db = db;
        }

        public async Task<int> AddUpdate(AppointmentViewModel model)
        {
            var startDate = DateTime.Parse(model.StartDate);
            // to calculate the end date based on duration + startdate
            var endDate = DateTime.Parse(model.StartDate).AddMinutes(Convert.ToDouble(model.Duration));

            if(model!= null && model.ID > 0)
            {
                //update appointment by fetching the appopintment.
                var appointment = _db.Appointments.FirstOrDefault();
                appointment.Title = model.Title;
                appointment.Description = model.Description;
                appointment.StartDate = startDate;
                appointment.EndDate = endDate;
                appointment.Duration = model.Duration;
                appointment.DoctorID = model.DoctorID;
                appointment.PatientID = model.PatientID;
                appointment.IsDoctorApproved = false; // error here ?!
                appointment.AdminID = model.AdminID;
                // save changes to the db, don't forget theh await
                await _db.SaveChangesAsync();
                return 1;
            }
            else
            {
                // create
                Appointment appointment = new()
                {
                    Title = model.Title,
                    Description = model.Description,
                    StartDate = startDate,
                    EndDate = endDate,
                    Duration = model.Duration,
                    DoctorID = model.DoctorID,
                    PatientID = model.PatientID,
                    IsDoctorApproved = false, // error here ?!
                    AdminID = model.AdminID
                };
                // push to the db
                _db.Appointments.Add(appointment);
                // save changes is async
                await _db.SaveChangesAsync();
                return 2; // return 2 for create, return 1 for update
            }

        }

        // we use Task<> to use async methods
        public async Task<int> ConfirmEvent(int ID)
        {
            // retrieve appointment
            var appointment = _db.Appointments.FirstOrDefault(x => x.ID == ID);

            // if appointment isn't null, set approval to true
            if (appointment != null)
            {
                appointment.IsDoctorApproved = true;
                return await _db.SaveChangesAsync(); // to save changes to the db
            }
            else
            {
                return 0;
            }
        }

        // task means it perform an action on db ???!!
        public async Task<int> Delete(int ID)
        {
            // retrieve appointment
            var appointment = _db.Appointments.FirstOrDefault(x => x.ID == ID);

            // if appointment isn't null, delete appointment
            if (appointment != null)
            {
                // remove appointment
                _db.Appointments.Remove(appointment);
                return await _db.SaveChangesAsync(); // to save changes to the db
            }
                return 0;
        }

        public List<AppointmentViewModel> DoctorsEventsByID(string doctorID)
        {
            return _db.Appointments.Where(x => x.DoctorID == doctorID).ToList().Select(c => new AppointmentViewModel()
            // we use projections since we want to return  a list of appointmentviewmodel and create an object
            // so we convert from apppointment to appointmentviewmodel
            {
                ID = c.ID,
                Description = c.Description,
                StartDate = c.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                EndDate = c.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Title = c.Title,
                Duration = c.Duration,
                IsDoctorApproved = c.IsDoctorApproved
            }).ToList();
            // we convert to a list
        }

        public AppointmentViewModel GetByID(int ID)
        {
            return _db.Appointments.Where(x => x.ID == ID).ToList().Select(c => new AppointmentViewModel()
            // we use projections since we want to return one appointmentviewmodel and create an object
            // so we convert from apppointment to appointmentviewmodel
            {
                ID = c.ID,
                Description = c.Description,
                StartDate = c.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                EndDate = c.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Title = c.Title,
                Duration = c.Duration,
                IsDoctorApproved = c.IsDoctorApproved,
                PatientID = c.PatientID,
                DoctorID = c.DoctorID,
                // to get the patient and doctor name, and one record only we use 
                // firstordefault()
                PatientName = _db.Users.Where(x=> x.Id == c.PatientID).Select(x=> x.Name).FirstOrDefault(),
                DoctorName = _db.Users.Where(x => x.Id == c.DoctorID).Select(x => x.Name).FirstOrDefault()
            }).SingleOrDefault();
            // to retrive one record
        }

        public List<DoctorViewModel> GetDoctorList()
        {
            // we use link and query syntax to join multiple tables
            // here we're accessing the users in _db.users table
            // we use projection to get the needed columns
            var doctors = (from users in _db.Users
                           join userRoles in _db.UserRoles on users.Id equals userRoles.UserId
                           join roles in _db.Roles.Where(x=>x.Name==Helper.Doctor) on userRoles.RoleId equals roles.Id
                           // here we're joining the user table with userRoles and roles tables to be able to fetch
                           // the doctor's ID's only, so first we join the userRoles with users where the id is matc
                           // then we do the same with the roles and userRole as well
                           // x=>x.Name==Helper.Doctor is called "lambda expression"
                           // This will get the doctor data and truncate all other.
                           select new DoctorViewModel   // this is called projection cause we're only retriving certain columns
                           {
                               // we retreive the needed columns ID and Name from DoctorViewModel
                               ID = users.Id,
                               Name = users.Name
                           }
                           ).ToList(); // we convert the ID and Name to list
            // Like this we retreive all the ID & name for users 
            // to make that for doctors only, we return doctors
            return doctors;
        }

        public List<PatientViewModel> GetPatientList()
        {
            // we use link and query syntax to join multiple tables
            // here we're accessing the users in _db.users table
            // we use projection to get the needed columns
            var patients = (from users in _db.Users
                           join userRoles in _db.UserRoles on users.Id equals userRoles.UserId
                           join roles in _db.Roles.Where(x => x.Name == Helper.Patient) on userRoles.RoleId equals roles.Id
                           // here we're joining the user table with userRoles and roles tables to be able to fetch
                           // the paptient's ID's only, so first we join the userRoles with users where the id is matc
                           // then we do the same with the roles and userRole as well
                           // x=>x.Name==Helper.Patient is called "lambda expression"
                           // This will get the patitent data and truncate all other.
                           select new PatientViewModel   // this is called projection cause we're only retriving certain columns
                           {
                               // we retreive the needed columns ID and Name from DoctorViewModel
                               ID = users.Id,
                               Name = users.Name
                           }
                           ).ToList(); // we convert the ID and Name to list
            // Like this we retreive all the ID & name for users 
            // to make that for patient only, we return patients
            return patients;
        }

        public List<AppointmentViewModel> PatientsEventsByID(string patientID)
        {
            return _db.Appointments.Where(x => x.PatientID == patientID).ToList().Select(c => new AppointmentViewModel()
            // we use projections since we want to return  a list of appointmentviewmodel and create an object
            // so we convert from apppointment to appointmentviewmodel
            {
                ID = c.ID,
                Description = c.Description,
                StartDate = c.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                EndDate = c.EndDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Title = c.Title,
                Duration = c.Duration,
                IsDoctorApproved = c.IsDoctorApproved
            }).ToList();
        }
    }
}
