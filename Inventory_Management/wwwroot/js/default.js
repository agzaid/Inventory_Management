
//APPS AJAX WRAPPER

var _Ajax = {
    WithoutParameters: function ($url, $method, $beforecallBack, $successcallBack, $errorcallBack) {
        debugger;
        $.ajax({
            type: $method,
            url: $url,
            contentType: 'application/json; charset=utf-8',
            // data: { jewellerId: filter, locale: 'en-US' },
            dataType: 'json',
            beforeSend: function () {
                $beforecallBack();
            },
            success: function (data) {
                $successcallBack(data);
            },
            error: function (xhr, textStatus, errorThrown) { $errorcallBack(xhr); }
        });
    },
    WithId: function ($url, id, $method, $beforecallBack, $successcallBack, $errorcallBack) {
        debugger;
        $.ajax({
            type: $method,
            url: $url + `/id =` + id,
            contentType: 'application/json; charset=utf-8',
            // data: { jewellerId: filter, locale: 'en-US' },
            dataType: 'json',
            beforeSend: function () {
                $beforecallBack();
            },
            success: function (data) {
                $successcallBack(data);
            },
            error: function (xhr, textStatus, errorThrown) { $errorcallBack(xhr); }
        });
    },
    GETWithParameters: function ($url, $params, $method,$token, $beforecallBack, $successcallBack, $errorcallBack) {
        debugger;
        $.ajax({
            type: $method,
            url: $url,
            // url: `/` + controller + `/` + action,
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify($params),
            headers: {
            "RequestVerificationToken": $token // Send the token in the request headers
            },
            dataType: 'json',
            beforeSend: function () {
                $beforecallBack();
            },
            success: function (data) {
                $successcallBack(data);
            },
            error: function (xhr, textStatus, errorThrown) { $errorcallBack(xhr); }
        });
    },
    GETWithParametersStringfy: function ($url, $params, $method, $beforecallBack, $successcallBack, $errorcallBack) {
        debugger;
        $.ajax({
            type: $method,
            url: $url,
            contentType: 'application/json; charset=utf-8',
            data: $params,
            dataType: 'json',
            beforeSend: function () {
                $beforecallBack();
            },
            success: function (data) {
                $successcallBack(data);
            },
            error: function (xhr, textStatus, errorThrown) { $errorcallBack(xhr); }
        });
    },
    ReturnHTML: function ($url, $params, $method, $beforecallBack, $successcallBack, $errorcallBack) {
        //debugger;
        $.ajax({
            type: $method,
            url: $url,
            contentType: 'application/json; charset=utf-8',
            data: $params,
            dataType: 'html',
            beforeSend: function () {
                $beforecallBack();
            },
            success: function (data) {
                $successcallBack(data);
            },
            error: function (xhr, textStatus, errorThrown) { $errorcallBack(xhr); }
        });
    }
}


//---------------------------SWEET Alert---------------------------------------------

var _SWAL = {
    Alert: function () {
        Swal.fire({
            title: 'هل تريد الاستمرار؟',
            icon: 'question',
            iconHtml: '؟',
            confirmButtonText: 'نعم',
            cancelButtonText: 'لا',
            showCancelButton: true,
            showCloseButton: true
        }).then((result) => {
            if (result.isConfirmed) {
                // _Ajax.CallWithId("index", "controller", 3);
                window.location = `@Url.Action("Index","Doctor",new { id = 3 })`;
            }
        });
    }, CustomAlertWithRedirect: function (controller, action, parameter) {
        Swal.fire({
            title: 'هل تريد الاستمرار؟',
            icon: 'question',
            iconHtml: '؟',
            confirmButtonText: 'نعم',
            cancelButtonText: 'لا',
            showCancelButton: true,
            showCloseButton: true
        }).then((result) => {
            if (result.isConfirmed) {
                // _Ajax.CallWithId("index", "controller", 3);
                window.location = `@Url.Action("` + action + `","` + controller + `",` + parameter + `)`;
            }
        });
    }, CustomAlert: function () {
        return new Promise((resolve) => {
            Swal.fire({
                title: 'هل تريد الاستمرار؟',
                icon: 'question',
                iconHtml: '؟',
                confirmButtonText: 'نعم',
                cancelButtonText: 'لا',
                showCancelButton: true,
                showCloseButton: true
            }).then((result) => {
                if (result.isConfirmed) {
                    resolve(true);
                } else
                    resolve(false);
            });
        });
    }, CustomAlertWithOwnMessage: function (mess, yes, no) {
        return new Promise((resolve) => {
            Swal.fire({
                title: mess,
                icon: 'question',
                iconHtml: '؟',
                confirmButtonText: yes,
                cancelButtonText: no,
                showCancelButton: true,
                showCloseButton: true
            }).then((result) => {
                if (result.isConfirmed) {
                    resolve(true);
                } else
                    resolve(false);
            });
        });
    },
    AlertWithoutNotification: function (message) {
        Swal.fire({
            icon: "error",
            text: message
        });
    },
}

var _TOASTR = {
    Success: function ($message) {
        toastr.success($message);
    },
    Error: function ($message) {
        toastr.error($message)
    }
}

