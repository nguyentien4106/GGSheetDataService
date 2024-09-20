"use strict";
import { showError, showSuccess, isNullOrEmpty } from "../common.js";

const form = document.getElementById("employee-validation");
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

function removeSheet() {
    $(this).parent().remove();
}

$(".remove-sheet").click(removeSheet);

$("#add-employee").click(function (event) {
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
        Pin: $("#pin").val(),
        Name: $("#name").val(),
        Password: $("#password").val(),
        Privilege: $("#privilege").val(),
        CardNumber: $("#cardnumber").val(),
    };

    if (form.checkValidity()) {
        event.preventDefault();
        $( "#loading" ).show();

        $.post("/Employees/Create", data)
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
