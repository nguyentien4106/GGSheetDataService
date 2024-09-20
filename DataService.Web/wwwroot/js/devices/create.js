"use strict";
import { showError, showSuccess, isNullOrEmpty } from "../common.js";

const form = document.getElementById("needs-validation");
$("#loading").hide()

// Loop over them and prevent submission
form.addEventListener(
    "submit",
    (event) => {
        if (!form.checkValidity()) {
            event.preventDefault();
            event.stopPropagation();
        }

        form.classList.add("was-validated");
    },
    false
);

$("#add-sheet").click(() => {
    const MAX_SHEETS = 5;
    const length = $(".sheet").length;
    if (length >= MAX_SHEETS) {
        showError(`Maximum sheets is ${MAX_SHEETS}`);
        return;
    }
    const $template = $("#templateSheet").clone(true);
    $template.addClass("sheet");
    $template.removeClass("d-none");
    $template.prependTo(".sheets");
});

function removeSheet() {
    $(this).parent().remove();
}

$(".remove-sheet").click(removeSheet);

$("#add-device").click(function (event) {
    const sheetDoms = $(".sheet");
    const sheets = Array.from(sheetDoms).map((val) => ({
        sheetName: $(val).find("input:first").val(),
        documentId: $(val).find("input:last").val(),
    }));

    const isDuplicated = sheets.map(item => item.documentId).some(item => sheets.indexOf(item) !== -1)
    console.log(isDuplicated)
    if (isDuplicated) {
        showError("Document ID is duplicated! Please check again.")
        return
    }
    const data = {
        Ip: $("#ip").val(),
        Port: $("#port").val(),
        CommKey: $("#commKey").val(),
        Sheets: sheets,
    };

    if (form.checkValidity()) {
        event.preventDefault();
        $( "#loading" ).show();

        $.post("/Devices/Create", data)
            .then((res) => {
                if (res.isSuccess) {
                    showSuccess();
                } else {
                    showError(res.message);
                }
            })
            .catch((e) => {
                showError("Some errors occur! Please contact admin to verify.");
            })
            .always(() => {
                $( "#loading" ).hide();
            });
    }
});
