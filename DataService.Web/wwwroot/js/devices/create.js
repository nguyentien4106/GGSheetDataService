import { showError, showSuccess, isNullOrEmpty } from "../common.js"

console.log("test")

$("#add-device").click(function () {
    $("form").prop("disabled", true)

    const sheetNames = $(".sheetName").map((i, el) => $(el).val()).get()
    const documentIds = $(".documentId").map((i, el) => $(el).val()).get()
    const emptyIds = documentIds.some(item => isNullOrEmpty(item))
    if (emptyIds) {
        showError("Please input all documentId field.")
        return
    }
    const data = {
        Ip: $("#ip").val(),
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
    if (isNullOrEmpty(data.Ip) || isNullOrEmpty(data.Port) || isNullOrEmpty(data.CommKey) || data.Sheets.length === 0) {
        showError("Please input all field.")
        return
    }


    $.post("https://localhost:7058/Devices/Create", data)
        .then(res => {
            console.log(res)
            if (res.isSuccess) {
                showSuccess()
            }
            else {
                showError(res.message)
            }
        })
        .catch(e => {
            showError("Some errors occur! Please contact admin to verify.")
        })
        .always(() => {
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