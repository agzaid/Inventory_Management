$(document).ready(function () {
    "use strict";
    $("#btnSearch").on("click", function () {
        var $filter = $("#filterSearch").val();
        var $inputSearch = $("#inputSearch").val();

        var parameter = {
            filter: $filter,
            inputSearch: $inputSearch
        };
        _Ajax.GETWithParametersStringfy($("#url").data("getpatient"), parameter, "GET",
            function () { },
            function (data) {
                var res = JSON.parse(data);
                if (res.Status == 0) {
                    if (res.Count == 1) {
                        MappingObjects(res.Patient[0]);
                        //to be removed from here
                        //ShowTable(res.Patient);
                        $("#bookButton").prop("disabled", false);

                    } else
                        ShowTable(res.Patient);
                    $("#bookButton").prop("disabled",false);

                } else
                    _SWAL.AlertWithoutNotification(res.Message);
            },
            function (xhr) {
                _SWAL.AlertWithoutNotification("Something went wrong..!!");
            });
        //alert("hlelll");
    })
    $('#listOfsamePatient').on("dblclick", "tr",function (e) {
        debugger;
        var data = $('#listOfsamePatient').DataTable().row(this).data();
        MappingObjects(data);
        $("#tableView").modal('toggle');
    });


    //var connection = new signalR.HubConnectionBuilder().withUrl("/notifyHub").build();

    ////Disable the send button until connection is established.
    //document.getElementById("sendButton").disabled = true;

    //connection.on("ReceiveMessage", function (user, message,number) {
    //    var li = document.createElement("li");
    //    document.getElementById("messagesList").appendChild(li);
    //    // We can assign user-supplied strings to an element's textContent because it
    //    // is not interpreted as markup. If you're assigning in any other way, you 
    //    // should be aware of possible script injection concerns.
    //    li.textContent = `${user} : ${message} --- ${number}`;
    //});

    //connection.start().then(function () {
    //    document.getElementById("sendButton").disabled = false;
    //}).catch(function (err) {
    //    return console.error(err.toString());
    //});

    //document.getElementById("sendButton").addEventListener("click", function (event) {
    //    var user = document.getElementById("userInput").value;
    //    var message = document.getElementById("messageInput").value;
    //    connection.invoke("SendMessage", user, message).catch(function (err) {
    //        return console.error(err.toString());
    //    });
    //    event.preventDefault();
    //});
});



function MappingObjects(pt) {
    debugger;
    $("#ArabicName").val(pt.ArabicName);
    $("#EnglishName").val(pt.EnglishName);
    $("#MobileNumber").val(pt.MobileNumber);
    $("#NationalID").val(pt.NationalID);
    $("#Gender").val(pt.Gender);
    $("#MaritalStatus").val(pt.MaritalStatus);
    $("#Religion").val(pt.Religion);
    $("#DateOfBirth").val(moment(pt.DateOfBirth).format('YYYY-MM-DD'));
}
function ShowTable(res) {
    debugger;
    $("#tableView").modal('toggle');
    listOfsamePatient = $("#listOfsamePatient").DataTable({
        destroy: true,
        autowidth: true,
        "processing": false,
        order: [[1, 'asc']],
        responsive: false,
        data: res,
        "columns": [
            { "data": "", title: "Seq", defaultContent: "", searchable: false, orderable: true, targets: 0, className: "text-center" },
            { "data": "ArabicName", title: "Arabic Name", className: "text-center" },
            { "data": "EnglishName", title: "English Name", className: "text-center" },
            { "data": "MobileNumber", title: "Mobile Number", className: "text-center" },
            { "data": "NationalID", title: "National ID", className: "text-center" },
            {
                "data": "MaritalStatus", title: "Marital Status", className: "text-center",
                render: function (data, type, row, meta) {
                    if (data == 0) {
                        return "Single"
                    } else
                    return "Married"
                }
            }
        ]
    });
    listOfsamePatient.on('order.dt search.dt', function () {
        listOfsamePatient.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}
