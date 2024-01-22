$(function () {


    $('.check-availability').click(function () {


        $.get("/booking/checkavailability", { id: $(this).data('villa-id') }, function (model) {
            var disableRanges = model.rangesBooked;
            //$("#flatpicker").flatpickr(
            //    {
            //        static: true,
            //        inline: true,
            //        showMonths: 1,
            //        mode: "range",
            //        minDate: "today",
            //        dateFormat: "Y-m-d",
            //        altInput: true,
            //        altFormat: "F j, Y",
            //        disable: disableRanges                    
                    
            //    }

            //);
        });
    });


});

