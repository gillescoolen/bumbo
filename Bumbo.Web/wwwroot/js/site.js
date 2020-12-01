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