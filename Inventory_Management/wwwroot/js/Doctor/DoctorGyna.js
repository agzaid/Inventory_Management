var i = 0;
var duplicationNum = 0;
var duplicationNumInv = 0;
$(function () {
    "use strict";

    $('#fMedication').select2();
    $('#fInvestigation').select2();

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
        debugger;
        var toAdd = $('#fMedication :selected').text();
        var doseAdd = $('#doseMedication').val();
        var splittedString = toAdd.split(' ');
        var countOfMed = $(".toAdd ").length;
        $('#medicationList').append('<li class="list ' + splittedString[0] + duplicationNum + '">' + toAdd + ' ( ' + doseAdd + ' )</li>');
        $('.' + splittedString[0] + duplicationNum).append('<input id=ownMedicationL class="medList" hidden value="' + toAdd + ' ( ' + doseAdd + ' )"/>');
        duplicationNum++;
        _GynaDoctor.InsertingIteration();
    });
    $('#ownMedicationAdd').on("click", function () {
        debugger;
        var toAdd = $('#medicationInput').val();
        if (toAdd != "") {
            var splittedString = toAdd.split(' ');
            $('#medicationList').append('<li class="list ' + splittedString[0] + duplicationNum + '">' + toAdd + '</li>');
            $('.' + splittedString[0] + duplicationNum).append('<input id=ownMedicationL class="medList" hidden value="' + toAdd + '"/>');
            duplicationNum++;
            _GynaDoctor.InsertingIteration();
            $("#medicationInput").val('');
        }
    });
    $('#fInvestigationAdd').on("click", function () {
        var toAdd = $('#fInvestigation :selected').text();
        var splittedString = toAdd.split(' ');
        $('#ivestigationList').append('<li class="list ' + splittedString[0] + duplicationNumInv + '">' + toAdd + '</li>');
        $('.' + splittedString[0] + duplicationNumInv).append('<input id=ownInvestigationL class="invList" hidden value="' + toAdd + '"/>');
        duplicationNumInv++;
        _GynaDoctor.InsertingIterationInvestigation();

    });
    $('#ownInvestigationAdd').on("click", function () {
        var toAdd = $('#investigationInput').val();
        if (toAdd != "") {
            var splittedString = toAdd.split(' ');
            $('#ivestigationList').append('<li class="list ' + splittedString[0] + duplicationNumInv + '">' + toAdd + '</li>');
            $('.' + splittedString[0] + duplicationNumInv).append('<input id=ownInvestigationL class="invList" hidden value="' + toAdd + '"/>');
            duplicationNumInv++;
            _GynaDoctor.InsertingIterationInvestigation();
            $("#investigationInput").val('');
        }
    });
    //$(document).on('click', 'li.list', function () {
    $(document).on('dblclick', 'li.list', function () {
        debugger;
        $(this).toggleClass('strike').fadeOut('slow');
        _GynaDoctor.InsertingIteration();
        _GynaDoctor.InsertingIterationInvestigation();
    });
    //$('input').focus(function () {
    //    debugger;
    //    $(this).val('');
    //});
    $('#saveOperation').on("click", function () {
        _Doctor.SaveOperation();
    });

});

var _GynaDoctor = {
    FmedicationAdd: function (event) {
        debugger;
        var id = event.target.id;
        var splittedId = id.split('-');
        var s = '#fMedication' + '-' + splittedId[1];
        var eev = $('#fMedication-' + splittedId[1]);
        var eev1 = eev[0];
        var toAdd = eev1.innerText;
        //var toAdd = $('#fMedication-'+ splittedId[1] + ':selected').text();
        var getDoseAdd = $('#doseMedication-' + splittedId[1]);
        var doseAdd = getDoseAdd.val();
        var splittedString = toAdd.split(' ');
        var countOfMed = $(".toAdd ").length;
        $('#medicationList-' + splittedId[1]).append('<li class="list ' + splittedString[0] + duplicationNum + '">' + toAdd + ' ( ' + doseAdd + ' )</li>');
        $('.' + splittedString[0] + duplicationNum).append('<input id=ownMedicationL class="medList" hidden value="' + toAdd + ' ( ' + doseAdd + ' )"/>');
        duplicationNum++;
        _GynaDoctor.InsertingIteration(splittedId[1]);
    },
    OwnMedicationAdd: function (event) {
        debugger;
        var id = event.target.id;
        var splittedId = id.split('-');
        var eev = $('#medicationInput-' + splittedId[1]);
        //var eev1 = eev[0];
        var toAdd = eev.val();
        //var toAdd = $('#medicationInput').val();
        if (toAdd != "") {
            var splittedString = toAdd.split(' ');
            $('#medicationList-' + splittedId[1]).append('<li class="list ' + splittedString[0] + duplicationNum + '">' + toAdd + '</li>');
            $('.' + splittedString[0] + duplicationNum).append('<input id=ownMedicationL class="medList" hidden value="' + toAdd + '"/>');
            duplicationNum++;
            _GynaDoctor.InsertingIteration(splittedId[1]);
            $("#medicationInput-" + splittedId[1]).val('');
        }
    },
    InsertingIteration: function (index) {
        var mdL = $("#ownMedicationL");
        if (mdL != null) {
            var removeEle = $("li.strike").remove();
            var medList = $(".medList");
            var iterate = 0;
            medList.each(function () {
                $(this).removeAttr('name');
                var d = $(this);
                $(this).attr("name", "[" + index + "].DoctorIndexVM.ownMedicationList[" + iterate + "]");
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
}
