import { showError, showSuccess, isNullOrEmpty } from "../common.js"

console.log("test")


$(".delete").click(function () {
    const result = confirm("Do you confirm to delete this device")
    const id = $(this).attr("device-id");
    console.log(id)
    if (result) {
        $.post("https://localhost:7058/Devices/Delete", id).then(res => console.log(res))
    }
})