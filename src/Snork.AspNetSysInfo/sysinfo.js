$(document).ready(function () {
    $('#tabstrip').kendoTabStrip({
        animation: {
            open:
            {
                effects: 'fadeIn'
            }
        }
    });

    $(".makeAGrid").each(function() {
        $(this).kendoGrid({
            height: 550,
            sortable: true
        });
    });
});