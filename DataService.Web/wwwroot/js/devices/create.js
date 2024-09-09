import { showError } from "../common.js"

console.log("test")

$("#add-device").click(function () {
    $("form").prop("disabled", true)

    const sheetNames = $(".sheetName").map((i, el) => $(el).val()).get()
    const documentIds = $(".documentId").map((i, el) => $(el).val()).get()
    const data = {
        IP: $("#ip").val(),
        Port: $("#port").val(),
        CommKey: $("#commKey").val(),
        Sheets: sheetNames.map((val, idx) => {
            return {
                SheetName: val,
                DocumentId: documentIds[idx],
                DeviceId: 0
            }
        })
    }
    console.log(data)
    $.post("https://localhost:7058/Devices/Create", data)
        .then(res => {
            console.log(res)
            if (res.isSuccess) {

            }
            else {
                showError(res.message)
            }
        })
        .catch(e => {
            showError("Some errors occur! Please contact admin to verify.")
        })
        .always(() => {
            $spinner.hide()
            $("#add-device").prop("disabled", false)
            $(".spinner-border").prop("hidden", true)
        })
});

$("#add-sheet").click(() => {
    const MAX_SHEETS = 5;
    const length = $(".sheet").length;
    if (length >= MAX_SHEETS) {
        showError(`Maximum sheets is ${MAX_SHEETS}`)
        return;
    }
    $(".sheet:first").clone().prependTo(".sheets")
})