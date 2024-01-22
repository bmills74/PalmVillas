$(function () {
    var currentHash = window.location.hash;

    if (currentHash) {
        $('.menu-item').removeClass('active');
        $('.menu-item').each(function () {
            if ($(this).attr('href').includes(currentHash)) {
                $(this).addClass('active');
            }
        });
    }
})

function getPostOptions(url, payload) {
    var options =  {              /* method name in code behind, and full path to my view*/
        url: url,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        data: payload,
        //contentType: "application/json; charset=utf-8",
        //dataType: "json"
    }
    return options;
}



function handleCredentialResponse(response) {

    const responsePayload = decodeJwtResponse(response.credential);
  
    var options = getPostOptions("/Index", responsePayload);

    $.post(options,  function (result) {
            $('#profile-div').empty();
            $('#profile-div').append(result);
        });


    console.log("ID: " + responsePayload.sub);
    console.log('Full Name: ' + responsePayload.name);
    console.log('Given Name: ' + responsePayload.given_name);
    console.log('Family Name: ' + responsePayload.family_name);
    console.log("Image URL: " + responsePayload.picture);
    console.log("Email: " + responsePayload.email);
}

function decodeJwtResponse(token) {
    return jwt_decode(token);
}