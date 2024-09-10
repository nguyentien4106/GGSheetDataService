import { showError, showSuccess, isNullOrEmpty } from "../common.js"
$("#loading").hide()

$(".delete").click(function () {
    const result = confirm("Do you confirm to delete this device")
    if (result) {
        const id = $(this).attr("device-id");
        $("#loading").show()

        $.post("https://localhost:7058/Devices/Delete/" + id).then(res => {
            if (res.isSuccess) {
                location.reload();
            }
            else {
                showError(res.message)
            }
        }).catch(e => {
            showError(e.message)
        }).finally(() => 
            $("#loading").hide()
        )
    }
})