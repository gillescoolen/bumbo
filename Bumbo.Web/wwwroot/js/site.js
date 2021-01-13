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



$(function () {
    $("input.customerAmount").focusout(function () {
        ChangeWorkingHours(this);
    });
    $("input.freightAmount").focusout(function () {
        ChangeWorkingHours(this);
    });
    $("input.weather").click(function () {
        ChangeWorkingHours(this);
    });


    function ChangeWorkingHours(field) {
        let customers = $(field).closest('div.form-row').find('.customerAmount').val();
        let freight = $(field).closest('div.form-row').find('.freightAmount').val();
        let weather = $(field).closest('div.form-row').find('.weather:checked').val();

        let input = $(field).closest('div.form-row').find('#workingHours');

        if (weather == 'regen')
            customers *= 0.6
        else if (weather == 'zon')
            customers *= 1.2;
        else if (weather == 'bewolkt')
            customers *= 0.7
        else if (weather == 'storm')
            customers *= 0.4

        customers /= 50;
        freight /= 100;

        let estimated = Math.round(customers * freight);
        if (estimated < 5) estimated = 5;

        input.val(estimated);
    }

});