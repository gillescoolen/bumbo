// @ts-nocheck
const url = 'api/schedule';

document.addEventListener('DOMContentLoaded', function () {
    const calendarElement = document.getElementById('calendar');
    const eventForm = document.getElementById('add-event');

    /* todo: replace jquery with vanilla js */
    $("#menu-toggle").click(function (e) {
        e.preventDefault();
        $("#wrapper").toggleClass("toggled");
    });

    const update = () => console.log('update...');

    const create = () => console.log('create...');

    const calendar = new FullCalendar.Calendar(calendarElement, {
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'timeGridWeek'
        },

        intitialView: 'timeGridWeek',
        initialDate: new Date().toISOString().split('T')[0],

        nowIndicator: true,
        navLinks: true,
        editable: true,

        eventClick: () => console.log('click...'),

        dateClick: () => console.log('click...'),

        eventDrop: update,
        eventResize: update,

        events: {
            url: 'api/schedule',
        }
    });

    eventForm.addEventListener("submit", (e) => create(e), false);

    calendar.render();
});


//TODO: Vanilla js?
$(function () {

    $("input.customerAmount").focusout(function () {
        ChangeWorkingHours(this);
    });
    $("input.freightAmount").focusout(function () {
        ChangeWorkingHours(this);
    });
    $("input.weather").focusout(function () {
        ChangeWorkingHours(this);
    });


    function ChangeWorkingHours(field) {
        let customers = $(field).closest('div.form-row').find('.customerAmount').val() / 100;
        let freight = $(field).closest('div.form-row').find('.freightAmount').val() * 0.75;
        let weather = $(field).closest('div.form-row').find('.weather').val();

        let input = $(field).closest('div.form-row').find('#workingHours');

        if (weather == 'regen')
            customers *= 0.5
        else if (weather == 'zon')
            customers *= 1.5;
        else if (weather == 'bewolkt')
            customers *= 0.7
        else if (weather == 'storm')
            customers *= 0.2

        input.val(Math.round(customers * freight));
    }

});