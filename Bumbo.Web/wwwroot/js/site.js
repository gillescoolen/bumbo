// @ts-nocheck
$(function () {
    $("input.customerAmount").focusout(() => changeWorkingHours(this));
    $("input.freightAmount").focusout(() => changeWorkingHours(this));
    $("input.weather").click(() => changeWorkingHours(this));

    const changeWorkingHours = (field) => {
        const freight = $(field).closest('div.form-row').find('.freightAmount').val();
        const weather = $(field).closest('div.form-row').find('.weather:checked').val();
        const input = $(field).closest('div.form-row').find('#workingHours');
        
        let customers = $(field).closest('div.form-row').find('.customerAmount').val();

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
		
		customers /= 50;
        freight /= 100;

		let estimated = Math.round(customers * freight);
        if (estimated < 5) estimated = 5;
		
        input.val(estimated);
    }

});