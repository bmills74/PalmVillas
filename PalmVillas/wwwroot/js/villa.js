$(function () {
    $('#reserve-button').click(function (e) {
       
        url = "/Book/BookingSummary?startDate=" + startDate + "&endDate="
            + endDate + "&villaId=" + villaId;
        window.location.href = url;
    });

    var options = {
       
        position: "below left",
        showMonths: 2,
        mode: "range",
        minDate: "today",
        dateFormat: "d/m/Y",
        altInput: true,
        altFormat: "F j, Y",
        // disable: [],
        // appendTo: $('.date-check').get(0),

        onChange: function (selectedDates, dateStr, instance) {

        },
    };

    setFlatpickr();

    $('#clear-button').click(function (e) {
        e.preventDefault();
        $('#pricing-div').addClass('hidden');
        $('.date-input').val('');
        setFlatpickr();
        $('.price-calc').text('');
        startDate = "";
        endDate = "";
    })

});

function setFlatpickr() {

    $(".flatpickr").flatpickr(
        {
            position: "below left",
            showMonths: 2,
            mode: "range",
            minDate: "today",
            dateFormat: "Y-m-d",

            altInput: true,
            altInputClass: "date-input form-control",
            altFormat: "F j, Y",
            disable: disableRanges,
            "plugins": [new rangePlugin({ input: ".check-out" })],
            onClose: function (selectedDates, dateStr, instance) {
                if (selectedDates.length > 1) {
                    startDate = dateStr.substring(0, 10);
                    endDate = dateStr.substring(14, 24);
                    var totalNights = (selectedDates[1] - selectedDates[0]) / (1000 * 60 * 60 * 24);
                    var totalUsd = totalNights * price;
                    var grandTotal = totalUsd + cleaning;
                    $('#price-per-night').text('$' + price + ' x ' + totalNights);
                    $('#total').text('$' + totalUsd);
                    $('#cleaning').text('$' + cleaning);
                    $('#grand-total').text('$' + grandTotal);
                    $('#pricing-div').removeClass('hidden');
                }

            }
        }

    );
}



