// to retreive the complete path for route eg: "https://google.com" >>
// location.protocol >> https, https
// location.host >> google.com / amazon.com / localhost
//const routeURL = location.protocol + "//" + location.host;

// main functions
$(document).ready(function () {
    $("#appointmentDate").kendoDateTimePicker({
        // to get current time
        value: new Date(),
        //dateInput: false
        mformat: "yyyy/MM/dd hh:mm tt"
    });
    InitializeCalendar();
});
let calendar; // to initialize the calendar outside of the if statement, so it can be used in other functions
// like calendar.refetchEvents();
function InitializeCalendar() {
    try {
        // creating calendar function
        let calendarEl = document.getElementById('calendar');

        // this to remove the annoying error that showed up outside of the appointment page.
        if (calendarEl != null) {
            calendar = new FullCalendar.Calendar(calendarEl, {
                initialView: 'dayGridMonth',
                headerToolbar: {
                    left: 'prev,next today',
                    center: 'title',
                    right: 'dayGridMonth,timeGridWeek,timeGridDay'
                },
                selectable: true,
                editable: false,
                // to show the partial view when clicked we use the select event
                select: function (event) {
                    // when we select anything
                    onShowModal(event, null);
                    // isEventDetail is null cause we're using for create
                },
                // calling events
                eventDisplay: 'block',
                events: function (fetchInfo, successCallback, failureCallback) {
                    // this needs an ajax call
                    $.ajax({
                        url: '/api/Appointment/GetCalendarData?doctorID=' + $("#doctorID").val(), 
                        type: 'GET',
                        dataType: 'JSON',
                        // if everything is okay, we create the response
                        success: function (response) {
                            let events = [];
                            if (response.status === 1)
                            {
                                // we need to iterate through all the appointments we have in respose and push them
                                // to events variable already declared to display
                                $.each(response.dataenum, function (i, data) {
                                    events.push({
                                        title: data.title,
                                        description: data.description,
                                        start: data.startDate,
                                        end: data.endDate,
                                        // add bg color, if approved make green, if not make red
                                        // data.small letter >> not case sensetive.
                                        backgroundColor: data.isDoctorApproved ? "#28a745" : "#dc3545",
                                        borderColor: "#162466",
                                        textColor: "white",
                                        id: data.id
                                    });
                                })
                            }
                            successCallback(events);
                        },
                        error: function (xhr) {
                            $.notify("Error", "error");

                        }

                    });
                },
                // what happens on we click on an event
                eventClick: function (info) {
                    // what even was clicked
                    // the ID will point to the appointment ID
                    getEventDetailsByEventID(info.event);
                }
            });
            calendar.render();
        }
    }
    catch (e) {
        alert(e); // display alert when an exception happens
    }
}

// when we click on a day, it shows the modal with the content we have in the div at partial view _AddEditAppointment
// isEventDetail is used when editing, when creating we pass null, but when we edit we pass the object
function onShowModal(obj, isEventDetail) {
    // we toggle the ID of the partital view <div> and show it.
    $("#appointmentInput").modal("show");
    // to populate obj, that happens when isEventDetail isn't null, so that means we're editing
    if (isEventDetail != null) {
        // these properties to show the appointment info  when clicked on the event
        $("#title").val(obj.title);
        $("#duration").val(obj.duration);
        $("#appointmentDate").val(obj.startDate);
        $("#description").val(obj.description);
        $("#doctorID").val(obj.doctorID);
        $("#patientID").val(obj.patientID);
        $("#ID").val(obj.id); // DON'T MAKE IT CAPITAL GODDAMIT
        // we use .val() for input
        // we use .html() to show on screen
        $("#lblDoctorName").html(obj.doctorName);
        $("#lblPatientName").html(obj.patientName);
        // we hide buttons and change status if approved
        if (obj.isDoctorApproved) {
            $("#lblStatus").html("Approved");
            $("#btnConfirm").addClass("d-none"); // hide it usinig addClass
            $("#btnSubmit").addClass("d-none");
        }
        else
        {
            $("#lblStatus").html("Pending");
            $("#btnConfirm").removeClass("d-none"); // to show Confirm button if not editing an existing appointment
            $("#btnSubmit").removeClass("d-none"); // to show submit button if not editing an existing appointment
        }
        $("#btnDelete").removeClass("d-none"); // to show delete button if not editing an existing appointment

    }
    else // when a date is clicked, the appointment date should be the date clicked not the current date
    // we got this by putting breakpoint on the earlier if statement, then examining the event object
    {
        $("#appointmentDate").val(obj.startStr + " " + new moment().format("hh:mm A"));
        $("ID").val(0); // because we call it whenever we create a new appointment
        $("#btnSubmit").removeClass("d-none"); // to show submit button if not editing an existing appointment
        $("#btnDelete").addClass("d-none"); // to hide delete button if not editing an existing appointment
    }
}

