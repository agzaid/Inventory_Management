﻿
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
    GETWithParameters: function ($url, $params, $method,$beforecallBack, $successcallBack, $errorcallBack) {
        debugger;
        $.ajax({
            type: $method,
            url: $url,
           // url: `/` + controller + `/` + action,
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify($params),
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
    },
    AlertWithoutNotification: function (message) {
        Swal.fire({
            icon: "error",
            text:message
        });
    },
}

//function _AjaxCall(action, controller) {
//    debugger;

//}
