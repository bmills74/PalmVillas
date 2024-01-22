$(function () {
    $('#imgInp').on('change', function () {

        var files = $(this);
        var firstFile = files[0];
        var newSrc = URL.createObjectURL(firstFile[0]);
        $('#image0').src = URL.createObjectURL(firstFile)

        // const files = imgInp.files
        // for (var i = 0; i < files.length; i++) {
        //     if (files[i] && i < 1) {
        //         var selector = `#image${i}`;
        //         var newSrc = URL.createObjectURL(files[i]);
        //         debugger;
        //         $('#image0').src = newSrc;
        //     }
        // }

    });
})