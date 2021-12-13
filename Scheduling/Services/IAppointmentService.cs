using Scheduling.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.Services
{
    // I referes to the Interface
    // change public class >> public interface
    public interface IAppointmentService
    {
        // methods needed inside appointmentservice interface
        // we want them to return name of doctor & id and name of patient & id
        // we create a view model for doctor and another for patient
        // we add 2 methods that retrieve all doctors & patients
        public List<DoctorViewModel> GetDoctorList(); // this to get doctor list and place inside list

        public List<PatientViewModel> GetPatientList(); // this to get a patient list and place inside list

        // create  a method to add/update any of the existing appointments
        public Task<int> AddUpdate(AppointmentViewModel model);


        // retreive all the appointments in the db for a particular doctor
        public List<AppointmentViewModel> DoctorsEventsByID(string doctorID);

        // retreive all the appointments in the db for all patients

        public List<AppointmentViewModel> PatientsEventsByID(string patientID);

        // popup model and display details
        // retreive one appointmentviewmodel

        public AppointmentViewModel GetByID(int ID);

        // implement delete appointment logic

        public Task<int> Delete(int ID);

        // confirm event
        // we use Task<> to create async functions

        public Task<int> ConfirmEvent(int ID);
    }
}
