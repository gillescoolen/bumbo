// @ts-nocheck
$(function () {
    $("input.customerAmount").focusout(function () {changeWorkingHours(this)});
    $("input.freightAmount").focusout(function () {changeWorkingHours(this)});
    $("input.weather").click(function () {changeWorkingHours(this)});
    
    $("#menu-toggle").click(function (e) {
        e.preventDefault();
        $("#wrapper").toggleClass("toggled");
    });
    
    const changeWorkingHours = (field) => {
        let freight = $(field).closest('div.form-row').find('.freightAmount').val() / 100;
        let weather = $(field).closest('div.form-row').find('.weather:checked').val();
        let input = $(field).closest('div.form-row').find('#workingHours');
        
        let customers = $(field).closest('div.form-row').find('.customerAmount').val();

        switch (weather) {
            case 'regen':
                customers *= 0.7
                break;
            case 'zon':
                customers *= 1.2
                break;
            case 'bewolkt':
                customers *= 0.9
                break;
            case 'storm':
                customers *= 0.6
                break;
            default:
                break;
        }
		
		customers /= 50;

		let estimated = Math.round(customers * freight);
        if (estimated < 5) estimated = 5;
		
        input.val(estimated);
    }
});