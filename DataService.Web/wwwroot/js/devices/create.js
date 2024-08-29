console.log("test")

const $spinner = $("#spinner")
$spinner.hide()

$("#add-device").click(function () {
    $spinner.show()
    $("#add-device").prop("disabled", true)
    $(".spinner-border").prop("hidden", false)
    $("form").prop("disabled", true)

    const sheetNames = $(".sheetName").map((i, el) => $(el).val()).get()
    const documentIds = $(".documentId").map((i, el) => $(el).val()).get()
    console.log(documentIds)
    const data = {
        IP: $("#ip").val(),
        Port: $("#port").val(),
        CommKey: $("#commKey").val(),
        Sheets: sheetNames.map((idx, val) => ({
            SheetName: val,
            DocumentId: documentIds[idx]
        }))
    }
    $.post("https://localhost:7058/Devices/Create", data)
        .then(res => {
            console.log(res)
            if (res.isSuccess) {

            }
            else {
                Toastify({
                    text: res.message,
                    duration: 3000,
                    destination: "https://github.com/apvarun/toastify-js",
                    newWindow: true,
                    close: true,
                    gravity: "top", // `top` or `bottom`
                    position: "center", // `left`, `center` or `right`
                    stopOnFocus: true, // Prevents dismissing of toast on hover
                    style: {
                        background: "linear-gradient(to right, #00b09b, #96c93d)",
                    },
                    onClick: function () { } // Callback after click
                }).showToast();
            }
        })
        .always(() => {
            $spinner.hide()
            $("#add-device").prop("disabled", false)
            $(".spinner-border").prop("hidden", true)
        })
});

$("#add-sheet").click(() => {
    $(".sheet:first").clone().appendTo(".sheets")
})