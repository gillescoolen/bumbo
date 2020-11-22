// @ts-nocheck
// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', function () {
  const calendarElement = document.getElementById('calendar');
  const eventForm = document.getElementById('add-event');

  eventForm.addEventListener("submit", (e) => create(e), false);

  const create = () => console.log('create...');


  const update = () => console.log('update...');

  const calendar = new FullCalendar.Calendar(calendarElement, {
    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'dayGridMonth,timeGridWeek,timeGridDay'
    },

    intitialView: 'timeGridWeek',
    initialDate: new Date().toISOString().split('T')[0],
    nowIndicator: true,
    navLinks: true,
    editable: true,

    eventClick: () => console.log('click...'),

    dateClick: () => jQuery('#event-add').modal('toggle'),

    eventDrop: update,
    eventResize: update,

    events: {
      url: 'api/schedule',
    }
  });

  calendar.render();
});