function onCloseModal() {
    // reset the form and ID after creating an appointment
    $("#appointmentForm")[0].reset();
    $("#ID").val(0);
    $("#title").val("");
    //$("#duration").val("");
    //$("#appointmentDate").val("");
    $("#description").val("");
    // $("#doctorID").val(""); we don't remove doctor ID or the dropdown for selected doctor will clear out as well
    //$("#patientID").val("");
    // we hide the modal when closed
    $("#appointmentInput").modal("hide");

}

function onSubmitForm() {
    // retrieve all elements & display them
    // we get the data from appointmentviwmodel
    // we work with the addeditappointment
    // this is the request data that needs to be submitted

    // we check validation here as well
    if (checkValidation()) {
        let requestData = {
            ID: parseInt($("#ID").val()),
            Title: $("#title").val(),
            StartDate: $("#appointmentDate").val(),
            Description: $("#description").val(),
            Duration: $("#duration").val(),
            DoctorID: $("#doctorID").val(), // inside appointment/index
            PatientID: $("#patientID").val()
        };
        // we use ajax to make the API call in appointmentapicontroller.cs
        // this is jquery code
        $.ajax({
            url: '/api/Appointment/SaveCalendarData',
            type: 'POST',
            data: JSON.stringify(requestData),
            contentType: 'application/json',
            // if everything is okay, we create the response
            success: function (response) {
                if (response.status === 1 || response.status === 2) // not adding the OR will result in creating the appointment
                // going to be "error" message instead of "successful"
                {
                    calendar.refetchEvents(); // to refresh the calendar and show the added appointment
                    $.notify(response.message, "success");
                    onCloseModal();
                }
                else {
                    $.notify(response.message, "error");

                }
            },
            error: function (xhr) {
                $.notify("Error", "error");

            }

        });
    }
    // validation if something isn't entered
    // we use it inside onSubmitForm()
    function checkValidation() {
        let isValid = true;
        // check title is empty or not
        if ($("#title").val() === undefined || $("#title").val() === "") {
            isValid = false;
            $("#title").addClass('error'); // add error class if it's empty
        }
        else
        {
            $("#title").removeClass('error'); // remove error class if it's not empty
        }

        // check date is empty or not
        if ($("#appointmentDate").val() === undefined || $("#appointmentDate").val() === "") {
            isValid = false;
            $("#appointmentDate").addClass('error'); // add error class if it's empty
        }
        else {
            $("#appointmentDate").removeClass('error'); // remove error class if it's not empty
        }
        return isValid;
    }
}

function getEventDetailsByEventID(info) {
    $.ajax({
        url: '/api/Appointment/GetCalendarDataByID/' + info.id, //id has to be small ??? why ???!!
        type: 'GET',
        dataType: 'JSON',
        // if everything is okay, we create the response
        success: function (response) {
            if (response.status === 1 && response.dataenum !== undefined) {
                // if condition is valid, we want to show onShowModal and populate it with our data from API
                onShowModal(response.dataenum, true); // to populate with the data from API
                // isEventDetail is true cause it's a details or update, so we pass true
            }
        },
        error: function (xhr) {
            $.notify("Error", "error");

        }

    });
}

// to refresh calendar whenever a new doctor is chosen

function onDoctorChange() {
    calendar.refetchEvents();
    // we change it on the appointment index page, where the doctor dropdown exists
}


// on both we need an .ajax call
// delete appointment
function onDeleteAppointment() {
    // to retrieve ID for the event
    let id = parseInt($("#ID").val());
    $.ajax({
        url: '/api/Appointment/DeleteAppointment/' + id,
        type: 'GET',
        dataType: 'JSON',
        // if everything is okay, we create the response
        success: function (response) {
            if (response.status === 1)
            // going to be "error" message instead of "successful"
            {
                // we notify success
                $.notify(response.message, "success");
                calendar.refetchEvents(); // to refresh the calendar and show the added appointment
                onCloseModal();
            }
            else {
                $.notify(response.message, "error");
            }
        },
        error: function (xhr) {
            $.notify("Error", "error");

        }

    });
}


// confirm appointment
function onConfirm() {
    // to retrieve ID for the event
    let id = parseInt($("#ID").val());
    $.ajax({
        url: '/api/Appointment/ConfirmAppointment/' + id,
        type: 'GET',
        dataType: 'JSON',
        // if everything is okay, we create the response
        success: function (response) {
            if (response.status === 1)
            // going to be "error" message instead of "successful"
            {
                // we notify success
                $.notify(response.message, "success");
                calendar.refetchEvents(); // to refresh the calendar and show the added appointment
                onCloseModal();
            }
            else {
                $.notify(response.message, "error");
            }
        },
        error: function (xhr) {
            $.notify("Error", "error");

        }

    });
}