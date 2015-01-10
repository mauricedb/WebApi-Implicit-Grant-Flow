$(function () {

    $('#btnUnsecure').click(function () {
        $.ajax('/api/demo').then(
            function (data) {
                $('#result').text(data);
            },
            function (err) {
                $('#result').text(JSON.stringify(err));
            }
        );

    });

    $('#btnLogin').click(function () {

        var authorizationUrl =
            location.protocol + "//" + location.host + '/authorize';
        var client_id = 'myclient';
        var redirect_uri =
            location.protocol + "//" + location.host + '/callback.html';
        var response_type = "token";
        var state = Date.now() + "" + Math.random();

        var url =
            authorizationUrl + "?" +
            "client_id=" + encodeURI(client_id) + "&" +
            "redirect_uri=" + encodeURI(redirect_uri) + "&" +
            "response_type=" + encodeURI(response_type) + "&" +
            "state=" + encodeURI(state);
        sessionStorage.state = state;
        sessionStorage.redirect_uri = location;
        window.location = url;

    });

    $('#btnSecure').click(function () {
        $.ajax('/api/demo', {
            headers: {
                Authorization: sessionStorage.token_type + ' ' + sessionStorage.access_token
            }
        }).then(
            function (data) {
                $('#result').text(data);
            },
            function (err) {
                $('#result').text(JSON.stringify(err));
            }
        );
    });
});
