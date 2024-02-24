$(document).ready(function () {
    $('#isActiveCheckbox').change(function () {
        if ($(this).is(':checked')) {
            $('#isInactiveCheckbox').prop('checked', false);
        }
    });

    $('#isInactiveCheckbox').change(function () {
        if ($(this).is(':checked')) {
            $('#isActiveCheckbox').prop('checked', false);
        }
    });
});
