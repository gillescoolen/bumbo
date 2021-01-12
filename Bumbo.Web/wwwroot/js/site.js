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


    const calendar = new FullCalendar.Calendar(calendarElement, {
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: ''
        },

        initialView: 'timeGridWeek',
        initialDate: new Date().toISOString().split('T')[0],

        nowIndicator: true,
        navLinks: true,
        editable: false,

        events: {
            url,
        }
    });

    calendar.render();
});


//TODO: Vanilla js?
$(function () {
    $("input.customerAmount").focusout(() => changeWorkingHours(this));
    $("input.freightAmount").focusout(() => changeWorkingHours(this));
    $("input.weather").click(() => changeWorkingHours(this));

    const changeWorkingHours = (field) => {
        const freight = $(field).closest('div.form-row').find('.freightAmount').val() * 0.75;
        const weather = $(field).closest('div.form-row').find('.weather:checked').val();
        const input = $(field).closest('div.form-row').find('#workingHours');
        
        let customers = $(field).closest('div.form-row').find('.customerAmount').val() / 100;

        switch (weather) {
            case 'regen':
                customers *= 0.5
                break;
            case 'zon':
                customers *= 1.5
                break;
            case 'bewolkt':
                customers *= 0.7
                break;
            case 'storm':
                customers *= 0.2
                break;
            default:
                break;
        }

        input.val(Math.round(customers * freight));
    }

});