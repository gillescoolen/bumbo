// @ts-nocheck
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