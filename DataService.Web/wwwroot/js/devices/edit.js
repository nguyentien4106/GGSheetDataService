'use strict'
import { showError, showSuccess, isNullOrEmpty } from "../common.js"

$("#loading").hide()
    
// Fetch all the forms we want to apply custom Bootstrap validation styles to
const form = document.getElementById('needs-validation')

// Loop over them and prevent submission
form.addEventListener('submit', event => {
    if (!form.checkValidity()) {
        event.preventDefault()
        event.stopPropagation()
    }

    form.classList.add('was-validated')

}, false)

$("#add-sheet").click(() => {
    const MAX_SHEETS = 5;
    const length = $(".sheet").length;
    if (length >= MAX_SHEETS) {
        showError(`Maximum sheets is ${MAX_SHEETS}`)
        return;
    }
    const $template = $("#templateSheet").clone(true)
    $template.removeClass("d-none")
    $template.addClass("sheet")
    $template.prependTo(".sheets")
    // $(item).find
    // console.log(item)

})

$(".remove-sheet").click(function(){
    $(this).parent().remove()
})

$("#update-device").click(function (event) {
    if (form.checkValidity()) {
        event.preventDefault()
        const sheetDoms = $(".sheet")
        const sheets = Array.from(sheetDoms).map((val) => ({
            Id: $(val).find("input:first").val(),
            sheetName: $(val).find("input:nth-child(2)").val(),
            documentId: $(val).find("input:last").val()
        }))
        const data = {
            Id: $("#id").val(),
            Ip: $("#ip").val(),
            Port: $("#port").val(),
            CommKey: $("#commKey").val(),
            Sheets: sheets
        }
        console.log(data)
        $.post("https://localhost:7058/Devices/Edit", data)
        .then(res => {
            if (res.isSuccess) {
                showSuccess()
            }
            else {
                showError(res.message)
            }
        })
        .catch(e => {
            showError("Some unexpected errors occur! Please contact admin to verify.")
        })
        .always(() => {
            $("#add-device").prop("disabled", false)
            $(".spinner-border").prop("hidden", true)
        })
    }
    
});

