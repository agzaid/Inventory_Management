$(function () {
    "use strict";

    $('#fMedication').select2();
    $('#fInvestigation').select2();
    var i = 0;
    var duplicationNum = 0;
    var duplicationNumInv = 0;

    $('#fMedicationAdd').on("click", function () {
        //debugger;
        //var currentVal = $('#patientMedication').val();
        //$('#patientMedication').append("<b><input name=medicationList[" + i + "] hidden/>" + currentVal + $('#fMedication :selected').text() + "</b><br>");
        //$('#patientMedication').val(currentVal = "" ? (currentVal + $.parseHTML("<br>")) : currentVal + $('#fMedication :selected').text());
        //i++;
    });
    $('#ownMedicationAdd').on("click", function () {
        console.log("hello2");
    });
    $('#fInvestigationAdd').on("click", function () {
        console.log("hello");
    });
    $('#ownInvestigationAdd').on("click", function () {
        console.log("hello");
    });


    //AG
    $('#fMedicationAdd').on("click", function () {
        var toAdd = $('#fMedication :selected').text();
        var doseAdd = $('#doseMedication').val();
        var splittedString = toAdd.split(' ');
        var countOfMed = $(".toAdd ").length;
        $('#medicationList').append('<li class="list ' + splittedString[0] + duplicationNum + '">' + toAdd + ' ( ' + doseAdd + ' )</li>');
        $('.' + splittedString[0] + duplicationNum).append('<input id=ownMedicationL class="medList" hidden value="' + toAdd + ' ( ' + doseAdd + ' )"/>');
        duplicationNum++;
        _Doctor.InsertingIteration();
    });
    $('#ownMedicationAdd').on("click", function () {
        var toAdd = $('#medicationInput').val();
        if (toAdd != "") {
            var splittedString = toAdd.split(' ');
            $('#medicationList').append('<li class="list ' + splittedString[0] + duplicationNum + '">' + toAdd + '</li>');
            $('.' + splittedString[0] + duplicationNum).append('<input id=ownMedicationL class="medList" hidden value="' + toAdd + '"/>');
            duplicationNum++;
            _Doctor.InsertingIteration();
            $("#medicationInput").val('');
        }
    });
    $('#fInvestigationAdd').on("click", function () {
        var toAdd = $('#fInvestigation :selected').text();
        var splittedString = toAdd.split(' ');
        $('#ivestigationList').append('<li class="list ' + splittedString[0] + duplicationNumInv + '">' + toAdd + '</li>');
        $('.' + splittedString[0] + duplicationNumInv).append('<input id=ownInvestigationL class="invList" hidden value="' + toAdd + '"/>');
        duplicationNumInv++;
        _Doctor.InsertingIterationInvestigation();

    });
    $('#ownInvestigationAdd').on("click", function () {
        var toAdd = $('#investigationInput').val();
        if (toAdd != "") {
            var splittedString = toAdd.split(' ');
            $('#ivestigationList').append('<li class="list ' + splittedString[0] + duplicationNumInv + '">' + toAdd + '</li>');
            $('.' + splittedString[0] + duplicationNumInv).append('<input id=ownInvestigationL class="invList" hidden value="' + toAdd + '"/>');
            duplicationNumInv++;
            _Doctor.InsertingIterationInvestigation();
            $("#investigationInput").val('');
        }
    });
    //$(document).on('click', 'li.list', function () {
    $(document).on('dblclick', 'li.list', function () {
        debugger;
        $(this).toggleClass('strike').fadeOut('slow');
        _Doctor.InsertingIteration();
        _Doctor.InsertingIterationInvestigation();
    });
    //$('input').focus(function () {
    //    debugger;
    //    $(this).val('');
    //});
    $('#saveOperation').on("click", function () {
        _Doctor.SaveOperation();
    });

});

var _Doctor = {
    InsertingIteration: function () {
        var mdL = $("#ownMedicationL");
        if (mdL != null) {
            var removeEle = $("li.strike").remove();
            var medList = $(".medList");
            var iterate = 0;
            medList.each(function () {
                $(this).removeAttr('name');
                var d = $(this);
                $(this).attr("name", "ownMedicationList[" + iterate + "]");
                iterate++;
            });
        }
    },
    InsertingIterationInvestigation: function () {
        var mdL = $("#ownInvestigationL");
        if (mdL != null) {
            var removeEle = $("li.strike").remove();
            var invList = $(".invList");
            var iterate = 0;
            invList.each(function () {
                $(this).removeAttr('name');
                var d = $(this);
                $(this).attr("name", "ownInvestigationList[" + iterate + "]");
                iterate++;
            });
        }
    },
    SaveOperation: function () {
        debugger;
        var on = $("#operationN").val();
        var or = $("#operativeR").val();
        var od = $("#operativeD").val();
        var pod = $("#pOperativeD").val();
        var pd = $("#prespectiveD").val();
        var opf = $("#operativeF").val();
        var adt = $("#anaethesiaDT").val();
        var mrn = $("#mrn").val();

        var $params = {
            operationN: on,
            operativeR: or,
            operativeD: od,
            pOperativeD: pod,
            prespectiveD: pd,
            operativeF: opf,
            anaethesiaDT: adt,
            mrn: mrn,
        }

        var $url = $("#url").data("createoperation");
        _Ajax.GETWithParametersStringfy($url, $params, "POST", function () { }, function () {
            debugger;
            toastr.success('Operation Added');
        }, function ()
        {
            debugger;
            console.log("sdfs");
        })
        //$.ajax({
        //    type: "GET",
        //    url: $url,
        //    success: function (data) {
        //        debugger;
        //       // $('.targeted').html(data);
        //    }
        //});


    }
}
